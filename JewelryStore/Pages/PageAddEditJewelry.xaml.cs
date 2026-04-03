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
    /// Логика взаимодействия для PageAddEditJewelry.xaml
    /// </summary>
    public partial class PageAddEditJewelry : Page
    {
        public Jewelry JewelryToEdit { get; set; }

        public PageAddEditJewelry()
        {
            InitializeComponent();
            this.Loaded += Page_Loaded;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window window)
            {
                this.MinWidth = 800;
                this.MinHeight = 700;
            }
            LoadComboBoxes();

            if (JewelryToEdit != null)
            {
                NameJewelryText.Text = JewelryToEdit.NameJewelry;
                JewelryTipCombo.SelectedValue = JewelryToEdit.IdJewelryTip;
                MaterialCombo.SelectedValue = JewelryToEdit.IdMaterial;
                StoneCombo.SelectedValue = JewelryToEdit.IdStone;
                SupplierCombo.SelectedValue = JewelryToEdit.IdSupplier;
                PriceText.Text = JewelryToEdit.PriceJewelry?.ToString();
                ImagePathText.Text = JewelryToEdit.ImagePath;
                SaveButton.Content = "✏️ Обновить";
            }
            else
            {
                SaveButton.Content = "➕ Добавить";
            }
        }

        private void LoadComboBoxes()
        {
            JewelryTipCombo.ItemsSource = AppData.AppConnect.model0db.JewelryTip.ToList();
            MaterialCombo.ItemsSource = AppData.AppConnect.model0db.Material.ToList();
            StoneCombo.ItemsSource = AppData.AppConnect.model0db.Stone.ToList();
            SupplierCombo.ItemsSource = AppData.AppConnect.model0db.Supplier.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Jewelry jewelry;

                if (JewelryToEdit == null)
                {
                    // Добавление
                    jewelry = new Jewelry();
                    AppData.AppConnect.model0db.Jewelry.Add(jewelry);
                }
                else
                {
                    // Редактирование
                    jewelry = JewelryToEdit;
                }

                jewelry.NameJewelry = NameJewelryText.Text;
                jewelry.IdJewelryTip = (int)JewelryTipCombo.SelectedValue;
                jewelry.IdMaterial = (int)MaterialCombo.SelectedValue;
                jewelry.IdStone = StoneCombo.SelectedValue as int?;
                jewelry.IdSupplier = (int)SupplierCombo.SelectedValue;
                jewelry.PriceJewelry = int.TryParse(PriceText.Text, out int price) ? price : 0;
                jewelry.ImagePath = ImagePathText.Text;

                AppData.AppConnect.model0db.SaveChanges();

                MessageBox.Show(JewelryToEdit == null ? "Товар добавлен!" : "Товар обновлён!");
                AppFrame.framemain.Navigate(new PageAdminPanel());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageAdminPanel());
        }
    }
}
