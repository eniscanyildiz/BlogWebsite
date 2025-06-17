using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;
        public PostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Posts (Blog ana sayfası, postları listeler)
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .ToListAsync();
            return View(posts);
        }

        // GET: Posts/Details/5 (Post detay + yorumlar + yorum formu)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Posts/Details/5 (Yorum ekleme)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Yorum boş olamaz!";
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            // Burada basitçe UserId = 1 olarak sabitledim, login yoksa, değiştirebilirsin.
            var comment = new Comment
            {
                PostId = postId,
                Content = content,
                CreatedAt = DateTime.Now,
                UserId = 1
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = postId });
        }

        // CRUD işlemleri (Create, Edit, Delete) için aşağıdakileri ekleyebilirsin:
        // Create (GET/POST), Edit (GET/POST), Delete (GET/POST)
        // Örnek: Create GET
        public IActionResult Create()
        {
            ViewData["categories"] = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string content, int categoryId)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content) || categoryId == 0)
            {
                ModelState.AddModelError("", "Tüm alanları doldurunuz.");
                return View();
            }

            var post = new Post
            {
                Title = title,
                Content = content,
                UserId = 1,
                CreatedAt = DateTime.Now,
                CategoryId = categoryId
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

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            ViewData["Categories"] = _context.Categories.ToList();
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string title, string content, int categoryId)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content) || categoryId == 0)
            {
                ModelState.AddModelError("", "Tüm alanları doldurunuz.");

                // Kategorileri tekrar gönder ki form bozulmasın
                ViewData["Categories"] = _context.Categories.ToList();
                var existingPost = await _context.Posts.FindAsync(id);
                return View(existingPost);
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Güncellenecek alanlar
            post.Title = title;
            post.Content = content;
            post.CategoryId = categoryId;
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

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // POST: Posts/Delete/5
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
    }
}

