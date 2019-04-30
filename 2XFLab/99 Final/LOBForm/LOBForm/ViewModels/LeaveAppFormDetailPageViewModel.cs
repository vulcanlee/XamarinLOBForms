using LOBForm.Helpers;
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
using System.Threading.Tasks;

namespace LOBForm.ViewModels
{

    public class LeaveAppFormDetailPageViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LeaveAppForm LeaveAppFormItem { get; set; }
        public LeaveAppForm LeaveAppFormSourceItem { get; set; }
        public ObservableCollection<string> LeaveCategoriesSource { get; set; } = new ObservableCollection<string>();
        public string LeaveCategorySelectedItem { get; set; }
        private LeaveCategoryRepository fooLeaveCategoryRepository = new LeaveCategoryRepository();
        public bool IsCreateRecordMode { get; set; } = false;
        public bool IsUpdaeRecordMode { get => !IsCreateRecordMode; }
        public TimeSpan BeginTime { get; set; }
        public TimeSpan CompleteTime { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand SelectAgentCommand { get; set; }
        private readonly INavigationService _navigationService;
        public readonly IPageDialogService _dialogService;

        public LeaveAppFormDetailPageViewModel(INavigationService navigationService,
            IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;


            AddCommand = new DelegateCommand(async () =>
            {
                #region 建立要新增紀錄的頁面參數，並且回傳到清單頁面
                var fooPItem = fooLeaveCategoryRepository.Items.FirstOrDefault(x => x.LeaveCategoryName == LeaveCategorySelectedItem);
                if (fooPItem != null)
                {
                    LeaveAppFormItem.Category = LeaveCategorySelectedItem;
                }
                var fooMyUser = new LoginRepository();
                await fooMyUser.ReadAsync();
                LeaveAppFormItem.Owner = fooMyUser.Item.MyUser;

                UpdateTime(LeaveAppFormItem);

                NavigationParameters fooPara = new NavigationParameters();
                fooPara.Add(MainHelper.CRUDItemKeyName, LeaveAppFormItem);
                fooPara.Add(MainHelper.CRUDFromDetailKeyName, MainHelper.CRUD_Create);

                await _navigationService.GoBackAsync(fooPara);
                #endregion

            });
            DeleteCommand = new DelegateCommand(async () =>
            {
                #region 建立要刪除紀錄的頁面參數，並且回傳到清單頁面
                var fooDel = await _dialogService.DisplayAlertAsync("警告", $"你確定要刪除這筆 {LeaveAppFormItem.Category} 紀錄嗎 ? ", "確定", "取消");
                if (fooDel == true)
                {
                    var fooPItem = fooLeaveCategoryRepository.Items.FirstOrDefault(x => x.LeaveCategoryName == LeaveCategorySelectedItem);
                    if (fooPItem != null)
                    {
                        LeaveAppFormItem.Category = LeaveCategorySelectedItem;
                    }
                    var fooMyUser = new LoginRepository();
                    await fooMyUser.ReadAsync();
                    LeaveAppFormItem.Owner = fooMyUser.Item.MyUser;

                    UpdateTime(LeaveAppFormItem);

                    NavigationParameters fooPara = new NavigationParameters();
                    fooPara.Add(MainHelper.CRUDItemKeyName, LeaveAppFormItem);
                    fooPara.Add(MainHelper.CRUDFromDetailKeyName, MainHelper.CRUD_Delete);

                    await _navigationService.GoBackAsync(fooPara);
                    #endregion
                }
            });
            SaveCommand = new DelegateCommand(async () =>
            {
                #region 建立要修改紀錄的頁面參數，並且回傳到清單頁面
                var fooPItem = fooLeaveCategoryRepository.Items.FirstOrDefault(x => x.LeaveCategoryName == LeaveCategorySelectedItem);
                if (fooPItem != null)
                {
                    LeaveAppFormItem.Category = LeaveCategorySelectedItem;
                }
                var fooMyUser = new LoginRepository();
                await fooMyUser.ReadAsync();
                LeaveAppFormItem.Owner = fooMyUser.Item.MyUser;

                UpdateTime(LeaveAppFormItem);

                NavigationParameters fooPara = new NavigationParameters();
                fooPara.Add(MainHelper.CRUDItemKeyName, LeaveAppFormItem);
                fooPara.Add(MainHelper.CRUDFromDetailKeyName, MainHelper.CRUD_Update);

                await _navigationService.GoBackAsync(fooPara);
                #endregion
            });
            SelectAgentCommand = new DelegateCommand(async () =>
            {
                NavigationParameters fooPara = new NavigationParameters();
                fooPara.Add(MainHelper.QueryUserAgent, LeaveAppFormItem.Clone());
                await _navigationService.NavigateAsync("LeaveAppFormAgent", fooPara);
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
            if (parameters.InternalParameters.ContainsKey(MainHelper.Prism__NavigationMode) == true)
            {
                var fooNaviModeValue = parameters.InternalParameters[MainHelper.Prism__NavigationMode].ToString();
                NavigationMode fooNaviMode = (NavigationMode)Enum.Parse(typeof(NavigationMode), fooNaviModeValue);
                if (fooNaviMode == NavigationMode.New)
                {
                    #region 第一次顯示這個頁面
                    if (parameters.ContainsKey(MainHelper.CRUDKeyName))
                    {
                        #region 設定現在可以使用的按鈕
                        var CRUDMode = parameters[MainHelper.CRUDKeyName] as string;
                        if (CRUDMode == MainHelper.CRUD_Create)
                        {
                            IsCreateRecordMode = true;
                        }
                        else
                        {
                            IsCreateRecordMode = false;
                        }
                        #endregion
                    }
                    if (parameters.ContainsKey(MainHelper.CRUDItemKeyName))
                    {
                        // 取得要維護的紀錄
                        LeaveAppFormItem = parameters[MainHelper.CRUDItemKeyName] as LeaveAppForm;

                        BeginTime = LeaveAppFormItem.BeginDate.TimeOfDay;
                        CompleteTime = LeaveAppFormItem.CompleteDate.TimeOfDay;
                    }

                    await LoadLeaveCategory();

                    #region 設定專案清單的預設項目
                    LeaveCategorySelectedItem = LeaveAppFormItem.Category;
                    #endregion
                    #endregion
                }
                else
                {
                    #region 從選取代理人頁面返回
                    if (parameters.ContainsKey(MainHelper.QueryUserAgent))
                    {
                        // 取得要維護的紀錄
                        LeaveAppFormItem = parameters[MainHelper.QueryUserAgent] as LeaveAppForm;
                    }
                    #endregion
                }
            }
        }

        private async Task LoadLeaveCategory()
        {
            await fooLeaveCategoryRepository.ReadAsync();
            LeaveCategoriesSource.Clear();
            foreach (var item in fooLeaveCategoryRepository.Items)
            {
                LeaveCategoriesSource.Add(item.LeaveCategoryName);
            }
        }

        void UpdateTime(LeaveAppForm leaveAppForm)
        {
            leaveAppForm.BeginDate = new DateTime(leaveAppForm.BeginDate.Date.Ticks + BeginTime.Ticks);
            leaveAppForm.CompleteDate = new DateTime(leaveAppForm.CompleteDate.Date.Ticks + CompleteTime.Ticks);
        }
    }
}
