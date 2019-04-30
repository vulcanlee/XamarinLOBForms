using Acr.UserDialogs;
using LOBForm.Helpers;
using LOBForm.Models;
using LOBForm.Repositories;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace LOBForm.ViewModels
{
    public class LeaveAppFormAgentViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LeaveAppForm LeaveAppFormItem { get; set; }
        public ObservableCollection<string> DepartmentsSource { get; set; } = new ObservableCollection<string>();
        public string DepartmentSelectedItem { get; set; } = "";
        public string UserName { get; set; } = ""; 
        public ObservableCollection<LAFAgentReslut> QueryAgentListSource { get; set; } = new ObservableCollection<LAFAgentReslut>();
        public LAFAgentReslut QueryAgentSelectedItem { get; set; }
        public DelegateCommand QueryAgentListCommand { get; set; }
        public DelegateCommand SelectAgentCommand { get; set; }
        private readonly INavigationService _navigationService;

        public LeaveAppFormAgentViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            QueryAgentListCommand = new DelegateCommand(async () =>
            {
                var fooProgressDialogConfig = new ProgressDialogConfig()
                {
                    MaskType = MaskType.Black,
                    Title = "請稍後，正在查詢可用代理人清單..."
                };
                using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
                {
                    APIResult fooResult;
                    var fooQueryAgentRepository = new QueryAgentRepository();
                    string fooDep = DepartmentSelectedItem ?? "";
                    fooResult = await fooQueryAgentRepository.Post(new LAFAgentQuery()
                    {
                         DepartmentName = fooDep,
                          Name = UserName
                    });
                    if (fooResult.Success == false)
                    {
                        if (await MainHelper.CheckAccessToken(fooResult) == false)
                        {
                            return;
                        }

                        try
                        {
                            var fooAlertConfig = new AlertConfig()
                            {
                                Title = "警告",
                                Message = $"查詢可用代理人清單發生了錯誤 {Environment.NewLine}{fooResult.Message}",
                                OkText = "確定"
                            };
                            CancellationTokenSource fooCancelSrc = new CancellationTokenSource(10000);
                            await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(fooAlertConfig, fooCancelSrc.Token);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                    else
                    {
                        QueryAgentListSource.Clear();
                        QueryAgentSelectedItem = null;
                        foreach (var item in fooQueryAgentRepository.Items)
                        {
                            QueryAgentListSource.Add(item);
                        }
                    }
                }

            });

            SelectAgentCommand = new DelegateCommand(async () =>
            {
                if (QueryAgentSelectedItem == null)
                {
                    var config = new Acr.UserDialogs.AlertConfig()
                    {
                        Title = "警告",
                        Message = $"您尚未選取請假代理人",
                        OkText = "確定",
                    };

                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(config);
                }
                else
                {
                    LeaveAppFormItem.AgentId = QueryAgentSelectedItem.MyUserId;
                    LeaveAppFormItem.AgentName = QueryAgentSelectedItem.Name;
                    NavigationParameters fooPara = new NavigationParameters();
                    fooPara.Add(MainHelper.QueryUserAgent, LeaveAppFormItem.Clone());
                    await _navigationService.GoBackAsync(fooPara);
                }
            });
        }

        public  void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(MainHelper.QueryUserAgent))
            {
                LeaveAppFormItem = parameters[MainHelper.QueryUserAgent] as LeaveAppForm;
            }

            var fooDepRepo = new DepartmentRepository();
            await fooDepRepo.ReadAsync();
            DepartmentsSource.Clear();
            DepartmentsSource.Add("");
            foreach (var item in fooDepRepo.Items)
            {
                DepartmentsSource.Add(item.DepartmentName);
            }

            UserName = LeaveAppFormItem.AgentName ?? "";
        }

    }
}
