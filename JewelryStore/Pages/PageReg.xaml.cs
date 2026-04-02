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
    }
}
