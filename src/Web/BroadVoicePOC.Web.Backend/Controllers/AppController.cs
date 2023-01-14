using BroadVoicePOC.Business.Interfaces;
using BroadVoicePOC.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace BroadVoicePOC.Web.Backend.Controllers
{
    /// <summary>
    /// Application related utilities.
    /// </summary>
    [Route("~/api/app")]
    [ApiController]
    public class AppController : Controller
    {
        private ILogger<AppController> _logger;
        private readonly IAppService _appService;

        public AppController(ILogger<AppController> logger, IAppService appService)
        {
            _logger = logger;
            _appService = appService;
        }

        // GET: api/BroadVoicePOC/app/ping
        /// <summary>
        /// Simple "are-you-alive?" method. Useful for testing if authentication is valid.
        /// </summary>
        /// <returns></returns>
        [Route("ping")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult Get()
        {
            return Ok();
        }

        // GET: api/BroadVoicePOC/app/seed
        /// <summary>
        /// Simple "are-you-alive?" method. Useful for testing if authentication is valid.
        /// </summary>
        /// <returns></returns>
        [Route("seed")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual IActionResult Seed()
        {
            try
            {
                return Ok(_appService.SeedDummyData());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}