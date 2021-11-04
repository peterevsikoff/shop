using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Xceed.Words.NET;
using Xceed.Document.NET;

namespace shop_sqlite
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        //var l;
        List<Tovar> l = new List<Tovar>();
        List<Zakaztime> lst_zakaztime = new List<Zakaztime>();
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                mainEntities shopent = new mainEntities();
                lst_kategory.ItemsSource = shopent.Kategories.ToList();
                lbl_vsego_kategory.Content = "Всего категорий: " + shopent.Kategories.Count().ToString();
                l = new List<Tovar>( shopent.Tovars.ToList().OrderByDescending(x => x.ID_tovar));
                dtg_tovar.ItemsSource = l;
                //dtg_tovar.ItemsSource = shopent.Tovars.ToList().OrderByDescending(x => x.ID_tovar);


                lbl_vsego_tovarov.Content = "Всего наименований: " + shopent.Tovars.Count().ToString();

                int obkol = 0;
                foreach (var t in shopent.Tovars)
                {
                    obkol += (int)t.Ostatok;
                }
                lbl_vsego_kol.Content = "Общее количество: " + obkol.ToString();

                double obsum = 0;
                foreach (var t in shopent.Tovars)
                {
                    obsum += (double)t.RoznCena;
                }
                lbl_vsego_summa.Content = "Общая сумма: " + obsum.ToString() + " BYN";



                dtg_zakaz.ItemsSource = (from t in shopent.Zakazs
                                         select new
                                         {
                                             ID_zakaz = t.ID_zakaz,
                                             DateZakaz = t.DateZakaz
                                         }).Distinct().ToList().OrderByDescending(x=>x.DateZakaz);
                lbl_all_zakaz.Content = "Всего заказов: " + dtg_zakaz.Items.Count.ToString();

            }
            catch { }
        }
        private SolidColorBrush hb = new SolidColorBrush(Colors.Orange);
        private SolidColorBrush nb = new SolidColorBrush(Colors.White);

        private void gridProducts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Tovar product = (Tovar)e.Row.DataContext;

            if (product.Ostatok <=1)
                e.Row.Background = hb;
            else
                e.Row.Background = nb;
        }
        private void dtg_zakaz_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                mainEntities shopent = new mainEntities();
                int b = int.Parse(lbl_tovar_in_zakaz.Content.ToString());
                var dtg = (from t in shopent.Tovars
                           join z in shopent.Zakazs on t.ID_tovar equals z.ID_tovar
                           where z.ID_zakaz == b
                           select new
                           {
                               Naimenovanie = t.Naimenovanie,
                               Kolvo = z.Kolvo,
                               Id_t = z.ID_tovar,
                               RoznCena = t.RoznCena
                           }).ToList();
                dtg_zakaz_po_tovaram.ItemsSource = dtg;
                lbl_tovar_in_zakaz2.Content = "Наименований: " + dtg_zakaz_po_tovaram.Items.Count.ToString();
                int k = 0;
                foreach (var d in dtg)
                {
                    k += (int)d.Kolvo;
                }
                
                lbl_tovar_in_zakaz_kolvo.Content = "Количество: " + k.ToString();

                double s = 0;
                foreach (var d in dtg)
                {
                    s += (double)d.RoznCena;
                }

                lbl_tovar_in_zakaz_summa.Content = "Сумма: " + s.ToString() + " BYN";
            }
            catch { }
        }
        private void btn_add_kategory_Click(object sender, RoutedEventArgs e)
        {
            mainEntities shopent = new mainEntities();
            Win_kategory win_kategory = new Win_kategory();
            if (win_kategory.ShowDialog() == true)
            {
                Kategory kategory = new Kategory()
                {
                    NaimenKategory = win_kategory.win_kategory_txt.Text
                };
                shopent.Kategories.Add(kategory);
                shopent.SaveChanges();

                mainEntities shopent_new = new mainEntities();
                lst_kategory.ItemsSource = shopent_new.Kategories.ToList();
                lbl_vsego_kategory.Content = "Всего категорий: " + shopent_new.Kategories.Count().ToString();
            }
        }
        private void btn_edit_kategory_Click(object sender, RoutedEventArgs e)
        {
            if (lst_kategory.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите категорию для изменения!");
            }
            else
            {
                mainEntities shopent = new mainEntities();
                Win_kategory win_kategory = new Win_kategory();

                Kategory path = lst_kategory.SelectedItem as Kategory;
                win_kategory.win_kategory_txt.Text = path.NaimenKategory;

                if (win_kategory.ShowDialog() == true)
                {
                    Kategory k = (from c in shopent.Kategories
                                  where c.NaimenKategory == path.NaimenKategory
                                  select c).SingleOrDefault<Kategory>();
                    k.NaimenKategory = win_kategory.win_kategory_txt.Text;
                    shopent.SaveChanges();

                    mainEntities shopent_new = new mainEntities();
                    lst_kategory.ItemsSource = shopent_new.Kategories.ToList();
                }
            }
        }
        private void btn_del_kategory_Click(object sender, RoutedEventArgs e)
        {
            if (lst_kategory.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите категорию для удаления!");
            }
            else
            {
                mainEntities shopent = new mainEntities();
                Kategory path = lst_kategory.SelectedItem as Kategory;

                bool result2 = shopent.Tovars.Any(u => u.ID_kategory == path.ID_kategory);
                if (result2 == true)
                {
                    MessageBox.Show("Категорию невозможно удалить! Есть товары данной категории!");

                }
                else
                {
                    var del_kategory = from d in shopent.Kategories
                                       where d.ID_kategory == path.ID_kategory
                                       select d;
                    shopent.Kategories.Remove(del_kategory.FirstOrDefault());
                    shopent.SaveChanges();

                    mainEntities shopent_new = new mainEntities();
                    lst_kategory.ItemsSource = shopent_new.Kategories.ToList();
                    lbl_vsego_kategory.Content = "Всего категорий: " + shopent_new.Kategories.Count().ToString();
                }
            }
        }
        private void lst_kategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mainEntities shopent = new mainEntities();
                Kategory path = lst_kategory.SelectedItem as Kategory;
                var dtg_tovar_itemsource = from t in shopent.Tovars
                                           where t.ID_kategory == path.ID_kategory
                                           select t;
                if (dtg_tovar_itemsource != null)
                {
                    l = new List<Tovar>(dtg_tovar_itemsource);
                    dtg_tovar.ItemsSource = l;
                    //dtg_tovar.ItemsSource = dtg_tovar_itemsource.ToList();
                    lbl_vsego_tovarov.Content = "Всего товаров: " + dtg_tovar_itemsource.Count().ToString();
                }
            }
            catch { }
        }

        private void btn_all_kategory_Click(object sender, RoutedEventArgs e)
        {
            lst_kategory.SelectedIndex = -1;
            mainEntities shopent = new mainEntities();
            l = new List<Tovar>(shopent.Tovars);
            dtg_tovar.ItemsSource = l;
            //dtg_tovar.ItemsSource = shopent.Tovars.ToList();
            lbl_vsego_tovarov.Content = "Всего товаров: " + shopent.Tovars.Count().ToString();
        }

        private void btn_add_tovar_Click(object sender, RoutedEventArgs e)
        {
            mainEntities shopent = new mainEntities();
            Win_tovar win_tovar = new Win_tovar();
            win_tovar.win_tovar_cmb_kategory.ItemsSource = shopent.Kategories.ToList();
            win_tovar.win_tovar_txt_optcena.Text = "0";
            win_tovar.win_tovar_txt_ostatok.Text = "0";
            win_tovar.win_tovar_txt_rozncena.Text = "0";

            if (win_tovar.ShowDialog() == true)
            {
                Kategory path = win_tovar.win_tovar_cmb_kategory.SelectedItem as Kategory;
                int id_kategory = (int)(from s in shopent.Kategories                       //id_kategory
                                        where s.NaimenKategory == path.NaimenKategory
                                        select s.ID_kategory).FirstOrDefault();
                Tovar tovar = new Tovar()
                {
                    ID_kategory = id_kategory,
                    Artikul = win_tovar.win_tovar_txt_artikul.Text,
                    Naimenovanie = win_tovar.win_tovar_txt_naimenovanie.Text,
                    OptCena = double.Parse(win_tovar.win_tovar_txt_optcena.Text.Replace('.', ',')),
                    RoznCena = double.Parse(win_tovar.win_tovar_txt_rozncena.Text.Replace('.', ',')),
                    Ostatok = int.Parse(win_tovar.win_tovar_txt_ostatok.Text)
                };
                shopent.Tovars.Add(tovar);
                shopent.SaveChanges();

                mainEntities shopent_new = new mainEntities();
                dtg_tovar.ItemsSource = shopent_new.Tovars.ToList();
                lbl_vsego_tovarov.Content = "Всего товаров: " + shopent_new.Tovars.Count().ToString();
            }
        }

        private void btn_edit_tovar_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_tovar.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите товар для изменения!");
            }
            else
            {
                mainEntities shopent = new mainEntities();
                Win_tovar win_tovar = new Win_tovar();

                Tovar path = dtg_tovar.SelectedItem as Tovar;
                win_tovar.win_tovar_cmb_kategory.ItemsSource = shopent.Kategories.ToList();


                for (int i = 0; i < win_tovar.win_tovar_cmb_kategory.Items.Count; i++)
                {
                    Kategory k = win_tovar.win_tovar_cmb_kategory.Items[i] as Kategory;
                    if (k.ID_kategory == path.ID_kategory)
                    {
                        //win_tovar.win_tovar_txt_ostatok.Text = i.ToString();
                        win_tovar.win_tovar_cmb_kategory.SelectedIndex = i;
                    }
                }
                win_tovar.win_tovar_txt_artikul.Text = path.Artikul;
                win_tovar.win_tovar_txt_naimenovanie.Text = path.Naimenovanie;
                win_tovar.win_tovar_txt_optcena.Text = path.OptCena.ToString();
                win_tovar.win_tovar_txt_rozncena.Text = path.RoznCena.ToString();
                win_tovar.win_tovar_txt_ostatok.Text = path.Ostatok.ToString();

                if (win_tovar.ShowDialog() == true)
                {
                    Tovar t = (from c in shopent.Tovars
                               where c.ID_tovar == path.ID_tovar
                               select c).SingleOrDefault<Tovar>();

                    Kategory p = win_tovar.win_tovar_cmb_kategory.SelectedItem as Kategory;
                    int id_kat = (int)(from s in shopent.Kategories                       //id_kategory
                                       where s.NaimenKategory == p.NaimenKategory
                                       select s.ID_kategory).FirstOrDefault();
                    t.ID_kategory = id_kat;
                    t.Artikul = win_tovar.win_tovar_txt_artikul.Text;
                    t.Naimenovanie = win_tovar.win_tovar_txt_naimenovanie.Text;
                    t.OptCena = double.Parse(win_tovar.win_tovar_txt_optcena.Text.Replace('.', ','));
                    t.RoznCena = double.Parse(win_tovar.win_tovar_txt_rozncena.Text.Replace('.',','));
                    t.Ostatok = int.Parse(win_tovar.win_tovar_txt_ostatok.Text);

                    shopent.SaveChanges();

                    mainEntities shopent_new = new mainEntities();
                    dtg_tovar.ItemsSource = shopent_new.Tovars.ToList();
                }
            }
        }

        private void btn_del_tovar_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_tovar.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите товар для удаления!");
            }
            else
            {
                mainEntities shopent = new mainEntities();


                Tovar path = dtg_tovar.SelectedItem as Tovar;


                          
                bool result1 = shopent.Zakazs.Any(u => u.ID_tovar == path.ID_tovar);
                if (result1 == true)
                {
                    MessageBox.Show("Товар невозможно удалить! Он есть в заказах");

                }
                else
                {
                    //MessageBox.Show("нет товара");
                    var del_tovar = from d in shopent.Tovars
                                    where d.ID_tovar == path.ID_tovar
                                    select d;


                    shopent.Tovars.Remove(del_tovar.FirstOrDefault());
                    shopent.SaveChanges();

                    mainEntities shopent_new = new mainEntities();
                    dtg_tovar.ItemsSource = shopent_new.Tovars.ToList();
                    lbl_vsego_tovarov.Content = "Всего товаров: " + shopent_new.Tovars.Count().ToString();
                }
               
                
            }
        }

        private void add_tovar_in_zakaz_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_tovar.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите товар для добавления в заказ!");
            }
            else
            {
                Win_zakaz_tovar win_zakaz_tovar = new Win_zakaz_tovar();

                mainEntities shopent = new mainEntities();
                Tovar path = dtg_tovar.SelectedItem as Tovar;
                win_zakaz_tovar.win_zakaz_tovar_artikul_lbl.Content = path.Artikul;
                win_zakaz_tovar.win_zakaz_tovar_naimenovanie_lbl.Content = path.Naimenovanie;
                win_zakaz_tovar.win_zakaz_tovar_optcena_lbl.Content = path.OptCena;
                win_zakaz_tovar.win_zakaz_tovar_rozncena_lbl.Content = path.RoznCena;
                win_zakaz_tovar.win_zakaz_tovar_txt_ostatok.Text = "1";

                if (win_zakaz_tovar.ShowDialog() == true)
                {
                    int k = (int)(from kt in shopent.Tovars
                             where kt.ID_tovar == (int)path.ID_tovar
                             select kt.Ostatok).FirstOrDefault();
                    if (int.Parse(win_zakaz_tovar.win_zakaz_tovar_txt_ostatok.Text)> k)
                    {
                        MessageBox.Show("Товара нет на складе!");
                    }
                    else
                    {
                        
                            int id_new_zakaz = 0;
                        try
                        {
                            id_new_zakaz = (from c in shopent.Zakazs
                                                    select c).Max(x => (int)x.ID_zakaz);
                        }
                        catch (System.InvalidOperationException)
                        {
                            id_new_zakaz = 0;
                        }

                        Zakaztime zakaztime = new Zakaztime()
                            {
                                ID_zakaz = id_new_zakaz,
                                ID_tovar = (int)path.ID_tovar,
                                Naimenovanie = path.Naimenovanie,
                                Kolvo = int.Parse(win_zakaz_tovar.win_zakaz_tovar_txt_ostatok.Text),
                                DateZakaz = DateTime.Today
                            };
                            lst_zakaztime.Add(zakaztime);
                            dtg_zakaz_po_tovaram.ItemsSource = (from t in lst_zakaztime
                                                                select new
                                                                {
                                                                    Naimenovanie = t.Naimenovanie,
                                                                    Kolvo = t.Kolvo
                                                                }).ToList();
                        
                    }
                    

                }
            }
        }

        private void btn_add_zakaz_Click(object sender, RoutedEventArgs e)
        {
            mainEntities shopent = new mainEntities();

            foreach (Zakaztime c in lst_zakaztime)
            {
                Zakaz zakaz = new Zakaz()
                {
                    ID_zakaz = c.ID_zakaz + 1,
                    ID_tovar = c.ID_tovar,
                    Kolvo = c.Kolvo,
                    DateZakaz = c.DateZakaz
                };
                shopent.Zakazs.Add(zakaz);

                Tovar t = (from z in shopent.Tovars
                           where z.ID_tovar == c.ID_tovar
                           select z).SingleOrDefault<Tovar>();
                t.Ostatok = t.Ostatok - c.Kolvo;
                shopent.SaveChanges();
            }
            mainEntities shopent_new = new mainEntities();
            dtg_tovar.ItemsSource = shopent_new.Tovars.ToList();
            dtg_zakaz.ItemsSource = ((from t in shopent_new.Zakazs
                                     select new
                                     {
                                         ID_zakaz = t.ID_zakaz,
                                         DateZakaz = t.DateZakaz
                                     }).Distinct()).ToList();
            dtg_zakaz.SelectedIndex = dtg_zakaz.Items.Count - 1;
            lst_zakaztime.Clear();
            lbl_all_zakaz.Content = "Всего заказов: " + dtg_zakaz.Items.Count.ToString();
            MessageBox.Show("Заказ оформлен!");
        }

        private void btn_del_zakaz_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_zakaz.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите заказ для отмены!");
            }
            else
            {
                mainEntities shopent = new mainEntities();
                int id = int.Parse(lbl_tovar_in_zakaz.Content.ToString());//id_zakaz
                var zak_del = from z in shopent.Zakazs
                              where z.ID_zakaz == id
                              select z;
                foreach (Zakaz c in zak_del)
                {
                    Tovar t = (from z in shopent.Tovars
                               where z.ID_tovar == c.ID_tovar
                               select z).FirstOrDefault<Tovar>();
                    t.Ostatok = t.Ostatok + c.Kolvo;
                }
                var del_zakaz = from d in shopent.Zakazs
                                where d.ID_zakaz == id
                                select d;
                foreach (Zakaz c in del_zakaz)
                {
                    shopent.Zakazs.Remove(c);
                }
                   
                shopent.SaveChanges();
                
                mainEntities shopent_new = new mainEntities();
                dtg_tovar.ItemsSource = shopent_new.Tovars.ToList();
                dtg_zakaz.ItemsSource = (from t in shopent_new.Zakazs
                                         select new
                                         {
                                             ID_zakaz = t.ID_zakaz,
                                             DateZakaz = t.DateZakaz
                                         }).Distinct().ToList();
                //dtg_zakaz.SelectedIndex = dtg_zakaz.Items.Count - 1;
                lst_zakaztime.Clear();
                dtg_zakaz_po_tovaram.ItemsSource = "";
                lbl_all_zakaz.Content = "Всего заказов: " + dtg_zakaz.Items.Count.ToString();
                lbl_tovar_in_zakaz2.Content = "";
            }

        }
        private void del_tovar_in_zakaz_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_zakaz_po_tovaram.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите товар для удаления!");
            }
            else
            {
                mainEntities shopent = new mainEntities();
                
                    int b = int.Parse(lbl_tovar_in_zakaz.Content.ToString());
                    int w = int.Parse(lbl_id_tovar.Content.ToString());
                    
                    var z = from c in shopent.Zakazs
                               where c.ID_zakaz == b & c.ID_tovar == w
                               select c;
                    shopent.Zakazs.Remove(z.FirstOrDefault());
                    shopent.SaveChanges();
                Tovar t = (from a in shopent.Tovars
                           where a.ID_tovar == w
                           select a).SingleOrDefault<Tovar>();
                t.Ostatok = t.Ostatok + int.Parse(lbl_kol_tovar.Content.ToString());
                shopent.SaveChanges();
               
                    mainEntities shopent_new = new mainEntities();
                    dtg_tovar.ItemsSource = shopent_new.Tovars.ToList();
                    int si = dtg_zakaz.SelectedIndex;
                    dtg_zakaz.SelectedIndex = -1;
                    dtg_zakaz.SelectedIndex = si;
                
            }
        }

        private void Txt_search_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_search_name.Text == "")
            {
                mainEntities shopent = new mainEntities();
                dtg_tovar.ItemsSource = shopent.Tovars.ToList();
                lbl_vsego_tovarov.Content = "Всего товаров: " + shopent.Tovars.Count().ToString();
            }
            else
            {
                mainEntities shopent = new mainEntities();
                //string n = txt_search_name.Text.ToLower();//.Trim(new char[] { ' ', ',' });
                var dtg_tovar_itemsource = from t in shopent.Tovars
                                           where t.Naimenovanie.ToLower().StartsWith(txt_search_name.Text.ToLower())
                                           select t;

                dtg_tovar.ItemsSource = dtg_tovar_itemsource.ToList();
                lbl_vsego_tovarov.Content = "Всего товаров: " + dtg_tovar_itemsource.Count().ToString();
            }
        }

        private void Txt_search_artikul_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_search_artikul.Text == "")
            {
                mainEntities shopent = new mainEntities();
                dtg_tovar.ItemsSource = shopent.Tovars.ToList();
                lbl_vsego_tovarov.Content = "Всего товаров: " + shopent.Tovars.Count().ToString();
            }
            else
            {
                mainEntities shopent = new mainEntities();
                var dtg_tovar_itemsource = from t in shopent.Tovars
                                           where t.Artikul.ToLower().StartsWith(txt_search_artikul.Text.ToLower())
                                           select t;

                dtg_tovar.ItemsSource = dtg_tovar_itemsource.ToList();
                lbl_vsego_tovarov.Content = "Всего товаров: " + dtg_tovar_itemsource.Count().ToString();
            }
        }

        private void Btn_edit_zakaz_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_zakaz_po_tovaram.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите товар для изменения!");
            }
            else
            {
                mainEntities shopent = new mainEntities();
                Win_change_tovarinzakaz win_change_tovarinzakaz = new Win_change_tovarinzakaz();
                win_change_tovarinzakaz.win_zakaz_change_naimenovanie_lbl.Content = lbl_name_tovar.Content.ToString();
                win_change_tovarinzakaz.win_zakaz_change_txt_ostatok.Text = lbl_kol_tovar.Content.ToString();


                if (win_change_tovarinzakaz.ShowDialog() == true)
                {
                    int b = int.Parse(lbl_tovar_in_zakaz.Content.ToString());
                    int w = int.Parse(lbl_id_tovar.Content.ToString());
                    //int id_tov = (int)(from s in shopent.Tovars                       //id_tovar
                    //                   where s.Naimenovanie == lbl_name_tovar.Content.ToString()
                    //                   select s.ID_tovar).FirstOrDefault();
                    //MessageBox.Show(id_tov.ToString());
                    Zakaz z = (from c in shopent.Zakazs
                               where c.ID_zakaz == b & c.ID_tovar == w
                               select c).SingleOrDefault<Zakaz>();
                    //MessageBox.Show(z.Kolvo.ToString());
                    z.Kolvo = int.Parse(win_change_tovarinzakaz.win_zakaz_change_txt_ostatok.Text);

                    //добавить отмену товара на склад
                    Tovar t = (from a in shopent.Tovars
                               where a.ID_tovar == w
                               select a).SingleOrDefault<Tovar>();
                    int ost = int.Parse(lbl_kol_tovar.Content.ToString());
                    if (ost > int.Parse(win_change_tovarinzakaz.win_zakaz_change_txt_ostatok.Text))
                    {
                        t.Ostatok = t.Ostatok + (ost - int.Parse(win_change_tovarinzakaz.win_zakaz_change_txt_ostatok.Text));
                    }
                    else
                    {
                        t.Ostatok = t.Ostatok - (int.Parse(win_change_tovarinzakaz.win_zakaz_change_txt_ostatok.Text) - ost);
                    }

                    shopent.SaveChanges();
                    mainEntities shopent_new = new mainEntities();
                    dtg_tovar.ItemsSource = shopent_new.Tovars.ToList();
                    int si = dtg_zakaz.SelectedIndex;
                    dtg_zakaz.SelectedIndex = -1;
                    dtg_zakaz.SelectedIndex = si;
                }
            }
        }

        private void Btn_easy_del_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_zakaz.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите заказ для удаления!");
            }
            else
            {
                mainEntities shopent = new mainEntities();
                int id = int.Parse(lbl_tovar_in_zakaz.Content.ToString());//id_zakaz
                var zak_del = from z in shopent.Zakazs
                              where z.ID_zakaz == id
                              select z;
                foreach (Zakaz c in zak_del)
                {
                    shopent.Zakazs.Remove(c);
                }

                shopent.SaveChanges();

                mainEntities shopent_new = new mainEntities();
                dtg_tovar.ItemsSource = shopent_new.Tovars.ToList();
                dtg_zakaz.ItemsSource = (from t in shopent_new.Zakazs
                                         select new
                                         {
                                             ID_zakaz = t.ID_zakaz,
                                             DateZakaz = t.DateZakaz
                                         }).Distinct().ToList();
                //dtg_zakaz.SelectedIndex = dtg_zakaz.Items.Count - 1;
                lst_zakaztime.Clear();
                dtg_zakaz_po_tovaram.ItemsSource = "";
                lbl_all_zakaz.Content = "Всего заказов: " + dtg_zakaz.Items.Count.ToString();
                lbl_tovar_in_zakaz2.Content = "";
            }
        }

        private void print_tov_Click(object sender, RoutedEventArgs e)
        {
            // путь к документу
            string pathDocument = AppDomain.CurrentDomain.BaseDirectory + "example.docx";

            // создаём документ
            DocX document = DocX.Create(pathDocument);

            // Вставляем параграф и указываем текст
            document.InsertParagraph("Товары магазина")
                .Font("Times New Roman")
                .FontSize(15)
                .Bold()
                .Alignment=Alignment.center;
            
            mainEntities shopent = new mainEntities();
            
            //var prin = shopent.Tovars.ToList().OrderByDescending(x => x.ID_tovar);
            var prin = l;
            //Tovar pth = dtg_tovar.SelectedItem as Tovar;
            //List<Tovar> pthall = new List<Tovar>();
            //// List<Tovar> pthall2 = dtg_tovar.Items as List<Tovar>;
            //List<DataGridRow> lrow = new List<DataGridRow>();
            //DataGridRow row = new DataGridRow();
            //for (int j=0; j<dtg_tovar.Items.Count;j++)
            //{
            //    lrow.Add((DataGridRow)dtg_tovar.ItemContainerGenerator.ContainerFromItem(j));

            //}
            //Tovar l = lrow.ElementAt(0) as Tovar;
            // создаём таблицу с 3 строками и 2 столбцами
            Xceed.Document.NET.Table table = document.AddTable(l.Count()+1, 5);
            // располагаем таблицу по центру
            table.Alignment = Alignment.center;
            // меняем стандартный дизайн таблицы
            table.Design = TableDesign.TableGrid;

            // заполнение ячейки текстом
            table.Rows[0].Cells[0].Paragraphs[0].Append("№ п/п").Font("Times New Roman").FontSize(13);
            table.Rows[0].Cells[1].Paragraphs[0].Append("Артикул").Font("Times New Roman").FontSize(13);
            table.Rows[0].Cells[2].Paragraphs[0].Append("Наименование").Font("Times New Roman").FontSize(13);
            table.Rows[0].Cells[3].Paragraphs[0].Append("Розничная цена").Font("Times New Roman").FontSize(13);
            table.Rows[0].Cells[4].Paragraphs[0].Append("Остаток").Font("Times New Roman").FontSize(13);

            //int i = 0;
            //for (int k=0;k<row.Item)
            //{ 

            //}
            //foreach (DataGridRow p in lrow)
            //{
            //    i++;

            //    table.Rows[i].Cells[0].Paragraphs[0].Append(i.ToString()).Font("Times New Roman").FontSize(13);
            //    table.Rows[i].Cells[1].Paragraphs[0].Append(p.Artikul).Font("Times New Roman").FontSize(13);
            //    table.Rows[i].Cells[2].Paragraphs[0].Append(p.Naimenovanie).Font("Times New Roman").FontSize(13);
            //    table.Rows[i].Cells[3].Paragraphs[0].Append(p.RoznCena.ToString()).Font("Times New Roman").FontSize(13);
            //    table.Rows[i].Cells[4].Paragraphs[0].Append(p.Ostatok.ToString()).Font("Times New Roman").FontSize(13);
            //}
            int i = 0;
            foreach (var p in prin)
            {
                i++;
                table.Rows[i].Cells[0].Paragraphs[0].Append(i.ToString()).Font("Times New Roman").FontSize(13);
                table.Rows[i].Cells[1].Paragraphs[0].Append(p.Artikul).Font("Times New Roman").FontSize(13);
                table.Rows[i].Cells[2].Paragraphs[0].Append(p.Naimenovanie).Font("Times New Roman").FontSize(13);
                table.Rows[i].Cells[3].Paragraphs[0].Append(p.RoznCena.ToString()).Font("Times New Roman").FontSize(13);
                table.Rows[i].Cells[4].Paragraphs[0].Append(p.Ostatok.ToString()).Font("Times New Roman").FontSize(13);
            }
            //for (int i =1; i<= shopent.Tovars.Count();i++)
            //{
            //    table.Rows[0].Cells[0].Paragraphs[0].Append("Артикул");
            //}






            // создаём параграф и вставляем таблицу
            document.InsertParagraph().InsertTableAfterSelf(table);
            // сохраняем документ
            document.Save();



        }
    }
}
