using BookAuthors.Data;
using BookAuthors.Models;
using BookAuthors.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookAuthors.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<List<SelectListItem>> GetAuthorOptionsAsync(IEnumerable<int>? selected = null)
        {
            var selectedSet = selected?.ToHashSet() ?? new HashSet<int>();
            return await _context.Authors
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.Name,
                    Selected = selectedSet.Contains(a.AuthorId)
                })
                .ToListAsync();
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Authors)
                .AsNoTracking()
                .ToListAsync();
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Authors)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return NotFound();
            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            var vm = new BookFormViewModel { AllAuthors = await GetAuthorOptionsAsync() };
            return View(vm);
        }

        // POST: Books/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllAuthors = await GetAuthorOptionsAsync(vm.SelectedAuthorIds);
                return View(vm);
            }

            var book = new Book { Title = vm.Title };

            if (vm.SelectedAuthorIds.Count > 0)
            {
                var authors = await _context.Authors
                    .Where(a => vm.SelectedAuthorIds.Contains(a.AuthorId))
                    .ToListAsync();
                foreach (var author in authors) book.Authors.Add(author);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Authors)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return NotFound();

            var selectedIds = book.Authors.Select(a => a.AuthorId).ToList();
            var vm = new BookFormViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                SelectedAuthorIds = selectedIds,
                AllAuthors = await GetAuthorOptionsAsync(selectedIds)
            };
            return View(vm);
        }

        // POST: Books/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookFormViewModel vm)
        {
            if (id != vm.BookId) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.AllAuthors = await GetAuthorOptionsAsync(vm.SelectedAuthorIds);
                return View(vm);
            }

            var book = await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return NotFound();

            book.Title = vm.Title;

            // Re-sync the many-to-many links
            book.Authors.Clear();
            if (vm.SelectedAuthorIds.Count > 0)
            {
                var authors = await _context.Authors
                    .Where(a => vm.SelectedAuthorIds.Contains(a.AuthorId))
                    .ToListAsync();
                foreach (var author in authors) book.Authors.Add(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Authors)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
