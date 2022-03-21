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
using System.Data.SqlClient;
// using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Data;
using System.Threading;
using LiveCharts; //Core of the library
using LiveCharts.Wpf; //The WPF controls
// using LiveCharts.WinForms; //the WinForm wrappers

namespace QUANLYGIANGVIEN.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Func<ChartPoint, string> PointLabel { get; set; }
        public Home()
        {
            InitializeComponent();

            // Lấy dữ liệu RealTime
            //while(true) {
                // Giang Vien
                string giangvien = getvalues("SELECT COUNT(*) FROM GIANGVIEN");
                lblsogiaovien.Content = giangvien;

                // Mon Hoc
                string monhoc = getvalues("SELECT COUNT(*) FROM MONHOC");
                lblsomonhoc.Content = monhoc;

                // Khoa
                string khoa = getvalues("SELECT COUNT(*) FROM KHOA");
                lblsokhoa.Content = khoa;

            //    // Delay 1s
            //    Thread.Sleep(1000);
            //}

            PointLabel = chartPoint => string.Format("{0}({1:p})", chartPoint.Y, chartPoint.Participation);
            DataContext = this;

            var converter = new BrushConverter();

            LiveCharts.SeriesCollection psc = new LiveCharts.SeriesCollection {
                new LiveCharts.Wpf.PieSeries {
                    Title = "Giảng Viên",
                    LabelPoint = PointLabel,
                    DataLabels = true,
                    Fill = (Brush)converter.ConvertFrom("#a177d6"),
                    Values = new LiveCharts.ChartValues<decimal> {
                        int.Parse(giangvien)
                    },
                },
                new LiveCharts.Wpf.PieSeries {
                    Title = "Môn Học",
                    LabelPoint = PointLabel,
                    DataLabels = true,
                    Fill = (Brush)converter.ConvertFrom("#1BBC9B"),
                    Values = new LiveCharts.ChartValues<decimal> {
                        int.Parse(monhoc)
                    },
                },
                new LiveCharts.Wpf.PieSeries {
                    Title = "Khoa",
                    LabelPoint = PointLabel,
                    DataLabels = true,
                    Fill = (Brush)converter.ConvertFrom("#f4bc72"),
                    Values = new LiveCharts.ChartValues<decimal> {
                        int.Parse(monhoc),
                    },
                }
            };
            QLGVCharts.LegendLocation = LegendLocation.Right;

            foreach (LiveCharts.Wpf.PieSeries ps in psc) {
                QLGVCharts.Series.Add(ps);
            }
        }

        private void pipChart_DataClick(object sender, LiveCharts.ChartPoint chartPoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;
            //clear selected slice
            foreach (PieSeries series in chart.Series)
            {
                series.PushOut = 0;

                var selectedSeries = (PieSeries)chartPoint.SeriesView;
                selectedSeries.PushOut = 8;
            }
        }

        private string getvalues(string sqlcommandline)
        {
            SqlConnection con = new SqlConnection(ConfigurationSettings.AppSettings["constr"]);
            con.Open();
            SqlCommand checkvalues = new SqlCommand(sqlcommandline, con);
            int cv = (int)checkvalues.ExecuteScalar();
            con.Close();
            return cv.ToString();
        }
    }
}
