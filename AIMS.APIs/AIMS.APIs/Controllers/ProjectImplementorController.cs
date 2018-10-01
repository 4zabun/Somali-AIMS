﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectImplementorController : ControllerBase
    {
        AIMSDbContext context;
        IProjectImplementorService projectImplementorService;

        public ProjectImplementorController(AIMSDbContext cntxt, IProjectImplementorService service)
        {
            this.context = cntxt;
            this.projectImplementorService = service;
        }

        /// <summary>
        /// Gets list of organizations
        /// </summary>
        /// <returns>Will return an array or json objects</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var organizations = projectImplementorService.GetAll();
            return Ok(organizations);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectImplementorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectImplementorService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPost]
        [Route("RemoveImplementor")]
        public IActionResult RemoveImplementor([FromBody] ProjectImplementorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = projectImplementorService.RemoveImplementor(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok("1");
        }
    }
}