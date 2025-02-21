using System.IO;
using System;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAppPaper.DataBase;
using System.Collections.Generic;

namespace WpfAppPaper.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Page
    {
        private Products _product;

        public AddEditProduct(Products product = null)
        {
            InitializeComponent();
            _product = product;
            LoadComponets();
            UpdateProdData();
        }

        private void LoadComponets()
        {
            TypeProdCB.ItemsSource = App.DB.ProductType.ToList();
            TypeProdCB.DisplayMemberPath = "Name";
        }

        private void UpdateProdData()
        {
            ProdIMG.Source = GetimageSources(_product.ProductImage);
            ArticlProdTB.Text = _product.Article;
            TypeProdCB.SelectedItem = _product.ProductType;
            NameProdTB.Text = $"{_product.ProductName}";
            List<Materials> materials = App.DB.Products
                .Where(p => p.ProductID == _product.ProductID)
                    .Join(App.DB.Warehouse, p => p.ProductID, w => w.ProductID, (p, w) => w)
                    .Join(App.DB.Materials, w => w.MaterialID, m => m.MaterialID, (w, m) => m)
                    .ToList();
            MaterialProdList.ItemsSource = materials;
            PriceProdTB.Text = _product.Price.ToString();
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
