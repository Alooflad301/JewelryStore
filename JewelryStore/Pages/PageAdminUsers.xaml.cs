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
    /// Логика взаимодействия для PageAdminUsers.xaml
    /// </summary>
    public partial class PageAdminUsers : Page
    {
        public PageAdminUsers()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new JewelryStoreEntities())
            {
                gridUsers.ItemsSource = db.User
                    .Include("Role")
                    .OrderBy(u => u.Login)
                    .ToList();
                tbStatus.Text = $"Пользователей: {db.User.Count()}";
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Простое добавление (диалог или новая страница)
            var newUser = new User
            {
                Login = "newuser",
                Password = "123",
                Phone = "+7(999)000-00-00",
                Email = "new@jewelry.ru",
                IdRole = 1
            };

            using (var db = new JewelryStoreEntities())
            {
                db.User.Add(newUser);
                db.SaveChanges();
                LoadUsers();
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int userId))
            {
                // Сохраняем изменения из DataGrid
                using (var db = new JewelryStoreEntities())
                {
                    var user = db.User.Find(userId);
                    if (user != null)
                    {
                        // Обновляем из gridUsers.SelectedItem или диалога
                        db.SaveChanges();
                        MessageBox.Show("✅ Пользователь обновлён!");
                        LoadUsers();
                    }
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int userId) &&
                MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new JewelryStoreEntities())
                {
                    var user = db.User.Find(userId);
                    if (user != null && user.IdUser != 1)  // Не удаляем админа!
                    {
                        db.User.Remove(user);
                        db.SaveChanges();
                        LoadUsers();
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }
    }
}
