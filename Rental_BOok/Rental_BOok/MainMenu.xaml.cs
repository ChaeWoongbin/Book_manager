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
    /// MainMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();

            Program_info.Text =
                "release Date : 2020.04.10\n" +
                "version : V0.1\n" +
                "complete : 로그인 \n" +
                "working :\n    Rental(packet)\n    graph(zed)\n    Search(DB)\n    UI \n" +
                "jerpi94"
                ;
        }

        private void Borrow_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Book_Borrow_View);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Book_Search_View);
        }

        private void Data_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Book_data_View);
        }

        private void login_click(object sender, RoutedEventArgs e)
        {
            if (Data.login_state)
            {
                if(MessageBox.Show("로그아웃 하시겠습니까?", "메세지" , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {

                }
                else
                {
                    return;
                }
                user_name_info.Content = "";
                user_address_info.Content = "";
                user_email_info.Content = "";
                user_note_info.Content = "";

                user_image.Source = new BitmapImage();
                user_image.Stretch = Stretch.Uniform;

                Data.login_state = false;
                user_btn.Content = "LogIn";

                Set_btn.Visibility = Visibility.Hidden;
            }
            else
            {
                Login_View login_View = new Login_View();
                login_View.ShowDialog();
                if (login_View.DialogResult == true)
                {
                    user_name_info.Content = Data.user.user_name;
                    user_address_info.Content = Data.user.user_address;
                    user_email_info.Content = Data.user.user_email;
                    user_note_info.Content = Data.user.user_note;

                    //MessageBox.Show(System.Environment.CurrentDirectory + @"\User\" + Data.user.user_image);
                    try
                    {                        
                        user_image.Source = new BitmapImage(new Uri(System.Environment.CurrentDirectory + @"\User\" + Data.user.user_image));
                    }
                    catch
                    {
                        user_image.Source = new BitmapImage(new Uri(@"Resource\User_default.png", UriKind.Relative)); // 이미지 찾기 실패시 기본 이미지 표시
                    }
                    user_image.Stretch = Stretch.Uniform;

                    Data.login_state = true;
                    user_btn.Content = "LogOut";
                    if(Data.user.user_level == "1")
                    {
                        Set_btn.Visibility = Visibility.Visible; // 운영자 계정일때만 Visible
                    }
                }
            }
            
        }

        private void setting_click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Admin_set);
        }
    }
}
