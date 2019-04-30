using Foundation;
using Prism;
using Prism.Ioc;
using System;
using UIKit;


namespace LOBForm.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //UINavigationBar.Appearance.BarTintColor = GetColorFromHexString("#FCB515");
            //UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes
            //{
            //    ForegroundColor = UIColor.White,
            //};

            global::Xamarin.Forms.Forms.Init();

            #region 第三方套件／插件的初始化

            #endregion

            LoadApplication(new App(new iOSInitializer()));

            return base.FinishedLaunching(app, options);
        }

        public UIColor GetColorFromHexString(string hexValue)
        {
            hexValue = hexValue.Substring(1, 6); // string will be passed in with a leading #
            var r = Convert.ToByte(hexValue.Substring(0, 2), 16);
            var g = Convert.ToByte(hexValue.Substring(2, 2), 16);
            var b = Convert.ToByte(hexValue.Substring(4, 2), 16);
            return UIColor.FromRGB(r, g, b);
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {

        }
    }
}
