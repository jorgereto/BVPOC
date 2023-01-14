using AutoMapper;
using BroadVoicePOC.DataAccess.Models;
using BroadVoicePOC.Model.DTO;

namespace BroadVoicePOC.Web.Backend
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Customer, CustomerDTO>();
            CreateMap<Salesperson, SalespersonDTO>();
            CreateMap<Product, ProductDTO>();
            CreateMap<Sale, SaleDTO>();
        }
    }
}
