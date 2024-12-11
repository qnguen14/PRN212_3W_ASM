using Microsoft.IdentityModel.Tokens;
using Snake.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Snake
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private UserService _service = new();


        public LoginWindow()
        {
            InitializeComponent();
        }
        

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var account = _service.Authenticate(TxtEmail.Text, TxtPassword.Password);

            if (account == null)
            {
                MessageBox.Show("Account not found! Please register an account before logging in"
                    , "No account detected"
                    , MessageBoxButton.OK
                    , MessageBoxImage.Information );
                return;
            }
            
            if (TxtEmail.Text.IsNullOrEmpty() || TxtPassword.Password.IsNullOrEmpty())
            {
                MessageBox.Show("Both Email Address and Password are required!", "No Credentials Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            

            if (account.Role == "player")
            {
                MainWindow m = new();
                m.CurrentUser = account;
                m.Show();
                this.Hide();
            }

        }


        private void RegiserButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow r = new();
            r.Show();
            this.Hide();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
