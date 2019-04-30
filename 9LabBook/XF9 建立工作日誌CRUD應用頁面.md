# XF9 建立工作日誌CRUD應用頁面

![](Icons/fa-search.png) 這份文件的完成結果專案原始碼，可以參考 `XamarinLOBForms\2XFLab\09WorkingLog`

我們要開始練習如何在 Xamarin.Forms 專案內，撰寫具有 CRUD 應用的表單功能，一般來說，在 Xamarin.Forms 應用程式內，要完成具有 CRUD 應用需求，至少需要提供兩個頁面，一個是顯示所有集合紀錄的頁面，另外一個是要顯示特定紀錄的詳細頁面；我們需要使用各種可用技術，能夠在這兩個頁面中傳遞物件資料，以便完成 CRUD 的應用。

在這個練習中，所有的 CRUD 之呼叫 Web API 行為，我們都撰寫在清單頁面的 ViewModel 中，而在明細資料頁面裡面，我們並不會去處理 CRUD 的相關 Web API 呼叫，而僅僅針對這單筆紀錄的異動做處理。

同樣的，只要使用者有查詢過工作日誌紀錄之後，我們會將結果快取到行動裝置的記憶卡內，下次再度進入到這個清單頁面，程式會將之前快取的資料取出來，顯示在螢幕上。

# 建立工作日誌清單畫面 頁面檢視 (View)與檢視模型(ViewModel)

* 滑鼠右擊 `Views` 資料夾，選擇 \[加入] > \[新增項目]

* 在 \[新增項目 LOBForm] 對話窗中，點選 \[已安裝] > \[Visual C# 項目] > \[Prism] > \[`Prism ContentPage (Xamarin.Forms)`]

  > 請注意，這裡需要選取 `Prism ContentPage (Xamarin.Forms)` 內容頁面 項目

* 在\[名稱] 欄位內，輸入 `WorkingLogPage`，之後點選 `新增` 按鈕

* 在 `Views` 資料夾內，打開 `WorkingLogPage.xaml`

* 使用底下 XAML 宣告標記語言，替換剛剛產生的頁面 XAML 內容

![](Icons/XAML.png)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:prism="clr-namespace:Prism.Mvvm;assembly=Prism.Forms"
             xmlns:behavior="clr-namespace:Prism.Behaviors;assembly=Prism.Forms"
             prism:ViewModelLocator.AutowireViewModel="True"
             x:Class="LOBForm.Views.WorkingLogPage"
             Title="工作日誌"
             x:Name="ThisPage">

    <ContentPage.ToolbarItems>
        <ToolbarItem 
            Text="新增"
            Command="{Binding AddCommand}">
        </ToolbarItem>
        <ToolbarItem 
            Text="更新"
            Command="{Binding DoRefreshCommand}">
        </ToolbarItem>
    </ContentPage.ToolbarItems>

    <Grid
        RowSpacing="0" ColumnSpacing="0"
        >
        <ListView
            ItemsSource="{Binding WorkingLogList}"
            SelectedItem="{Binding WorkingLogSelectedItem}"
            IsPullToRefreshEnabled="True"
            IsRefreshing="{Binding IsRefreshing}"
            RefreshCommand="{Binding DoRefreshCommand}"
            HasUnevenRows="True"
            SeparatorVisibility="None"
            >
            <ListView.Behaviors>
                <behavior:EventToCommandBehavior
                    EventName="ItemTapped"
                    Command="{Binding ItemTappedCommand}"/>
            </ListView.Behaviors>
            <ListView.ItemTemplate>
                <DataTemplate>
                    <ViewCell>
                        <Grid
                            RowSpacing="0" ColumnSpacing="0"
                            >
                            <Grid.RowDefinitions>
                                <RowDefinition Height="*"/>
                                <RowDefinition Height="*"/>
                            </Grid.RowDefinitions>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="100"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <BoxView
                                Grid.RowSpan="2" Grid.ColumnSpan="2"
                                Color="White"/>

                            <Label
                                Grid.Row="0" Grid.Column="1"
                                Margin="5,0"
                                Text="{Binding Title}"
                                FontSize="20"
                                TextColor="DarkGray"
                                />
                            <Label
                                Grid.Row="1" Grid.Column="1"
                                Margin="5,0"
                                Text="{Binding Project.ProjectName}"
                                FontSize="20"
                                TextColor="DarkGray"
                                />
                            <Label
                                Grid.Row="1" Grid.Column="1"
                                Margin="5,0"
                                HorizontalOptions="End"
                                Text="{Binding LogDate, StringFormat='{0:yyyy-MM-dd}'}"
                                TextColor="DarkGray"
                                FontSize="16"/>
                            <Grid
                                RowSpacing="0" ColumnSpacing="0"
                                Grid.Row="0" Grid.Column="0"
                                Grid.RowSpan="2"
                                >
                                <BoxView
                                    Margin="5,0,0,0"
                                    Color="ForestGreen"/>
                                <Label
                                    HorizontalOptions="Center" VerticalOptions="Center"
                                    Margin="5,0"
                                    Text="{Binding Hours, StringFormat='{0} 小時'}"
                                    TextColor="White"
                                    FontSize="20">
                                </Label>
                            </Grid>

                            <BoxView
                                Grid.RowSpan="2" Grid.ColumnSpan="2"
                                Margin="5,0"
                                HorizontalOptions="Fill" VerticalOptions="End"
                                HeightRequest="2"
                                Color="Orange"/>
                        </Grid>
                    </ViewCell>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
    </Grid>

</ContentPage>
```

* 在 `ViewModels` 資料夾內，打開 `WorkingLogPageViewModel.cs`

* 在這個 View Model 類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
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
```

* 使用底下 C# 程式碼，替換這個類別內容

![](Icons/csharp.png)

```csharp
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
```

# 建立工作日誌紀錄編修畫面 頁面檢視 (View)與檢視模型(ViewModel)

* 滑鼠右擊 `Views` 資料夾，選擇 \[加入] > \[新增項目]

* 在 \[新增項目 LOBForm] 對話窗中，點選 \[已安裝] > \[Visual C# 項目] > \[Prism] > \[`Prism ContentPage (Xamarin.Forms)`]

  > 請注意，這裡需要選取 `Prism ContentPage (Xamarin.Forms)` 內容頁面 項目

* 在\[名稱] 欄位內，輸入 `WorkingLogDetailPage`，之後點選 `新增` 按鈕

* 在 `Views` 資料夾內，打開 `WorkingLogDetailPage.xaml`

* 使用底下 XAML 宣告標記語言，替換剛剛產生的頁面 XAML 內容

![](Icons/XAML.png)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:prism="clr-namespace:Prism.Mvvm;assembly=Prism.Forms"
             prism:ViewModelLocator.AutowireViewModel="True"
             x:Class="LOBForm.Views.WorkingLogDetailPage"
             Title="工作日誌編輯">

    <Grid
        RowSpacing="0" ColumnSpacing="0"
        >
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="55"/>
        </Grid.RowDefinitions>

        <ScrollView
            Grid.Row="0"
            Margin="10"
           Orientation="Vertical"
            >
            <StackLayout
                Spacing="0"
                Orientation="Vertical"
                >
                <Label
                    Text="主題"
                    />
                <Entry
                    Text="{Binding WorkingLogItem.Title}"
                    />
                <Label
                    Text="摘要"
                    />
                <Editor
                    HeightRequest="200"
                    Text="{Binding WorkingLogItem.Summary}"
                    />
                <Label
                    Text="日期"
                    />
                <DatePicker
                    Format="yyyy-MM-dd"
                    Date="{Binding WorkingLogItem.LogDate, StringFormat='{0:yyyy-MM-dd}'}"
                    />
                <Label
                    Text="時間長度"
                    />
                <Entry
                    Text="{Binding WorkingLogItem.Hours}"
                    />
                <Label
                    Text="專案名稱"
                    />
                <Picker
                    ItemsSource="{Binding ProjectsSource}"
                    SelectedItem="{Binding ProjectSelectedItem}"
                    />
            </StackLayout>
        </ScrollView>
        <StackLayout
            Grid.Row="1"
            Spacing="0"
            Orientation="Vertical"
            >
            
            <!--儲存修改與刪除按鈕-->
            <Grid
                RowSpacing="0" ColumnSpacing="0"
                IsVisible="{Binding IsUpdaeRecordMode}"
                >
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="2*"/>
                </Grid.ColumnDefinitions>
                <Button
                    Grid.Column="0"
                    Text="刪除"
                    BackgroundColor="Red"
                    TextColor="White"
                    HeightRequest="50"
                    Command="{Binding DeleteCommand}"/>
                <Button
                    Grid.Column="1"
                    Text="儲存"
                    BackgroundColor="Green"
                    TextColor="White"
                    HeightRequest="50"
                    Command="{Binding SaveCommand}"/>
            </Grid>
            
            <!--新增紀錄使用的按鈕-->
            <Grid
                RowSpacing="0" ColumnSpacing="0"
                IsVisible="{Binding IsCreateRecordMode}"
                >
                <Button
                    Text="新增"
                    BackgroundColor="Green"
                    TextColor="White"
                    HeightRequest="50"
                    Command="{Binding AddCommand}"/>
            </Grid>

        </StackLayout>
    </Grid>
  
</ContentPage>
```

* 在 `ViewModels` 資料夾內，打開 `WorkingLogDetailPageViewModel.cs`

* 在 `ViewModels` 資料夾內，打開 `WorkingLogDetailPageViewModel.cs`

* 在這個 View Model 類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
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
```

* 使用底下 C# 程式碼，替換這個類別內容

![](Icons/csharp.png)

```csharp
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
```

# 問題研究

![](Icons/fa-question-circle30.png) 進入到工作日誌清單頁面，點選導航工具列上的更新按鈕，看看是否會取得最新的資料？

![](Icons/fa-question-circle30.png) 強制關閉這個 App，並且重新開啟，接著進入到工作日誌頁面，看看工作日誌集合資料，是否馬上就出現了？

![](Icons/fa-question-circle30.png) 進入到工作日誌清單頁面，實際新增一筆紀錄，看看是否會出現？

![](Icons/fa-question-circle30.png) 進入到工作日誌清單頁面，點選剛剛新增紀錄，接著修改內容，並且儲存之後，看看是否有更新呢？

![](Icons/fa-question-circle30.png) 進入到工作日誌清單頁面，點選剛剛修改紀錄，看看是否會被刪除？

![](Icons/fa-question-circle30.png) 當進行上面三個測試動作的時候，請同步確認後端資料庫是否也會同步進行資料表紀錄的異動？

![](Icons/fa-question-circle30.png) 請嘗試在不同行動平台下，都重覆執行一次這個專案，看看有甚麼差異？

![](Icons/fa-question-circle30.png) 我們利用 `ToolbarItem` 建立一個工具列按鈕，提供新增記錄功能；不過，請思考一下，如何做出具有浮動新增按鈕效果的功能？

![](Icons/fa-question-circle30.png) 當使用者在 ListView 中點選了某筆紀錄，我們要在 ViewModel 內，如何知道使用者點選了哪筆紀錄呢？

![](Icons/fa-question-circle30.png) 學習這個練習中，如何在不同頁面傳送物件資料的方法，這裡共有兩種技巧，一個是從 A 頁面導航到 B 頁面，將各種物件，從 A 頁面傳送到 B 頁面內；另外一個是，當要從 B 頁面返回到 A 頁面，如何將 B 頁面中的相關物件，傳遞回 A 頁面？

![](Icons/fa-question-circle30.png) 當我們要修改紀錄的時候，為什麼要把點選紀錄物件，使用 `WorkingLogSelectedItem.Clone()` 來深層複製成為另外一個物件，接著才要傳送到工作日誌的明細頁面呢？

![](Icons/fa-question-circle30.png) `NavigationParameters.InternalParameters` 這個物件是甚麼？為什麼我們要在 `OnNavigatedTo` 事件內使用到它呢？

![](Icons/fa-question-circle30.png) 學習了解 `NavigationParameters` 這個類別的用法。

![](Icons/fa-question-circle30.png) 在查詢、新增、修改、刪除的 Web API 呼叫方法內，都有使用到 `new CancellationTokenSource(10000);` 這樣的物件，是要幫助我們達成甚麼樣的效果嗎？

![](Icons/fa-question-circle30.png) 同樣的，當我們在呼叫 Web API 的時候，當回傳結果之後，都會執行 `MainHelper.CheckAccessToken()` 方法，它是做甚麼用途，是要解決甚麼問題呢？

![](Icons/fa-question-circle30.png) 請了解這個練習中，關於 CRUD 應用的相關程式碼作法。

![](Icons/fa-question-circle30.png) 在工作日誌紀錄編修頁面，我們要如何判斷此次的紀錄異動是屬於 新增 還是 修改？

![](Icons/fa-question-circle30.png) 工作日誌紀錄編修頁面，由於紀錄的新增與修改，會使用到不同的按鈕(這些按鈕又會使用不同的版面配置)，我們要如何做到可以動態的根據當時紀錄的狀態，確切地顯示出適當的按鈕。

![](Icons/fa-question-circle30.png) 工作日誌紀錄編修頁面，我們需要選取相對應的專案名稱，而這個專案名稱清單，我們在應用程式啟動的時候，已經透過 ProjectReporitory 物件取回並儲存到手機上了，我們要如何讓使用者可以使用下拉選單的方式，選取適當的專案名稱呢？

![](Icons/fa-question-circle30.png) 若在修改紀錄模式，我們要如何設訂下拉選單的預設選項項目。

![](Icons/fa-question-circle30.png) 在新增或者修改紀錄時候，若我們輸入了不正確的資料欄位值，我們要如何避免、檢查、排除這樣的問題發生了？(後端/Xamarin.Forms)。


