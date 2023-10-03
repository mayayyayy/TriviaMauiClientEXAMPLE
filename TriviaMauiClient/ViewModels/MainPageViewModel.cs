﻿
using TriviaMauiClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TriviaMauiClient.Models;
using TriviaMauiClient.Views;
using System.Text.Json;

namespace TriviaMauiClient.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        private string _userName;
        public string UserName
        { 
            get => _userName;
            set { if (_userName != value) { _userName = value; if (!ValidateUser()) { ShowUserNameError = true; UserErrorMessage = ErrorMessages.INVALID_USERNAME; } else { ShowUserNameError = true; UserErrorMessage = string.Empty; } OnPropertyChange(); OnPropertyChange(nameof(IsButtonEnabled)); } } }



        private bool _showUserNameError;
        public bool ShowUserNameError
        {
            get => _showUserNameError; set
            {
                if (_showUserNameError != value)
                {
                    _showUserNameError = value; OnPropertyChange();
                }
            }
        }

        private string _userErrorMessage;
        public string UserErrorMessage { get => _userErrorMessage; set { if (_userErrorMessage != value) { _userErrorMessage = value; OnPropertyChange(); } } }

        private string _password;
        public string Password { get => _password; set { if (_password != value) { _password = value; if (!ValidatePassWord()) { ShowPasswordError = true; PasswordErrorMessage = ErrorMessages.INVALID_PASSWORD; } else { PasswordErrorMessage = string.Empty; ShowPasswordError = false; } ; OnPropertyChange(); OnPropertyChange(nameof(IsButtonEnabled)); } } }

        private string _passwordError;
        public string PasswordError { get => _passwordError; set { if (_passwordError != value) { _passwordError = value; OnPropertyChange(); } } }

        private bool _showPasswordError;
        public bool ShowPasswordError { get => _showPasswordError; set { if (_showPasswordError != value) { _showPasswordError = value; OnPropertyChange(); } } }
        private string _passwordErrorMessage;
        public string PasswordErrorMessage { get => _passwordErrorMessage; set { if (_passwordErrorMessage != value) { _passwordErrorMessage = value; OnPropertyChange(); } } }

        private bool _showLoginError;
        public bool ShowLoginError { get => _showLoginError; set { if (_showLoginError != value) { _showLoginError = value; OnPropertyChange(); } } }
        private string _loginErrorMessage;
        public string LoginErrorMessage { get => _loginErrorMessage; set { if (_loginErrorMessage != value) { _loginErrorMessage = value; OnPropertyChange(); } } }


        public bool IsButtonEnabled { get { return ValidatePage(); } }
        private bool ValidateUser()
        {
            return !(string.IsNullOrEmpty(_userName) || _userName.Length < 3);
        }
        private bool ValidatePassWord()
        {
            return !string.IsNullOrEmpty(Password);
        }

        private bool ValidatePage()
        {
            return ValidateUser() && ValidatePassWord();
        }


        private TriviaService _service;

        public ICommand LogInCommand { get; protected set; }
        public MainPageViewModel(TriviaService service)
        {
            this._service = service;
            UserName = string.Empty;
            Password=string.Empty;

            LogInCommand = new Command(async () =>
            {
                ShowLoginError = false;
                try
                {
                    var lvm=new LoadingPageViewModel() { IsBusy = true };
                    await AppShell.Current.Navigation.PushModalAsync(new LoadingPage(lvm));

                    var user = await service.LogInAsync(UserName, Password);

                    lvm.IsBusy = false;
                    await AppShell.Current.Navigation.PopModalAsync();
                    if (!user.Success)
                    {
                        ShowLoginError = true;
                        LoginErrorMessage = user.Message;
                    }
                    else
                    {
                        await AppShell.Current.DisplayAlert("התחברת", "אישור להתחלת משחק", "אישור");
                        await SecureStorage.Default.SetAsync("LoggedUser", JsonSerializer.Serialize(user.User));
                        await AppShell.Current.GoToAsync("Game");
                    }



                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                   
                    await AppShell.Current.Navigation.PopModalAsync();
                }
                  

            });
        }

    }

}