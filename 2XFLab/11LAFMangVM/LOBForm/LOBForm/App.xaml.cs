using Prism;
using Prism.Ioc;
using LOBForm.ViewModels;
using LOBForm.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Unity;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LOBForm
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("SplashPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<NaviPage>();
            containerRegistry.RegisterForNavigation<MDPage>();
            containerRegistry.RegisterForNavigation<SplashPage>();
            containerRegistry.RegisterForNavigation<LoginPage>();
            containerRegistry.RegisterForNavigation<AboutPage>();
            containerRegistry.RegisterForNavigation<OnCallPage>();
            containerRegistry.RegisterForNavigation<WorkingLogPage>();
            containerRegistry.RegisterForNavigation<WorkingLogDetailPage>();
            containerRegistry.RegisterForNavigation<LeaveAppFormPage>();
            containerRegistry.RegisterForNavigation<LeaveAppFormDetailPage>();
            containerRegistry.RegisterForNavigation<LeaveAppFormManagerPage>();
        }
    }
}
