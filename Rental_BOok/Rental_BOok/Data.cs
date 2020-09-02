using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rental_BOok
{
    public partial class Data
    {
        // 페이지 
        public static MainMenu MainMenu = new MainMenu();
        public static Book_Borrow_View Book_Borrow_View = new Book_Borrow_View();
        public static Book_Search_View Book_Search_View = new Book_Search_View();
        public static Book_data_View Book_data_View = new Book_data_View();
        public static Admin_set Admin_set = new Admin_set();

        // 소켓 ( 대여 ) - 서버 1:1
        public static DB_con DB_con = new DB_con();

        // 유저 정보
        public static login_info user = new login_info();
        // 책 정보
        public static Book_info book = new Book_info();

        public static bool login_state = false;
    }

    public class login_info
    {
        public string user_name { get; set; }
        public string user_address { get; set; }
        public string user_email { get; set; }
        public string user_note { get; set; }
        public string user_image { get; set; }
        public string user_level { get; set; }
    }

    public class Book_info
    {
        public string Book_name { get; set; }
        public string Book_Genre { get; set; }
        public string Book_author { get; set; }
        public string Book_note { get; set; }
        public string Book_image { get; set; }
        public string Book_ID { get; set; }
    }
}
