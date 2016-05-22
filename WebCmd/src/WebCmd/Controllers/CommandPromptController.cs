using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebCmd.Services;
using WebCmd.Models.CommandPrompt;
using System.Security.Claims;
using System.Net;
using Microsoft.Extensions.Logging;

namespace WebCmd.Controllers
{
    [Authorize]
    public class CommandPromptController : Controller
    {
        private readonly CommandPromptService _commandPromptService;
        private readonly ILogger<CommandPromptController> _logger;

        public CommandPromptController(CommandPromptService commandPromptService, ILoggerFactory loggerFactory)
        {
            _commandPromptService = commandPromptService;
            _logger = loggerFactory.CreateLogger<CommandPromptController>();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RunCommand([FromBody] CommandInfoVM commandInfoVM)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var result = _commandPromptService.RunCommand(userId, commandInfoVM);
                _logger.LogInformation($"{DateTime.Now} : Command {commandInfoVM.Command} strted with path {commandInfoVM.Path}");
                return Json(result);
            }
            catch (InvalidOperationException e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogInformation($"{DateTime.Now} : Expected error: {e.Message}");
                return Json(e.Message);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogCritical($"{DateTime.Now} : Critical error: {ex.Message} {ex.StackTrace}");
                return Json("Invalid operation on server");
            }
        }

        [HttpPost]
        public JsonResult CancelCommand([FromBody] CommandInfoVM commandInfoVM)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var result = _commandPromptService.CancelCommand(userId, commandInfoVM);
                _logger.LogInformation($"{DateTime.Now} : Command {commandInfoVM.Command} with path {commandInfoVM.Path} cancelled.");
                return Json(result);
            }
            catch (InvalidOperationException e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogInformation($"{DateTime.Now} : Expected error: {e.Message}");
                return Json(e.Message);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogCritical($"{DateTime.Now} : Critical error: {ex.Message} {ex.StackTrace}");
                return Json("Invalid operation on server");
            }
        }
    }
}