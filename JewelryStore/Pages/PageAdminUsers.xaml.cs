using JewelryStore.AppData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JewelryStore.Pages
{
    public partial class PageAdminUsers : Page
    {
        private JewelryStoreEntities db;
        public ObservableCollection<User> UsersList { get; set; }
        public ObservableCollection<Role> RoleList { get; set; }
        private bool _isEdited = false;

        public PageAdminUsers()
        {
            InitializeComponent();
            db = AppConnect.model0db;
            LoadData();
        }

        private void LoadData()
        {
            UsersList = new ObservableCollection<User>(db.User.Include("Role").ToList());
            RoleList = new ObservableCollection<Role>(db.Role.ToList());
            UsersGrid.ItemsSource = UsersList;
            _isEdited = false;
            SaveBtn.IsEnabled = false;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            MessageBox.Show("Список пользователей обновлён!");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SaveChanges();
                MessageBox.Show("Изменения пользователей сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser && selectedUser.IdUser != CurrentUser.IdUser)
            {
                if (MessageBox.Show($"Удалить пользователя {selectedUser.Login}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.User.Remove(selectedUser);
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Пользователь удалён!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите другого пользователя (не себя)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UsersGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _isEdited = true;
            SaveBtn.IsEnabled = true;
        }

        private void UsersGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
        }

        private void UsersGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (_isEdited) SaveBtn.IsEnabled = true;
        }
    }
}