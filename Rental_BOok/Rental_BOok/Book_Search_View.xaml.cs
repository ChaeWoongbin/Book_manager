using System;
using System.Collections.Generic;
using System.Data;
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
    /// Book_Search_View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Book_Search_View : UserControl
    {
        public Book_Search_View()
        {
            InitializeComponent();
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.MainMenu);
        }
        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Book_data_View);
        }

        private void Rental_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Book_Borrow_View);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sel_book = "";
            try
            {
                sel_book = ((ListBox)sender).SelectedItem.ToString();
            }
            catch
            {
                return;
            }

            string query = "select * from books where book_name = '{0}';";
            query = string.Format(query, sel_book);

            DataTable searchs = Data.DB_con.GetTable(query);

            Book_img.Source = new BitmapImage();
            Book_img.Stretch = Stretch.Uniform;



            // 리팩토링 시 DB값 지역변수 저장 필요

            try
            {
                Book_img.Source = new BitmapImage(new Uri(System.Environment.CurrentDirectory + @"\Books\" + searchs.Rows[0]["Image_Path"] + ".png"));
            }
            catch (Exception ex)
            {
                Book_img.Source = new BitmapImage(new Uri(@"Resource\iconmonstr-book-1-240.png", UriKind.Relative)); // 이미지 찾기 실패시 기본 이미지 표시
            }
            lblname.Content = searchs.Rows[0]["Book_name"];
            lblgenre.Content = searchs.Rows[0]["Book_Genre"];
            lblauthor.Content = searchs.Rows[0]["Book_author"];
            if (searchs.Rows[0]["Rental_User_ID"].ToString() == "1")
            {
                lblstate.Content = "대여가능";
            }
            else
            {
                string query_history = "SELECT DATE_ADD(Rental_Date, INTERVAL 7 DAY) from books_history where book_name = '{0}';";
                query_history = string.Format(query_history, sel_book);

                string return_date = (Data.DB_con.read_str(query_history)).Substring(0, 10);

                lblstate.Content = "~" + return_date;
            }

        }

        private void btn_search(object sender, RoutedEventArgs e)
        {
            search_list.Items.Clear();

            if (search_text.Text == "")
            {
                MessageBox.Show("검색어를 확인해주세요");
                return;
            }

            string target = search_text.Text;

            string query = "select book_name from books where book_name like '%{0}%';";
            query = string.Format(query, target);

            DataTable searchs = Data.DB_con.GetTable(query);

            foreach (DataRow element in searchs.Rows)
            {
                search_list.Items.Add(element["book_name"]);
            }
        }
    }
}
