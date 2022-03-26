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
    public partial class monhoc : Page
    {
        public monhoc()
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
                cbchucnang.SelectedIndex == 4)
            {
                hidetextboxmoi();
                ftextbox(1);
                checkedbtn = 1;
            }
            else if (cbchucnang.SelectedIndex == 3)
            {
                showtextboxmoi();
                ftextbox(0);
                checkedbtn = 1;
            }
            else { checkedbtn = 0; }

            if (cbchucnang.SelectedIndex == 0)
            {
                readwritetextbox(1);
                txtthongbao.Text = "Bạn đã chọn nhập môn học";
            }
            else if (cbchucnang.SelectedIndex == 1)
            {
                readwritetextbox(0);
                txtthongbao.Text = "Bạn đã chọn xem môn học";
            }
            else if (cbchucnang.SelectedIndex == 2)
            {
                readwritetextbox(0);
                txtmamh.IsReadOnly = false;
                txtthongbao.Text = "Bạn đã chọn xóa môn học";
            }
            else if (cbchucnang.SelectedIndex == 3)
            {
                readwritetextbox(0);
                txtmamh.IsReadOnly = false;
                txtthongbao.Text = "Bạn đã chọn sửa môn học";
            }
            else if (cbchucnang.SelectedIndex == 4)
            {
                readwritetextbox(1);
                txtthongbao.Text = "Bạn đã chọn tìm môn học";
            }
            else if (cbchucnang.SelectedIndex == -1)
            {
                txtthongbao.Text = "Bạn chưa chọn môn học";
            }

            if (checkedbtn == 1)
            {
                btnrun.IsEnabled = true;
            }
            else if (checkedbtn == 0)
            {
                btnrun.IsEnabled = true;
            }
        }

        private void btnrun_Click(object sender, RoutedEventArgs e)
        {
            if (cbchucnang.SelectedIndex == 0)
            {
                if (String.IsNullOrEmpty(txtmamh.Text)) { txtthongbao.Text = "Bạn chưa nhập mã môn học"; }
                else if (String.IsNullOrEmpty(txttenmh.Text)) { txtthongbao.Text = "Bạn chưa nhập tên môn học"; }
                else if (String.IsNullOrEmpty(txtsotiet.Text)) { txtthongbao.Text = "Bạn chưa nhập số tiết"; }
                else if (String.IsNullOrEmpty(txtmakhoa.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa"; }
                else { nhapmonhoc(); }
            }
            else if (cbchucnang.SelectedIndex == 1)
            {
                xemmonhoc();
            }
            else if (cbchucnang.SelectedIndex == 2)
            {
                if (String.IsNullOrEmpty(txtmamh.Text)) { txtthongbao.Text = "Bạn chưa nhập mã môn học"; }
                else { xoamonhoc(); xemmonhoc(); }
            }
            else if (cbchucnang.SelectedIndex == 3)
            {
                if (String.IsNullOrEmpty(txtmamh.Text)) { txtthongbao.Text = "Bạn chưa nhập mã môn học"; }
                else if (String.IsNullOrEmpty(txttenmhmoi.Text)) { txtthongbao.Text = "Bạn chưa nhập tên môn học mới"; }
                else if (String.IsNullOrEmpty(txtsotietmoi.Text)) { txtthongbao.Text = "Bạn chưa nhập số tiết mới"; }
                else if (String.IsNullOrEmpty(txtmakhoamoi.Text)) { txtthongbao.Text = "Bạn chưa nhập mã khoa mới"; }
                else { capnhatmonhoc(); checkmonhoc(); }
            }
            else if (cbchucnang.SelectedIndex == 4)
            {
                if (String.IsNullOrEmpty(txtmamh.Text)) { txtthongbao.Text = "Bạn chưa nhập mã môn học"; }
                else { timmonhoc(); }
            }
            else { txtthongbao.Text = ""; }
        }

        private void nhapmonhoc()
        {
            con.Open();
            SqlCommand checkmamh = new SqlCommand("SELECT COUNT(MAMH) FROM MONHOC WHERE ([MAMH] = @MAMH)", con);
            checkmamh.Parameters.AddWithValue("@MAMH", txtmamh.Text);
            int mamh = (int)checkmamh.ExecuteScalar();
            con.Close();
            if (mamh > 0)
            {
                txtthongbao.Text = "Môn Học Đã Tồn Tại !!!";
                checkmonhoc();
            }
            else
            {
                con.Open();
                SqlCommand checkmakhoa = new SqlCommand("SELECT COUNT(MAKHOA) FROM KHOA WHERE ([MAKHOA] = @MAKHOA)", con);
                checkmakhoa.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
                int makhoa = (int)checkmakhoa.ExecuteScalar();
                con.Close();
                if (makhoa > 0)
                {
                    con.Open();
                    SqlCommand nhapmh = new SqlCommand("SP_NHAPMONHOC", con);
                    nhapmh.CommandType = CommandType.StoredProcedure;
                    nhapmh.Parameters.AddWithValue("@MAMH", txtmamh.Text);
                    nhapmh.Parameters.AddWithValue("@TENMH", txttenmh.Text);
                    nhapmh.Parameters.AddWithValue("@SOTIET", txtsotiet.Text);
                    nhapmh.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
                    nhapmh.ExecuteNonQuery();
                    nhapmh.CommandType = CommandType.StoredProcedure;
                    con.Close();

                    txtthongbao.Text = "Nhập môn học thành công";

                    checkmonhoc();
                }
                else
                {
                    if (String.IsNullOrEmpty(txtmamh.Text))
                    {
                        txtthongbao.Text = "Bạn chưa nhập mã môn học";
                    }
                    else
                    {
                        txtthongbao.Text = "Mã Khoa Không Tồn Tại";
                    }
                }
            }
        }

        private void xemmonhoc()
        {
            con.Open();
            SqlCommand xemmh = new SqlCommand("SP_XEMMONHOC", con);
            xemmh.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sda = new SqlDataAdapter(xemmh);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrmonhoc.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void xoamonhoc()
        {
            con.Open();
            SqlCommand xoamh = new SqlCommand("SP_XOAMONHOC", con);
            xoamh.CommandType = CommandType.StoredProcedure;
            xoamh.Parameters.AddWithValue("@MAMH", txtmamh.Text);
            SqlDataAdapter sda = new SqlDataAdapter(xoamh);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrmonhoc.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void capnhatmonhoc()
        {
            con.Open();
            SqlCommand suamh = new SqlCommand("SP_CAPNHATMONHOC", con);
            suamh.CommandType = CommandType.StoredProcedure;
            suamh.Parameters.AddWithValue("@MAMHCU", txtmamh.Text);
            suamh.Parameters.AddWithValue("@TENMHMOI", txttenmhmoi.Text);
            suamh.Parameters.AddWithValue("@SOTIETMOI", txtsotietmoi.Text);
            suamh.Parameters.AddWithValue("@MAKHOAMOI", txtmakhoamoi.Text);
            SqlDataAdapter sda = new SqlDataAdapter(suamh);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrmonhoc.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void timmonhoc()
        {
            con.Open();
            SqlCommand timmh = new SqlCommand("SP_TIMMONHOC", con);
            timmh.CommandType = CommandType.StoredProcedure;
            timmh.Parameters.AddWithValue("@MAMH", txtmamh.Text);
            timmh.Parameters.AddWithValue("@TENMH", txttenmh.Text);
            timmh.Parameters.AddWithValue("@SOTIET", txtsotiet.Text);
            timmh.Parameters.AddWithValue("@MAKHOA", txtmakhoa.Text);
            SqlDataAdapter sda = new SqlDataAdapter(timmh);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrmonhoc.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void hidetextboxmoi()
        {
            txttenmhmoi.Visibility = Visibility.Hidden;
            txtsotietmoi.Visibility = Visibility.Hidden;
            txtmakhoamoi.Visibility = Visibility.Hidden;
        }

        private void showtextboxmoi()
        {
            txttenmhmoi.Visibility = Visibility.Visible;
            txtsotietmoi.Visibility = Visibility.Visible;
            txtmakhoamoi.Visibility = Visibility.Visible;
        }

        private void ftextbox(int a)
        {
            if (a == 0)
            {
                txttenmh.Visibility = Visibility.Hidden;
                txtsotiet.Visibility = Visibility.Hidden;
                txtmakhoa.Visibility = Visibility.Hidden;
            }
            else if (a == 1)
            {
                txttenmh.Visibility = Visibility.Visible;
                txtsotiet.Visibility = Visibility.Visible;
                txtmakhoa.Visibility = Visibility.Visible;
            }
        }

        private void checkmonhoc()
        {
            con.Open();
            SqlCommand view = new SqlCommand("SP_CHECKMONHOC", con);
            view.CommandType = CommandType.StoredProcedure;
            view.Parameters.AddWithValue("@MAMH", txtmamh.Text);
            SqlDataAdapter sda = new SqlDataAdapter(view);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dtgrmonhoc.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void readwritetextbox(int a)
        {
            if (a == 1)
            {
                txtmamh.IsReadOnly = false;
                txttenmh.IsReadOnly = false;
                txtsotiet.IsReadOnly = false;
                txtmakhoa.IsReadOnly = false;
            }
            else if (a == 0)
            {
                txtmamh.IsReadOnly = true;
                txttenmh.IsReadOnly = true;
                txtsotiet.IsReadOnly = true;
                txtmakhoa.IsReadOnly = true;
            }
        }

        private void txtsotietmoi_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]");
        }

        private void txtsotiet_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]");
        }
    }
}
