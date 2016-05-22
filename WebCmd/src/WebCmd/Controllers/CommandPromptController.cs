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

namespace WebCmd.Controllers
{
    [Authorize]
    public class CommandPromptController : Controller
    {
        private readonly CommandPromptService _commandPromptService;

        public CommandPromptController(CommandPromptService commandPromptService)
        {
            _commandPromptService = commandPromptService;
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
                return Json(result);
            }
            catch (InvalidOperationException e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(e.Message);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
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
                return Json(result);
            }
            catch (InvalidOperationException e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(e.Message);
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Invalid operation on server");
            }
        }
    }
}