using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadVoicePOC.Model.DTO
{
    public class SaleDTO
    {
        public string City { get; set; }
        public string State { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public SalespersonDTO Salesperson { get; set; }
        public CustomerDTO Customer { get; set; }
        public ProductDTO Product { get; set; }
    }
}
