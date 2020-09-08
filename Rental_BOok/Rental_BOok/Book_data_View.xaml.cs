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
using ZedGraph;

namespace Rental_BOok
{
    /// <summary>
    /// Book_data_View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Book_data_View : UserControl
    {
        public Book_data_View()
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

        private void Rental_Click(object sender, RoutedEventArgs e)
        {
            Grid parent_grid = (Grid)this.Parent;
            parent_grid.Children.Clear();
            parent_grid.Children.Add(Data.Book_Borrow_View);
        }


        private void MakeChart()
        {
            CreateGraph(ZedG);
        }

        private void CreateGraph(ZedGraphControl zgc)
        {
            GraphPane pane = zgc.GraphPane;
            pane.Chart.Fill = new ZedGraph.Fill(System.Drawing.Color.WhiteSmoke);

            pane.CurveList.Clear();

            // temp -12월 , 올해 검색
            DataTable get_data = Data.DB_con.GetTable(@"SELECT month,COUNT(Book_ID) AS count
                                                        FROM temp_month AS A left OUTER join books_history AS B 
                                                        ON A.month = month(B.Rental_Date) AND YEAR(B.rental_date) >= YEAR(NOW()) AND YEAR(B.rental_date) < YEAR(NOW())+1
                                                        Group BY month");
            int itemscount = get_data.Rows.Count;

            string[] labels = new string[itemscount];

            double[] YValues1 = new double[itemscount];

            double[] XValues = new double[itemscount];

            
            for (int i = 0; i < get_data.Rows.Count; i++)
            {
                object label = grid_month.FindName($"month_{i+1}");
                ((System.Windows.Controls.Label)label).Content = get_data.Rows[i]["count"];
                //labels[i] = $"{System.DateTime.Now.Year.ToString()} - {get_data.Rows[i]["month"]}";
                labels[i] = $"{get_data.Rows[i]["month"]}";
                XValues[i] = Convert.ToDouble(get_data.Rows[i]["count"]);
            }

            pane.XAxis.Type = AxisType.Text;
            pane.XAxis.MajorTic.IsBetweenLabels = true;
            pane.XAxis.Scale.TextLabels = labels;

            ZedGraph.BarItem myBar = pane.AddBar("Step", null, XValues, System.Drawing.Color.AliceBlue);
            myBar.Bar.Fill = new ZedGraph.Fill(System.Drawing.Color.YellowGreen);

            pane.BarSettings.Type = ZedGraph.BarType.Cluster;
            
            pane.BarSettings.MinBarGap = 0.0f; // 옆 막대 와 거리

            pane.BarSettings.MinClusterGap = 1.0f; // 각 그래프 사이 거리
            
            zgc.AxisChange();

            zgc.Invalidate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MakeChart();
            info.Content = $"{System.DateTime.Now.Year} 년 검색 결과 ";
        }
    }
}
