using System.Windows;
using WpfAppPaper.DataBase;

namespace WpfAppPaper
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static WipingEntities1 DB = new WipingEntities1();

    }
}
