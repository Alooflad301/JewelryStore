using JewelryStore.AppData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JewelryStore.Pages
{
    public partial class PageAdminOrders : Page
    {
        private JewelryStoreEntities db;
        public List<Order> OrdersList { get; set; }
        public List<StatusOrder> StatusList { get; set; }

        public PageAdminOrders()
        {
            InitializeComponent();
            db = AppConnect.model0db; 
            LoadData();
        }

        private void LoadData()
        {
            OrdersList = db.Order.Include("StatusOrder").ToList();
            StatusList = db.StatusOrder.ToList();
            OrdersGrid.ItemsSource = OrdersList;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            MessageBox.Show("Список обновлён!");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SaveChanges();
                MessageBox.Show("Статусы заказов обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(); // Перезагружаем для актуального отображения
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OrdersGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SaveBtn.IsEnabled = true; // Активируем кнопку после редактирования
        }
    }
}