
using BookingIO.Models;
using BookingIO.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Reflection.Metadata;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Xml.Linq;
using System;
using BookingIO.Controllers;

public class SpaceTypeController : Controller
{
    private readonly SpaceTypeServices _spaceTypeService;

    public SpaceTypeController(SpaceTypeServices spaceTypeService)
    {
        _spaceTypeService = spaceTypeService;
    }

    public IActionResult Create()
    {
        return View("CadastrarSpaceType");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] string name)
    {
        var result = await _spaceTypeService.Create(name);

        return Ok(result);
    }
   
}

