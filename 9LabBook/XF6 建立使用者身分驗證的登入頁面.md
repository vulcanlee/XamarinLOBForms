# XF6 建立使用者身分驗證的登入頁面

![](Icons/fa-search.png) 這份文件的完成結果專案原始碼，可以參考 `XamarinLOBForms\2XFLab\06Login`

我們需要來為這個 App 做安全性的把關，那就是無法通過身分驗證的使用者，是無法來使用與操作這個 App；因此，我們設計了這個使用者登入頁面，讓使用者輸入帳號與密碼，並且透過呼叫遠端 Web API 來檢查這組帳密，是否為合法的使用者。一旦確認該使用者為合法使用者，我們就需要把存取權杖永久保存在手機中，當我們要呼叫其他的 Web API 的時候，就可以讀取出來使用了。

# 建立使用者身分驗證的登入 頁面檢視 (View)與檢視模型(ViewModel)

* 滑鼠右擊 `Views` 資料夾，選擇 \[加入] > \[新增項目]

* 在 \[新增項目 LOBForm] 對話窗中，點選 \[已安裝] > \[Visual C# 項目] > \[Prism] > \[`Prism ContentPage (Xamarin.Forms)`]

  > 請注意，這裡需要選取 `Prism ContentPage (Xamarin.Forms)` 內容頁面 項目

* 在\[名稱] 欄位內，輸入 `LoginPage`，之後點選 `新增` 按鈕

* 在 `Views` 資料夾內，打開 `LoginPage.xaml`

* 使用底下 XAML 宣告標記語言，替換剛剛產生的頁面 XAML 內容

![](Icons/XAML.png)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:prism="clr-namespace:Prism.Mvvm;assembly=Prism.Forms"
             prism:ViewModelLocator.AutowireViewModel="True"
             x:Class="LOBForm.Views.LoginPage"
             BackgroundColor="#FCB515">

    <Grid
        RowSpacing="0" ColumnSpacing="0"
        >
        <Grid.RowDefinitions>
            <RowDefinition Height="1*"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="50"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!--輸入帳號與密碼的 UI-->
        <Grid
            Grid.Row="1"
            Margin="20,0,20,0"
            RowSpacing="0" ColumnSpacing="0"
            >
            <Grid.RowDefinitions>
                <RowDefinition Height="70"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="30"/>
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="60"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <BoxView
                Grid.Row="0" Grid.Column="0"
                Grid.RowSpan="4" Grid.ColumnSpan="2"
                Color="White"
                Opacity="0.2"/>
            
            <!--輸入帳號與密碼的標題-->
            <!--<BoxView
                Grid.Row="0" Grid.Column="0"
                Grid.ColumnSpan="2"
                Color="LightBlue"
                Opacity="0.6"/>-->
            <Label
                Grid.ColumnSpan="2"
                HorizontalOptions="Center" VerticalOptions="Center"
                Text="請輸入帳號與密碼"
                FontSize="28"
                TextColor="ForestGreen"
                />
            
            <!--帳號UI-->
            <Label
                Grid.Row="1" Grid.Column="0"
                HorizontalOptions="End" VerticalOptions="Center"
                FontSize="20"
                Text="帳號"/>
            <Entry
                Grid.Row="1" Grid.Column="1"
                Margin="10,0"
                HorizontalOptions="Fill" VerticalOptions="Center"
                Placeholder="請輸入員工代號"
                Text="{Binding Account}"
                />

            <!--密碼UI-->
            <Label
                Grid.Row="2" Grid.Column="0"
                HorizontalOptions="End" VerticalOptions="Center"
                FontSize="20"
                Text="密碼"/>
            <Entry
                Grid.Row="2" Grid.Column="1"
                Margin="10,0"
                HorizontalOptions="Fill" VerticalOptions="Center"
                Placeholder="請輸入密碼"
                IsPassword="True"
                Text="{Binding Password}"
                />

        </Grid>
        
        <!--按鈕與選項 UI-->
        <Grid
            Grid.Row="3"
            RowSpacing="0" ColumnSpacing="0"
            >
            <StackLayout
                Spacing="0"
                Margin="40,0"
                Orientation="Vertical"
                >

                <Grid
                    RowSpacing="0" ColumnSpacing="0"
                    >
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*"/>
                        <ColumnDefinition Width="Auto"/>
                    </Grid.ColumnDefinitions>
                    <Label
                        Grid.Column="0"
                        HorizontalOptions="Start" VerticalOptions="Center"
                        Text="使用 Get 方式進行身分驗證"
                        />
                    <Switch
                        Grid.Column="1"
                        IsToggled="{Binding UsingHttpGet}"/>
                </Grid>
                
                <Button
                    Margin="0,40,0,0"
                    Text="登入"
                    Command="{Binding LoginCommand}"/>
            </StackLayout>
        </Grid>
    </Grid>
</ContentPage>
```

* 在 `ViewModels` 資料夾內，打開 `LoginPageViewModel.cs`

* 在這個 View Model 類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using Acr.UserDialogs;
using LOBForm.Models;
using LOBForm.Repositories;
using Prism.Navigation;
using System.ComponentModel;```

* 使用底下 C# 程式碼，替換這個類別內容

![](Icons/csharp.png)

```csharp
    public class LoginPageViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Account { get; set; } = "";
        public string Password { get; set; } = "";
        public bool UsingHttpGet { get; set; } = true;

        public DelegateCommand LoginCommand { get; set; }


        private readonly INavigationService _navigationService;

        public LoginPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            LoginCommand = new DelegateCommand(async () =>
            {
                var fooLoginRepository = new LoginRepository();

                var fooProgressDialogConfig = new ProgressDialogConfig()
                {
                    MaskType = MaskType.Black,
                    Title = "請稍後，正在身分驗證中..."
                };
                using (Acr.UserDialogs.UserDialogs.Instance.Progress(fooProgressDialogConfig))
                {
                    APIResult fooResult;
                    if (UsingHttpGet == true)
                    {
                        fooResult = await fooLoginRepository.GetAsync(Account, Password);
                    }
                    else
                    {
                        fooResult = await fooLoginRepository.PostAsync(Account, Password);
                    }
                    if (fooResult.Success == false)
                    {
                        var config = new Acr.UserDialogs.AlertConfig()
                        {
                            Title = "警告",
                            Message = $"進行使用者身分驗證失敗，原因：{Environment.NewLine}{fooResult.Message}",
                            OkText = "確定",
                        };

                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(config);
                    }
                    else
                    {
                        var fooSystemStatusRepository = new SystemStatusRepository();
                        await fooSystemStatusRepository.ReadAsync();
                        fooSystemStatusRepository.Item.LoginMethodAction = UsingHttpGet;
                        fooSystemStatusRepository.Item.AccessToken = fooLoginRepository.Item.AccessToken;
                        await fooSystemStatusRepository.SaveAsync();
                        await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/AboutPage");
                    }
                }
            });

#if DEBUG
            Account = "user1";
            Password = "pwd1";
#endif
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            var fooSystemStatusRepository = new SystemStatusRepository();
            await fooSystemStatusRepository.ReadAsync();
            UsingHttpGet = fooSystemStatusRepository.Item.LoginMethodAction;
        }

    }
```

![](Icons/ion-ios-lightbulb30.png) 請了解這個頁面檢視 (View) 的 XAML 宣告標記語言內容，與檢視模型 (ViewModel) 裡面 C# 程式碼所提供的商業邏輯的程式碼寫法。

# 修正起始畫面更新完資料後，顯示登入頁面

* 在 `ViewModels` 資料夾內，打開 `SplashPageViewModel.cs`

* 找到這行程式碼 `//await _navigationService.NavigateAsync("xf:///LoginPage");` 將其註解解除掉。

* 完成後的片段程式碼，如下所示
* 
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
        //await _navigationService.NavigateAsync("xf:///MDPage/NaviPage/AboutPage");
    }
}
```

# 問題研究

![](Icons/fa-question-circle30.png) 您可以嘗試當裝置或者模擬器，無法連上網路的狀況下，看看，是否會造成應用程式崩潰，或者會發生甚麼情況？

將這個專案，分別在不同行動裝置平台上(您可以使用模擬器或者是實體裝置)，實際執行這個專案，看看運作成果？

![](Icons/fa-question-circle30.png) 為什麼我們要反覆在不同頁面中，測試網路不通的情境下，會造成甚麼情況？

![](Icons/fa-question-circle30.png) 若登入驗證過程中，因為某些因素，造成 Web API 執行時間過久，我們要如何設計逾期(Timeout)機制，超過我們指定的時間，就是為此次呼叫 Web API 是不成功的？

![](Icons/fa-question-circle30.png) 若您輸入了不正確的帳號與密碼，會發生甚麼情況。

![](Icons/fa-question-circle30.png) 若您輸入了正確的帳號與密碼，會發生甚麼情況。

![](Icons/fa-question-circle30.png) 接著，您重新啟動這個 App，那麼，又會發生甚麼問題，您知道問題在哪裡嗎？

![](Icons/fa-question-circle30.png) 請從 `LoginPage` 這個頁面的 XAML 宣告項目中，了解到如何使用 `Grid` (或者 StackLayout / ScrollView) 版面配置項目，幫助我們做出具有 RWD 效果的 App呢？

![](Icons/fa-question-circle30.png) 在 `LoginPageViewModel`的建構函式中，為什麼我們需要使用 `DEBUG` 這個條件式編譯的符號，他麼目的是甚麼呢？

![](Icons/fa-question-circle30.png) 使用 `Acr.UserDialogs.UserDialogs.Instance.Progress` 這樣的敘述，是要為了解決甚麼樣的問題嗎？

![](Icons/fa-question-circle30.png) 當使用者完成身分檢查與確認之後，我們如何取得此次授權的存取權杖，並且儲存在手機內；如此，就算這個應用程式被強制關閉而再度啟動之後，我們一樣可以取得這個存取權杖。

![](Icons/fa-question-circle30.png) 為什麼我們只要在這個 `OnNavigatedTo` 導航事件中撰寫程式碼，而不選擇另外兩個導航事件的？




