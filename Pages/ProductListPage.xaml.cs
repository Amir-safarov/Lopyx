using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private void LoadProducts()
        {
            _allProducts = App.DB.Products.ToList();
            _filteredProducts = _allProducts;
        }

        private void LoadTypeFilterComboBox()
        {
            var productTypes = App.DB.ProductType.ToList();
            productTypes.Insert(0, new ProductType { Name = AllTypes });

            TypeFilterCB.ItemsSource = productTypes;
            TypeFilterCB.DisplayMemberPath = "Name";
            TypeFilterCB.SelectedIndex = 0;
            SortListCB.SelectedIndex = 0;
            AscendingSortCheckB.IsChecked = true;
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
            UpdatePageButtons();
        }

        private void UpdateNavigationButtons()
        {
            PrevPageButton.IsEnabled = _currentPage > 0;
            NextPageButton.IsEnabled = (_currentPage + 1) * PageSize < _filteredProducts.Count();
        }

        private void UpdatePageButtons()
        {
            PageButtonsPanel.Children.Clear();

            int totalPages = (int)Math.Ceiling((double)_filteredProducts.Count() / PageSize);

            for (int i = 0; i < totalPages; i++)
            {
                Button button = new Button
                {
                    Content = (i + 1).ToString(),
                    Width = 30,
                    Margin = new Thickness(2),
                    Background = _currentPage == i ? Brushes.LightGray : Brushes.White,
                    BorderBrush = Brushes.Gray
                };

                int pageIndex = i;
                button.Click += (s, e) => GoToPage(pageIndex);

                PageButtonsPanel.Children.Add(button);
            }
        }

        private void GoToPage(int pageIndex)
        {
            _currentPage = pageIndex;
            RefreshList();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if ((_currentPage + 1) * PageSize < _filteredProducts.Count())
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

        private void NameSearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchProducts();
        }

        private void SearchProducts()
        {
            string searchText = NameSearchTB.Text.ToLower().Trim();

            _filteredProducts = _allProducts
                .Where(x => x.ProductName.ToLower().Contains(searchText))
                .ToList();

            RefreshList();
        }

        private void TypeFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = TypeFilterCB.SelectedItem as ProductType;
            if (selectedType.Name == AllTypes)
                SearchProducts();
            else
                _filteredProducts = _allProducts.Where(x => x.ProductTypeID == selectedType.ID);

            RefreshList();
        }

        private void AscendingSortCheckB_Checked(object sender, RoutedEventArgs e) => SortProducts();

        private void SortListCB_SelectionChanged(object sender, SelectionChangedEventArgs e) => SortProducts();

        private void SortProducts()
        {
            if (SortListCB.SelectedIndex == 1)
                _filteredProducts = AscendingSortCheckB.IsChecked == true ?
                    _filteredProducts.OrderBy(x => x.ProductName) :
                    _filteredProducts.OrderByDescending(x => x.ProductName);

            RefreshList();
        }

        private void AddNewProdBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditProduct());
        }
    }
}
