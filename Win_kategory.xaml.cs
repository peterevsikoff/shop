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
    /// Логика взаимодействия для Win_kategory.xaml
    /// </summary>
    public partial class Win_kategory : Window
    {
        public Win_kategory()
        {
            InitializeComponent();
        }
        private void btn_win_kategory_ok_Click(object sender, RoutedEventArgs e)
        {
            if (win_kategory_txt.Text == "")
            {
                MessageBox.Show("Заполните поле категории!", "Ошибка!");
            }
            else
            {
                this.DialogResult = true;
            }
        }

        private void btn_win_kategory_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
