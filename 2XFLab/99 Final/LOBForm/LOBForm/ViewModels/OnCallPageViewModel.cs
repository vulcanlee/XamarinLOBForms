using Acr.UserDialogs;
using LOBForm.Models;
using LOBForm.Repositories;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LOBForm.ViewModels
{

    public class OnCallPageViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsRefreshing { get; set; } = false;
        public DelegateCommand<OnCallPhone> CallPhoneCommand { get; set; }
        public DelegateCommand DoRefreshCommand { get; set; }

        public ObservableCollection<OnCallPhone> OnCallPhoneList { get; set; } = new ObservableCollection<OnCallPhone>();
        private readonly INavigationService _navigationService;
        public readonly IPageDialogService _dialogService;
        public OnCallPageViewModel(INavigationService navigationService,
            IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            CallPhoneCommand = new DelegateCommand<OnCallPhone>(async x =>
            {
                Plugin.Messaging.CrossMessaging.Current.PhoneDialer.MakePhoneCall(
                    x.PhoneNumber, x.Title);
            });

            DoRefreshCommand = new DelegateCommand(async () =>
            {
                IsRefreshing = true;
                #region 進行緊急電話清單更新
                APIResult fooResult;
                var fooProgressDialogConfig = new ProgressDialogConfig()
                {
                    Title = "請稍後，正在進行緊急電話清單更新中...",
                    IsDeterministic = false,
                };
                using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
                {
                    var fooOnCallPhoneRepository = new OnCallPhoneRepository();
                    fooResult = await fooOnCallPhoneRepository.GetAsync();
                    if (fooResult.Success == false)
                    {
                        try
                        {
                            var fooAlertConfig = new AlertConfig()
                            {
                                Title = "警告",
                                Message = $"更新資料發生了錯誤 {Environment.NewLine}{fooResult.Message}",
                                OkText = "確定"
                            };
                            CancellationTokenSource fooCancelSrc = new CancellationTokenSource(3000);
                            await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(fooAlertConfig, fooCancelSrc.Token);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                }
                if (fooResult.Success == true)
                {
                    await Refresh();
                }
                IsRefreshing = false;
                #endregion
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            await Refresh();
        }

        private async Task Refresh()
        {
            var fooOnCallPhoneRepository = new OnCallPhoneRepository();
            await fooOnCallPhoneRepository.ReadAsync();
            OnCallPhoneList.Clear();
            foreach (var item in fooOnCallPhoneRepository.Items)
            {
                OnCallPhoneList.Add(item);
            }
        }
    }

}
