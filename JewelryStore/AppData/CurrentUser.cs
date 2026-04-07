using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JewelryStore.AppData
{
    public static class CurrentUser
    {
        public static int? IdUser { get; set; }
        public static string Login { get; set; }
        public static int IdRole { get; set; }
        public static bool IsAdmin => IdRole == 2;
    }

    public static class ShoppingCart
    {
        private static JewelryStoreEntities GetNewContext()
        {
            return new JewelryStoreEntities();
        }

        // Добавление товара в корзину
        public static void AddItem(int jewelryId, int quantity = 1)
        {
            if (CurrentUser.IdUser == 0)
            {
                MessageBox.Show("Войдите в аккаунт для добавления в корзину!");
                return;
            }

            using (var db = GetNewContext())
            {
                var existing = db.CartItem
                    .FirstOrDefault(c => c.IdUser == CurrentUser.IdUser && c.IdJewelry == jewelryId);

                if (existing != null)
                    existing.Quantity += quantity;
                else
                {
                    db.CartItem.Add(new CartItem
                    {
                        IdUser = CurrentUser.IdUser,
                        IdJewelry = jewelryId,
                        Quantity = quantity
                    });
                }
                db.SaveChanges();
            }
        }

        // Получить все товары корзины
        public static List<CartItem> GetItems()
        {
            using (var db = GetNewContext())
            {
                return db.CartItem
                    .Where(c => c.IdUser == CurrentUser.IdUser)
                    .Include("Jewelry")
                    .Include("Jewelry.JewelryTip")
                    .Include("Jewelry.Material")
                    .Include("Jewelry.Stone")
                    .Include("Jewelry.Supplier")
                    .ToList();
            }
        }

        // ИТОГОВАЯ ЦЕНА (decimal для PriceJewelry)
        public static decimal GetTotalPrice()  // ← ОДИН метод!
        {
            using (var db = GetNewContext())
            {
                return db.CartItem
                    .Where(c => c.IdUser == CurrentUser.IdUser)
                    .Include(c => c.Jewelry)
                    .ToList()
                    .Sum(c => (decimal)c.Quantity * (c.Jewelry.PriceJewelry ?? 0m));
            }
        }

        // Удалить товар
        public static void RemoveItem(int cartItemId)
        {
            using (var db = GetNewContext())
            {
                var item = db.CartItem.Find(cartItemId);
                if (item != null)
                {
                    db.CartItem.Remove(item);
                    db.SaveChanges();
                }
            }
        }

        // Очистить корзину
        public static void Clear()
        {
            using (var db = GetNewContext())
            {
                var items = db.CartItem.Where(c => c.IdUser == CurrentUser.IdUser).ToList();
                db.CartItem.RemoveRange(items);
                db.SaveChanges();
            }
        }
        public static void UpdateQuantity(int cartItemId, int delta)
        {
            using (var db = GetNewContext())
            {
                var item = db.CartItem.Find(cartItemId);
                if (item == null) return;

                // Явно разыменовываем nullable
                int currentQty = item.Quantity ?? 1;
                int newQty = currentQty + delta;

                // Math.Min с int (НЕ int?)
                item.Quantity = Math.Min(99, Math.Max(1, newQty));

                db.SaveChanges();
            }
        }
    }
}
