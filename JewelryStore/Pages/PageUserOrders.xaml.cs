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
    /// Логика взаимодействия для PageUserOrders.xaml
    /// </summary>
    public partial class PageUserOrders : Page
    {
        private int? selectedStatusId = null;

        public PageUserOrders()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void LoadOrders()
        {
            using (var db = ShoppingCart.GetNewContext())
            {
                var orders = db.Order
                    .Where(o => o.IdUser == AppData.CurrentUser.IdUser)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                // Применяем фильтр в памяти (C# 7.3)
                if (selectedStatusId.HasValue)
                    orders = orders.Where(o => o.IdStatusOrder == selectedStatusId.Value).ToList();

                // Проекция для отображения
                listOrders.ItemsSource = orders.Select(o => new
                {
                    IdOrder = o.IdOrder,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone ?? "",
                    CustomerEmail = o.CustomerEmail ?? "",
                    DeliveryAddress = o.DeliveryAddress ?? "",
                    OrderDate = o.OrderDate,
                    StatusName = db.StatusOrder.FirstOrDefault(s => s.IdStatusOrder == o.IdStatusOrder)?.NameStatusOrder ?? "Неизвестно",
                    TotalPrice = o.TotalPrice ?? 0,
                    JewelryName = db.Jewelry.FirstOrDefault(j => j.IdJewelry == o.IdJewelry)?.NameJewelry ?? "Разные товары",
                    Quantity = o.Quantity ?? 0
                }).ToList();

                UpdateStats();
            }
        }

        private void UpdateStats()
        {
            using (var db = ShoppingCart.GetNewContext())
            {
                var totalOrders = db.Order.Count(o => o.IdUser == AppData.CurrentUser.IdUser);
                var totalSum = db.Order.Where(o => o.IdUser == AppData.CurrentUser.IdUser)
                                      .Sum(o => (int?)(o.TotalPrice ?? 0) ?? 0);

                tbStats.Text = $"Заказов: {totalOrders} | На сумму: {totalSum:N0} руб.";
            }
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusFilter.SelectedItem is ComboBoxItem item)
            {
                string content = item.Content.ToString();
                switch (content)
                {
                    case "Все":
                        selectedStatusId = null;
                        break;
                    case "Новый":
                        selectedStatusId = 1;
                        break;
                    case "Подтверждён":
                        selectedStatusId = 2;
                        break;
                    case "В обработке":
                        selectedStatusId = 3;
                        break;
                    case "Отправлен":
                        selectedStatusId = 4;
                        break;
                    case "Доставлен":
                        selectedStatusId = 5;
                        break;
                    default:
                        selectedStatusId = null;
                        break;
                }
                LoadOrders();
            }
        }
        private void ViewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int orderId))
            {
                AppFrame.framemain.Navigate(new PageOrderDetails(orderId));
            }
        }

        private void listOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOrders.SelectedItem != null)
            {
                ViewOrder_Click(null, null);  // Альтернативный способ
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void BackCatalogButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageJewelryCatalog());
        }
    }
}
