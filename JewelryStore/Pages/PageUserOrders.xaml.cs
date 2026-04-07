using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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
            using (var db = AppData.AppConnect.model0db)
            {
                var query = db.Order
                    .Where(o => o.IdUser == AppData.CurrentUser.IdUser)
                    .Include(o => o.Jewelry)
                    .Include(o => o.StatusOrder)
                    .Include(o => o.Jewelry.JewelryTip);
                if (selectedStatusId.HasValue)
                    query = query.Where(o => o.IdStatusOrder == selectedStatusId.Value);

                listOrders.ItemsSource = query.OrderByDescending(o => o.OrderDate).ToList();
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
            using (var db = AppData.AppConnect.model0db)
            {
                var totalOrders = db.Order.Count(o => o.IdUser == AppData.CurrentUser.IdUser);
                var totalSum = db.Order.Sum(o => (decimal?)(o.IdUser == AppData.CurrentUser.IdUser ? o.TotalPrice : 0) ?? 0);

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
                    case "В обработке":
                        selectedStatusId = 2;
                        break;
                    case "Отправлен":
                        selectedStatusId = 3;
                        break;
                    case "Доставлен":
                        selectedStatusId = 4;
                        break;
                    default:
                        selectedStatusId = null;
                        break;
                }
                LoadOrders();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }
    }
}
