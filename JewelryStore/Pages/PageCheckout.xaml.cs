using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JewelryStore;
using JewelryStore.AppData;

namespace JewelryStore.Pages
{
    public partial class PageCheckout : Page
    {
        private List<CartItem> cartItems;

        public PageCheckout()
        {
            InitializeComponent();
            LoadCart();
        }

        private void LoadCart()
        {
            cartItems = ShoppingCart.GetItems();
            listCartPreview.ItemsSource = cartItems;
            tbTotalPreview.Text = $"Итого: {ShoppingCart.GetTotalPrice():N0} руб.";
        }

        private void ConfirmOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(CustomerNameText.Text) ||
                string.IsNullOrWhiteSpace(PhoneText.Text) ||
                string.IsNullOrWhiteSpace(EmailText.Text) ||
                string.IsNullOrWhiteSpace(AddressText.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            using (var db = ShoppingCart.GetNewContext())
            {
                // Создаём заказ (поля из БД)
                var order = new Order
                {
                    IdUser = AppData.CurrentUser.IdUser,
                    CustomerName = CustomerNameText.Text,
                    CustomerPhone = PhoneText.Text,      // ← CustomerPhone!
                    CustomerEmail = EmailText.Text,      // ← CustomerEmail!
                    DeliveryAddress = AddressText.Text,
                    OrderDate = DateTime.Now,
                    IdStatusOrder = 1,  // "Новый"
                    TotalPrice = (int)ShoppingCart.GetTotalPrice()  // int из БД
                };
                db.Order.Add(order);
                db.SaveChanges();

                // Копируем товары из корзины → OrderItem
                foreach (var cartItem in cartItems)
                {
                    db.OrderItem.Add(new OrderItem
                    {
                        IdOrder = order.IdOrder,
                        IdJewelry = cartItem.IdJewelry.Value,
                        Quantity = cartItem.Quantity,
                        UnitPrice = (int)(cartItem.Jewelry.PriceJewelry ?? 0),  // int!
                        TotalPrice = (int)(cartItem.Quantity * (cartItem.Jewelry.PriceJewelry ?? 0))
                    });
                }
                db.SaveChanges();

                // Очищаем корзину
                ShoppingCart.Clear();
            }

            MessageBox.Show($"✅ Заказ оформлен!\nТелефон: {PhoneText.Text}");
            AppFrame.framemain.Navigate(new PageUserOrders());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageCart());
        }
    }
}