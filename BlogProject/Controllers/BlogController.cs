using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Blog
        public async Task<IActionResult> Index()
        {
            var blogPosts = await _context.Posts.Include(b => b.Category).ToListAsync();
            return View(blogPosts);
        }

        // GET: Admin/Blog/Create
        public IActionResult Create()
        {
            ViewData["categories"] = _context.Categories.ToList();
            return View();
        }

        // POST: Admin/Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post blogPost)
        {
            var post = new Post
            {
                Title = blogPost.Title,
                Content = blogPost.Content,
                UserId = "287ed3cd-531f-42d5-9429-60085c50037e", 
                CreatedAt = DateTime.Now,
                CategoryId = blogPost.CategoryId
            };
            _context.Add(post);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Veritabanına kaydetme başarısız oldu.");
            return View();
        }

        // GET: Admin/Blog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var blogPost = await _context.Posts.FindAsync(id);
            if (blogPost == null)
                return NotFound();

            ViewData["categories"] = _context.Categories.ToList();
            return View(blogPost);
        }

        // POST: Admin/Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post blogPost)
        {

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Güncellenecek alanlar
            post.Title = blogPost.Title;
            post.Content = blogPost.Content;
            post.CategoryId = blogPost.CategoryId;
            post.CreatedAt = DateTime.Now;

            _context.Update(post);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Güncelleme başarısız oldu.");

            ViewData["Categories"] = _context.Categories.ToList();
            return View(post);
        }

        // GET: Admin/Blog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var blogPost = await _context.Posts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (blogPost == null)
                return NotFound();

            return View(blogPost);
        }

        // POST: Admin/Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}

