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
    [Route("api/customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;

        public CustomerController(ICustomerService customerService, ILoggerFactory loggerFactory)
        {
            _customerService = customerService;
            _logger = loggerFactory.CreateLogger<CustomerController>();
        }

        // POST: api/BroadVoicePOC/customer
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDTO))]
        public virtual IActionResult GetCustomer(int id)
        {
            try
            {
                return Ok(_customerService.GetCustomer(id));
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