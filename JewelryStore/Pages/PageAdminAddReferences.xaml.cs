using JewelryStore.AppData;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JewelryStore.Pages
{
    public partial class PageAdminAddReferences : Page
    {
        private JewelryStoreEntities db = AppConnect.model0db;

        public PageAdminAddReferences()
        {
            InitializeComponent();
            if (!CurrentUser.IsAdmin)
            {
                MessageBox.Show("Доступ только для админов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void AddJewelryTip_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TipNameTxt.Text))
            {
                MessageBox.Show("Введите название типа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                db.JewelryTip.Add(new JewelryTip { NameJewelryTip = TipNameTxt.Text.Trim() });
                db.SaveChanges();
                MessageBox.Show("Тип украшения добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                TipNameTxt.Clear();
                TipNameTxt.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MatNameTxt.Text) || !int.TryParse(MatProbaTxt.Text, out int proba))
            {
                MessageBox.Show("Введите название материала и пробу (число)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                db.Material.Add(new Material
                {
                    NameMaterial = MatNameTxt.Text.Trim(),
                    Proba = proba
                });
                db.SaveChanges();
                MessageBox.Show($"Материал '{MatNameTxt.Text.Trim()} ({proba})' добавлен!", "Успех");
                MatNameTxt.Clear();
                MatProbaTxt.Clear();
                MatNameTxt.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message, "Ошибка");
            }
        }

        private void AddStone_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StoneNameTxt.Text) || !double.TryParse(StoneWeightTxt.Text, out double weight))
            {
                MessageBox.Show("Введите название камня и вес (число)!", "Ошибка");
                return;
            }

            try
            {
                db.Stone.Add(new Stone
                {
                    NameStone = StoneNameTxt.Text.Trim(),
                    ColorStone = StoneColorTxt.Text.Trim(),
                    WeightStone = weight
                });
                db.SaveChanges();
                MessageBox.Show("Камень добавлен!", "Успех");
                StoneNameTxt.Clear();
                StoneColorTxt.Clear();
                StoneWeightTxt.Clear();
                StoneNameTxt.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SuppNameTxt.Text))
            {
                MessageBox.Show("Введите название поставщика!", "Ошибка");
                return;
            }

            try
            {
                db.Supplier.Add(new Supplier
                {
                    NameSupplier = SuppNameTxt.Text.Trim(),
                    PhoneSupplier = SuppPhoneTxt.Text.Trim(),
                    EmailSupplier = SuppEmailTxt.Text.Trim()
                });
                db.SaveChanges();
                MessageBox.Show("Поставщик добавлен!", "Успех");
                SuppNameTxt.Clear();
                SuppPhoneTxt.Clear();
                SuppEmailTxt.Clear();
                SuppNameTxt.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private void AddStatusOrder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StatusNameTxt.Text))
            {
                MessageBox.Show("Введите название статуса!", "Ошибка");
                return;
            }

            try
            {
                db.StatusOrder.Add(new StatusOrder { NameStatusOrder = StatusNameTxt.Text.Trim() });
                db.SaveChanges();
                MessageBox.Show("Статус заказа добавлен!", "Успех");
                StatusNameTxt.Clear();
                StatusNameTxt.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        private void RefreshAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.Dispose();
                db = new JewelryStoreEntities();
                AppConnect.model0db = db; // Обновляем глобальный контекст
                MessageBox.Show("Контекст обновлён! Все изменения видны в других страницах.", "Обновлено");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления: " + ex.Message);
            }
        }
    }
}