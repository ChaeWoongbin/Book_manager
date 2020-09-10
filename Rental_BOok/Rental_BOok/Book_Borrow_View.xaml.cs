using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Windows.Threading;

namespace Rental_BOok
{
    /// <summary>
    /// Book_Borrow_VIew.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Book_Borrow_View : UserControl
    {
        private Socket m_ServerSocket;
        private Dictionary<string, Socket> m_ClientSocket;
        public Dictionary<string, string> m_ClientSocket_value;
        private byte[] szData;

        private delegate void ThisDelegate();


        public Book_Borrow_View()
        {
            InitializeComponent();
            OpenServer();
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

        private void OpenServer()
        {
            m_ClientSocket = new Dictionary<string, Socket>();
            m_ClientSocket_value = new Dictionary<string, string>();

            m_ClientSocket = new Dictionary<string, Socket>();
            // N개 소켓 변수
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // InterNetWork ( IPv4 ), Stream( 양방향 연결 기반의 바이트 스트림 ), TCP( TCP 프로토콜 )
            IPEndPoint ipep = new IPEndPoint(IPAddress.Loopback, 9797); // ipaddress.parse()
            // 모든 IP의 9797포트 연결 ( EndPoint - 네트워크 목적지 (device))
            m_ServerSocket.Bind(ipep);
            // 엔드포인트 서버소켓 설정
            m_ServerSocket.Listen(20);
            // 수신, 연결 큐 대기연결 수 20

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            //비동기 소켓
            args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            //비동기작업 완료 이벤트
            m_ServerSocket.AcceptAsync(args);
            //비동기작업에 사용할 개체 (args) 설정
        }
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = e.AcceptSocket;

            string ip_ = ((IPEndPoint)ClientSocket.RemoteEndPoint).Address.ToString();
            string port_ = ((IPEndPoint)ClientSocket.RemoteEndPoint).Port.ToString();

            //string[] array = a.Split(new char[] { ':' });
            /*
            if(array[0] != "192.168.0.45")
            {
                SetText(array[0] + " : 허용되지 않은 IP");
                return;
            }
            */

            // 클라이언트 소켓 생성
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThisDelegate)delegate ()
            {
                //keys.Items.Add(ip_ + ":" + port_);
            });
            m_ClientSocket.Add(ip_ + ":" + port_, ClientSocket);
            m_ClientSocket_value.Add(ip_ + ":" + port_, "0");
            // 클라이언트 소켓 Dictionary 추가    소켓번호, 소켓

            if (m_ClientSocket != null)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                szData = new byte[1024]; // 패킷 크기 1024
                int bytesRec = ClientSocket.Receive(szData); // 수신 바이트 1024
                args.SetBuffer(szData, 0, szData.Length); // 버퍼설정

                ClientSocket.ReceiveAsync(args);

                args.UserToken = m_ClientSocket;
                args.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
            }
            e.AcceptSocket = null;
            m_ServerSocket.AcceptAsync(e);
        }
        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket ClientSocket = (Socket)sender;
            if (ClientSocket.Connected && e.BytesTransferred > 0)
            {
                byte[] szData = e.Buffer;    // 데이터 수신

                //string sData = Encoding.UTF8.GetString(szData);
                //sData = sData.Replace("\0", "").Trim();
                //byte[] sDataz = Encoding.UTF8.GetBytes(sData);
                string sData = Encoding.UTF8.GetString(szData, 0, e.BytesTransferred);
                sData += "\n" + ByteArrayToString(szData, e.BytesTransferred);

                string sData_point = m_ClientSocket.FirstOrDefault(x => x.Value == ClientSocket).Key.ToString() + " : " + sData;

                // 4 헤더 4 책id 
                if (ByteArrayToString(szData, e.BytesTransferred).Substring(0, 4) == "3935")
                {
                    string ip_ = ((IPEndPoint)ClientSocket.RemoteEndPoint).Address.ToString();
                    string port_ = ((IPEndPoint)ClientSocket.RemoteEndPoint).Port.ToString();
                    string str = ByteArrayToString(szData, e.BytesTransferred);
                    try
                    {
                        //m_ClientSocket_value[ip_ + ":" + port_] = str.Substring(4, 4);
                        Borrow_event(str.Substring(4, 4));
                    }
                    catch { }
                }
                try
                {
                    byte[] sDataz = stringtobyte(sData_point);
                    ClientSocket.Send(sDataz, sDataz.Length, SocketFlags.None);
                }
                catch
                {
                    byte[] sDataz = Encoding.UTF8.GetBytes(sData);
                    ClientSocket.Send(sDataz, sDataz.Length, SocketFlags.None);
                }


                for (int i = 0; i < szData.Length; i++)
                {
                    szData[i] = 0;
                }
                e.SetBuffer(szData, 0, e.Buffer.Length);
                ClientSocket.ReceiveAsync(e);
            }
            else
            {
                ClientSocket.Disconnect(false);
                ClientSocket.Dispose();
                m_ClientSocket.Remove(m_ClientSocket.FirstOrDefault(x => x.Value == ClientSocket).Key);
            }

        }

        public enum Book_state
        {
            From = 0,
            To = 1,
        }
        Book_state flag = Book_state.From;

        private void Borrow_event(string book_id)
        {
            string id = Convert.ToInt16(book_id,16).ToString();
            string query = $"Select * from books where Book_iD = {id} ";
            DataTable book_info = Data.DB_con.GetTable(query);

            borrow_event.IsEnabled = true;

            try
            {
                Book_img.Source = new BitmapImage(new Uri(System.Environment.CurrentDirectory + @"\Books\" + book_info.Rows[0]["Image_path"] + ".png"));
            }
            catch (Exception ex)
            {
                Book_img.Source = new BitmapImage(new Uri(@"Resource\iconmonstr-book-1-240.png", UriKind.Relative)); // 이미지 찾기 실패시 기본 이미지 표시
            }
            lblname.Content = book_info.Rows[0]["Book_ID"].ToString();
            lblgenre.Content = book_info.Rows[0]["Book_Genre"].ToString(); ;
            lblauthor.Content = book_info.Rows[0]["Book_author"].ToString();
            lblstate.Content = book_info.Rows[0]["Book_Note"].ToString();

            if (book_info.Rows[0]["Rental_User_ID"].ToString() == "1")
            {
                flag = Book_state.From;
                borrow_event.Content = "대여하기";
            }
            else
            {
                flag = Book_state.To;
                borrow_event.Content = "반납하기";
            }
        }


        public static string ByteArrayToString(byte[] ba, int cnt)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (int i = 0; i < cnt; i++)
                hex.Append(ba[i].ToString("X2"));
            return hex.ToString();
        }

        public static byte[] stringtobyte(string covertString) // 패킷으로 보내기 ( 03 없이 <-> Encoding.UTF8.GetBytes(string) )
        {
            byte[] convertArr = new byte[covertString.Length / 2];

            for (int i = 0; i < convertArr.Length; i++)
            {
                convertArr[i] = Convert.ToByte(covertString.Substring(i * 2, 2), 16);
            }
            return convertArr;
        }

        private void borrow_event_Click(object sender, RoutedEventArgs e)
        {
            if(flag == Book_state.From) // 대여 ( DB 소유자 변경 )
            {

            }
            else // 반남 ( DB 소유자 -> 1 )
            {
                
                //string query = $"UPDATE `book_l`.`books` SET `Rental_User_ID`='{}' WHERE  `Book_ID`={};";   0 - 로그인 유저(비로그인 생각)  1- 책id
                //Data.DB_con.write_query(query);

                //메세지 박스 출력 (완료) 후 화면갱신
            }
        }
    }
}
