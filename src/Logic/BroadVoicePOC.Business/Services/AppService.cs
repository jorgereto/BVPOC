using AutoMapper;
using Bogus;
using BroadVoicePOC.Business.Interfaces;
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
    public class AppService : IAppService
    {
        private readonly ILogger _logger;
        private readonly BroadVoicePOCContext _dbContext;
        private readonly IMapper _mapper;

        #region .ctors
        public AppService(ILoggerFactory loggerFactory, IMapper mapper, BroadVoicePOCContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<AppService>();
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Dispose() { }
        #endregion

        public bool SeedDummyData()
        {
            _dbContext.Sales.RemoveRange(_dbContext.Sales);
            _dbContext.Customers.RemoveRange(_dbContext.Customers);
            _dbContext.Salespeople.RemoveRange(_dbContext.Salespeople);
            _dbContext.Products.RemoveRange(_dbContext.Products);
            _dbContext.SaveChanges();

            var customerFaker = new Faker<Customer>()
                .RuleFor(o => o.Id, f => 0)
                .RuleFor(o => o.Sales, f => null)
                .RuleFor(o => o.Email, f => f.Person.Email)
                .RuleFor(o => o.Name, f => f.Name.FullName());

            var salespersonFaker = new Faker<Salesperson>()
                .RuleFor(o => o.Id, f => 0)
                .RuleFor(o => o.Sales, f => null)
                .RuleFor(o => o.Email, f => f.Person.Email)
                .RuleFor(o => o.Name, f => f.Name.FullName());

            var productFaker = new Faker<Product>()
                .RuleFor(o => o.Id, f => 0)
                .RuleFor(o => o.Sales, f => null)
                .RuleFor(o => o.Code, f => f.Random.Replace("###-##-####"))
                .RuleFor(o => o.Name, f => f.Commerce.Product())
                .RuleFor(o => o.Cost, f => decimal.Parse(f.Commerce.Price()));

            _dbContext.Customers.AddRange(customerFaker.Generate(10));
            _dbContext.Salespeople.AddRange(salespersonFaker.Generate(100));
            _dbContext.Products.AddRange(productFaker.Generate(1000));
            _dbContext.SaveChanges();

            var minCustomerId = _dbContext.Customers.OrderBy(x => x.Id).First().Id;
            var minProductId = _dbContext.Products.OrderBy(x => x.Id).First().Id;
            var minSalespersonId = _dbContext.Salespeople.OrderBy(x => x.Id).First().Id;
            var saleFaker = new Faker<Sale>()
                .RuleFor(o => o.Id, f => 0)
                .RuleFor(o => o.Product, f => null)
                .RuleFor(o => o.Customer, f => null)
                .RuleFor(o => o.Salesperson, f => null)
                .RuleFor(o => o.ProductId, f => f.Random.Int(minProductId, minProductId + 999))
                .RuleFor(o => o.CustomerId, f => f.Random.Int(minCustomerId, minCustomerId + 9))
                .RuleFor(o => o.SalespersonId, f => f.Random.Int(minSalespersonId, minSalespersonId + 99))
                .RuleFor(o => o.City, f => f.Address.City())
                .RuleFor(o => o.State, f => f.Address.State())
                .RuleFor(o => o.Date, f => f.Date.Past(10))
                .RuleFor(o => o.Price, f => decimal.Parse(f.Commerce.Price()));

            _dbContext.Sales.AddRange(saleFaker.Generate(10000));
            _dbContext.SaveChanges();


            return true;
        }

    }
}
