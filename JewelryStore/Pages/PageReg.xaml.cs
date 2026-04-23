using JewelryStore.AppData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JewelryStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageReg.xaml
    /// </summary>
    public partial class PageReg : Page
    {
        public PageReg()
        {
            InitializeComponent();
            this.Loaded += PageReg_Loaded;
        }

        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                MessageBox.Show("Введите логин!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoginBox.Focus();
                LoginBox.SelectAll();
                return;
            }

            if (string.IsNullOrWhiteSpace(TelevonBox.Text) || !ValidatePhone(TelevonBox.Text))
            {
                MessageBox.Show("Введите корректный телефон в формате: +7 (999) 123-45-67 или 89991234567.",
                                "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                TelevonBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailBox.Text) || !ValidateEmail(EmailBox.Text))
            {
                MessageBox.Show("Введите корректный e‑mail адрес.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(Password1.Password))
            {
                MessageBox.Show("Введите пароль!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                Password1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(Password2.Password) || Password1.Password != Password2.Password)
            {
                MessageBox.Show("Пароли не совпадают или одно из полей пусто.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                Password2.Focus();
                return;
            }

            if (LoginBox.Text.Length < 3)
            {
                MessageBox.Show("Логин должен содержать не менее 3 символов.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoginBox.Focus();
                return;
            }

            if (Password1.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                Password1.Focus();
                return;
            }

            if (AppConnect.model0db.User.Any(x => x.Login == LoginBox.Text.Trim()))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginBox.Focus();
                LoginBox.SelectAll();
                return;
            }

            try
            {
                if (AppConnect.model0db.User.Count(x => x.Login == LoginBox.Text) > 0)
                {
                    MessageBox.Show("Пользователь с таким логином есть!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (String.IsNullOrEmpty(LoginBox.Text) ||
                    String.IsNullOrEmpty(Password1.Password) ||
                    String.IsNullOrWhiteSpace(Password1.Password) ||
                    String.IsNullOrWhiteSpace(LoginBox.Text))
                {
                    MessageBox.Show("Не заполнены все поля!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                User userObj = new User()
                {
                    Login = LoginBox.Text,
                    Password = Password1.Password,
                    Phone = TelevonBox.Text,
                    Email = EmailBox.Text,
                    IdRole = 1
                    
                };
                AppConnect.model0db.User.Add(userObj);
                AppConnect.model0db.SaveChanges();
                MessageBox.Show("Данные успешно добавлены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                AppData.AppFrame.framemain.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении данных!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Password2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password1.Password != Password2.Password)
            {
                RegBut.IsEnabled = false;
                Password2.Background = Brushes.LightCoral;
                Password2.BorderBrush = Brushes.Red;
            }
            else
            {
                RegBut.IsEnabled = true;
                Password2.Background = Brushes.LightGreen;
                Password2.BorderBrush = Brushes.Green;
            }
        }

        private void PageReg_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window window)
            {
                window.MinWidth = 300;
                window.MinHeight = 500;
            }
        }

        private void HomeBut_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.GoBack();
        }
        private bool ValidatePhone(string phone)
        {
            phone = phone?.Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "").Replace("-", "").Trim();
            return !string.IsNullOrEmpty(phone) && phone.Length >= 10 && phone.Length <= 15 && phone.All(char.IsDigit);
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
