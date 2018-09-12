﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using AIMS.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        AIMSDbContext context;
        IUserService userService;

        public UserController(AIMSDbContext cntxt, IUserService service)
        {
            this.context = cntxt;
            this.userService = service;
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = userService.Add(user);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            var configuredSmtpClient = HttpContext.RequestServices.GetRequiredService<SmtpClient>();
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        public IActionResult Token([FromBody] AuthenticateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SecurityHelper sHelper = new SecurityHelper();
            model.Password = sHelper.GetPasswordHash(model.Password);
            var foundUser = userService.AuthenticateUser(model.UserName, model.Password);

            if (!string.IsNullOrEmpty(foundUser.UserName))
            {
                return Ok(foundUser);
            }
            return Unauthorized();
        }

        [HttpPost]
        public IActionResult EditUserOrganization([FromBody] EditUserOrganization model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = userService.UpdateOrganization(model.UserId, model.OrganizationId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }

        [HttpPost]
        public IActionResult EditPassword([FromBody] EditUserPassword model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = userService.UpdatePassword(model.UserId, model.Password);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Success);
        }
    }
}