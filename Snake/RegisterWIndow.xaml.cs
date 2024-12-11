using Snake.BLL.Services;
using Snake.DAL.Models;
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
    /// Interaction logic for RegisterWIndow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private UserService _service = new();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

            if (!TxtPassword.Password.Equals(TxtRePassword.Password))
            {
                MessageBox.Show("Password is not the same!", "Error registering", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (_service.Exist(TxtEmail.Text.Trim()))
            {
                MessageBox.Show("Account already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_service.IsValidEmail(TxtEmail.Text.Trim()))
            {
                MessageBox.Show("Email format isn't valid!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            User user = new User()
            {
                Username = TxtEmail.Text,
                Password = TxtPassword.Password,
                Role = "player",
                Email = TxtEmail.Text,
                CreatedAt = DateTime.Now,
            };

            _service.Register(user);
            MessageBox.Show("Account created!", "Registered successfully", MessageBoxButton.OK, MessageBoxImage.Information);
            LoginWindow w = new();
            w.Show();
            this.Close();
            return;

        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow w = new();
            w.Show();
            this.Close();
        }
    }
}
