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
using Microsoft.Win32;
using System.IO;

namespace Lopushok
{
    /// <summary>
    /// Логика взаимодействия для ProductInfo.xaml
    /// </summary>
    public partial class ProductInfo : Window
    {
        public DataBase database { get; set; }
        public Product product { get; set; }
        List<ProductType> productTypes = new List<ProductType>();
        public MainWindow MainWindow { get; set; }

        

        public ProductInfo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (ProductType productType in database.ProductType)
            {
                productTypes.Add(productType);
                typeListBox.Items.Add(productType.Title);
            }
            if (product != null)
            {
                artTxtBox.Text = product.ArticleNumber;
                nameTxtBox.Text = product.Title;
                for (int i = 0; i < productTypes.Count; i++)
                {
                    if (productTypes[i].ID == product.ProductTypeID)
                    {
                        typeListBox.SelectedIndex = i;
                        break;
                    }
                }
                nbmTxtBox.Text = product.ProductionWorkshopNumber.ToString();
                prcntTxtBox.Text = product.ProductionPersonCount.ToString();
                if (product.Image != null && product.Image.Length != 0)//Проверка на наличие картинки
                {
                    BitmapImage image1 = new BitmapImage(new Uri($@"{Environment.CurrentDirectory}\{product.Image}"));// Так указывается полный путь к файлу, относительная ссылка почему-то не работает
                    imageBox.Stretch = Stretch.Uniform;
                    imageBox.Source = image1;
                }
                else // картинка заглушка
                {
                    BitmapImage image1 = new BitmapImage(new Uri($@"{Environment.CurrentDirectory}\products\picture.png"));
                    imageBox.Stretch = Stretch.Uniform;
                    imageBox.Source = image1;
                }
                PriceTxtBox.Text = product.MinCostForAgent.ToString();
                DesTxtBox.Text = product.Description;

            }
            this.Focus();
        }

        private void imageBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string fileName;
            string fileSavePath = Environment.CurrentDirectory + "\\products";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Выберите фото";
            ofd.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                fileName = ofd.SafeFileName;
                fileSavePath += $"\\{fileName}";
                BitmapImage image1 = new BitmapImage(new Uri($@"{ofd.FileName}"));// Так указывается полный путь к файлу, относительная ссылка почему-то не работает
                imageBox.Stretch = Stretch.Uniform;
                imageBox.Source = image1;
                if (ofd.FileName.Contains("\\products"))
                {
                    product.Image = $"\\products\\{ofd.SafeFileName}";
                }
                else
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageBox.Source));
                    using (FileStream stream = new FileStream(fileSavePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                        encoder.Save(stream);
                    product.Image = $"\\products\\{ofd.SafeFileName}";
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (product == null)
            {
                product = new Product();
                database.Product.Add(product);
            }
            try
            {
                product.ArticleNumber = artTxtBox.Text;
                product.Title = nameTxtBox.Text;
                if (typeListBox.SelectedIndex >= 0)
                    product.ProductTypeID = productTypes[typeListBox.SelectedIndex].ID;
                product.ProductionPersonCount = Convert.ToInt32(prcntTxtBox.Text);
                product.ProductionWorkshopNumber = Convert.ToInt32(nbmTxtBox.Text);
                product.Description = DesTxtBox.Text;
                product.MinCostForAgent = Convert.ToDecimal(PriceTxtBox.Text);
                database.SaveChanges();
                this.DialogResult = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка " + ex.Message);
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (product != null)
            {
                database.Product.Remove(product);
            }
            try
            {
                database.SaveChanges();
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка " + ex.Message);
            }
        }
    }
}
