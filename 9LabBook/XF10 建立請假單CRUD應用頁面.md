# XF10 建立請假單CRUD應用頁面

然後，我們要開始練習如何在 Xamarin.Forms 專案內，撰寫請假單 CRUD 應用，同樣的，我們一樣需要設計兩個頁面，一個是顯示所有集合紀錄的頁面，另外一個是要顯示特定紀錄的詳細頁面；我們需要使用各種可用技術，能夠在這兩個頁面中傳遞物件資料，以便完成 CRUD 的應用。

在這個練習中，所有的 CRUD 之呼叫 Web API 行為，我們都撰寫在清單頁面的 ViewModel 中，而在明細資料頁面裡面，我們並不會去處理 CRUD 的相關 Web API 呼叫，而僅僅針對這單筆紀錄的異動做處理。

同樣的，只要使用者有查詢過請假單紀錄之後，我們會將結果快取到行動裝置的記憶卡內，下次再度進入到這個清單頁面，程式會將之前快取的資料取出來，顯示在螢幕上。

![](Icons/fa-search.png) 這份文件的完成結果專案原始碼，可以參考 `XamarinLOBForms\2XFLab\10LAF`

# 建立請假單畫面 頁面檢視 (View)與檢視模型(ViewModel)

* 滑鼠右擊 `Views` 資料夾，選擇 \[加入] > \[新增項目]

* 在 \[新增項目 LOBForm] 對話窗中，點選 \[已安裝] > \[Visual C# 項目] > \[Prism] > \[`Prism ContentPage (Xamarin.Forms)`]

  > 請注意，這裡需要選取 `Prism ContentPage (Xamarin.Forms)` 內容頁面 項目

* 在\[名稱] 欄位內，輸入 `LeaveAppFormPage`，之後點選 `新增` 按鈕

* 在 `Views` 資料夾內，打開 `LeaveAppFormPage.xaml`

* 使用底下 XAML 宣告標記語言，替換剛剛產生的頁面 XAML 內容

![](Icons/XAML.png)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:prism="clr-namespace:Prism.Mvvm;assembly=Prism.Forms"
             xmlns:behavior="clr-namespace:Prism.Behaviors;assembly=Prism.Forms"
             prism:ViewModelLocator.AutowireViewModel="True"
             x:Class="LOBForm.Views.LeaveAppFormPage"
             Title="請假單"
             x:Name="ThisPage"
             >

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
            ItemsSource="{Binding LeaveAppFormList}"
            SelectedItem="{Binding LeaveAppFormSelectedItem}"
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
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="100"/>
                            </Grid.ColumnDefinitions>
                            <BoxView
                                Grid.RowSpan="2" Grid.ColumnSpan="2"
                                Color="White"/>

                            <Label
                                Grid.Row="0" Grid.Column="0"
                                Margin="5,0"
                                Text="{Binding Category}"
                                FontSize="20"
                                TextColor="Black"
                                />

                            <StackLayout
                                Grid.Row="1" Grid.Column="0"
                                Spacing="0"
                                Orientation="Horizontal"
                                >
                                <Label
                                    Margin="5,0,0,0"
                                    Text="{Binding BeginDate, StringFormat='{0:yyyy-MM-dd hh:mm} ~ '}"
                                    FontSize="16"
                                    TextColor="DarkGray"
                                    />
                                <Label
                                    Margin="0,0,5,0"
                                    Text="{Binding CompleteDate, StringFormat='{0:yyyy-MM-dd hh:mm}'}"
                                    FontSize="16"
                                    TextColor="DarkGray"
                                    />
                            </StackLayout>

                            <Label
                                Grid.Row="0" Grid.Column="1"
                                Margin="5,0"
                                Text="{Binding FormsStatus}"
                                FontSize="20"
                                TextColor="Brown"
                                />

                            <Label
                                Grid.Row="1" Grid.Column="1"
                                Margin="5,0"
                                Text="{Binding ApproveResult}"
                                FontSize="20"
                                TextColor="Red"
                                />

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

* 在 `ViewModels` 資料夾內，打開 `LeaveAppFormPageViewModel.cs`

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
public class LeaveAppFormPageViewModel : INotifyPropertyChanged, INavigationAware
{
    public event PropertyChangedEventHandler PropertyChanged;
    public LeaveAppForm LeaveAppFormSelectedItem { get; set; }
    public ObservableCollection<LeaveAppForm> LeaveAppFormList { get; set; } = new ObservableCollection<LeaveAppForm>();
    public bool IsRefreshing { get; set; } = false;
    private LeaveCategoryRepository fooLeaveCategoryRepository = new LeaveCategoryRepository();
    public DelegateCommand DoRefreshCommand { get; set; }
    public DelegateCommand AddCommand { get; set; }
    public DelegateCommand ItemTappedCommand { get; set; }
    private readonly INavigationService _navigationService;
 
    public LeaveAppFormPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        AddCommand = new DelegateCommand(async () =>
        {
            var fooItem = new LeaveAppForm()
            {
                BeginDate = DateTime.Now.Date.AddDays(1),
                CompleteDate = DateTime.Now.Date.AddDays(2),
                FormDate = DateTime.Now
            };
            NavigationParameters fooPara = new NavigationParameters();
            fooPara.Add(MainHelper.CRUDItemKeyName, fooItem);
            fooPara.Add(MainHelper.CRUDKeyName, MainHelper.CRUD_Create);
 
            await _navigationService.NavigateAsync("LeaveAppFormDetailPage", fooPara);
        });
        ItemTappedCommand = new DelegateCommand(async () =>
        {
            NavigationParameters fooPara = new NavigationParameters();
            fooPara.Add(MainHelper.CRUDItemKeyName, LeaveAppFormSelectedItem.Clone());
            fooPara.Add(MainHelper.CRUDKeyName, MainHelper.CRUD_Update);
 
            await _navigationService.NavigateAsync("LeaveAppFormDetailPage", fooPara);
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
                    var fooItem = parameters[MainHelper.CRUDItemKeyName] as LeaveAppForm;
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
 
    private async Task RefreshCache()
    {
        var fooLeaveAppFormRepository = new LeaveAppFormRepository();
        await fooLeaveAppFormRepository.ReadAsync(MainHelper.LeaveAppFormUserMode);
        LeaveAppFormList.Clear();
        foreach (var item in fooLeaveAppFormRepository.Items)
        {
            LeaveAppFormList.Add(item);
        }
    }
 
    public async Task RetriveRecords()
    {
        #region 進行請假單清單更新
        APIResult fooResult;
        IsRefreshing = true;
        var fooProgressDialogConfig = new ProgressDialogConfig()
        {
            Title = "請稍後，正在進行請假單清單更新中...",
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
                Mode = MainHelper.LeaveAppFormUserMode
            });
 
            if (fooResult.Success == false)
            {
                if(await MainHelper.CheckAccessToken(fooResult) == false)
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
 
    public async Task CreateRecord(LeaveAppForm leaveAppForm)
    {
        #region 進行請假單新增
        APIResult fooResult;
        IsRefreshing = true;
        var fooProgressDialogConfig = new ProgressDialogConfig()
        {
            Title = "請稍後，正在進行請假單清單新增中...",
            IsDeterministic = false,
        };
        using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
        {
            var fooLeaveAppFormRepository = new LeaveAppFormRepository();
            fooResult = await fooLeaveAppFormRepository.PostAsync(leaveAppForm);
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
 
    public async Task DeleteRecord(LeaveAppForm leaveAppForm)
    {
        #region 進行請假單刪除
        APIResult fooResult;
        IsRefreshing = true;
        var fooProgressDialogConfig = new ProgressDialogConfig()
        {
            Title = "請稍後，正在進行請假單清單刪除中...",
            IsDeterministic = false,
        };
        using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
        {
            var fooLeaveAppFormRepository = new LeaveAppFormRepository();
            fooResult = await fooLeaveAppFormRepository.DeleteAsync(leaveAppForm);
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
 
    public async Task UpdateRecord(LeaveAppForm leaveAppForm)
    {
        #region 進行請假單清單更新
        APIResult fooResult;
        IsRefreshing = true;
        var fooProgressDialogConfig = new ProgressDialogConfig()
        {
            Title = "請稍後，正在進行請假單清單更新中...",
            IsDeterministic = false,
        };
        using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
        {
            var fooLeaveAppFormRepository = new LeaveAppFormRepository();
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

# 建立請假單紀錄編修畫面 頁面檢視 (View)與檢視模型(ViewModel)

* 滑鼠右擊 `Views` 資料夾，選擇 \[加入] > \[新增項目]

* 在 \[新增項目 LOBForm] 對話窗中，點選 \[已安裝] > \[Visual C# 項目] > \[Prism] > \[`Prism ContentPage (Xamarin.Forms)`]

  > 請注意，這裡需要選取 `Prism ContentPage (Xamarin.Forms)` 內容頁面 項目

* 在\[名稱] 欄位內，輸入 `LeaveAppFormDetailPage`，之後點選 `新增` 按鈕

* 在 `Views` 資料夾內，打開 `LeaveAppFormDetailPage.xaml`

* 使用底下 XAML 宣告標記語言，替換剛剛產生的頁面 XAML 內容

![](Icons/XAML.png)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:prism="clr-namespace:Prism.Mvvm;assembly=Prism.Forms"
             prism:ViewModelLocator.AutowireViewModel="True"
             x:Class="LOBForm.Views.LeaveAppFormDetailPage"
             Title="請假單編輯"
             >

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
                    Text="假別"
                    />
                <Picker
                    ItemsSource="{Binding LeaveCategoriesSource}"
                    SelectedItem="{Binding LeaveCategorySelectedItem}"
                    />
                <Label
                    Text="開始日期"
                    />
                <DatePicker
                    Format="yyyy-MM-dd"
                    Date="{Binding LeaveAppFormItem.BeginDate, StringFormat='{0:yyyy-MM-dd}'}"
                    />
                <Label
                    Text="開始時間"
                    />
                <TimePicker
                    Time="{Binding BeginTime}"
                    />
                <Label
                    Text="結束日期"
                    />
                <DatePicker
                    Format="yyyy-MM-dd"
                    Date="{Binding LeaveAppFormItem.CompleteDate, StringFormat='{0:yyyy-MM-dd}'}"
                    />
                <Label
                    Text="結束時間"
                    />
                <TimePicker
                    Time="{Binding CompleteTime}"
                    />
                <Label
                    Text="請假原因"
                    />
                <Editor
                    HeightRequest="200"
                    Text="{Binding LeaveAppFormItem.LeaveCause}"
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

* 在 `ViewModels` 資料夾內，打開 `LeaveAppFormDetailPageViewModel.cs`

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
            NavigationParameters fooPara = new NavigationParameters();
            fooPara.Add(MainHelper.CRUDItemKeyName, LeaveAppFormItem);
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
            LeaveAppFormItem = parameters[MainHelper.CRUDItemKeyName] as LeaveAppForm;
 
            BeginTime = LeaveAppFormItem.BeginDate.TimeOfDay;
            CompleteTime = LeaveAppFormItem.CompleteDate.TimeOfDay;
        }
 
        await LoadLeaveCategory();
 
        #region 設定專案清單的預設項目
        LeaveCategorySelectedItem = LeaveAppFormItem.Category;
        #endregion
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
}
```

# 問題研究

![](Icons/fa-question-circle30.png) 進入到請假單清單頁面，點選導航工具列上的更新按鈕，看看是否會取得最新的資料？

![](Icons/fa-question-circle30.png) 強制關閉這個 App，並且重新開啟，接著進入到請假單頁面，看看請假單集合資料，是否馬上就出現了？

![](Icons/fa-question-circle30.png) 進入到請假單清單頁面，實際新增一筆紀錄，看看是否會出現？

![](Icons/fa-question-circle30.png) 進入到請假單清單頁面，點選剛剛新增紀錄，接著修改內容，並且儲存之後，看看是否有更新呢？

![](Icons/fa-question-circle30.png) 進入到請假單清單頁面，點選剛剛修改紀錄，看看是否會被刪除？

![](Icons/fa-question-circle30.png) 當進行上面三個測試動作的時候，請同步確認後端資料庫是否也會同步進行資料表紀錄的異動？

![](Icons/fa-question-circle30.png) 請嘗試在不同行動平台下，都重覆執行一次這個專案，看看有甚麼差異？

![](Icons/fa-question-circle30.png) 為何請假時間無法做修正，該如何進行這個問題的除錯與解決此一問題呢？

![](Icons/fa-question-circle30.png) 在 ViewModel 中，關於 CRUD 處理的程式碼，造成 ViewModel 擁有過多的程式碼，使得 ViewModel 較難維護，有沒有可以讓 ViewModel 變成比較清爽的程式設計方法？

![](Icons/fa-question-circle30.png) 在這整套練習中，我們要如何確認，當使用者登入 App 之後，這個 App 只能夠存取這個使用者登入認證後，與這個帳號有關的紀錄呢？我們要如何修正後端 Web API 程式碼，讓整個城市更加的安全、有效呢？

![](Icons/fa-question-circle30.png) 在這份練習中，對於請假單的紀錄編修，我們並沒有更詳盡的資料完整性檢查，例如：請假開始時間不可遠於請假結束時間、某些請假類型，最少請假時間單位為一天等等，您可以試著修改這部分，讓這個專案更加完整。



