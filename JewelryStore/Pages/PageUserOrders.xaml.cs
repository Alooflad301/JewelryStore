using JewelryStore.AppData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            using (var db = ShoppingCart.GetNewContext())
            {
                var statuses = db.StatusOrder.ToList();
                var items = new ObservableCollection<object>
                {
                    new { IdStatusOrder = 0, NameStatusOrder = "Все" }
                };
                foreach (var status in statuses)
                {
                    items.Add(status);
                }

                StatusFilter.ItemsSource = items;
                StatusFilter.SelectedIndex = 0;
            }

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

                var selected = StatusFilter.SelectedItem as dynamic;
                var selectedId = selected?.IdStatusOrder as int?;

                if (selectedId.HasValue && selectedId.Value > 0)
                {
                    orders = orders.Where(o => o.IdStatusOrder == selectedId.Value).ToList();
                }

                listOrders.ItemsSource = orders.Select(o => new
                {
                    IdOrder = o.IdOrder,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone ?? "",
                    CustomerEmail = o.CustomerEmail ?? "",
                    DeliveryAddress = o.DeliveryAddress ?? "",
                    OrderDate = o.OrderDate,
                    StatusName = db.StatusOrder
                        .FirstOrDefault(s => s.IdStatusOrder == o.IdStatusOrder)?.NameStatusOrder ?? "Неизвестно",
                    TotalPrice = o.TotalPrice ?? 0,
                    JewelryName = db.Jewelry
                        .FirstOrDefault(j => j.IdJewelry == o.IdJewelry)?.NameJewelry ?? "Разные товары",
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
                var totalSum = db.Order
                    .Where(o => o.IdUser == AppData.CurrentUser.IdUser)
                    .Sum(o => (int?)(o.TotalPrice ?? 0) ?? 0);

                tbStats.Text = $"Заказов: {totalOrders} | На сумму: {totalSum:N0} руб.";
            }
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedStatusId = null;

            var selected = StatusFilter.SelectedItem as dynamic;
            if (selected != null)
            {
                int? id = selected.IdStatusOrder as int?;
                if (id > 0)
                    selectedStatusId = id;
            }

            LoadOrders();
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
                ViewOrder_Click(null, null);
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