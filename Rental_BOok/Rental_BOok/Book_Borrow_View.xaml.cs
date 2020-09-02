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

namespace Rental_BOok
{
    /// <summary>
    /// Book_Borrow_VIew.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Book_Borrow_View : UserControl
    {
        public Book_Borrow_View()
        {
            InitializeComponent();
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.MainMenu);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Book_Search_View);
        }

        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Book_data_View);
        }
    }
}
