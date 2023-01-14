using AutoMapper;
using BroadVoicePOC.Business.Interfaces;
using BroadVoicePOC.Common.Exceptions;
using BroadVoicePOC.DataAccess.Data;
using BroadVoicePOC.DataAccess.Models;
using BroadVoicePOC.Model.DTO;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPush;

namespace BroadVoicePOC.Business.Services
{
    public class SalesService : ISalesService
    {
        private readonly ILogger _logger;
        private readonly BroadVoicePOCContext _dbContext;
        private readonly IMapper _mapper;

        #region .ctors
        public SalesService(ILoggerFactory loggerFactory, IMapper mapper, BroadVoicePOCContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<SalesService>();
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Dispose() { }
        #endregion

        public SaleDTO GetSale(int id)
        {
            try
            {
                var sale = _dbContext.Sales
                    .Include(x => x.Salesperson)
                    .Include(x => x.Product)
                    .Include(x => x.Customer)
                    .Single(x => x.Id == id);
                var saleDTO = _mapper.Map<SaleDTO>(sale);
                return saleDTO;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidInputException(ex.Message);
            }
        }

        public List<SaleDTO> SearchSales(SearchDTO model)
        {
            ExpressionStarter<Sale> predicate = PredicateBuilder.New<Sale>(true);
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.Sale?.Salesperson?.Name))
                {
                    predicate = predicate.And(x => x.Salesperson.Name != null && x.Salesperson.Name.Contains(model.Sale.Salesperson.Name));
                }
                if (!string.IsNullOrEmpty(model.Sale?.Product?.Code))
                {
                    predicate = predicate.And(x => x.Product.Code.Contains(model.Sale.Product.Code));
                }
                if (!string.IsNullOrEmpty(model.Sale?.Customer?.Email))
                {
                    predicate = predicate.And(x => x.Customer.Email == model.Sale.Customer.Email);
                }
                if (!string.IsNullOrEmpty(model.Sale?.State))
                {
                    predicate = predicate.And(x => x.State == model.Sale.State);
                }
                if (!string.IsNullOrEmpty(model.Sale?.City))
                {
                    predicate = predicate.And(x => x.City == model.Sale.City);
                }
                if (model.StartDate.HasValue)
                {
                    predicate = predicate.And(x => x.Date >= model.StartDate);
                }
                if (model.EndDate.HasValue)
                {
                    predicate = predicate.And(x => x.Date <= model.EndDate);
                }
            }

            var query = _dbContext.Set<Sale>()
            .AsExpandable()
            .Where(predicate)
            .Include(x => x.Salesperson)
            .Include(x => x.Product)
            .Include(x => x.Customer);

            var dbResult = query.ToList();
            var result = _mapper.Map<List<SaleDTO>>(dbResult);
            return result;
        }
    }
}
