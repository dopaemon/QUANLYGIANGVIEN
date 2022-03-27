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
    /// Lógica de interacción para NotesPage.xaml
    /// </summary>
    public partial class khoa : Page
    {
        public khoa()
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
                txtthongbao.Text = "Bạn đã chọn nhập khoa";
            } else if (cbchucnang.SelectedIndex == 1) {
                readwritetextbox(0);
                txtthongbao.Text = "Bạn đã chọn xem khoa";
            } else if (cbchucnang.SelectedIndex == 2) {
                readwritetextbox(0);
                txtmakhoa.IsReadOnly = false;
                txtthongbao.Text = "Bạn đã chọn xóa khoa";
            } else if (cbchucnang.SelectedIndex == 3) {
                readwritetextbox(0);
                txtmakhoa.IsReadOnly = false;
                txtthongbao.Text = "Bạn đã chọn sửa khoa";
            } else if (cbchucnang.SelectedIndex == 4) {
                readwritetextbox(1);
                txtthongbao.Text = "Bạn đã chọn tìm khoa";
            } else if (cbchucnang.SelectedIndex == -1) {
                txtthongbao.Text = "Bạn chưa chọn chức năng"; }

            if (checkedbtn == 1) { btnrun.IsEnabled = true; }
            else if (checkedbtn == 0) { btnrun.IsEnabled = true; }
        }

        private void btnrun_Click(object sender, RoutedEventArgs e)
        {
            if (cbchucnang.SelectedIndex == 0) {
                if (String.IsNullOrEmpty(txtmakhoa.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa"; }
                else if (String.IsNullOrEmpty(txttenkhoa.Text)) { txtthongbao.Text = "Bạn chưa nhập tên khoa"; }
                else if (String.IsNullOrEmpty(txtdienthoai.Text)) { txtthongbao.Text = "Bạn chưa nhập điện thoại"; }
                else { nhapkhoa(); }
            } else if (cbchucnang.SelectedIndex == 1) {
                xemkhoa();
            } else if (cbchucnang.SelectedIndex == 2) {
                if (String.IsNullOrEmpty(txtmakhoa.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa"; }
                else { xoakhoa(); xemkhoa(); }
            } else if (cbchucnang.SelectedIndex == 3) {
                if (String.IsNullOrEmpty(txtmakhoa.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa"; }
                else if (String.IsNullOrEmpty(txttenkhoamoi.Text)) { txtthongbao.Text = "Bạn chưa nhập tên khoa mới"; }
                else if (String.IsNullOrEmpty(txtdienthoaimoi.Text)) { txtthongbao.Text = "Bạn chưa nhập điện thoại mới"; }
                else { capnhatkhoa(); checkkhoa(); }
            } else if (cbchucnang.SelectedIndex == 4) {
                if (String.IsNullOrEmpty(txtmakhoa.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa"; }
                else { timkhoa(); }
            } else { txtthongbao.Text = ""; }
        }

        private void nhapkhoa()
        {
            con.Open();
            SqlCommand checkmakhoa = new SqlCommand("SELECT COUNT(MAKHOA) FROM KHOA WHERE ([MAKHOA] = @MAKHOA)", con);
            checkmakhoa.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
            int makhoa = (int)checkmakhoa.ExecuteScalar();
            con.Close();
            if (makhoa > 0) {
                txtthongbao.Text = "Khoa Đã Tồn Tại !!!";
                checkkhoa();
            } else if (String.IsNullOrEmpty(txtmakhoa.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa";
            } else {
                con.Open();
                SqlCommand nhapkhoa = new SqlCommand("SP_NHAPKHOA", con);
                nhapkhoa.CommandType = CommandType.StoredProcedure;
                nhapkhoa.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
                nhapkhoa.Parameters.AddWithValue("@TENKHOA", txttenkhoa.Text);
                nhapkhoa.Parameters.AddWithValue("@DIENTHOAI", txtdienthoai.Text);
                nhapkhoa.ExecuteNonQuery();
                nhapkhoa.CommandType = CommandType.StoredProcedure;
                con.Close();

                txtthongbao.Text = "Nhập khoa thành công";

                checkkhoa();
            }
        }

        private void xemkhoa()
        {
            con.Open();
            SqlCommand xemkhoa = new SqlCommand("SP_XEMKHOA", con);
            xemkhoa.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sda = new SqlDataAdapter(xemkhoa);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrkhoa.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void xoakhoa()
        {
            con.Open();
            SqlCommand xoakhoa = new SqlCommand("SP_XOAKHOA", con);
            xoakhoa.CommandType = CommandType.StoredProcedure;
            xoakhoa.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
            SqlDataAdapter sda = new SqlDataAdapter(xoakhoa);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrkhoa.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void capnhatkhoa()
        {
            con.Open();
            SqlCommand suakhoa = new SqlCommand("SP_CAPNHATKHOA", con);
            suakhoa.CommandType = CommandType.StoredProcedure;
            suakhoa.Parameters.AddWithValue("@MAKHOACU", txtmakhoa.Text);
            suakhoa.Parameters.AddWithValue("@TENKHOAMOI", txttenkhoamoi.Text);
            suakhoa.Parameters.AddWithValue("@DIENTHOAIMOI", txtdienthoaimoi.Text);
            SqlDataAdapter sda = new SqlDataAdapter(suakhoa);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrkhoa.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void timkhoa()
        {
            con.Open();
            SqlCommand timkhoa = new SqlCommand("SP_TIMKHOA", con);
            timkhoa.CommandType = CommandType.StoredProcedure;
            timkhoa.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
            timkhoa.Parameters.AddWithValue("@TENKHOA", txttenkhoa.Text);
            timkhoa.Parameters.AddWithValue("@DIENTHOAI", txtdienthoai.Text);
            SqlDataAdapter sda = new SqlDataAdapter(timkhoa);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrkhoa.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void checkkhoa()
        {
            con.Open();
            SqlCommand view = new SqlCommand("SP_CHECKKHOA", con);
            view.CommandType = CommandType.StoredProcedure;
            view.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
            SqlDataAdapter sda = new SqlDataAdapter(view);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrkhoa.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void hidetextboxmoi()
        {
            txttenkhoamoi.Visibility = Visibility.Hidden;
            txtdienthoaimoi.Visibility = Visibility.Hidden;
        }

        private void showtextboxmoi()
        {
            txttenkhoamoi.Visibility = Visibility.Visible;
            txtdienthoaimoi.Visibility = Visibility.Visible;
        }

        private void ftextbox(int a)
        {
            if (a == 0) {
                txttenkhoa.Visibility = Visibility.Hidden;
                txtdienthoai.Visibility = Visibility.Hidden; } 
            else if (a == 1) {
                txttenkhoa.Visibility = Visibility.Visible;
                txtdienthoai.Visibility = Visibility.Visible; }
        }

        private void readwritetextbox(int a)
        {
            if (a == 1) {
                txtmakhoa.IsReadOnly = false;
                txttenkhoa.IsReadOnly = false;
                txtdienthoai.IsReadOnly = false;
            } else if (a == 0) {
                txtmakhoa.IsReadOnly = true;
                txttenkhoa.IsReadOnly = true;
                txtdienthoai.IsReadOnly = true;
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
