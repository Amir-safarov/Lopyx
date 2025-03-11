using System.IO;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAppPaper.DataBase;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;

namespace WpfAppPaper.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Page
    {
        private Products _product;
        private byte[] _prodImageData;
        public AddEditProduct(Products product = null)
        {
            InitializeComponent();
            _product = product;
            LoadComponets();
            UpdateProdViewData();
        }

        private void LoadComponets()
        {
            TypeProdCB.ItemsSource = App.DB.ProductType.ToList();
            TypeProdCB.DisplayMemberPath = "Name";

            MaterialsCB.ItemsSource = App.DB.Materials.ToList();
            MaterialsCB.DisplayMemberPath = "MaterialName";

            if (ProductIsNull())
            {
                AddMaterial.Visibility = Visibility.Hidden;
                RemoveSelectedMaterialBtn.Visibility = Visibility.Hidden;
                RemoveBtn.Visibility = Visibility.Collapsed;
            }

        }

        private void UpdateProdViewData()
        {
            if (_product == null)
                return;
            ProdIMG.Source = GetImageSources(_product.ProductImage);
            ArticlProdTB.Text = _product.Article;
            TypeProdCB.SelectedItem = _product.ProductType;
            NameProdTB.Text = $"{_product.ProductName}";
            UpdateMaterials();
            PriceProdTB.Text = _product.Price.ToString();
        }

        private void UpdateProdData()
        {
            if (_product == null)
                return;
            if (_prodImageData != null)
                _product.ProductImage = _prodImageData;
            _product.Article = ArticlProdTB.Text;
            _product.ProductTypeID = (TypeProdCB.SelectedItem as ProductType).ID;
            _product.ProductName = NameProdTB.Text;
            UpdateMaterials();
            _product.Price = decimal.Parse(PriceProdTB.Text);
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

        private BitmapImage GetImageSources(byte[] byteImage)
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
                return null;
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
            App.DB.SaveChanges();
            UpdateMaterials();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProductIsNull())
            {
                StringBuilder error = new StringBuilder();
                if (string.IsNullOrWhiteSpace(NameProdTB.Text))
                    error.AppendLine("Наименование продукта не ведено!");
                if ((TypeProdCB.SelectedItem as ProductType) == null)
                    error.AppendLine("Не выбран тип продукта!");
                if (string.IsNullOrWhiteSpace(PriceProdTB.Text))
                    error.AppendLine("Цена для поставщика не введена!");
                if (string.IsNullOrWhiteSpace(ArticlProdTB.Text))
                    error.AppendLine("Артикул продукта пуст!");
                if (error.Length > 0)
                {
                    MessageBox.Show($"{error}", "Ошибка");
                    return;
                }
                else
                {
                    Products products = new Products()
                    {
                        Price = int.Parse(PriceProdTB.Text),
                        Article = ArticlProdTB.Text,
                        ProductImage = _prodImageData,
                        ProductName = NameProdTB.Text,
                        ProductTypeID = (TypeProdCB.SelectedItem as ProductType).ID
                    };
                    App.DB.Products.Add(products);
                    App.DB.SaveChanges();
                    MessageBox.Show("Запись добавлена");
                }
            }
            else
            {
                UpdateProdData();
                App.DB.SaveChanges();
                MessageBox.Show("Запись обновленны");
            }
            NavigationService.Navigate(new ProductListPage());
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductListPage());
        }

        private void EditImage()
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "*.png|*.png|All files (*.*)|*.*"
            };
            if (openFile.ShowDialog().GetValueOrDefault())
            {
                _prodImageData = File.ReadAllBytes(openFile.FileName);
                ProdIMG.Source = new BitmapImage(new Uri(openFile.FileName)); ;
            }
            if (_product != null)
                App.DB.SaveChanges();
        }

        private void EditProdIMGBtn_Click(object sender, RoutedEventArgs e)
        {
            EditImage();
        }

        private void PriceProdTB_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
                e.Handled = true;
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProductIsNull())
                return;
            App.DB.Products.Remove(_product);
            App.DB.SaveChanges();
            MainWindow.Current.Navigate(new ProductListPage());
        }

        private bool ProductIsNull()
        {
            return _product == null;
        }

        private void RemoveSelectedMaterialBtn_Click(object sender, RoutedEventArgs e)
        {
            Materials selectedMaterial = MaterialProdList.SelectedItem as Materials;
            if (selectedMaterial == null)
                return;
            var warehouseRecord = App.DB.Warehouse.FirstOrDefault(w => w.ProductID == _product.ProductID && w.MaterialID == selectedMaterial.MaterialID);

            if (warehouseRecord != null)
            {
                App.DB.Warehouse.Remove(warehouseRecord);
                App.DB.SaveChanges();

                UpdateMaterials();
            }

        }
    }
}
