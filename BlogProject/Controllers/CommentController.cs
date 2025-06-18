using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogProject.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;
        public CommentController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Admin/Comment
        public async Task<IActionResult> Index()
        {
            //
            var comments = await _context.Comments.ToListAsync();
            return View(comments);
        }
        // POST: Admin/Comment
        //...

        // GET: Admin/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
                return NotFound();

            return View(comment);
        }
        // POST: Admin/Delete
        //...
    }
}

