using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using WpfAppPaper.DataBase;

namespace WpfAppPaper.UserControlls
{
    /// <summary>
    /// Логика взаимодействия для ProductUserControll.xaml
    /// </summary>
    public partial class ProductUserControll : UserControl
    {
        private Products _product;
        public ProductUserControll(Products product)
        {
            InitializeComponent();
            _product = product;
            UpdateData();
        }

        private void UpdateData()
        {
            ProdIMG.Source = GetimageSources(_product.ProductImage);
            ArticlProdTB.Text = _product.Article;
            TypeNameProdTB.Text = $"{_product.ProductType.Name} | {_product.ProductName}";
            IQueryable<Materials> materials = App.DB.Products
                .Where(p => p.ProductID == _product.ProductID)
                    .Join(App.DB.Warehouse, p => p.ProductID, w => w.ProductID, (p, w) => w)
                    .Join(App.DB.Materials, w => w.MaterialID, m => m.MaterialID, (w, m) => m);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Материалы: ");
            foreach (var s in materials)
                stringBuilder.Append(s.MaterialName + " ");
            MaterialProdTB.Text = $"{stringBuilder}";

            float prodPrice = 0;
            foreach (var s in materials)
                prodPrice += (float)s.Cost;
            if (prodPrice == 0)
                PriceProdTB.Text = _product.Price.ToString();
            else
                PriceProdTB.Text = prodPrice.ToString();
        }

        private BitmapImage GetimageSources(byte[] byteImage)
        {
            if (byteImage != null)
            {
                MemoryStream memoryStream = new MemoryStream(byteImage);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = memoryStream;
                image.EndInit();
                return image;
            }
            else
                return new BitmapImage(new Uri(@"\Resources\picture.png", UriKind.Relative));
        }
    }
}
