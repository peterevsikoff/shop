//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace shop_sqlite
{
    using System;
    using System.Collections.Generic;
    
    public partial class Zakaz
    {
        public long ID_zakaz { get; set; }
        public long ID_tovar { get; set; }
        public Nullable<long> Kolvo { get; set; }
        public Nullable<System.DateTime> DateZakaz { get; set; }
    
        public virtual Tovar Tovar { get; set; }
    }
}
