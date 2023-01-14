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
    [Route("api/salesperson")]
    public class SalespersonController : Controller
    {
        private readonly ISalespersonService _salespersonService;
        private readonly ILogger _logger;

        public SalespersonController(ISalespersonService salespersonService, ILoggerFactory loggerFactory)
        {
            _salespersonService = salespersonService;
            _logger = loggerFactory.CreateLogger<SalespersonController>();
        }

        // POST: api/BroadVoicePOC/salesperson
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SalespersonDTO))]
        public virtual IActionResult GetSalesperson(int id)
        {
            try
            {
                return Ok(_salespersonService.GetSalesperson(id));
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

    }
}