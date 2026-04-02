using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryStore.AppData
{
    public static class CurrentUser
    {
        public static int? IdUser { get; set; }
        public static string Login { get; set; }
        public static int IdRole { get; set; }
        public static bool IsAdmin => IdRole == 2;
    }
}
