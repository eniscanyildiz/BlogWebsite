using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BlogProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View(new SqlQueryViewModel());
        }

        [HttpPost]
        public IActionResult Index(SqlQueryViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SqlQuery))
            {
                model.ErrorMessage = "Sorgu boş olamaz.";
                return View(model);
            }

            try
            {
                // Sadece SELECT komutuna izin ver
                if (!model.SqlQuery.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    model.ErrorMessage = "Sadece SELECT sorgularına izin verilmektedir.";
                    return View(model);
                }

                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(model.SqlQuery, connection))
                using (var adapter = new SqlDataAdapter(command))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    model.Result = dt;
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Sorgu çalıştırılırken hata oluştu: {ex.Message}";
            }

            return View(model);
        }
    }
}

