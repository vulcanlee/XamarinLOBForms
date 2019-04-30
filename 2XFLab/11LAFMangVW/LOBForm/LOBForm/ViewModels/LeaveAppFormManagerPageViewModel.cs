using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using LOBForm.Models;
using LOBForm.Repositories;
using Prism.Navigation;
using Prism.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using LOBForm.Helpers;

namespace LOBForm.ViewModels
{
    public class LeaveAppFormManagerPageViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LeaveAppForm LeaveAppFormSelectedItem { get; set; }
        public ObservableCollection<LeaveAppForm> LeaveAppFormList { get; set; } = new ObservableCollection<LeaveAppForm>();
        public bool IsRefreshing { get; set; } = false;
        public DelegateCommand DoRefreshCommand { get; set; }
        public DelegateCommand<LeaveAppForm> ApproveCommand { get; set; }
        public DelegateCommand<LeaveAppForm> DenyCommand { get; set; }
        private readonly INavigationService _navigationService;

        public LeaveAppFormManagerPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            ApproveCommand = new DelegateCommand<LeaveAppForm>(async (x) =>
            {
                await ApproveDenyRecord(x, true);
            });
            DenyCommand = new DelegateCommand<LeaveAppForm>(async (x) =>
            {
                await ApproveDenyRecord(x, false);
            });
            DoRefreshCommand = new DelegateCommand(async () =>
            {
                await RetriveRecords();
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
            #region 檢查與確認，該頁面是第一次顯示，還是由上一頁面返回到這個頁面
            if (parameters.InternalParameters.ContainsKey(MainHelper.Prism__NavigationMode) == true)
            {
                var fooNaviModeValue = parameters.InternalParameters[MainHelper.Prism__NavigationMode].ToString();
                NavigationMode fooNaviMode = (NavigationMode)Enum.Parse(typeof(NavigationMode), fooNaviModeValue);
                if (fooNaviMode == NavigationMode.New)
                {
                    #region 第一次顯示這個頁面
                    await RefreshCache();
                    #endregion
                }
                else
                {
                    #region 從別的頁面回報這個頁面
                    #endregion
                }
            }
            else
            {
            }
            #endregion
        }

        private async Task RefreshCache()
        {
            var fooLeaveAppFormRepository = new LeaveAppFormRepository();
            await fooLeaveAppFormRepository.ReadAsync(MainHelper.LeaveAppFormManagerMode);
            LeaveAppFormList.Clear();
            foreach (var item in fooLeaveAppFormRepository.Items)
            {
                if (item.ApproveResult == "尚未審核")
                {
                    LeaveAppFormList.Add(item);
                }
            }
        }

        public async Task RetriveRecords()
        {
            #region 進行待審核請假單清單更新
            APIResult fooResult;
            IsRefreshing = true;
            var fooProgressDialogConfig = new ProgressDialogConfig()
            {
                Title = "請稍後，正在進行待審核請假單清單更新中...",
                IsDeterministic = false,
            };
            using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
            {
                var fooLoginRepository = new LoginRepository();
                await fooLoginRepository.ReadAsync();
                var fooLeaveAppFormRepository = new LeaveAppFormRepository();
                fooResult = await fooLeaveAppFormRepository.PostByUserIDAsync(new LeaveAppFormByUserModel()
                {
                    Account = fooLoginRepository.Item.MyUser.EmployeeID,
                    Mode = MainHelper.LeaveAppFormManagerMode
                });

                if (fooResult.Success == false)
                {
                    if (await MainHelper.CheckAccessToken(fooResult) == false)
                    {
                        IsRefreshing = false;
                        return;
                    }

                    try
                    {
                        var fooAlertConfig = new AlertConfig()
                        {
                            Title = "警告",
                            Message = $"更新資料發生了錯誤 {Environment.NewLine}{fooResult.Message}",
                            OkText = "確定"
                        };
                        CancellationTokenSource fooCancelSrc = new CancellationTokenSource(10000);
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(fooAlertConfig, fooCancelSrc.Token);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
            if (fooResult.Success == true)
            {
                await RefreshCache();
            }
            IsRefreshing = false;
            #endregion
        }

        public async Task ApproveDenyRecord(LeaveAppForm leaveAppForm, bool isApprove)
        {
            #region 進行審核請假單
            APIResult fooResult;
            IsRefreshing = true;
            var fooProgressDialogConfig = new ProgressDialogConfig()
            {
                Title = "請稍後，正在進行審核請假單清單中...",
                IsDeterministic = false,
            };
            using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
            {
                var fooLeaveAppFormRepository = new LeaveAppFormRepository();
                if (isApprove == true)
                {
                    leaveAppForm.ApproveResult = "已審核";
                    leaveAppForm.FormsStatus = "已生效";
                }
                else
                {
                    leaveAppForm.ApproveResult = "被否決";
                    leaveAppForm.FormsStatus = "未生效";
                }
                fooResult = await fooLeaveAppFormRepository.PutAsync(leaveAppForm);
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
                            Message = $"新增資料發生了錯誤 {Environment.NewLine}{fooResult.Message}",
                            OkText = "確定"
                        };
                        CancellationTokenSource fooCancelSrc = new CancellationTokenSource(10000);
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(fooAlertConfig, fooCancelSrc.Token);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
            if (fooResult.Success == true)
            {
                await RetriveRecords();
            }
            IsRefreshing = false;
            #endregion
        }
    }

}
