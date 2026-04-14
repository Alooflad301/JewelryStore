using JewelryStore.AppData;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
                NameJewelryText.Text = JewelryToEdit.NameJewelry;
                JewelryTipCombo.SelectedValue = JewelryToEdit.IdJewelryTip;
                MaterialCombo.SelectedValue = JewelryToEdit.IdMaterial;
                StoneCombo.SelectedValue = JewelryToEdit.IdStone;
                SupplierCombo.SelectedValue = JewelryToEdit.IdSupplier;
                PriceText.Text = JewelryToEdit.PriceJewelry?.ToString();
                ImagePathText.Text = JewelryToEdit.ImagePath;
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
            JewelryTipCombo.ItemsSource = AppConnect.model0db.JewelryTip.ToList();
            MaterialCombo.ItemsSource = AppConnect.model0db.Material.ToList();
            StoneCombo.ItemsSource = AppConnect.model0db.Stone.ToList();
            SupplierCombo.ItemsSource = AppConnect.model0db.Supplier.ToList();
        }

        private void LoadImagePreview()
        {
            if (!string.IsNullOrEmpty(ImagePathText.Text))
            {
                string projectImagesFolder = GetProjectImagesFolder();
                string imagePath = Path.Combine(projectImagesFolder, Path.GetFileName(ImagePathText.Text));

                if (File.Exists(imagePath))
                {
                    ImagePreview.Source = new BitmapImage(new Uri(imagePath));
                }
            }
        }

        private string GetProjectImagesFolder()
        {
            // Динамический путь к Images в папке проекта (3 уровня вверх от bin/Debug)
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Images"));
        }

        private void SelectImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Выберите изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string projectImagesFolder = GetProjectImagesFolder();
                    Directory.CreateDirectory(projectImagesFolder);

                    string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    string fileExt = Path.GetExtension(openFileDialog.FileName);
                    string newFileName = fileName + fileExt;
                    int counter = 1;

                    // Уникальное имя
                    while (File.Exists(Path.Combine(projectImagesFolder, newFileName)))
                    {
                        newFileName = $"{fileName}_{counter++}{fileExt}";
                    }

                    string targetPath = Path.Combine(projectImagesFolder, newFileName);
                    File.Copy(openFileDialog.FileName, targetPath, true);

                    ImagePathText.Text = $"{newFileName}";
                    ImagePreview.Source = new BitmapImage(new Uri(targetPath));

                    MessageBox.Show($"Изображение сохранено:\n{targetPath}", "Готово!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameJewelryText.Text))
            {
                MessageBox.Show("Введите название украшения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameJewelryText.Focus();
                NameJewelryText.SelectAll();
                return;
            }

            if (JewelryTipCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип украшения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                JewelryTipCombo.Focus();
                return;
            }

            if (MaterialCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите материал!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                MaterialCombo.Focus();
                return;
            }

            if (SupplierCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите поставщика!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                SupplierCombo.Focus();
                return;
            }

            if (!decimal.TryParse(PriceText.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену (больше 0)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceText.Focus();
                PriceText.SelectAll();
                return;
            }

            // Проверка на длину имени
            if (NameJewelryText.Text.Length < 2)
            {
                MessageBox.Show("Название украшения должно содержать не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameJewelryText.Focus();
                return;
            }

            try
            {
                Jewelry jewelry;

                if (JewelryToEdit == null)
                {
                    jewelry = new Jewelry();
                    AppConnect.model0db.Jewelry.Add(jewelry);
                }
                else
                {
                    jewelry = JewelryToEdit;
                }

                jewelry.NameJewelry = NameJewelryText.Text?.Trim();
                jewelry.IdJewelryTip = JewelryTipCombo.SelectedValue is int tipId ? tipId : 1;
                jewelry.IdMaterial = MaterialCombo.SelectedValue is int matId ? matId : 1;
                jewelry.IdStone = StoneCombo.SelectedValue as int?;
                jewelry.IdSupplier = SupplierCombo.SelectedValue is int suppId ? suppId : 1;
                jewelry.PriceJewelry = price;
                jewelry.ImagePath = ImagePathText.Text;

                AppConnect.model0db.SaveChanges();

                MessageBox.Show(JewelryToEdit == null ? "Товар добавлен!" : "Товар обновлён!");
                AppFrame.framemain.Navigate(new PageAdminPanel());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageAdminPanel());
        }
    }
}