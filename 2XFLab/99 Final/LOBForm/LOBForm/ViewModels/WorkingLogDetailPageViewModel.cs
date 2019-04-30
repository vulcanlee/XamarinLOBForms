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

    public class WorkingLogDetailPageViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public WorkingLog WorkingLogItem { get; set; }
        public WorkingLog WorkingLogSourceItem { get; set; }
        public bool IsCreateRecordMode { get; set; } = false;
        public bool IsUpdaeRecordMode { get => !IsCreateRecordMode; }
        public ObservableCollection<string> ProjectsSource { get; set; } = new ObservableCollection<string>();
        public string ProjectSelectedItem { get; set; }
        private ProjectRepository fooProjectRepository = new ProjectRepository();
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        private readonly INavigationService _navigationService;
        public readonly IPageDialogService _dialogService;
        public WorkingLogDetailPageViewModel(INavigationService navigationService,
            IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;

            AddCommand = new DelegateCommand(async () =>
            {
                #region 建立要新增紀錄的頁面參數，並且回傳到清單頁面
                var fooPItem = fooProjectRepository.Items.FirstOrDefault(x => x.ProjectName == ProjectSelectedItem);
                if(fooPItem!=null)
                {
                    WorkingLogItem.Project = fooPItem;
                }
                var fooMyUser = new LoginRepository();
                await fooMyUser.ReadAsync();
                WorkingLogItem.Owner = fooMyUser.Item.MyUser;
                NavigationParameters fooPara = new NavigationParameters();
                fooPara.Add(MainHelper.CRUDItemKeyName, WorkingLogItem);
                fooPara.Add(MainHelper.CRUDFromDetailKeyName, MainHelper.CRUD_Create);

                await _navigationService.GoBackAsync(fooPara);
                #endregion

            });
            DeleteCommand = new DelegateCommand(async () =>
            {
                #region 建立要刪除紀錄的頁面參數，並且回傳到清單頁面
                var fooDel = await _dialogService.DisplayAlertAsync("警告", $"你確定要刪除這筆 {WorkingLogItem.Title} 紀錄嗎 ? ", "確定", "取消");
                if (fooDel == true)
                {
                    var fooPItem = fooProjectRepository.Items.FirstOrDefault(x => x.ProjectName == ProjectSelectedItem);
                    if (fooPItem != null)
                    {
                        WorkingLogItem.Project = fooPItem;
                    }
                    var fooMyUser = new LoginRepository();
                    await fooMyUser.ReadAsync();
                    WorkingLogItem.Owner = fooMyUser.Item.MyUser;
                    NavigationParameters fooPara = new NavigationParameters();
                    fooPara.Add(MainHelper.CRUDItemKeyName, WorkingLogItem);
                    fooPara.Add(MainHelper.CRUDFromDetailKeyName, MainHelper.CRUD_Delete);

                    await _navigationService.GoBackAsync(fooPara);
                    #endregion
                }
            });
            SaveCommand = new DelegateCommand(async () =>
            {
                #region 建立要修改紀錄的頁面參數，並且回傳到清單頁面
                var fooPItem = fooProjectRepository.Items.FirstOrDefault(x => x.ProjectName == ProjectSelectedItem);
                if (fooPItem != null)
                {
                    WorkingLogItem.Project = fooPItem;
                }
                var fooMyUser = new LoginRepository();
                await fooMyUser.ReadAsync();
                WorkingLogItem.Owner = fooMyUser.Item.MyUser;
                NavigationParameters fooPara = new NavigationParameters();
                fooPara.Add(MainHelper.CRUDItemKeyName, WorkingLogItem);
                fooPara.Add(MainHelper.CRUDFromDetailKeyName, MainHelper.CRUD_Update);

                await _navigationService.GoBackAsync(fooPara);
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
                WorkingLogItem = parameters[MainHelper.CRUDItemKeyName] as WorkingLog;
            }

            await LoadProjects();

            #region 設定專案清單的預設項目
            var fooProject = fooProjectRepository.Items.FirstOrDefault(x => x.ProjectId == WorkingLogItem.Project.ProjectId);
            if (fooProject != null)
            {
                ProjectSelectedItem = fooProject.ProjectName;
            }
            #endregion
        }

        private async Task LoadProjects()
        {
            await fooProjectRepository.ReadAsync();
            ProjectsSource.Clear();
            foreach (var item in fooProjectRepository.Items)
            {
                ProjectsSource.Add(item.ProjectName);
            }
        }
    }
}
