using System;
using System.Collections.Generic;
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
            return new JewelryStoreEntities();  // Новый контекст каждый раз!
        }

        public static void AddItem(int jewelryId, int quantity = 1)
        {
            using (var db = GetNewContext())
            {
                var existing = db.CartItem.FirstOrDefault(c => c.IdUser == CurrentUser.IdUser && c.IdJewelry == jewelryId);
                if (existing != null)
                    existing.Quantity += quantity;
                else
                {
                    db.CartItem.Add(new CartItem { IdUser = CurrentUser.IdUser, IdJewelry = jewelryId, Quantity = quantity });
                }
                db.SaveChanges();
            }
        }
    }
}
