using Acr.UserDialogs;
using LOBForm.Helpers;
using LOBForm.Repositories;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LOBForm.ViewModels
{
    public class MDPageViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand<string> MenuCommand { get; set; }
        public string UserName { get; set; }
        public bool IsManager { get; set; } = false;
        private readonly INavigationService _navigationService;
        public readonly IPageDialogService _dialogService;

        public MDPageViewModel(INavigationService navigationService,
            IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            MenuCommand = new DelegateCommand<string>(async (x) =>
            {
                switch (x)
                {
                    #region 工作日誌
                    case "工作日誌":
                        await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/WorkingLogPage");
                        break;
                    #endregion

                    #region 請假單
                    case "請假單":
                        await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/LeaveAppFormPage");
                        break;
                    #endregion

                    #region 請假單審核
                    case "請假單審核":
                        await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/LeaveAppFormManagerPage");
                        break;
                    #endregion

                    #region 緊急電話清單
                    case "緊急電話清單":
                        await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/OnCallPage");
                        break;
                    #endregion

                    #region 關  於
                    case "關  於":
                        await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/AboutPage");
                        break;
                    #endregion

                    #region 登  出
                    case "登  出":
                        var fooResult = await _dialogService.DisplayAlertAsync("提醒",
                            "您確定要進行登出作業嗎?", "是", "取消");
                        if (fooResult == true)
                        {
                            #region 要進行登出，所以，清空本機快取資料
                            var fooProgressDialogConfig = new ProgressDialogConfig()
                            {
                                MaskType = MaskType.Black,
                                Title = "請稍後，正在進行登出中..."
                            };
                            using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
                            {
                                await MainHelper.CleanRepositories();
                            }
                            await _navigationService.NavigateAsync("xf:///LoginPage");
                            #endregion
                        }
                        break;
                    #endregion
                    default:
                        break;
                }
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
            var fooMyUser = new LoginRepository();
            await fooMyUser.ReadAsync();
            UserName = fooMyUser.Item.MyUser.Name;
            IsManager = fooMyUser.Item.MyUser.IsManager;
        }

    }
}
