﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogProject.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


    }
}

