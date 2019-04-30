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
    public class WorkingLogPageViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsRefreshing { get; set; } = false;
        public ObservableCollection<WorkingLog> WorkingLogList { get; set; } = new ObservableCollection<WorkingLog>();
        public WorkingLog WorkingLogSelectedItem { get; set; }
        public DelegateCommand DoRefreshCommand { get; set; }
        public DelegateCommand ItemTappedCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        private readonly INavigationService _navigationService;

        public WorkingLogPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            ItemTappedCommand = new DelegateCommand(async () =>
            {
                NavigationParameters fooPara = new NavigationParameters();
                fooPara.Add(MainHelper.CRUDItemKeyName, WorkingLogSelectedItem.Clone());
                fooPara.Add(MainHelper.CRUDKeyName, MainHelper.CRUD_Update);

                await _navigationService.NavigateAsync("WorkingLogDetailPage", fooPara);
            });

            AddCommand = new DelegateCommand(async () =>
            {
                var fooItem = new WorkingLog()
                {
                    LogDate = DateTime.Now,
                };
                NavigationParameters fooPara = new NavigationParameters();
                fooPara.Add(MainHelper.CRUDItemKeyName, fooItem);
                fooPara.Add(MainHelper.CRUDKeyName, MainHelper.CRUD_Create);

                await _navigationService.NavigateAsync("WorkingLogDetailPage", fooPara);
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
                    if (parameters.ContainsKey(MainHelper.CRUDFromDetailKeyName))
                    {
                        var fooItem = parameters[MainHelper.CRUDItemKeyName] as WorkingLog;
                        var fooAction = parameters[MainHelper.CRUDFromDetailKeyName] as string;
                        if (fooAction == MainHelper.CRUD_Create)
                        {
                            await CreateRecord(fooItem);
                        }
                        else if (fooAction == MainHelper.CRUD_Delete)
                        {
                            await DeleteRecord(fooItem);
                        }
                        else if (fooAction == MainHelper.CRUD_Update)
                        {
                            await UpdateRecord(fooItem);
                        }
                    }
                    #endregion
                }
            }
            else
            {
            }
            #endregion
        }

        /// <summary>
        /// 從本地端檔案，讀取快取資料
        /// </summary>
        /// <returns></returns>
        private async Task RefreshCache()
        {
            var fooWorkingLogRepository = new WorkingLogRepository();
            await fooWorkingLogRepository.ReadAsync();
            WorkingLogList.Clear();
            foreach (var item in fooWorkingLogRepository.Items)
            {
                WorkingLogList.Add(item);
            }
        }

        /// <summary>
        /// 進行工作日誌清單更新
        /// </summary>
        /// <returns></returns>
        public async Task RetriveRecords()
        {
            #region 進行工作日誌清單更新
            APIResult fooResult;
            IsRefreshing = true;
            var fooProgressDialogConfig = new ProgressDialogConfig()
            {
                Title = "請稍後，正在進行工作日誌清單更新中...",
                IsDeterministic = false,
            };
            using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
            {
                var fooLoginRepository = new LoginRepository();
                await fooLoginRepository.ReadAsync();
                var fooWorkingLogRepository = new WorkingLogRepository();
                fooResult = await fooWorkingLogRepository.GetByUserIDAsync(fooLoginRepository.Item.MyUser.EmployeeID);
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

        public async Task CreateRecord(WorkingLog workingLog)
        {
            #region 進行工作日誌新增
            APIResult fooResult;
            IsRefreshing = true;
            var fooProgressDialogConfig = new ProgressDialogConfig()
            {
                Title = "請稍後，正在進行工作日誌清單新增中...",
                IsDeterministic = false,
            };
            using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
            {
                var fooWorkingLog = new WorkingLogRepository();
                fooResult = await fooWorkingLog.PostAsync(workingLog);
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

        public async Task DeleteRecord(WorkingLog workingLog)
        {
            #region 進行工作日誌刪除
            APIResult fooResult;
            IsRefreshing = true;
            var fooProgressDialogConfig = new ProgressDialogConfig()
            {
                Title = "請稍後，正在進行工作日誌清單刪除中...",
                IsDeterministic = false,
            };
            using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
            {
                var fooWorkingLog = new WorkingLogRepository();
                fooResult = await fooWorkingLog.DeleteAsync(workingLog);
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
                            Message = $"刪除資料發生了錯誤 {Environment.NewLine}{fooResult.Message}",
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

        public async Task UpdateRecord(WorkingLog workingLog)
        {
            #region 進行工作日誌清單更新
            APIResult fooResult;
            IsRefreshing = true;
            var fooProgressDialogConfig = new ProgressDialogConfig()
            {
                Title = "請稍後，正在進行工作日誌清單更新中...",
                IsDeterministic = false,
            };
            using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
            {
                var fooWorkingLog = new WorkingLogRepository();
                fooResult = await fooWorkingLog.PutAsync(workingLog);
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
                await RetriveRecords();
            }
            IsRefreshing = false;
            #endregion
        }
    }

}
