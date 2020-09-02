using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Rental_BOok
{
    /// <summary>
    /// add_user.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class add_user : Window
    {
        DB_con db;
        public add_user()
        {
            InitializeComponent();

            db = Data.DB_con;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (new_id.Text.Length < 4)
            {
                MessageBox.Show("아이디 길이는 4 이상입니다.");
                return;
            }

            if (new_pw.Text.Length < 4)
            {
                MessageBox.Show("비밀번호 길이는 4 이상입니다.");
                return;
            }

            string id_check = db.read_str("select login_id from users where login_id = '" + new_id.Text + "'");

            if (id_check == new_id.Text)
            {
                MessageBox.Show("이미 존재하는 ID 입니다.");
                return;
            }
            else
            {
                string insert_query = "Insert into users (login_id, login_pwd, User_name, User_address, User_email, user_level, Image_path, Note) VALUES ( '" + new_id.Text + "','" + new_pw.Text + "','" + user_name.Text + "','" + new_address.Text + "','" + new_email.Text + "','3" + "','" + new_id.Text + ".png' ,'" + new_nte.Text + "') ";
                Clipboard.SetText(insert_query);
                db.write_query(insert_query);
                SaveAsPng(GetBitmapFromControl(image), new_id.Text);

                MessageBox.Show("완료되었습니다.");
                this.Close();

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
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

        // 이미지 저장
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

        public void SaveAsPng(RenderTargetBitmap src, string id)
        {
            string filePath = @"User\" + id + ".png";

            FileStream stream = new FileStream(filePath, FileMode.Create);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            encoder.Save(stream);

            stream.Close();
        }
    }
}
