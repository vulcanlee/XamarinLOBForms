# XF7 建立導航抽屜與關於頁面

![](Icons/fa-search.png) 這份文件的完成結果專案原始碼，可以參考 `XamarinLOBForms\2XFLab\07NaviDrawer`

我們終於要開始設計這個 App的首頁了，這個首頁頁面相當的簡單，只有一張圖片，可是在這個練習中，主要是要練習如何設計出一個導航抽屜的畫面與相關商業邏輯；使用者可以透過導航抽屜的操作，切換到不同的頁面來進行更多功能的操作，另外，也可以點選登出功能，進行登出作業。

# 建立關於 頁面檢視 (View)與檢視模型(ViewModel)

* 滑鼠右擊 `Views` 資料夾，選擇 \[加入] > \[新增項目]

* 在 \[新增項目 LOBForm] 對話窗中，點選 \[已安裝] > \[Visual C# 項目] > \[Prism] > \[`Prism ContentPage (Xamarin.Forms)`]

  > 請注意，這裡需要選取 `Prism ContentPage (Xamarin.Forms)` 內容頁面 項目

* 在\[名稱] 欄位內，輸入 `AboutPage`，之後點選 `新增` 按鈕

* 在 `Views` 資料夾內，打開 `AboutPage.xaml`

* 使用底下 XAML 宣告標記語言，替換剛剛產生的頁面 XAML 內容

![](Icons/XAML.png)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:prism="clr-namespace:Prism.Mvvm;assembly=Prism.Forms"
             prism:ViewModelLocator.AutowireViewModel="True"
             x:Class="LOBForm.Views.AboutPage"
             Title="關於"
             BackgroundColor="#FFA861">

    <Grid
        RowSpacing="0" ColumnSpacing="0"
        >
        <Grid.RowDefinitions>
            <RowDefinition Height="150"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <Image
            HorizontalOptions="Center" VerticalOptions="Center"
            HeightRequest="130" WidthRequest="130"
            Aspect="Fill">
            <!--因為每個平台放置圖片的路徑不同，因此，使用 OnPlatform 來設定不同平台下的不同屬性值-->
            <Image.Source>
                <OnPlatform x:TypeArguments="ImageSource"
                            Android="Logo.png"
                            iOS="Logo.png"
                            WinPhone="Assets/Images/Logo.png"
                            />
            </Image.Source>
        </Image>

        <Grid
            Grid.Row="1"
            RowSpacing="0" ColumnSpacing="0"
            >
            <Label
                HorizontalOptions="Center" VerticalOptions="Center"
                Margin="20,0"
                FontSize="48"
                TextColor="White"
                Opacity="0.5"
                HorizontalTextAlignment="Center"
                Text="Xamarin.Forms 跨平台行動開發一日實戰營"/>
        </Grid>
    </Grid>
  
</ContentPage>
```

# 修正導航抽屜 頁面檢視 (View)與檢視模型(ViewModel)

* 在 `Views` 資料夾，打開 `MDPage.xaml`

* 找到這個 `<Grid></Grid>` XAML 語言，使用底下 XAML 宣告標記語言，替換 `<Grid></Grid>` XAML 內容

![](Icons/XAML.png)

```xml
            <Grid
                RowSpacing="0" ColumnSpacing="0"
                >
                <Grid.RowDefinitions>
                    <RowDefinition Height="150"/>
                    <RowDefinition Height="*"/>
                </Grid.RowDefinitions>

                <!--這裡宣告最上方的使用者登入資訊與公司名稱的樣貌-->
                <Grid
                    RowSpacing="0" ColumnSpacing="0">
                    <BoxView
                        Color="#FCB515"/>
                    <Label
                        Text="{Binding UserName, StringFormat='使用者名稱：{0}'}"
                        TextColor="White"
                        Opacity="0.5"
                        FontSize="Medium"
                        Margin="10,20,0,0"
                        HorizontalOptions="Start" VerticalOptions="Start"
                        />
                    <Label
                        Text="Xamarin 實驗室"
                        TextColor="Yellow"
                        Opacity="0.8"
                        FontSize="Large"
                        Margin="10,0,0,20"
                        HorizontalOptions="Start" VerticalOptions="End"
                        />
                </Grid>

                <!--這裡使用捲動面板配置，宣告各種可以使用的功能-->
                <Grid
                    Grid.Row="1"
                    RowSpacing="0" ColumnSpacing="0"
                    BackgroundColor="#edd193"
                    HorizontalOptions="Fill" VerticalOptions="FillAndExpand"
                    >
                    <ScrollView
                        Orientation="Vertical"
                        >
                        <StackLayout
                            Margin="10,20"
                            Spacing="0"
                            Orientation="Vertical">

                            <Label
                                Margin="20,20"
                                HorizontalOptions="Start" VerticalOptions="Center"
                                Text="工作日誌"
                                FontSize="24"
                                TextColor="Black"
                                Opacity="0.7">
                                <Label.GestureRecognizers>
                                    <TapGestureRecognizer
                                        Command="{Binding MenuCommand}"
                                        CommandParameter="工作日誌"/>
                                </Label.GestureRecognizers>
                            </Label>

                            <Label
                                Margin="20,20"
                                HorizontalOptions="Start" VerticalOptions="Center"
                                Text="請假單"
                                FontSize="24"
                                TextColor="Black"
                                Opacity="0.7">
                                <Label.GestureRecognizers>
                                    <TapGestureRecognizer
                                        Command="{Binding MenuCommand}"
                                        CommandParameter="請假單"/>
                                </Label.GestureRecognizers>
                            </Label>

                            <Label
                                Margin="20,20"
                                HorizontalOptions="Start" VerticalOptions="Center"
                                Text="請假單審核"
                                FontSize="24"
                                TextColor="Black"
                                Opacity="0.7"
                                IsVisible="{Binding IsManager}">
                                <Label.GestureRecognizers>
                                    <TapGestureRecognizer
                                        Command="{Binding MenuCommand}"
                                        CommandParameter="請假單審核"/>
                                </Label.GestureRecognizers>
                            </Label>

                            <Label
                                Margin="20,20"
                                HorizontalOptions="Start" VerticalOptions="Center"
                                Text="緊急電話清單"
                                FontSize="24"
                                TextColor="Black"
                                Opacity="0.7">
                                <Label.GestureRecognizers>
                                    <TapGestureRecognizer
                                        Command="{Binding MenuCommand}"
                                        CommandParameter="緊急電話清單"/>
                                </Label.GestureRecognizers>
                            </Label>

                            <Label
                                Margin="20,20"
                                HorizontalOptions="Start" VerticalOptions="Center"
                                Text="關  於"
                                FontSize="24"
                                TextColor="Black"
                                Opacity="0.7">
                                <Label.GestureRecognizers>
                                    <TapGestureRecognizer
                                        Command="{Binding MenuCommand}"
                                        CommandParameter="關  於"/>
                                </Label.GestureRecognizers>
                            </Label>

                            <Label
                                Margin="20,20"
                                HorizontalOptions="Start" VerticalOptions="Center"
                                Text="登  出"
                                FontSize="24"
                                TextColor="Black"
                                Opacity="0.7">
                                <Label.GestureRecognizers>
                                    <TapGestureRecognizer
                                        Command="{Binding MenuCommand}"
                                        CommandParameter="登  出"/>
                                </Label.GestureRecognizers>
                            </Label>
                        </StackLayout>
                    </ScrollView>
                </Grid>
            </Grid>
```

* 在 `ViewModels` 資料夾，打開 `MDPageViewModel.cs`

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using Acr.UserDialogs;
using LOBForm.Helpers;
using LOBForm.Repositories;
using Prism.Navigation;
using Prism.Services;
using System.ComponentModel;
```

* 使用底下程式碼，替換這個檢視模型 ViewModel 類別

![](Icons/csharp.png)

```csharp
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
```

# 修正導航抽屜 頁面檢視 (View)與檢視模型(ViewModel)

* 在 `ViewModels` 資料夾，打開 `SplashPageViewModel.cs`

* 找到這個 `//await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/AboutPage");` 將這行程式碼解除註解，底下是修正完成後的程式碼

![](Icons/csharp.png)

```csharp
if (fooAllSuccess == true)
{
    LoadingMessage = "系統資料更新完成";
    var fooSystemStatusRepository = new SystemStatusRepository();
    await fooSystemStatusRepository.ReadAsync();
    if (string.IsNullOrEmpty(fooSystemStatusRepository.Item.AccessToken))
    {
        await _navigationService.NavigateAsync("xf:///LoginPage");
    }
    else
    {
        await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/AboutPage");
    }
}
```

# 進行跨平台實際測試

* 請在不同平台下執行這個 App，輸入不正確的帳號與密碼，看看有甚麼反應，還是一樣可以正常登入到關於首頁中。

* 請在不同平台下執行這個 App，輸入正確的帳號與密碼，看看是否可以正常登入到關於首頁中。

* 請在不同平台下執行這個 App，當進入到 App 首頁之後，請試著操作與顯示導航抽屜頁面，看看抽屜的內容。

# 問題研究

![](Icons/fa-question-circle30.png) 我們在 `MDPage.xaml` 頁面中加入許多 XAML 項目，其實，我們只是為了要做一件事情，是甚麼事情呢？

![](Icons/fa-question-circle30.png) 為什麼我們都要在 `Grid` 這個版面配置項目中，指定這個 `RowSpacing="0" ColumnSpacing="0"` 屬性設定呢？這有甚麼特殊目的嗎？

![](Icons/fa-question-circle30.png) 同樣的，在 `StackLayou` 版面配置中，同樣的也有設定 `Spacing="0"` 這個屬性設定，是有同樣的目的嗎？

![](Icons/fa-question-circle30.png) 在導航抽屜中，我們規劃了許多功能選項，可是在導航抽屜的檢視模型 `MDPageViewModel` 中，卻只有一個命令存在，我們是要如何區隔，是哪個導航抽屜的項目被使用者選擇了呢？

![](Icons/fa-question-circle30.png) 為什麼在導航抽屜的導航頁面方法引數字串內，我們都要使用 `xf:///` 這個字串作為開頭呢？

![](Icons/fa-question-circle30.png) 當使用者要進行登出作業的時候，為什麼我們要做清空本機資料快取的動作呢？

![](Icons/fa-question-circle30.png) 在 ViewModel中，我們要如何取得使用者輸入的帳號與密碼呢？

![](Icons/fa-question-circle30.png) 





