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
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Проверка прав админа
            if (!AppData.CurrentUser.IsAdmin)
            {
                MessageBox.Show("Доступ только для админов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                AppFrame.framemain.GoBack();
                return;
            }

            LoadJewelryTipList();
            LoadMaterialList();
            LoadStoneList();
            LoadSupplierList();
            LoadStatusOrderList();
        }

        // === ЗАГРУЗКА СПИСКОВ ===

        private void LoadJewelryTipList()
        {
            listJewelryTip.ItemsSource = db.JewelryTip.ToList();
            listJewelryTip.DisplayMemberPath = "NameJewelryTip";
        }

        private void LoadMaterialList()
        {
            listMaterial.ItemsSource = db.Material.ToList();
            listMaterial.DisplayMemberPath = "NameMaterial";
        }

        private void LoadStoneList()
        {
            listStone.ItemsSource = db.Stone.ToList();
            listStone.DisplayMemberPath = "NameStone";
        }

        private void LoadSupplierList()
        {
            listSupplier.ItemsSource = db.Supplier.ToList();
            listSupplier.DisplayMemberPath = "NameSupplier";
        }

        private void LoadStatusOrderList()
        {
            listStatusOrder.ItemsSource = db.StatusOrder.ToList();
            listStatusOrder.DisplayMemberPath = "NameStatusOrder";
        }

        // === ДОБАВЛЕНИЕ ТИПОВ УКРАШЕНИЙ ===
        private void AddJewelryTip_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TipNameTxt.Text))
            {
                MessageBox.Show("Введите название типа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TipNameTxt.Focus();
                TipNameTxt.SelectAll();
                return;
            }

            if (TipNameTxt.Text.Length < 2)
            {
                MessageBox.Show("Название типа должно содержать не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TipNameTxt.Focus();
                return;
            }

            try
            {
                db.JewelryTip.Add(new JewelryTip
                {
                    NameJewelryTip = TipNameTxt.Text.Trim()
                });

                db.SaveChanges();
                LoadJewelryTipList();

                TipNameTxt.Clear();
                TipNameTxt.Focus();
                MessageBox.Show("✅ Тип украшения добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // === ДОБАВЛЕНИЕ МАТЕРИАЛОВ ===
        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MatNameTxt.Text))
            {
                MessageBox.Show("Введите название материала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                MatNameTxt.Focus();
                return;
            }

            if (!int.TryParse(MatProbaTxt.Text, out int proba) || proba <= 0 || proba > 9999)
            {
                MessageBox.Show("Введите корректную пробу (целое число от 1 до 9999)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                MatProbaTxt.Focus();
                MatProbaTxt.SelectAll();
                return;
            }

            if (MatNameTxt.Text.Length < 2)
            {
                MessageBox.Show("Название материала должно содержать не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                MatNameTxt.Focus();
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
                LoadMaterialList();

                MatNameTxt.Clear();
                MatProbaTxt.Clear();
                MatNameTxt.Focus();
                MessageBox.Show($"✅ Материал '{MatNameTxt.Text.Trim()} ({proba})' добавлен!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message, "Ошибка");
            }
        }

        // === ДОБАВЛЕНИЕ КАМНЕЙ ===
        private void AddStone_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StoneNameTxt.Text))
            {
                MessageBox.Show("Введите название камня!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                StoneNameTxt.Focus();
                return;
            }

            if (!double.TryParse(StoneWeightTxt.Text, out double weight) || weight <= 0 || weight > 1000)
            {
                MessageBox.Show("Введите корректный вес камня (число от 0.01 до 1000)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                StoneWeightTxt.Focus();
                StoneWeightTxt.SelectAll();
                return;
            }

            if (StoneNameTxt.Text.Length < 2)
            {
                MessageBox.Show("Название камня должно содержать не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                StoneNameTxt.Focus();
                return;
            }

            try
            {
                db.Stone.Add(new Stone
                {
                    NameStone = StoneNameTxt.Text.Trim(),
                    ColorStone = StoneColorTxt.Text?.Trim(),
                    WeightStone = weight
                });

                db.SaveChanges();
                LoadStoneList();

                StoneNameTxt.Clear();
                StoneColorTxt.Clear();
                StoneWeightTxt.Clear();
                StoneNameTxt.Focus();
                MessageBox.Show("✅ Камень добавлен!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        // === ДОБАВЛЕНИЕ ПОСТАВЩИКОВ ===
        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SuppNameTxt.Text))
            {
                MessageBox.Show("Введите название поставщика!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                SuppNameTxt.Focus();
                return;
            }

            if (SuppPhoneTxt.Text?.Length > 0)
            {
                string phoneClean = SuppPhoneTxt.Text.Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "").Replace("-", "");
                if (phoneClean.Length < 10 || phoneClean.Length > 15 || !phoneClean.All(char.IsDigit))
                {
                    MessageBox.Show("Введите корректный телефон (минимум 10 цифр).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SuppPhoneTxt.Focus();
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(SuppEmailTxt.Text) && !IsValidEmail(SuppEmailTxt.Text))
            {
                MessageBox.Show("Введите корректный e‑mail адрес.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                SuppEmailTxt.Focus();
                return;
            }

            if (SuppNameTxt.Text.Length < 2)
            {
                MessageBox.Show("Название поставщика должно содержать не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                SuppNameTxt.Focus();
                return;
            }

            try
            {
                db.Supplier.Add(new Supplier
                {
                    NameSupplier = SuppNameTxt.Text.Trim(),
                    PhoneSupplier = SuppPhoneTxt.Text?.Trim(),
                    EmailSupplier = SuppEmailTxt.Text?.Trim()
                });

                db.SaveChanges();
                LoadSupplierList();

                SuppNameTxt.Clear();
                SuppPhoneTxt.Clear();
                SuppEmailTxt.Clear();
                SuppNameTxt.Focus();
                MessageBox.Show("✅ Поставщик добавлен!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        // === ДОБАВЛЕНИЕ СТАТУСОВ ЗАКАЗОВ ===
        private void AddStatusOrder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StatusNameTxt.Text))
            {
                MessageBox.Show("Введите название статуса!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusNameTxt.Focus();
                return;
            }

            if (StatusNameTxt.Text.Length < 2)
            {
                MessageBox.Show("Название статуса должно содержать не менее 2 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusNameTxt.Focus();
                return;
            }

            try
            {
                db.StatusOrder.Add(new StatusOrder
                {
                    NameStatusOrder = StatusNameTxt.Text.Trim()
                });

                db.SaveChanges();
                LoadStatusOrderList();

                StatusNameTxt.Clear();
                StatusNameTxt.Focus();
                MessageBox.Show("✅ Статус заказа добавлен!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }
        }

        // Валидация email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch { return false; }
        }
    }
}