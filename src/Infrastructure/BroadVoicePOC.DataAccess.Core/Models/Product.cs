﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace BroadVoicePOC.DataAccess.Models
{
    public partial class Product
    {
        public Product()
        {
            Sales = new HashSet<Sale>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal? Cost { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}