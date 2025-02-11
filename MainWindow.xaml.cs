using System;
using System.Collections.Generic;
using System.IO;
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
using WpfAppPaper.Pages;

namespace WpfAppPaper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current;
        public MainWindow()
        {
            InitializeComponent();
            if (Current == null)
                Current = this;
            else return;
            Navigate(new ProductListPage());
        }

        public void Navigate(Page page)=>
            MainFrame.NavigationService.Navigate(page);
    }
}
