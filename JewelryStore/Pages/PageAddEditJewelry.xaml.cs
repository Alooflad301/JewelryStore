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
        public Jewelry JewelryToEdit { get; set; } // null = добавить; not null = редактировать

        public PageAddEditJewelry()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Минимальный размер окна
            if (Window.GetWindow(this) is Window window)
            {
                window.MinWidth = 800;
                window.MinHeight = 800;
            }

            LoadComboBoxes();

            // Заполнение поля редактирования
            if (JewelryToEdit != null)
            {
                NameJewelryText.Text = JewelryToEdit.NameJewelry?.Trim();
                JewelryTipCombo.SelectedValue = JewelryToEdit.IdJewelryTip;
                MaterialCombo.SelectedValue = JewelryToEdit.IdMaterial;
                StoneCombo.SelectedValue = JewelryToEdit.IdStone;
                SupplierCombo.SelectedValue = JewelryToEdit.IdSupplier;
                PriceText.Text = JewelryToEdit.PriceJewelry?.ToString("0.00"); // decimal
                ImagePathText.Text = JewelryToEdit.ImagePath?.Trim();

                SaveButton.Content = "✏️ Обновить";
                LoadImagePreview();
            }
            else
            {
                SaveButton.Content = "➕ Добавить";
            }
        }

        // Загрузка комбобоксов (справочники)
        private void LoadComboBoxes()
        {
            try
            {
                using (var db = new JewelryStoreEntities()) // Новый контекст
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

        // Подгрузка preview изображения
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
                    // ImagePreview.Source = null; // Если ошибка
                }
            }
        }

        // Выбор изображения
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

                    string fileName = Path.GetFileName(openFileDialog.FileName);       // имя файла как есть
                    string filePathInImages = Path.Combine(projectImagesFolder, fileName);

                    // Если файл уже есть в папке Images — используем его имя, не копируем
                    if (!File.Exists(filePathInImages))
                    {
                        // Копируем файл в папку Images
                        File.Copy(openFileDialog.FileName, filePathInImages, overwrite: true);
                    }

                    // Всегда устанавливаем имя файла из Image (существующее или только что скопированное)
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

        // Сохранение/Обновление товара
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация полей — как у тебя уже есть
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
                // Создаём свежий контекст, чтобы не было конфликтов с глобальным
                using (var db = new JewelryStoreEntities())
                {
                    Jewelry jewelry;

                    if (JewelryToEdit == null)
                    {
                        // Новый товар
                        jewelry = new Jewelry();
                        db.Jewelry.Add(jewelry);
                    }
                    else
                    {
                        // Редактирование: загружаем объект по Id (чистый контекст!)
                        int id = JewelryToEdit.IdJewelry;
                        jewelry = db.Jewelry.FirstOrDefault(x => x.IdJewelry == id);

                        if (jewelry == null)
                        {
                            MessageBox.Show("Товар не найден в базе данных.");
                            return;
                        }
                    }

                    // Обновляем поля
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

                    // Обновляем глобальный контекст, если хочешь видеть изменения сразу
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

        // Отмена (назад к панели)
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageAdminPanel());
        }
    }
}