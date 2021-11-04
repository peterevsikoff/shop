using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shop_sqlite
{
    public class Zakaztime
    {
        public int ID_zakaz { get; set; }
        public int ID_tovar { get; set; }
        public string Naimenovanie { get; set; }
        public int Kolvo { get; set; }
        public DateTime DateZakaz { get; set; }
    }
}
