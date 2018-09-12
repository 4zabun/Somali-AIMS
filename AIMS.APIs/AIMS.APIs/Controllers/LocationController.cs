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
    public class LocationController : ControllerBase
    {
        AIMSDbContext context;
        ILocationService locationService;

        public LocationController(AIMSDbContext cntxt, ILocationService service)
        {
            this.context = cntxt;
            this.locationService = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var locations = locationService.GetAll();
            return Ok(locations);
        }

        [HttpPost]
        public IActionResult Post([FromBody] LocationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = locationService.Add(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }

        [HttpPut]
        public IActionResult Put(int id, [FromBody] LocationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest("Invalid id provided");
            }

            var response = locationService.Update(id, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.ReturnedId);
        }
    }
}