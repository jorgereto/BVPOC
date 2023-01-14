using AutoMapper;
using BroadVoicePOC.Business.Interfaces;
using BroadVoicePOC.Common.Exceptions;
using BroadVoicePOC.DataAccess.Data;
using BroadVoicePOC.DataAccess.Models;
using BroadVoicePOC.Model.DTO;
using LinqKit;
using Microsoft.AspNetCore.Http;
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
    public class ProductService : IProductService
    {
        private readonly ILogger _logger;
        private readonly BroadVoicePOCContext _dbContext;
        private readonly IMapper _mapper;

        #region .ctors
        public ProductService(ILoggerFactory loggerFactory, IMapper mapper, BroadVoicePOCContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<ProductService>();
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Dispose() { }
        #endregion

        public ProductDTO GetProduct(int id)
        {
            try
            {
                var product = _dbContext.Products.Single(x => x.Id == id);
                var productDTO = _mapper.Map<ProductDTO>(product);
                return productDTO;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidInputException(ex.Message);
            }
        }

    }
}
