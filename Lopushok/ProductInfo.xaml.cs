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
using System.Windows.Shapes;

namespace Lopushok
{
    /// <summary>
    /// Логика взаимодействия для ProductInfo.xaml
    /// </summary>
    public partial class ProductInfo : Window
    {
        public DataBase database { get; set; }
        public Product product { get; set; } = null;
        

        public ProductInfo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            productViewSource.Source = database.Product.ToList();
            System.Windows.Data.CollectionViewSource productTypeViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productTypeViewSource")));
            productTypeViewSource.Source = database.ProductType.ToList();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            //System.Windows.Data.
            // Загрузите данные, установив свойство CollectionViewSource.Source:
            // productViewSource.Source = [универсальный источник данных]
            
            // Загрузите данные, установив свойство CollectionViewSource.Source:
            // productTypeViewSource.Source = [универсальный источник данных]
        }
    }
}
