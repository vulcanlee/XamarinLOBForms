# XF8 建立企業緊急電話清單頁面

![](Icons/fa-search.png) 這份文件的完成結果專案原始碼，可以參考 `XamarinLOBForms\2XFLab\08OnCall`

我們開始要把從資料庫內讀取出來的資料，顯示在螢幕畫面上，並且讓使用者與其進行互動操作；因為在這裡要顯示的資料，我們在應用程式啟動的時候，就已經從後端 Web API 取得了，並且快取到行動裝置記憶卡中，因此，在這個練習中，我將會從記憶卡中把資料讀取出來。

# 建立企業緊急電話清單畫面 頁面檢視 (View)與檢視模型(ViewModel)

* 滑鼠右擊 `Views` 資料夾，選擇 \[加入] > \[新增項目]

* 在 \[新增項目 LOBForm] 對話窗中，點選 \[已安裝] > \[Visual C# 項目] > \[Prism] > \[`Prism ContentPage (Xamarin.Forms)`]

  > 請注意，這裡需要選取 `Prism ContentPage (Xamarin.Forms)` 內容頁面 項目

* 在\[名稱] 欄位內，輸入 `OnCallPage`，之後點選 `新增` 按鈕

* 在 `Views` 資料夾內，打開 `OnCallPage.xaml`

* 使用底下 XAML 宣告標記語言，替換剛剛產生的頁面 XAML 內容

![](Icons/XAML.png)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:prism="clr-namespace:Prism.Mvvm;assembly=Prism.Forms"
             prism:ViewModelLocator.AutowireViewModel="True"
             x:Class="LOBForm.Views.OnCallPage"
             Title="各部門緊急電話清單"
             x:Name="ThisPage"
             >

    <ContentPage.ToolbarItems>
        <ToolbarItem 
            Text="更新"
            Command="{Binding DoRefreshCommand}"/>
    </ContentPage.ToolbarItems>

    <Grid
        RowSpacing="0" ColumnSpacing="0"
        >
        <ListView
            ItemsSource="{Binding OnCallPhoneList}"
            IsPullToRefreshEnabled="True"
            IsRefreshing="{Binding IsRefreshing}"
            RefreshCommand="{Binding DoRefreshCommand}"
            HasUnevenRows="True"
            >
            <ListView.ItemTemplate>
                <DataTemplate>
                    <ViewCell>
                        <Grid
                            RowSpacing="0" ColumnSpacing="0"
                            >
                            <Grid.RowDefinitions>
                                <RowDefinition Height="2*"/>
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
                                Text="{Binding Title}"
                                FontSize="20"
                                TextColor="DarkGray"
                                />
                            <Label
                                Grid.Row="1" Grid.Column="0"
                                Margin="5,0"
                                Text="{Binding PhoneNumber}"
                                TextColor="DarkGray"
                                FontSize="16"/>
                            <Grid
                                RowSpacing="0" ColumnSpacing="0"
                                Grid.Row="0" Grid.Column="1"
                                Grid.RowSpan="2"
                                >
                                <BoxView
                                    Margin="0,0,5,0"
                                    Color="ForestGreen"/>
                                <Label
                                    HorizontalOptions="Center" VerticalOptions="Center"
                                    Margin="5,5"
                                    Text="打電話"
                                    TextColor="White"
                                    FontSize="24">
                                    <Label.GestureRecognizers>
                                        <TapGestureRecognizer
                                            Command="{Binding BindingContext.CallPhoneCommand, Source={x:Reference ThisPage}}"
                                            CommandParameter="{Binding .}"/>
                                    </Label.GestureRecognizers>
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

* 在 `ViewModels` 資料夾內，打開 `OnCallPageViewModel.cs`

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
```

* 使用底下 C# 程式碼，替換這個類別內容

![](Icons/csharp.png)

```csharp
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
```

# 問題研究

![](Icons/fa-question-circle30.png) 請點選撥打電話按鈕，會發生甚麼情況。

![](Icons/fa-question-circle30.png) 找一台沒有電話功能的平板(或者可以使用模擬器)，請點選撥打電話按鈕，會發生甚麼情況。

![](Icons/fa-question-circle30.png) 為什麼要另外設計一個工具列按鈕，這個按鈕是要來更新資料，ListView 不是有支援手勢更新功能嗎？

![](Icons/fa-question-circle30.png) 學習如何使用 `ListView` 來顯示集合紀錄方面的資料，以及 `ListView` 這個控制項經常會用到的屬性使用方式；最後，透過這個練習，孰悉如何安排與顯示每筆紀錄要顯示 XAML 宣告標記語言的用法。

![](Icons/fa-question-circle30.png) 在打電話的文字的點擊手勢辨識器屬性項目 (`TapGestureRecognize`) 中，使用到了這樣的 `Source={x:Reference ThisPage}` XAML，這代表甚麼意思，為什麼要這麼做呢？

![](Icons/fa-question-circle30.png) 學習如何在 `ListView` 的 `ViewCell` 內，自行設定要產生紀錄分隔線的控制項，而不是使用 `ListView` 預設提供的。

![](Icons/fa-question-circle30.png) 這個練習中，ListView 所顯示的內容，具有 RWD 能力，可以試著旋轉螢幕(或者在不同規格的手機裝置或者模擬器)，看看不同螢幕配置下，所產生的效果；也試著了解如何做到這樣的設計方式。

![](Icons/fa-question-circle30.png) 在 ViewModel 中，關於要在 ListView 控制項內顯示的資料，我們使用了 `ObservableCollection` 類別來定義；若我們使用了 `List` 這個類別，會達到同樣的效果嗎？那麼，這兩個類別有甚麼差異呢？

![](Icons/fa-question-circle30.png) 在 `private async Task Refresh()` 方法中，我們將取得的公司緊急聯絡電話紀錄，一筆一筆的加入到 `OnCallPhoneList` 物件內，有沒有更有效率的做法呢？

![](Icons/fa-question-circle30.png) 我們要如何設定，才能夠讓用者觸發了手勢更新機制，當使用者觸發了手勢更新功能，我們要在 ViewModel 內，如何更新到最新的資料呢？


