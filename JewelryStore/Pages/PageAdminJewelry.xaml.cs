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
    /// Логика взаимодействия для PageAdminJewelry.xaml
    /// </summary>
    public partial class PageAdminJewelry : Page
    {
        public PageAdminJewelry()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadJewelryList();
            CheckAdminRole();
        }

        private void LoadJewelryList()
        {
            try
            {
                listProduct.ItemsSource = AppData.AppConnect.model0db.Jewelry.ToList();
                tbCounter.Text = $"Всего товаров: {listProduct.Items.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = listProduct.SelectedItem != null;
            DeleteButton.IsEnabled = listProduct.SelectedItem != null;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageAddEditJewelry());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (listProduct.SelectedItem is Jewelry selectedJewelry)
            {
                var page = new PageAddEditJewelry();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (listProduct.SelectedItem is Jewelry selectedJewelry)
            {
                var result = MessageBox.Show($"Удалить товар '{selectedJewelry.NameJewelry}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        AppData.AppConnect.model0db.Jewelry.Remove(selectedJewelry);
                        AppData.AppConnect.model0db.SaveChanges();
                        LoadJewelryList();
                        MessageBox.Show("Товар удален!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}");
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadJewelryList();
        }

        private void CheckAdminRole()
        {
            if (!AppData.CurrentUser.IsAdmin)
            {
                AddButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
