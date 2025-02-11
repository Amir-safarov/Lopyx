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
        private List<Products> _allProducts; 
        private int _currentPage = 0; 
        private const int PageSize = 20; 

        public ProductListPage()
        {
            InitializeComponent();
            LoadProducts();
            RefreshList();
        }

        private void LoadProducts()
        {
            _allProducts = App.DB.Products.ToList(); 
        }

        private void RefreshList()
        {
            ProductWrap.Children.Clear(); 

            var productsToShow = _allProducts
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
            if ((_currentPage + 1) * PageSize < _allProducts.Count)
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
            NextPageButton.IsEnabled = (_currentPage + 1) * PageSize < _allProducts.Count;
        }
    }
}
