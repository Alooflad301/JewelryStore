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
    /// Логика взаимодействия для PageOrderDetails.xaml
    /// </summary>
    public partial class PageOrderDetails : Page
    {
        public PageOrderDetails(int orderId)
        {
            InitializeComponent();
            LoadOrderDetails(orderId);
        }

        private void LoadOrderDetails(int orderId)
        {
            using (var db = ShoppingCart.GetNewContext())
            {
                var orderItems = db.OrderItem
                    .Where(oi => oi.IdOrder == orderId)
                    .Select(oi => new
                    {
                        oi.IdOrderItem,
                        oi.Quantity,
                        oi.UnitPrice,
                        oi.TotalPrice,
                        Jewelry = oi.Jewelry,
                        JewelryTip = oi.Jewelry.JewelryTip
                    }).ToList();

                var order = db.Order.FirstOrDefault(o => o.IdOrder == orderId);

                if (order != null)
                {
                    tbOrderNumber.Text = $"Заказ #{order.IdOrder}";
                    listOrderItems.ItemsSource = orderItems;
                    tbOrderTotal.Text = $"Итого: {order.TotalPrice:N0} руб.";
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageUserOrders());
        }
    }
}
