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
    public class SalespersonService : ISalespersonService
    {
        private readonly ILogger _logger;
        private readonly BroadVoicePOCContext _dbContext;
        private readonly IMapper _mapper;

        #region .ctors
        public SalespersonService(ILoggerFactory loggerFactory, IMapper mapper, BroadVoicePOCContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<SalespersonService>();
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Dispose() { }
        #endregion

        public SalespersonDTO GetSalesperson(int id)
        {
            try
            {
                var salesperson = _dbContext.Salespeople.Single(x => x.Id == id);
                var salespersonDTO = _mapper.Map<SalespersonDTO>(salesperson);
                return salespersonDTO;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidInputException(ex.Message);
            }
        }

    }
}
