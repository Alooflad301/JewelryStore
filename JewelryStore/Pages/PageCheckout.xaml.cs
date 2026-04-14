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
            if (Window.GetWindow(this) is Window window)
            {
                window.MinWidth = 900;
                window.MinHeight = 650;
            }
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
            if (!ValidatePhone(PhoneText.Text))
            {
                MessageBox.Show("Введите корректный телефон (минимум 10 цифр).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneText.Focus();
                return;
            }

            if (!ValidateEmail(EmailText.Text))
            {
                MessageBox.Show("Введите корректный e‑mail.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailText.Focus();
                return;
            }

            // Проверка, что в корзине есть товары
            var cartItems = ShoppingCart.GetItems();
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста! Добавьте хотя бы один товар перед оформлением заказа.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                AppFrame.framemain.Navigate(new PageCart());
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

        private bool ValidatePhone(string phone)
        {
            phone = phone?.Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "").Replace("-", "");
            return !string.IsNullOrEmpty(phone) && phone.Length >= 10 && phone.Length <= 15 && phone.All(char.IsDigit);
        }

        // Простая валидация email
        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch { return false; }
        }

        private void GeneratePDFReceipt(int orderId, string customerName, string phone, decimal totalPrice)
        {
            try
            {
                string ordersFolder = Path.Combine(GetProjectFolder(), "Orders");
                Directory.CreateDirectory(ordersFolder);
                string pdfPath = Path.Combine(ordersFolder, $"чек_заказ_{orderId}.pdf");

                // Загружаем товары заказа из БД
                using (var db = ShoppingCart.GetNewContext())
                {
                    var orderItems = db.OrderItem
                        .Where(o => o.IdOrder == orderId)
                        .Select(o => new
                        {
                            Name = o.Jewelry.NameJewelry ?? "<без имени>",
                            Quantity = o.Quantity ?? 1,
                            Price = o.UnitPrice ?? 0m,
                            TotalPrice = o.TotalPrice ?? 0m
                        })
                        .ToList();

                    // QR код (маленький)
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                        $"Заказ #{orderId}\\n{customerName}\\n{phone}\\n{totalPrice:N0} руб.",
                        QRCodeGenerator.ECCLevel.M);

                    PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                    byte[] qrCodeImageBytes = qrCode.GetGraphic(10); // Меньший размер!

                    // Русский шрифт
                    string fontPath = @"C:\Windows\Fonts\arial.ttf"; // Arial с кириллицей
                    BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                    Document document = new Document(PageSize.A4, 40, 40, 60, 60);
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));
                    document.Open();

                    iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 22, Font.BOLD);
                    iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 16, Font.BOLD);
                    iTextSharp.text.Font normalFont = new iTextSharp.text.Font(baseFont, 13);
                    iTextSharp.text.Font totalFont = new iTextSharp.text.Font(baseFont, 20, Font.BOLD);

                    // Заголовок
                    Paragraph title = new Paragraph("ЮВЕЛИРНЫЙ МАГАЗИН", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20
                    };
                    
                    document.Add(title);
                    
                    Paragraph title2 = new Paragraph("'The Stones of Eden'", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20
                    };
                    document.Add(title2);

                    Paragraph orderTitle = new Paragraph($"ЧЕК № {orderId}", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 10
                    };
                    document.Add(orderTitle);

                    // Данные клиента
                    document.Add(new Paragraph($"Клиент: {customerName}", normalFont) { SpacingAfter = 5 });
                    document.Add(new Paragraph($"Телефон: {phone}", normalFont) { SpacingAfter = 5 });
                    document.Add(new Paragraph($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm}", normalFont) { SpacingAfter = 20 });

                    // Заголовок таблицы товаров
                    Paragraph tableHeader = new Paragraph("Товары в заказе", headerFont)
                    {
                        SpacingAfter = 10
                    };
                    document.Add(tableHeader);

                    // Таблица: Наименование | Количество | Цена за шт. | Сумма
                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 3f, 1f, 1f, 1f });

                    // Заголовки столбцов
                    table.AddCell(new PdfPCell(new Phrase("Товар(ы)", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase("Кол‑во", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase("Цена", normalFont)));
                    table.AddCell(new PdfPCell(new Phrase("Сумма", normalFont)));

                    foreach (var item in orderItems)
                    {
                        table.AddCell(new PdfPCell(new Phrase(item.Name, normalFont)));
                        table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), normalFont)));
                        table.AddCell(new PdfPCell(new Phrase($"{item.Price:N0} ₽", normalFont)));
                        table.AddCell(new PdfPCell(new Phrase($"{item.TotalPrice:N0} ₽", normalFont)));
                    }

                    document.Add(table);

                    // Итоговая сумма заказа
                    Paragraph total = new Paragraph($"ИТОГО: {totalPrice:N0} ₽", totalFont)
                    {
                        Alignment = Element.ALIGN_RIGHT,
                        SpacingBefore = 10,
                        SpacingAfter = 20
                    };
                    document.Add(total);

                    // QR код
                    iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(qrCodeImageBytes);
                    qrImage.ScaleAbsolute(120f, 120f);
                    qrImage.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    document.Add(qrImage);

                    // Подвал
                    iTextSharp.text.Font footerFont = new iTextSharp.text.Font(baseFont, 12, Font.ITALIC);
                    Paragraph footer = new Paragraph("Спасибо за покупку!\nСканируйте QR‑код для проверки заказа", footerFont)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    document.Add(footer);

                    document.Close();

                    // Автооткрытие PDF
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(pdfPath)
                    {
                        UseShellExecute = true
                    });
                }
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