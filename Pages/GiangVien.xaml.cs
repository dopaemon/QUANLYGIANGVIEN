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
using System.Text.RegularExpressions;

namespace QUANLYGIANGVIEN.Pages
{
    /// <summary>
    /// Lógica de interacción para SoundsPage.xaml
    /// </summary>
    public partial class GiangVien : Page
    {
        public GiangVien()
        {
            InitializeComponent();
            hidetextboxmoi();
            readwritetextbox(0);
            btnrun.IsEnabled = false;
        }

        int checkedbtn = 0;
        SqlConnection con = new SqlConnection(ConfigurationSettings.AppSettings["constr"]);

        private void btnc_Click(object sender, RoutedEventArgs e)
        {
            if (cbchucnang.SelectedIndex == 0 || 
                cbchucnang.SelectedIndex == 1 || 
                cbchucnang.SelectedIndex == 2 || 
                cbchucnang.SelectedIndex == 4) {
                hidetextboxmoi();
                ftextbox(1);
                checkedbtn = 1;
            } else if (cbchucnang.SelectedIndex == 3) {
                showtextboxmoi();
                ftextbox(0);
                checkedbtn = 1;
            } else { checkedbtn = 0; }

            if (cbchucnang.SelectedIndex == 0) {
                readwritetextbox(1);
                txtthongbao.Text = "Bạn đã chọn nhập giảng viên";
            } else if (cbchucnang.SelectedIndex == 1) {
                readwritetextbox(0);
                txtthongbao.Text = "Bạn đã chọn xem giảng viên";
            } else if (cbchucnang.SelectedIndex == 2) {
                readwritetextbox(0);
                txtmagv.IsReadOnly = false;
                txtthongbao.Text = "Bạn đã chọn xóa giảng viên";
            } else if (cbchucnang.SelectedIndex == 3) {
                readwritetextbox(0);
                txtmagv.IsReadOnly = false;
                txtthongbao.Text = "Bạn đã chọn sửa giảng viên";
            } else if (cbchucnang.SelectedIndex == 4) {
                readwritetextbox(1);
                txtthongbao.Text = "Bạn đã chọn tìm giảng viên";
            } else if (cbchucnang.SelectedIndex == -1) {
                txtthongbao.Text = "Bạn chưa chọn chức năng\n";
            }

            if (checkedbtn == 1) { 
                btnrun.IsEnabled = true; 
            } else if (checkedbtn == 0) {
                btnrun.IsEnabled = true;
            }
        }

        private void btnrun_Click(object sender, RoutedEventArgs e)
        {
            if (cbchucnang.SelectedIndex == 0) {
                if (String.IsNullOrEmpty(txtmagv.Text)) { txtthongbao.Text = "Bạn chưa nhập mã giảng viên"; }
                else if (String.IsNullOrEmpty(txthoten.Text)) { txtthongbao.Text = "Bạn chưa nhập họ và tên giảng viên"; }
                else if (String.IsNullOrEmpty(txtdiachi.Text)) { txtthongbao.Text = "Bạn chưa nhập địa chỉ giảng viên"; }
                else if (String.IsNullOrEmpty(txtdienthoai.Text)) { txtthongbao.Text = "Bạn chưa nhập điện thoại giảng viên"; }
                else if (String.IsNullOrEmpty(txtmakhoa.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa giảng viên"; }
                else { nhapgiangvien(); }
            } else if (cbchucnang.SelectedIndex == 1) {
                xemgiangvien();
            } else if (cbchucnang.SelectedIndex == 2) {
                if (String.IsNullOrEmpty(txtmagv.Text)) { txtthongbao.Text = "Bạn chưa nhập mã giảng viên"; }
                else { xoagiangvien(); }
            } else if (cbchucnang.SelectedIndex == 3) {
                if (String.IsNullOrEmpty(txtmagv.Text)) { txtthongbao.Text = "Bạn chưa nhập mã giảng viên"; }
                else if (String.IsNullOrEmpty(txthotenmoi.Text)) { txtthongbao.Text = "Bạn chưa nhập họ và tên mới của giảng viên"; }
                else if (String.IsNullOrEmpty(txtdiachimoi.Text)) { txtthongbao.Text = "Bạn chưa nhập địa chỉ mới của giảng viên"; }
                else if (String.IsNullOrEmpty(txtdienthoaimoi.Text)) { txtthongbao.Text = "Bạn chưa nhập điện thoại mới của giảng viên"; }
                else if (String.IsNullOrEmpty(txtmakhoamoi.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa mới của giảng viên"; }
                else {capnhatgiangvien(); }
            } else if (cbchucnang.SelectedIndex == 4) {
                if (String.IsNullOrEmpty(txtmagv.Text)) { txtthongbao.Text = "Bạn chưa nhập mã giảng viên"; }
                else { timgiangvien(); }
            } else { txtthongbao.Text = ""; }
        }

        private void nhapgiangvien()
        {
            con.Open();
            SqlCommand checkmagv = new SqlCommand("SELECT COUNT(MAGV) FROM GIANGVIEN WHERE ([MAGV] = @MAGV)", con);
            checkmagv.Parameters.AddWithValue("@MAGV", txtmagv.Text);
            int magv = (int)checkmagv.ExecuteScalar();
            con.Close();
            if (magv > 0) {
                txtthongbao.Text = "Giảng Viên Đã Tồn Tại !!!";
                con.Open();
                SqlCommand view = new SqlCommand("SP_CHECKGIANGVIEN", con);
                view.CommandType = CommandType.StoredProcedure;
                view.Parameters.AddWithValue("@MAGV", txtmagv.Text);
                SqlDataAdapter sda = new SqlDataAdapter(view);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dtgrgiangvien.ItemsSource = dt.DefaultView;
                con.Close();
            } else {
                con.Open();
                SqlCommand checkmakhoa = new SqlCommand("SELECT COUNT(MAKHOA) FROM KHOA WHERE ([MAKHOA] = @MAKHOA)", con);
                checkmakhoa.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
                int makhoa = (int)checkmakhoa.ExecuteScalar();
                con.Close();
                if (makhoa > 0) {
                    con.Open();
                    SqlCommand nhapgv = new SqlCommand("SP_NHAPGIANGVIEN", con);
                    nhapgv.CommandType = CommandType.StoredProcedure;
                    nhapgv.Parameters.AddWithValue("@MAGV", txtmagv.Text);
                    nhapgv.Parameters.AddWithValue("@HOTEN", txthoten.Text);
                    nhapgv.Parameters.AddWithValue("@DIACHI", txtdiachi.Text);
                    nhapgv.Parameters.AddWithValue("@DIENTHOAI", txtdienthoai.Text);
                    nhapgv.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
                    nhapgv.ExecuteNonQuery();
                    nhapgv.CommandType = CommandType.StoredProcedure;
                    con.Close();

                    txtthongbao.Text = "Nhập giảng viên thành công";

                    con.Open();
                    SqlCommand view = new SqlCommand("SP_CHECKGIANGVIEN", con);
                    view.CommandType = CommandType.StoredProcedure;
                    view.Parameters.AddWithValue("@MAGV", txtmagv.Text);
                    SqlDataAdapter sdaa = new SqlDataAdapter(view);
                    DataTable dtt = new DataTable();
                    sdaa.Fill(dtt);
                    dtgrgiangvien.ItemsSource = dtt.DefaultView;
                    con.Close();
                } else {
                    if (String.IsNullOrEmpty(txtmagv.Text)) { txtthongbao.Text = "Bạn chưa nhập mã giảng viên"; 
                    } else { 
                        txtthongbao.Text = "Mã Khoa Không Tồn Tại"; 
                    }
                }
            }
        }

        private void xemgiangvien()
        {
            con.Open();
            SqlCommand xemgv = new SqlCommand("SP_XEMGIANGVIEN", con);
            xemgv.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sda = new SqlDataAdapter(xemgv);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrgiangvien.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void xoagiangvien()
        {
            con.Open();
            SqlCommand xoagv = new SqlCommand("SP_XOAGIANGVIEN", con);
            xoagv.CommandType = CommandType.StoredProcedure;
            xoagv.Parameters.AddWithValue("@MAGV", txtmagv.Text);
            SqlDataAdapter sda = new SqlDataAdapter(xoagv);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrgiangvien.ItemsSource = dt.DefaultView;
            con.Close();

            con.Open();
            SqlCommand xemgv = new SqlCommand("SP_XEMGIANGVIEN", con);
            xemgv.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sdaa = new SqlDataAdapter(xemgv);
            DataTable dtt = new DataTable();
            sdaa.Fill(dtt);
            dtgrgiangvien.ItemsSource = dtt.DefaultView;
            con.Close();
        }

        private void capnhatgiangvien()
        {
            con.Open();
            SqlCommand suagv = new SqlCommand("SP_CAPNHATGIANGVIEN", con);
            suagv.CommandType = CommandType.StoredProcedure;
            suagv.Parameters.AddWithValue("@MAGVCU", txtmagv.Text);
            suagv.Parameters.AddWithValue("@HOTENMOI", txthotenmoi.Text);
            suagv.Parameters.AddWithValue("@DIACHIMOI", txtdiachimoi.Text);
            suagv.Parameters.AddWithValue("@DIENTHOAIMOI", txtdienthoaimoi.Text);
            suagv.Parameters.AddWithValue("@MAKHOAMOI", txtmakhoamoi.Text);
            SqlDataAdapter sda = new SqlDataAdapter(suagv);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrgiangvien.ItemsSource = dt.DefaultView;
            con.Close();

            con.Open();
            SqlCommand view = new SqlCommand("SP_CHECKGIANGVIEN", con);
            view.CommandType = CommandType.StoredProcedure;
            view.Parameters.AddWithValue("@MAGV", txtmagv.Text);
            SqlDataAdapter sdaa = new SqlDataAdapter(view);
            DataTable dtt = new DataTable();
            sdaa.Fill(dtt);
            dtgrgiangvien.ItemsSource = dtt.DefaultView;
            con.Close();
        }

        private void timgiangvien()
        {
            con.Open();
            SqlCommand timkiemgv = new SqlCommand("SP_TIMGIANGVIEN", con);
            timkiemgv.CommandType = CommandType.StoredProcedure;
            timkiemgv.Parameters.AddWithValue("@MAGV", txtmagv.Text);
            timkiemgv.Parameters.AddWithValue("@HOTEN", txthoten.Text);
            timkiemgv.Parameters.AddWithValue("@DIACHI", txtdiachi.Text);
            timkiemgv.Parameters.AddWithValue("@DIENTHOAI", txtdienthoai.Text);
            timkiemgv.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
            SqlDataAdapter sda = new SqlDataAdapter(timkiemgv);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrgiangvien.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void hidetextboxmoi()
        {
            txthotenmoi.Visibility = Visibility.Hidden;
            txtdiachimoi.Visibility = Visibility.Hidden;
            txtdienthoaimoi.Visibility = Visibility.Hidden;
            txtmakhoamoi.Visibility = Visibility.Hidden;
        }

        private void showtextboxmoi()
        {
            txthotenmoi.Visibility = Visibility.Visible;
            txtdiachimoi.Visibility = Visibility.Visible;
            txtdienthoaimoi.Visibility = Visibility.Visible;
            txtmakhoamoi.Visibility = Visibility.Visible;
        }

        private void ftextbox(int a)
        {
            if (a == 0) {
                txthoten.Visibility = Visibility.Hidden;
                txtdiachi.Visibility = Visibility.Hidden;
                txtdienthoai.Visibility = Visibility.Hidden;
                txtmakhoa.Visibility = Visibility.Hidden;
            } else if (a == 1) {
                txthoten.Visibility = Visibility.Visible;
                txtdiachi.Visibility = Visibility.Visible;
                txtdienthoai.Visibility = Visibility.Visible;
                txtmakhoa.Visibility = Visibility.Visible;
            }
        }

        private void readwritetextbox(int a)
        {
            if (a == 1) {
                txtmagv.IsReadOnly = false;
                txthoten.IsReadOnly = false;
                txtdiachi.IsReadOnly = false;
                txtdienthoai.IsReadOnly = false;
                txtmakhoa.IsReadOnly = false;
            } else if (a == 0) {
                txtmagv.IsReadOnly = true;
                txthoten.IsReadOnly = true;
                txtdiachi.IsReadOnly = true;
                txtdienthoai.IsReadOnly = true;
                txtmakhoa.IsReadOnly = true;
            }
        }

        private void txtdienthoaimoi_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtdienthoai_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
    }
}
