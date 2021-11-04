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
using System.Windows.Shapes;

namespace shop_sqlite
{
    /// <summary>
    /// Логика взаимодействия для Win_zakaz_tovar.xaml
    /// </summary>
    public partial class Win_zakaz_tovar : Window
    {
        public Win_zakaz_tovar()
        {
            InitializeComponent();
        }
        private void btn_win_zakaz_tovar_ok_Click(object sender, RoutedEventArgs e)
        {
            if (win_zakaz_tovar_txt_ostatok.Text == "")
            {
                MessageBox.Show("Заполните поле Количество!", "Ошибка!");
            }
            else
            {
                this.DialogResult = true;
            }
        }

        private void btn_win_zakaz_tovar_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
