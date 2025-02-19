using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfAppPaper.DataBase;
using WpfAppPaper.UserControlls;

namespace WpfAppPaper.Pages
{
    public partial class ProductListPage : Page
    {
        private IEnumerable<Products> _allProducts;
        private IEnumerable<Products> _filteredProducts;

        private int _currentPage = 0;

        private const string AllTypes = "Все типы";
        private const int PageSize = 20;

        public ProductListPage()
        {
            InitializeComponent();
            LoadProducts();
            LoadTypeFilterComboBox();
            RefreshList();
        }

        private void LoadTypeFilterComboBox()
        {
            List<ProductType> materialTypes = App.DB.ProductType.ToList();
            ProductType defaultType = new ProductType()
            {
                Name = AllTypes
            };
            materialTypes.Insert(0, defaultType);
            TypeFilterCB.ItemsSource = materialTypes;
            TypeFilterCB.DisplayMemberPath = "Name";
            TypeFilterCB.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            _allProducts = App.DB.Products.ToList();
            _filteredProducts = _allProducts;
        }

        private void RefreshList()
        {
            ProductWrap.Children.Clear();

            var productsToShow = _filteredProducts
                .Skip(_currentPage * PageSize)
                .Take(PageSize)
                .ToList();

            foreach (var product in productsToShow)
            {
                ProductWrap.Children.Add(new ProductUserControll(product));
            }

            UpdateNavigationButtons();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if ((_currentPage + 1) * PageSize < _allProducts.Count())
            {
                _currentPage++;
                RefreshList();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                RefreshList();
            }
        }

        private void UpdateNavigationButtons()
        {
            PrevPageButton.IsEnabled = _currentPage > 0;
            NextPageButton.IsEnabled = (_currentPage + 1) * PageSize < _allProducts.Count();
        }

        private void NameSearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchText();
        }

        private void SearchText()
        {
            string searchText = NameSearchTB.Text.ToLower().Trim();

            _filteredProducts = _allProducts.Where(x =>
                $"{x.ProductName}".ToLower().Contains(searchText)
            );
            RefreshList();
        }

        private void TypeFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductType selectedProductType = TypeFilterCB.SelectedItem as ProductType;
            if (selectedProductType.Name == AllTypes)
                SearchText();
            else
                _filteredProducts = _allProducts.Where(x => x.ProductTypeID == (selectedProductType).ID);
            RefreshList();
        }

        private void AscendingSortCheckB_Checked(object sender, RoutedEventArgs e)
        {
            SortByUniq();
        }

        private void SortListCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortByUniq();
        }

        private void SortByUniq()
        {
            if (SortListCB.SelectedIndex == 0)
            {
                SearchText();
            }
            if (SortListCB.SelectedIndex == 1)
            {
                if (AscendingSortCheckB.IsChecked == true)
                    _filteredProducts = _allProducts.OrderBy(x => x.ProductName).ToList();
                else
                    _filteredProducts = _allProducts.OrderByDescending(x => x.ProductName).ToList();
            }
            if (SortListCB.SelectedIndex == 2)
            {
                if (AscendingSortCheckB.IsChecked == true)

                    _filteredProducts = _allProducts.OrderBy(x => x.Price).ToList();
                else
                    _filteredProducts = _allProducts.OrderByDescending(x => x.Price).ToList();

            }
            RefreshList();
        }
    }
}
