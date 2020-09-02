using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Admin_set.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Admin_set : UserControl
    {
        string User_ID;
        string Book_ID;
        string User_img_path;
        string Book_img_path;

        public Admin_set()
        {
            InitializeComponent();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.MainMenu);

        }
        private void User_del(object sender, RoutedEventArgs e)
        {
            User.Visibility = Visibility.Visible;
            Book_A.Visibility = Visibility.Hidden;
            Book_D.Visibility = Visibility.Hidden;

        }
        private void Book_Add(object sender, RoutedEventArgs e)
        {
            User.Visibility = Visibility.Hidden;
            Book_A.Visibility = Visibility.Visible;
            Book_D.Visibility = Visibility.Hidden;

        }

        private void Book_del(object sender, RoutedEventArgs e)
        {
            User.Visibility = Visibility.Hidden;
            Book_A.Visibility = Visibility.Hidden;
            Book_D.Visibility = Visibility.Visible;

        }

        // ----- user del -------------
        private void btn_search(object sender, RoutedEventArgs e)
        {
            search_list.Items.Clear();

            if (search_text.Text == "")
            {
                MessageBox.Show("검색어를 확인해주세요");
                return;
            }

            string target = search_text.Text;

            string query = "select User_name from users where User_name like '%{0}%';";
            query = string.Format(query, target);

            DataTable searchs = Data.DB_con.GetTable(query);

            foreach (DataRow element in searchs.Rows)
            {
                search_list.Items.Add(element["login_id"]);
            }
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

            string query = "select * from users where login_id = '{0}';";
            query = string.Format(query, sel_book);

            DataTable searchs = Data.DB_con.GetTable(query);

            user_img.Source = new BitmapImage();
            user_img.Stretch = Stretch.Uniform;
            

            // 리팩토링 시 DB값 지역변수 저장 필요
            try
            {
                user_img.Source = new BitmapImage(new Uri(System.Environment.CurrentDirectory + @"\User\" + Data.user.user_image));
            }
            catch
            {
                user_img.Source = new BitmapImage(new Uri(@"Resource\User_default.png", UriKind.Relative)); // 이미지 찾기 실패시 기본 이미지 표시
            }

            lblname.Content = searchs.Rows[0]["User_name"];
            lbladdress.Content = searchs.Rows[0]["User_address"];
            lblemail.Content = searchs.Rows[0]["User_Email"];
            User_ID = searchs.Rows[0]["User_ID"].ToString();
        }

        private void btn_all_search(object sender, RoutedEventArgs e)
        {
            search_list.Items.Clear();
            
            string query = "select login_id from users;";

            DataTable searchs = Data.DB_con.GetTable(query);

            foreach (DataRow element in searchs.Rows)
            {
                search_list.Items.Add(element["login_id"]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("삭제 하시겠습니까?", "메세지", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

            }
            else
            {
                return;
            }
            string target = User_ID;

            string query = "DELETE FROM users WHERE `User_ID`={0};";

            query = string.Format(query, target);

            Data.DB_con.write_query(query);

            // 이미지 삭제
            FileInfo fileDel = new FileInfo(System.Environment.CurrentDirectory + @"\User\" + Data.user.user_image);

            if (fileDel.Exists) // 삭제할 파일이 있는지
            {
                fileDel.Delete(); // 없어도 에러안남
            }


            // 삭제 후 재검색
            lblname.Content = "";
            lbladdress.Content = "";
            lblemail.Content = "";
            User_ID = "";

            search_list.Items.Clear();

            query = "select login_id from users;";

            DataTable searchs = Data.DB_con.GetTable(query);

            foreach (DataRow element in searchs.Rows)
            {
                search_list.Items.Add(element["login_id"]);
            }
        }
        
        // ---------- 책 추가 -------------
        private void get_img_button(object sender, RoutedEventArgs e)
        {
            // SAVE_PATH
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Image | *.jpg; *.png; *.jpeg; *.gif; *.bmp";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string selected = dialog.FileName;
                image.Stretch = Stretch.Uniform;
                image.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        private void add_book_button(object sender, RoutedEventArgs e)
        {
            if (new_book_name.Text.Length < 1)
            {
                MessageBox.Show("책 이름은 필수 입니다.");
                return;
            }

            string insert_query = "Insert into books (Book_name, Book_Genre, Book_author, Rental_User_ID, Image_path, Note) VALUES ( '"  + new_book_name.Text + "','" + new_book_genre.Text + "','" + new_book_author.Text + "','" + "1" + "','" + new_book_name.Text + "','" + new_book_note.Text + "') ";
            Clipboard.SetText(insert_query);
            Data.DB_con.write_query(insert_query);
            SaveAsPng(GetBitmapFromControl(image), new_book_name.Text);

            MessageBox.Show("완료되었습니다.");
        }

        public RenderTargetBitmap GetBitmapFromControl(Image view)
        {
            Size size = new Size(300, 400);
            if (size.IsEmpty)
                return null;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), size));
                context.Close();
            }

            result.Render(drawingvisual);

            return result;
        }

        public void SaveAsPng(RenderTargetBitmap src, string book)
        {
            string filePath = @"Books\" + book + ".png";

            FileStream stream = new FileStream(filePath, FileMode.Create);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            encoder.Save(stream);

            stream.Close();
        }

        private void ListBox_book_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            book_img.Source = new BitmapImage();
            book_img.Stretch = Stretch.Uniform;


            // 리팩토링 시 DB값 지역변수 저장 필요
            try
            {

                BitmapImage image = new BitmapImage();

                using (var stream = File.OpenRead(System.Environment.CurrentDirectory + @"\books\" + searchs.Rows[0]["image_path"].ToString() + ".png"))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();

                    book_img.Source = image;
                    Book_img_path = stream.Name;
                    /*
                    book_img.BeginInit();
                    book_img.Source = new BitmapImage(new Uri(stream.Name));
                    Book_img_path = stream.Name;
                    book_img.EndInit();*/
                }
                /*
                book_img.BeginInit();
                book_img.Source = new BitmapImage(new Uri(System.Environment.CurrentDirectory + @"\books\" + searchs.Rows[0]["image_path"].ToString() + ".png"));
                Book_img_path = System.Environment.CurrentDirectory + @"\books\" + searchs.Rows[0]["image_path"].ToString() + ".png";
                book_img.EndInit();
                */
            }
            catch
            {
                book_img.BeginInit();
                book_img.Source = new BitmapImage(new Uri(@"Resource\iconmonstr-book-1-240.png", UriKind.Relative)); // 이미지 찾기 실패시 기본 이미지 표시
                Book_img_path = "";
                book_img.EndInit();
            }
            MessageBox.Show(System.Environment.CurrentDirectory + @"\books\" + searchs.Rows[0]["image_path"].ToString() + ".png");
            lblbook.Content = searchs.Rows[0]["Book_name"];
            lblGenre.Content = searchs.Rows[0]["Book_Genre"];
            lblAuthor.Content = searchs.Rows[0]["Book_author"];
            lblNote.Content = searchs.Rows[0]["Note"].ToString();
            Book_ID = searchs.Rows[0]["Book_ID"].ToString();
        }

        private void btn_book_search(object sender, RoutedEventArgs e)
        {
            search_book_list.Items.Clear();

            if (search_book_text.Text == "")
            {
                MessageBox.Show("검색어를 확인해주세요");
                return;
            }

            string target = search_book_text.Text;

            string query = "select Book_name from books where Book_name like '%{0}%';";
            query = string.Format(query, target);

            DataTable searchs = Data.DB_con.GetTable(query);

            foreach (DataRow element in searchs.Rows)
            {
                search_book_list.Items.Add(element["book_name"]);
            }
        }

        private void btn_book_all_search(object sender, RoutedEventArgs e)
        {
            search_book_list.Items.Clear();

            string query = "select book_name from books;";

            DataTable searchs = Data.DB_con.GetTable(query);

            foreach (DataRow element in searchs.Rows)
            {
                search_book_list.Items.Add(element["book_name"]);
            }
        }

        private void btn_book_del(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("삭제 하시겠습니까?", "메세지", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

            }
            else
            {
                return;
            }
            string target = Book_ID;

            string query = "DELETE FROM books WHERE `Book_ID`={0};";

            query = string.Format(query, target);

            Data.DB_con.write_query(query);
            // 이미지 삭제
            FileInfo fileDel = new FileInfo(Book_img_path);

            if (fileDel.Exists) // 삭제할 파일이 있는지
            {
                fileDel.Delete(); // 없어도 에러안남
            }

            // 삭제 후 재검색
            lblbook.Content = "";
            lblGenre.Content = "";
            lblAuthor.Content = "";
            Book_ID = "";

            search_book_list.Items.Clear();

            query = "select Book_name from books;";

            DataTable searchs = Data.DB_con.GetTable(query);

            foreach (DataRow element in searchs.Rows)
            {
                search_book_list.Items.Add(element["Book_name"]);
            }
        }
    }
}
