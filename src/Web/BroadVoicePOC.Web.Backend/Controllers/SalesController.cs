using BroadVoicePOC.Business.Interfaces;
using BroadVoicePOC.Common.Constants.Enums;
using BroadVoicePOC.Common.Exceptions;
using BroadVoicePOC.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BroadVoicePOC.Web.Backend.Controllers
{
    [Route("api/sales")]
    public class SalesController : Controller
    {
        private readonly ISalesService _salesService;
        private readonly ILogger _logger;

        public SalesController(ISalesService salesService, ILoggerFactory loggerFactory)
        {
            _salesService = salesService;
            _logger = loggerFactory.CreateLogger<SalesController>();
        }

        // POST: api/BroadVoicePOC/sales
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SaleDTO))]
        public virtual IActionResult GetSale(int id)
        {
            try
            {
                return Ok(_salesService.GetSale(id));
            }
            catch(InvalidInputException ex)
            {
                _logger.LogError(ex, null);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // POST: api/BroadVoicePOC/sales/dispatch
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SaleDTO>))]
        public virtual IActionResult Dispatch([FromBody] SearchDTO model)
        {
            try
            {
                return Ok(_salesService.SearchSales(model));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}