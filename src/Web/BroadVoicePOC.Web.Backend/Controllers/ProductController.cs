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
    [Route("api/product")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        public ProductController(IProductService productService, ILoggerFactory loggerFactory)
        {
            _productService = productService;
            _logger = loggerFactory.CreateLogger<ProductController>();
        }

        // POST: api/BroadVoicePOC/product
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        public virtual IActionResult GetProduct(int id)
        {
            try
            {
                return Ok(_productService.GetProduct(id));
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