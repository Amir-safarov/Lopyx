using System.IO;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAppPaper.DataBase;
using System.Collections.Generic;
using System.Windows;
using System.Text;

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

            MaterialsCB.ItemsSource = App.DB.Materials.ToList();
            MaterialsCB.DisplayMemberPath = "MaterialName";

            if (_product == null)
                AddMaterial.Visibility = Visibility.Hidden;
        }

        private void UpdateProdData()
        {
            ProdIMG.Source = GetimageSources(_product.ProductImage);
            ArticlProdTB.Text = _product.Article;
            TypeProdCB.SelectedItem = _product.ProductType;
            NameProdTB.Text = $"{_product.ProductName}";
            UpdateMaterials();
            PriceProdTB.Text = _product.Price.ToString();
        }

        private void UpdateMaterials()
        {
            List<Materials> materials = App.DB.Products
                .Where(p => p.ProductID == _product.ProductID)
                    .Join(App.DB.Warehouse, p => p.ProductID, w => w.ProductID, (p, w) => w)
                    .Join(App.DB.Materials, w => w.MaterialID, m => m.MaterialID, (w, m) => m)
                    .ToList();
            MaterialProdList.ItemsSource = materials;
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

        private void AddMaterial_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddNewMaterial();
        }

        private void AddNewMaterial()
        {
            Materials selectedMaterial = MaterialsCB.SelectedItem as Materials;
            if (selectedMaterial == null)
                return;
            Warehouse warehouse = new Warehouse()
            {
                ProductID = _product.ProductID,
                MaterialID = selectedMaterial.MaterialID,
                Quantity = 1
            };
            App.DB.Warehouse.Add(warehouse);
            UpdateMaterials();
            //App.DB.SaveChanges();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_product == null)
            {
                //TODO сделаит доавбление
                StringBuilder stringBuilder = new StringBuilder();
                if()
            }
            else
            {
                App.DB.SaveChanges();
                MessageBox.Show("Запись обновленны");
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductListPage());
        }

        private void EditProdIMGBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
