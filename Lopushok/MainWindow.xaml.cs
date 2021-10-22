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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lopushok
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase dataBase = new DataBase();
        List<Product> prdList = new List<Product>();// Список из 20 продктов для их отображения;
        List<Grid> gridList = new List<Grid>();// Список из 20 Grid в котрых будет отображятся информация о продуктах
        List<int> pages = new List<int>();//Список страниц
        List<string> filterList = new List<string>();
        Grid productGrid = new Grid();
        List<Product> prdListAll = new List<Product>();//содержит информацию о всех продуктах, нужен для одновременной поиска, сортировки и фильтрации
        List<Product> prdListFilt = new List<Product>();
        bool sortType = true;

        //StackPanel productPanel = new StackPanel();
        int nowPage;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            filterList.Add("Все типы");
            filterBox.Items.Add("-Все типы-");
            foreach (MaterialType materialType in dataBase.MaterialType)
            {
                filterList.Add(materialType.Title);
                filterBox.Items.Add(materialType.Title);
            }
            
            foreach (Product product in dataBase.Product)//заполнение списка продукции
            {
                prdListAll.Add(product);
            }
            createProductGrid(productGrid);
            PagesLoad();
            nowPage = pages[0];
            pageNumber.Text = $"{nowPage}/{pages.Count}";
            ProductListLoad(nowPage);
            panelFill();
        }
        private void panelFill()//Заполняте страницу стеками с продуктами
        {
            HookUpEventHandlers1();
            gridList.Clear();
            productPanel.Children.Clear();
            productPanel.Height = (productGrid.MaxHeight + 10) * 20;
            for (int i = 0; i < prdList.Count;i++)
            {
                gridList.Add(productGrid);
                gridList[i] = productGridFill(prdList[i]);
                productPanel.Children.Add(gridList[i]);
            }
            HookUpEventHandlers();
        }
        private Grid productGridFill(Product product) //Заполняет сетку информацией при этом эту сетку потом возварщает 
        {
            Grid grid = new Grid();
            createProductGrid(grid);//Создание макета сетки

            //Картинка
            if (product.Image.Length > 0)//Проверка на наличие картинки
            {
                Image image = new Image();
                BitmapImage image1 = new BitmapImage(new Uri($@"{Environment.CurrentDirectory}\{product.Image}"));// Так указывается полный путь к файлу, относительная ссылка почему-то не работает
                image.Stretch = Stretch.Uniform;
                image.Source = image1;
                Grid.SetColumn(image, 0);// задает в каком столбце в сетке будет находится
                Grid.SetRowSpan(image, 3);// объеденяет заданное кол-во строк для данного элемента
                grid.Children.Add(image);
            }
            else // картинка заглушка
            {
                Image image = new Image();
                BitmapImage image1 = new BitmapImage(new Uri($@"{Environment.CurrentDirectory}\products\picture.png"));
                image.Stretch = Stretch.Uniform;
                image.Source = image1;
                Grid.SetColumn(image, 0);
                Grid.SetRowSpan(image, 3);
                grid.Children.Add(image);
            }
            

            //Тип и название
            TextBlock txtTypeName = new TextBlock();
            txtTypeName.Margin = new Thickness(5,0,0,0);//отступ для красоты
            txtTypeName.Text = $"{product.ProductType.Title}|{product.Title}";
            txtTypeName.FontSize = 22;
            txtTypeName.FontWeight = FontWeights.Bold;
            txtTypeName.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(txtTypeName, 1);
            Grid.SetRow(txtTypeName, 0);// задает в какой строке в сетке будет находится
            grid.Children.Add(txtTypeName);

            //Артикул
            TextBlock txtArticle = new TextBlock();
            txtArticle.Margin = new Thickness(5, 0, 0, 0);
            txtArticle.Text = $"Артикул: {product.ArticleNumber}";
            txtArticle.FontSize = 16;
            Grid.SetColumn(txtArticle, 1);
            Grid.SetRow(txtArticle, 1);
            grid.Children.Add(txtArticle);

            //Материалы
            TextBlock txtMaterials = new TextBlock();
            txtMaterials.Margin = new Thickness(5, 0, 0, 0);
            string mtrList = "";
            foreach (ProductMaterial productMaterial in product.ProductMaterial)// оно так работает, там в принципе можно найти, что угодно, если оно как-либо связанно
            {
                mtrList += $"{productMaterial.Material.Title}; ";
            }
            txtMaterials.Text = mtrList;
            txtMaterials.FontSize = 16;
            txtMaterials.TextWrapping = TextWrapping.Wrap;
            Grid.SetColumn(txtMaterials, 1);
            Grid.SetRow(txtMaterials, 2);
            grid.Children.Add(txtMaterials);

            //Цена
            TextBlock txtPrice = new TextBlock();
            txtPrice.Text = $"Стоимость:\n{product.MinCostForAgent} руб.";
            txtPrice.FontSize = 22;
            txtPrice.TextAlignment = TextAlignment.Center;
            txtPrice.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(txtPrice, 2);
            Grid.SetRowSpan(txtPrice, 3);
            grid.Children.Add(txtPrice);

            //
            return grid;

        }
        private void createProductGrid(Grid prdGrid)//Создает сетку в которой будет хранится информация о продукте
        {
            //Цвет
            prdGrid.Background = Brushes.White;

            //Высота
            prdGrid.MaxHeight = 150;
            prdGrid.MinHeight = 150;

            //Ширина

            //Рамки
            prdGrid.Margin = new Thickness(0,5,0,5);

            //Строки
            RowDefinition rowDef1 = new RowDefinition();
            rowDef1.Height = new GridLength(30, GridUnitType.Star);// Здесь задается ширина в пропорциях, то есть здесь длина будет состовлять 30% от всех длины строки
            RowDefinition rowDef2 = new RowDefinition();
            rowDef2.Height = new GridLength(20, GridUnitType.Star);
            RowDefinition rowDef3 = new RowDefinition();
            rowDef3.Height = new GridLength(50, GridUnitType.Star);
            prdGrid.RowDefinitions.Add(rowDef1);
            prdGrid.RowDefinitions.Add(rowDef2);
            prdGrid.RowDefinitions.Add(rowDef3);

            //Столбцы
            ColumnDefinition colDef1 = new ColumnDefinition();
            colDef1.Width = new GridLength(20, GridUnitType.Star);
            ColumnDefinition colDef2 = new ColumnDefinition();
            colDef2.Width = new GridLength(60, GridUnitType.Star);
            ColumnDefinition colDef3 = new ColumnDefinition();
            colDef3.Width = new GridLength(20, GridUnitType.Star);
            prdGrid.ColumnDefinitions.Add(colDef1);
            prdGrid.ColumnDefinitions.Add(colDef2);
            prdGrid.ColumnDefinitions.Add(colDef3);
        }
        private void PagesLoad()//Загружает кол-во страниц
        {
            pages.Clear();
            int productCount = prdListAll.Count();
            int pageNumber = 0;
            while (productCount > 0)
            {
                pageNumber++;
                productCount -= 20;
                pages.Add(pageNumber);
            }
        }
        private void ProductListLoad(int page)//Загружает 20 продуктов, которые будут отображаться на выбранной странице
        {
            prdList.Clear();
            int prdListCount = 0;
            foreach (Product product in prdListAll)
            {
                prdListCount++;
                if (prdListCount > page * 20)
                {
                    break;
                }
                else if ((prdListCount > ((page * 20)-20)) && (prdListCount <= page*20))
                {
                    prdList.Add(product);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) //пролистывает страницы <--
        {
            if (nowPage == pages.First())
            {
                return;
            }
            else
            {
                nowPage -= 1;
                pageNumber.Text = $"{nowPage}/{pages.Count}";
                ProductListLoad(nowPage);
                panelFill();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)//пролистывает страницы -->
        {
            if (nowPage == pages.Last())
            {
                return;
            }
            else
            {
                nowPage += 1;
                pageNumber.Text = $"{nowPage}/{pages.Count}";
                ProductListLoad(nowPage);
                panelFill();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            prdListAll.Clear();
            foreach (Product product in dataBase.Product)
            {
                if (product.Title.ToLower().Contains(searchBox.Text.ToLower()))
                {
                    if (filterBox.SelectedIndex > 0)
                    {
                        Filter(product);
                    }
                    else
                    {
                        prdListAll.Add(product);
                    }
                }
            }
            if (sortBox.SelectedIndex > 0)
            {
                Sort();
            }
            PagesLoad();
            ProductListLoad(nowPage);
            if (prdListAll.Count > 0)
                pageNumber.Text = $"{nowPage}/{pages.Count}";
            else
                pageNumber.Text = $"0/{pages.Count}";
            panelFill();


        }

        private void Sort()
        {
            switch (sortBox.SelectedIndex)
            {
                case 0:
                    if (sortType == true)
                    {
                        prdListAll.Sort((x, y) => string.Compare(x.Title, y.Title));
                    }
                    else if (sortType == false)
                    {
                        prdListAll.Sort((x, y) => y.Title.CompareTo(x.Title));
                    }
                    ProductListLoad(nowPage);
                    panelFill();
                    break;
                case 1:
                    if (sortType == true)
                    {
                        prdListAll.Sort((x, y) => x.ProductionWorkshopNumber.ToString().CompareTo(y.ProductionWorkshopNumber.ToString()));
                    }
                    else if (sortType == false)
                    {
                        prdListAll.Sort((x, y) => y.ProductionWorkshopNumber.ToString().CompareTo(x.ProductionWorkshopNumber.ToString()));
                    }
                    ProductListLoad(nowPage);
                    panelFill();
                    break;
                case 2:
                    if (sortType == true)
                    {
                        prdListAll.Sort((x, y) => x.MinCostForAgent.CompareTo(y.MinCostForAgent));
                    }
                    else if (sortType == false)
                    {
                        prdListAll.Sort((x, y) => y.MinCostForAgent.CompareTo(x.MinCostForAgent));
                    }
                    ProductListLoad(nowPage);
                    panelFill();
                    break;
            }
        }

        private void sortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sort();
        }

        private void btnUpDown_Click(object sender, RoutedEventArgs e)
        {
            if (sortType == true)
            {
                sortType = false;
                btnUpDown.Content = "▼";
            }
            else if (sortType == false)
            {
                sortType = true;
                btnUpDown.Content = "▲";
            }
            Sort();
        }

        private void filterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            prdListAll.Clear();
            foreach (Product product in dataBase.Product)
            {
                if (product.Title.ToLower().Contains(searchBox.Text.ToLower()))
                {
                    if (filterBox.SelectedIndex != 0)
                    {
                        Filter(product);
                    }
                    else
                    {
                        prdListAll.Add(product);
                    }
                }
            }
            if (sortBox.SelectedIndex > 0)
            {
                Sort();
            }
            PagesLoad();
            ProductListLoad(nowPage);
            if (prdListAll.Count > 0)
                pageNumber.Text = $"{nowPage}/{pages.Count}";
            else
                pageNumber.Text = $"0/{pages.Count}";
            panelFill();


        }

        private void Filter(Product product)
        {
            bool mtrCheck;
            if (filterBox.SelectedIndex > 0)
            {
                mtrCheck = false;
                foreach (ProductMaterial productMaterial in product.ProductMaterial)
                {
                    if (productMaterial.Material.MaterialType.Title == filterBox.SelectedItem.ToString())
                    {
                        mtrCheck = true;
                    }
                }
                if (mtrCheck == true)
                {
                    prdListAll.Add(product);
                }
            }
        }
        private void HookUpEventHandlers1()
        {
            foreach (var p in gridList)
            {
                p.MouseDown -= P_MouseDown;
            }
        }
        private void HookUpEventHandlers()
        {
            foreach (var p in gridList)
            {
                p.MouseDown += P_MouseDown;
            }
        }

        private void P_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProductInfo productInfo = new ProductInfo();
            productInfo.Show();
            this.Hide();
        }
    }
}
