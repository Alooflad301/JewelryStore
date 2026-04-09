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
    /// Логика взаимодействия для PageCart.xaml
    /// </summary>
    public partial class PageCart : Page
    {
        public PageCart()
        {
            InitializeComponent();
            Loaded += PageCart_Loaded;
        }

        private void PageCart_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCart();
        }

        private void LoadCart()
        {
            listCart.ItemsSource = ShoppingCart.GetItems();
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            tbTotal.Text = $"Итого: {ShoppingCart.GetTotalPrice():N0} руб.";
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int id))
                ShoppingCart.UpdateQuantity(id, +1);
            LoadCart();
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int id))
                ShoppingCart.UpdateQuantity(id, -1);
            LoadCart();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int id))
            {
                ShoppingCart.RemoveItem(id);
                LoadCart();
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
           AppFrame.framemain.Navigate(new PageCheckout());
        }

        private void CatalogResButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageJewelryCatalog());
        }
    }
}
