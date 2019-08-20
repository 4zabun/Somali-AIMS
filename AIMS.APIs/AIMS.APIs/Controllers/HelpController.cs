﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIMS.Models;
using AIMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIMS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpController : ControllerBase
    {
        IHelpService service;

        public HelpController(IHelpService helpService)
        {
            service = helpService;
        }

        [HttpGet("GetProjectFields")]
        public IActionResult GetProjectFields()
        {
            return Ok(service.GetHelpForProjectFields());
        }

        [HttpGet("GetProjectFunderFields")]
        public IActionResult GetProjectFunderFields()
        {
            return Ok(service.GetHelpForProjectFunderFields());
        }

        [HttpGet("GetProjectImplementerFields")]
        public IActionResult GetProjectImplementerFields()
        {
            return Ok(service.GetHelpForProjectImpelenterFields());
        }

        [HttpGet("GetProjectDisbursementsFields")]
        public IActionResult GetProjectDisbursementsFields()
        {
            return Ok(service.GetHelpForProjectDisbursementFields());
        }

        [HttpGet("GetExpectedDisbursementsFields")]
        public IActionResult GetExpectedDisbursementsFields()
        {
            return Ok(service.GetHelpForProjectExpectedDisbursementFields());
        }

        [HttpGet("GetProjectDocumentsFields")]
        public IActionResult GetProjectDocumentsFields()
        {
            return Ok(service.GetHelpForProjectDocumentsFields());
        }

        [HttpPost("AddProjectHelp")]
        public IActionResult AddProjectHelp([FromBody] ProjectHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProject(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("AddProjectFunderHelp")]
        public IActionResult AddProjectFunderHelp([FromBody] ProjectFunderHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectFunder(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("AddProjectImplementerHelp")]
        public IActionResult AddProjectImplementerHelp([FromBody] ProjectImplementerHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectImplementer(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("AddProjectDisbursementHelp")]
        public IActionResult AddProjectDisbursementHelp([FromBody] ProjectDisbursementHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectDisbursement(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("AddProjectExpectedDisbursementHelp")]
        public IActionResult AddProjectExpectedDisbursementHelp([FromBody] ExpectedDisbursementHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectExpectedDisbursements(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }

        [HttpPost("AddProjectDocumentHelp")]
        public IActionResult AddProjectDocumentHelp([FromBody] ProjectDocumentHelp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = service.AddHelpForProjectDocument(model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(true);
        }
    }
}