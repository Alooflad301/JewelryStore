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
    /// Логика взаимодействия для PageJewelryCatalog.xaml
    /// </summary>
    public partial class PageJewelryCatalog : Page
    {
        public PageJewelryCatalog()
        {
            InitializeComponent();
            listProduct.ItemsSource = AppConnect.model0db.Jewelry.ToList();
            Fill();
            this.Loaded += Jc;
        }
        public void Fill()
        {
            ComdoSort.Items.Add("Цена");
            ComdoSort.Items.Add("По возрастанию цены");
            ComdoSort.Items.Add("По убыванию цены");
            ComdoSort.SelectedIndex = 0;
            ComboFilter.SelectedIndex = 0;
            var category = AppConnect.model0db.JewelryTip;
            ComboFilter.Items.Add("Тип украшения");
            foreach (var c in category)
            {
                ComboFilter.Items.Add(c.NameJewelryTip);
            }
            ComdoMat.SelectedIndex = 0;
            var categorya = AppConnect.model0db.Material;
            ComdoMat.Items.Add("Тип Материала");
            foreach (var a in categorya)
            {
                ComdoMat.Items.Add($"{a.NameMaterial}({a.Proba})");
            }
            ComdoStone.SelectedIndex = 0;
            var categoryb = AppConnect.model0db.Stone;
            ComdoStone.Items.Add("Тип Камня");
            foreach (var b in categoryb)
            {
                ComdoStone.Items.Add(b.NameStone);
            }
            ComdoSup.SelectedIndex = 0;
            var categorys = AppConnect.model0db.Supplier;
            ComdoSup.Items.Add("Бренд");
            foreach (var s in categorys)
            {
                ComdoSup.Items.Add(s.NameSupplier);
            }
        }
        public void Sbros()
        {
            ComdoSort.SelectedIndex = 0;
            ComboFilter.SelectedIndex = 0;
            ComdoMat.SelectedIndex = 0;
            ComdoStone.SelectedIndex = 0;
            ComdoSup.SelectedIndex = 0;
            TextSearch.Text = string.Empty;
        }
        Jewelry[] JewelriesList()
        {
            try
            {
                List<Jewelry> recipes = AppConnect.model0db.Jewelry.ToList();
                if (TextSearch != null)
                {
                    recipes = recipes.Where(x => x.NameJewelry.ToLower().Contains(TextSearch.Text.ToLower())).ToList();
                }
                if (ComboFilter.SelectedIndex > 0)
                {
                    switch (ComboFilter.SelectedIndex)
                    {
                        case 1:
                            recipes = recipes.Where(x => x.IdJewelryTip == 1).ToList();
                            break;
                        case 2:
                            recipes = recipes.Where(x => x.IdJewelryTip == 2).ToList();
                            break;
                        case 3:
                            recipes = recipes.Where(x => x.IdJewelryTip == 3).ToList();
                            break;
                        case 4:
                            recipes = recipes.Where(x => x.IdJewelryTip == 4).ToList();
                            break;
                        case 5:
                            recipes = recipes.Where(x => x.IdJewelryTip == 5).ToList();
                            break;
                        case 6:
                            recipes = recipes.Where(x => x.IdJewelryTip == 6).ToList();
                            break;
                        case 7:
                            recipes = recipes.Where(x => x.IdJewelryTip == 7).ToList();
                            break;
                        case 8:
                            recipes = recipes.Where(x => x.IdJewelryTip == 8).ToList();
                            break;
                        case 9:
                            recipes = recipes.Where(x => x.IdJewelryTip == 9).ToList();
                            break;
                        case 10:
                            recipes = recipes.Where(x => x.IdJewelryTip == 10).ToList();
                            break;
                    }
                }
                if (ComdoMat.SelectedIndex > 0)
                {
                    switch (ComdoMat.SelectedIndex)
                    {
                        case 1:
                            recipes = recipes.Where(x => x.IdMaterial == 1).ToList();
                            break;
                        case 2:
                            recipes = recipes.Where(x => x.IdMaterial == 2).ToList();
                            break;
                        case 3:
                            recipes = recipes.Where(x => x.IdMaterial == 3).ToList();
                            break;
                        case 4:
                            recipes = recipes.Where(x => x.IdMaterial == 4).ToList();
                            break;
                        case 5:
                            recipes = recipes.Where(x => x.IdMaterial == 5).ToList();
                            break;
                        case 6:
                            recipes = recipes.Where(x => x.IdMaterial == 6).ToList();
                            break;
                        case 7:
                            recipes = recipes.Where(x => x.IdMaterial == 7).ToList();
                            break;
                        case 8:
                            recipes = recipes.Where(x => x.IdMaterial == 8).ToList();
                            break;
                        case 9:
                            recipes = recipes.Where(x => x.IdMaterial == 9).ToList();
                            break;
                        case 10:
                            recipes = recipes.Where(x => x.IdMaterial == 10).ToList();
                            break;
                    }
                }
                if (ComdoStone.SelectedIndex > 0)
                {
                    switch (ComdoStone.SelectedIndex)
                    {
                        case 1:
                            recipes = recipes.Where(x => x.IdStone == 1).ToList();
                            break;
                        case 2:
                            recipes = recipes.Where(x => x.IdStone == 2).ToList();
                            break;
                        case 3:
                            recipes = recipes.Where(x => x.IdStone == 3).ToList();
                            break;
                        case 4:
                            recipes = recipes.Where(x => x.IdStone == 4).ToList();
                            break;
                        case 5:
                            recipes = recipes.Where(x => x.IdStone == 5).ToList();
                            break;
                        case 6:
                            recipes = recipes.Where(x => x.IdStone == 6).ToList();
                            break;
                        case 7:
                            recipes = recipes.Where(x => x.IdStone == 7).ToList();
                            break;
                        case 8:
                            recipes = recipes.Where(x => x.IdStone == 8).ToList();
                            break;
                        case 9:
                            recipes = recipes.Where(x => x.IdStone == 9).ToList();
                            break;
                        case 10:
                            recipes = recipes.Where(x => x.IdStone == 10).ToList();
                            break;
                    }
                }
                if (ComdoSup.SelectedIndex > 0)
                {
                    switch (ComdoSup.SelectedIndex)
                    {
                        case 1:
                            recipes = recipes.Where(x => x.IdSupplier == 1).ToList();
                            break;
                        case 2:
                            recipes = recipes.Where(x => x.IdSupplier == 2).ToList();
                            break;
                        case 3:
                            recipes = recipes.Where(x => x.IdSupplier == 3).ToList();
                            break;
                        case 4:
                            recipes = recipes.Where(x => x.IdSupplier == 4).ToList();
                            break;
                        case 5:
                            recipes = recipes.Where(x => x.IdSupplier == 5).ToList();
                            break;
                        case 6:
                            recipes = recipes.Where(x => x.IdSupplier == 6).ToList();
                            break;
                        case 7:
                            recipes = recipes.Where(x => x.IdSupplier == 7).ToList();
                            break;
                        case 8:
                            recipes = recipes.Where(x => x.IdSupplier == 8).ToList();
                            break;
                        case 9:
                            recipes = recipes.Where(x => x.IdSupplier == 9).ToList();
                            break;
                        case 10:
                            recipes = recipes.Where(x => x.IdSupplier == 10).ToList();
                            break;
                    }
                }
                if (ComdoSort.SelectedIndex > 0)
                {
                    switch (ComdoSort.SelectedIndex)
                    {
                        case 1:
                            recipes = recipes.OrderBy(x => x.PriceJewelry).ToList();
                            break;
                        case 2:
                            recipes = recipes.OrderByDescending(x => x.PriceJewelry).ToList();
                            break;
                    }
                }
                if (recipes.Count > 0)
                {
                    tbCounter.Text = "Найдено " + recipes.Count + " товар(ов)";

                }
                else
                {
                    tbCounter.Text = "Не найдено";
                }
                return recipes.ToArray();
            }
            catch
            {
                MessageBox.Show("Повторите попытку позже");
                return null;
            }
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listProduct.ItemsSource = JewelriesList();

        }

        private void ComdoSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listProduct.ItemsSource = JewelriesList();
        }

        private void TextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            listProduct.ItemsSource = JewelriesList();
        }

        private void ComdoMat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listProduct.ItemsSource = JewelriesList();
        }

        private void ComdoStone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listProduct.ItemsSource = JewelriesList();
        }

        private void ComdoSup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listProduct.ItemsSource = JewelriesList();
        }

        private void Jc(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window window)
            {
                this.MinWidth = 1350;
                this.MinHeight = 450;
            }
        }

        private void SbrosBut_Click(object sender, RoutedEventArgs e)
        {
            Sbros();
        }

        private void AdminPanelBottun_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppData.CurrentUser.IsAdmin)
            {
                AdminPanelBottun.Visibility = Visibility.Collapsed;
            }
        }

        private void AdminPanelBottun_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageAdminPanel());

        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag?.ToString(), out int jewelryId))
            {
                ShoppingCart.AddItem(jewelryId);
                MessageBox.Show("Добавлено в корзину!");
            }
        }

        private void CartBut_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageCart());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageUserOrders());
        }
    }
}
