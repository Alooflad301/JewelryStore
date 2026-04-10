using iTextSharp.text;
using iTextSharp.text.pdf;
using JewelryStore;
using JewelryStore.AppData;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Diagnostics;

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
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppData.CurrentUser.IdUser.HasValue)
            {
                using (var db = ShoppingCart.GetNewContext())
                {
                    var user = db.User.FirstOrDefault(u => u.IdUser == AppData.CurrentUser.IdUser);
                    if (user != null)
                    {
                        CustomerNameText.Text = user.Login;  // Или добавьте FullName в БД
                        PhoneText.Text = user.Phone ?? "";
                        EmailText.Text = user.Email ?? "";
                    }
                }
            }

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
                GeneratePDFReceipt(order.IdOrder, CustomerNameText.Text, PhoneText.Text, (decimal)ShoppingCart.GetTotalPrice());

                MessageBox.Show($"✅ Заказ оформлен!\nЧек: Orders/чек_заказ_{order.IdOrder}.pdf");

                ShoppingCart.Clear();
            }

            MessageBox.Show($"✅ Заказ оформлен!\nТелефон: {PhoneText.Text}");
            AppFrame.framemain.Navigate(new PageUserOrders());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.framemain.Navigate(new PageCart());
        }
        private void GeneratePDFReceipt(int orderId, string customerName, string phone, decimal totalPrice)
        {
            try
            {
                string ordersFolder = Path.Combine(GetProjectFolder(), "Orders");
                Directory.CreateDirectory(ordersFolder);
                string pdfPath = Path.Combine(ordersFolder, $"чек_заказ_{orderId}.pdf");

                // QR код (маленький)
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                    $"Заказ #{orderId}\n{customerName}\n{phone}\n{totalPrice:N0} руб.",
                    QRCodeGenerator.ECCLevel.M);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImageBytes = qrCode.GetGraphic(10); // Меньший размер!

                // Русский шрифт
                string fontPath = @"C:\Windows\Fonts\arial.ttf"; // Arial с кириллицей
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                Document document = new Document(PageSize.A4, 40, 40, 60, 60);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));
                document.Open();

                // Заголовок
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 22, Font.BOLD);
                Paragraph title = new Paragraph("🍒 ЮВЕЛИРНЫЙ МАГАЗИН", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                // Номер заказа
                Paragraph orderTitle = new Paragraph("ЧЕК № " + orderId, titleFont);
                orderTitle.Alignment = Element.ALIGN_CENTER;
                orderTitle.SpacingAfter = 30;
                document.Add(orderTitle);

                // Данные (русский шрифт)
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 14);
                document.Add(new Paragraph("👤 Клиент: " + customerName, normalFont) { SpacingAfter = 5 });
                document.Add(new Paragraph("📞 Телефон: " + phone, normalFont) { SpacingAfter = 5 });
                document.Add(new Paragraph("📅 Дата: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), normalFont) { SpacingAfter = 20 });

                // Итоговая сумма
                iTextSharp.text.Font totalFont = new iTextSharp.text.Font(baseFont, 24, Font.BOLD);
                Paragraph total = new Paragraph("💰 ИТОГО К ОПЛАТЕ:\n" + totalPrice.ToString("N0") + " ₽", totalFont);
                total.Alignment = Element.ALIGN_CENTER;
                total.SpacingBefore = 10;
                total.SpacingAfter = 30;
                document.Add(total);

                // QR код (маленький, по центру)
                iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(qrCodeImageBytes);
                qrImage.ScaleAbsolute(120f, 120f); // Фиксированный размер 120x120
                qrImage.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                document.Add(qrImage);

                // Подвал
                iTextSharp.text.Font footerFont = new iTextSharp.text.Font(baseFont, 12, Font.ITALIC);
                Paragraph footer = new Paragraph("Спасибо за покупку!\nСканируйте QR-код для проверки заказа", footerFont);
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();

                // Автооткрытие PDF
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(pdfPath)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка PDF: " + ex.Message);
            }
        }
        private string GetProjectFolder()
{
    return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
}
    }
}