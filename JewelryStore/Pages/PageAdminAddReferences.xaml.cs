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

        // Отображение существующих типов украшений
        private void LoadJewelryTipList()
        {
            var items = db.JewelryTip.Select(t => new
            {
                Id = t.IdJewelryTip,
                Name = t.NameJewelryTip
            }).ToList();

            listJewelryTip.ItemsSource = items;
            listJewelryTip.DisplayMemberPath = "Name";
            listJewelryTip.SelectedValuePath = "Id";
        }

        // Отображение материалов
        private void LoadMaterialList()
        {
            var items = db.Material.Select(m => new
            {
                Id = m.IdMaterial,
                Name = m.NameMaterial,
                Proba = m.Proba
            }).ToList();

            listMaterial.ItemsSource = items;
            listMaterial.DisplayMemberPath = "Name";
            listMaterial.SelectedValuePath = "Id";
        }

        // Отображение камней
        private void LoadStoneList()
        {
            var items = db.Stone.Select(s => new
            {
                Id = s.IdStone,
                Name = s.NameStone,
                Color = s.ColorStone,
                Weight = s.WeightStone
            }).ToList();

            listStone.ItemsSource = items;
            listStone.DisplayMemberPath = "Name";
            listStone.SelectedValuePath = "Id";
        }

        // Отображение поставщиков
        private void LoadSupplierList()
        {
            var items = db.Supplier.Select(s => new
            {
                Id = s.IdSupplier,
                Name = s.NameSupplier,
                Phone = s.PhoneSupplier,
                Email = s.EmailSupplier
            }).ToList();

            listSupplier.ItemsSource = items;
            listSupplier.DisplayMemberPath = "Name";
            listSupplier.SelectedValuePath = "Id";
        }

        // Отображение статусов заказов
        private void LoadStatusOrderList()
        {
            var items = db.StatusOrder.Select(s => new
            {
                Id = s.IdStatusOrder,
                Name = s.NameStatusOrder
            }).ToList();

            listStatusOrder.ItemsSource = items;
            listStatusOrder.DisplayMemberPath = "Name";
            listStatusOrder.SelectedValuePath = "Id";
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

            // Телефон необязательный, но если есть — проверяем
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

            // Email необязательный, но если есть — проверяем
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

        // === ОБНОВЛЕНИЕ КОНТЕКСТА БД (и списков) ===
        private void RefreshAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.Dispose();
                db = new JewelryStoreEntities();
                AppConnect.model0db = db;

                LoadJewelryTipList();
                LoadMaterialList();
                LoadStoneList();
                LoadSupplierList();
                LoadStatusOrderList();

                MessageBox.Show("✅ Контекст обновлён! Все изменения видны в других страницах.", "Обновлено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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