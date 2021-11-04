using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для Win_change_tovarinzakaz.xaml
    /// </summary>
    public partial class Win_change_tovarinzakaz : Window
    {
        public Win_change_tovarinzakaz()
        {
            InitializeComponent();
        }

        private void Btn_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
