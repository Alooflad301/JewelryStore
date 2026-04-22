using JewelryStore.AppData;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JewelryStore.Pages
{
    public partial class PageAddEditJewelry : Page
    {
        public Jewelry JewelryToEdit { get; set; } 

        public PageAddEditJewelry()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window window)
            {
                window.MinWidth = 800;
                window.MinHeight = 800;
            }

            LoadComboBoxes();

            if (JewelryToEdit != null)
            {
                NameJewelryText.Text = JewelryToEdit.NameJewelry?.Trim();
                JewelryTipCombo.SelectedValue = JewelryToEdit.IdJewelryTip;
                MaterialCombo.SelectedValue = JewelryToEdit.IdMaterial;
                StoneCombo.SelectedValue = JewelryToEdit.IdStone;
                SupplierCombo.SelectedValue = JewelryToEdit.IdSupplier;
                PriceText.Text = JewelryToEdit.PriceJewelry?.ToString("0.00");
                ImagePathText.Text = JewelryToEdit.ImagePath?.Trim();

                SaveButton.Content = "✏️ Обновить";
                LoadImagePreview();
            }
            else
            {
                SaveButton.Content = "➕ Добавить";
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var db = new JewelryStoreEntities())
                {
                    JewelryTipCombo.ItemsSource = db.JewelryTip.ToList();
                    MaterialCombo.ItemsSource = db.Material.ToList();
                    StoneCombo.ItemsSource = db.Stone.ToList();
                    SupplierCombo.ItemsSource = db.Supplier.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadImagePreview()
        {
            if (!string.IsNullOrWhiteSpace(ImagePathText.Text))
            {
                string projectImagesFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Images"));
                string imagePath = Path.Combine(projectImagesFolder, ImagePathText.Text.Trim());

                try
                {
                    if (File.Exists(imagePath))
                    {
                        ImagePreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath));
                    }
                }
                catch
                {
                }
            }
        }

        private void SelectImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Выберите изображение товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string projectImagesFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Images"));
                    Directory.CreateDirectory(projectImagesFolder);

                    string fileName = Path.GetFileName(openFileDialog.FileName);      
                    string filePathInImages = Path.Combine(projectImagesFolder, fileName);

                    if (!File.Exists(filePathInImages))
                    {
                        File.Copy(openFileDialog.FileName, filePathInImages, overwrite: true);
                    }
                    ImagePathText.Text = fileName;
                    ImagePreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(filePathInImages));

                    MessageBox.Show($"Изображение готово: {fileName}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameJewelryText.Text))
            {
                MessageBox.Show("Введите название товара.");
                return;
            }

            if (!decimal.TryParse(PriceText.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            try
            {
                using (var db = new JewelryStoreEntities())
                {
                    Jewelry jewelry;

                    if (JewelryToEdit == null)
                    {
                        jewelry = new Jewelry();
                        db.Jewelry.Add(jewelry);
                    }
                    else
                    {
                        int id = JewelryToEdit.IdJewelry;
                        jewelry = db.Jewelry.FirstOrDefault(x => x.IdJewelry == id);

                        if (jewelry == null)
                        {
                            MessageBox.Show("Товар не найден в базе данных.");
                            return;
                        }
                    }

                    jewelry.NameJewelry = NameJewelryText.Text.Trim();
                    jewelry.IdJewelryTip = (int)JewelryTipCombo.SelectedValue;
                    jewelry.IdMaterial = (int)MaterialCombo.SelectedValue;
                    jewelry.IdStone = StoneCombo.SelectedValue as int?;
                    jewelry.IdSupplier = (int)SupplierCombo.SelectedValue;
                    jewelry.PriceJewelry = price;
                    jewelry.ImagePath = !string.IsNullOrWhiteSpace(ImagePathText.Text)
                        ? ImagePathText.Text.Trim()
                        : null;

                    db.SaveChanges();

                    AppData.AppConnect.model0db.Entry(JewelryToEdit).CurrentValues.SetValues(jewelry);

                    MessageBox.Show(JewelryToEdit == null ? "🛒 Товар добавлен!" : "✅ Товар обновлён!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }

            AppFrame.framemain.Navigate(new PageAdminPanel());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageAdminPanel());
        }
    }
}