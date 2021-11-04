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
    /// Логика взаимодействия для Win_tovar.xaml
    /// </summary>
    public partial class Win_tovar : Window
    {
        public Win_tovar()
        {
            InitializeComponent();
        }
        private void btn_win_tovar_ok_Click(object sender, RoutedEventArgs e)
        {
            if (win_tovar_txt_artikul.Text == "")
            {
                MessageBox.Show("Заполните поле Артикул!", "Ошибка!");
            }
            else
            {
                this.DialogResult = true;
            }
        }

        private void btn_win_tovar_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
