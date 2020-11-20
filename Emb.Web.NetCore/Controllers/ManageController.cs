using System;
using System.Threading.Tasks;
using Emb.Core.Models;
using Emb.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Emb.Web.NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageController : ControllerBase
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly MessageBrokerService _messageBrokerService;

        public ManageController(ApplicationSettings applicationSettings, MessageBrokerService messageBrokerService)
        {
            _applicationSettings = applicationSettings;
            _messageBrokerService = messageBrokerService;
        }

        public async Task<IActionResult> RunOnce()
        {
            try 
            {
                await _messageBrokerService.RunOnceAsync(_applicationSettings.DataFlows);
                return new JsonResult(new { status = true });
            }
            catch (Exception)
            {
                return new JsonResult(new { status = false });
            }
        }
    }
}
