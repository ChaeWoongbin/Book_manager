using System;
using System.Collections.Generic;
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

    }
}
