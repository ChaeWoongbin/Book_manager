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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // DB table, id, pw, port, ip
            Data.DB_con.connect("book_l", "pi", "pi", "3306", "192.168.0.10");
            /*
            string query = "select user_email from users where user_id = '2'";
            string str = Data.DB_con.read_str(query);
            MessageBox.Show(str);
            */
            ViewGrid.Children.Add(Data.MainMenu);
        }

        private void Set_Socket()
        {

        }
    }
}
