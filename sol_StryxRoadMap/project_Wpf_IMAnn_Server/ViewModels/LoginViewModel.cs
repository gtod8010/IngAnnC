using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows;
using Wpf_IMAnn_Server.Views;
using System.Windows.Controls;

namespace Wpf_IMAnn_Server.ViewModels
{
    public class LoginViewModel : Screen
    {
        IWindowManager manager = new WindowManager();
        public void LoginButtonClick()
        {
            LoginView LoginView = (LoginView)(this.GetView());
            bool LoginResult = TryLogin(ID, LoginView.PassWord.Password);
            if (LoginResult == true)
                OpenShellView();
            else
                MessageBox.Show("아이디 또는 패스워드가 틀렸습니다.", "로그인 오류");
        }

        private void OpenShellView()
        {
            ShellViewModel shellViewModel = new ShellViewModel();
            manager.ShowWindow(shellViewModel, null, null);
            LoginView LoginView = (LoginView)(this.GetView());
            Window.GetWindow(LoginView).Close();
        }

        private bool TryLogin(string id, string pw)
        {
            return true;
        }

        private string _ID;

        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
    }
}
