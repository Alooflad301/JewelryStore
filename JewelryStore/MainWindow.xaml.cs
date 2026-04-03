using JewelryStore.Pages;
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

namespace JewelryStore
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigated += MainFrame_Navigated;
            AppData.AppConnect.model0db = new AppData.JewelryStoreEntities();
            AppData.AppFrame.framemain = MainFrame;
            MainFrame.Navigate(new Pages.PageAftoriz());
            
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is PageReg)
            {
                this.MinWidth = 300;
                this.MinHeight = 500;
            }
            else if (e.Content is PageAftoriz)
            {
                this.MinWidth = 440;
                this.MinHeight = 420;
            }
            else if (e.Content is PageJewelryCatalog)
            {
                this.MinWidth = 1250;
                this.MinHeight = 450;
            }
            else if (e.Content is PageAddEditJewelry)
            {
                this.MinWidth = 800;
                this.MinHeight = 700;
            }
        }
    }
}
