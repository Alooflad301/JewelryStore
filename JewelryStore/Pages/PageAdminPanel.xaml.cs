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
    /// Логика взаимодействия для PageAdminPanel.xaml
    /// </summary>
    public partial class PageAdminPanel : Page
    {
        public PageAdminPanel()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Adminfarme.Navigate(new PageAdminJewelry());
        }

        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageJewelryCatalog());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Adminfarme.Navigate(new PageAdminUsers());
        }
    }
}
