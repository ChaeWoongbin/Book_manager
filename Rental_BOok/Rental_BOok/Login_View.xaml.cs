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
using System.Windows.Shapes;

namespace Rental_BOok
{
    /// <summary>
    /// Login_View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login_View : Window
    {
        string id;
        string password;
        string user_id;
        bool login_state = false;
        string login_pw;
        int login_level;
        string login_info;

        DB_con db;

        public Login_View()
        {
            InitializeComponent();
            db = Data.DB_con;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            login_state = Data.login_state;
            try
            {
                if (login_state == false)
                {

                    this.id = ID.Text;
                    this.password = PW.Password;
                    login_level = 0;

                    DataTable login_table = db.GetTable("select * from book_l.users where login_id = '" + id + "'");
                    //MessageBox.Show(info); // - 로그인 id 존재 확인

                    login_pw = login_table.Rows[0]["login_pwd"].ToString(); // -로그인 id password
                    user_id = login_table.Rows[0]["login_id"].ToString(); // -로그인 id password

                    if (PW.Password == login_pw)
                    {
                        login_state = true;
                        //db.Insert("insert into login_log(id) VALUES ( '" + id + "')"); // 로그 기록
                    }
                    else
                    {
                        MessageBox.Show("비밀번호를 확인하세요");
                        return;
                    }
                    
                    Data.user.user_name = login_table.Rows[0]["User_name"].ToString();
                    Data.user.user_note = login_table.Rows[0]["Note"].ToString();
                    Data.user.user_image = login_table.Rows[0]["image_path"].ToString();
                    Data.user.user_email = login_table.Rows[0]["User_email"].ToString();
                    Data.user.user_address = login_table.Rows[0]["User_address"].ToString();
                    Data.user.user_level = login_table.Rows[0]["User_Level"].ToString();

                    if (login_table.Rows[0]["image_path"] == DBNull.Value)
                    {
                        Data.user.user_image = @"User_default.png";
                    }

                    this.DialogResult = true;
                    Data.login_state = true;
                    MessageBox.Show("로그인 되었습니다.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" 아이디와 비밀번호를 확인하세요", "로그인 에러");
                //MessageBox.Show(ex.ToString());
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) // 창 이동
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("관리자에게 문의하세요");

            add_user join = new add_user();
            join.Show();

            this.Close();            
        }
    }
}
