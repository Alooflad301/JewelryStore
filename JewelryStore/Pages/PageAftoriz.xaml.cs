using JewelryStore.AppData;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JewelryStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAftoriz.xaml
    /// </summary>
    public partial class PageAftoriz : Page
    {
        public PageAftoriz()
        {
            InitializeComponent();
            this.Loaded += Aftor_Loaded;
        }

        private void Regtodt_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageReg());
        }

        private void DaBtn_Click(object sender, RoutedEventArgs e)
        {
            {
                try
                {
                    var userObj = AppData.AppConnect.model0db.User.FirstOrDefault(x => x.Login == TextLogin.Text && x.Password == PassBox.Password);
                    if (userObj == null)
                    {
                        MessageBox.Show("Такого пользователя нет", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("Добро пожаловать " + userObj.Login + "!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                        AppFrame.framemain.Navigate(new PagePrivet());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка" + ex.Message.ToString(), "Критическая ошибка приложения", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageReg());
        }

        private void Aftor_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window window)
            {
                this.MinWidth = 440;
                this.MinHeight = 420;
            }
        }
    }
}

