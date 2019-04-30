using Acr.UserDialogs;
using LOBForm.Models;
using LOBForm.Repositories;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LOBForm.ViewModels
{
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
}
