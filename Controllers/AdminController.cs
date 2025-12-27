using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectPart1.Data;
using projectPart1.Models;

namespace projectPart1.Controllers
{
    public class AdminController : Controller
    {
        private readonly ArtGalleryDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ArtGalleryDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Dashboard
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("AccessDenied");
            }

            var stats = new AdminDashboardViewModel
            {
                TotalArtists = await _context.Artists.CountAsync(),
                TotalArtworks = await _context.Artworks.CountAsync(),
                TotalExhibitions = await _context.Exhibitions.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                ActiveArtists = await _context.Artists.Where(a => a.Status == ArtistStatus.Active).CountAsync(),
                ActiveExhibitions = await _context.Exhibitions.Where(e => e.Status == ExhibitionStatus.Active).CountAsync(),
                RecentArtworks = await _context.Artworks
                    .Include(a => a.Artist)
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(stats);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        #region Artist Management (CRUD Operations)

        public async Task<IActionResult> Artists(string searchTerm, int? status)
        {
            var query = _context.Artists.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Name.Contains(searchTerm) || a.Email.Contains(searchTerm));
            }

            if (status.HasValue)
            {
                query = query.Where(a => (int)a.Status == status.Value);
            }

            var artists = await query
                .Include(a => a.Artworks)
                .OrderByDescending(a => a.JoinedDate)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;
            return View(artists);
        }

        public IActionResult CreateArtist()
        {
            return View(new Artist());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArtist(Artist artist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingArtist = await _context.Artists
                        .FirstOrDefaultAsync(a => a.Email == artist.Email);

                    if (existingArtist != null)
                    {
                        ModelState.AddModelError("Email", "An artist with this email already exists.");
                        return View(artist);
                    }

                    artist.JoinedDate = DateTime.Now;
                    artist.Status = ArtistStatus.Active;

                    _context.Artists.Add(artist);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Artist created successfully!";
                    _logger.LogInformation("Artist created: {ArtistName}", artist.Name);
                    return RedirectToAction(nameof(Artists));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating artist");
                    ModelState.AddModelError("", "An error occurred while creating the artist.");
                }
            }
            return View(artist);
        }

        public async Task<IActionResult> EditArtist(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.Artworks)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtist(int id, Artist artist)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var duplicateEmail = await _context.Artists
                        .AnyAsync(a => a.Email == artist.Email && a.Id != id);

                    if (duplicateEmail)
                    {
                        ModelState.AddModelError("Email", "An artist with this email already exists.");
                        return View(artist);
                    }

                    _context.Update(artist);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Artist updated successfully!";
                    _logger.LogInformation("Artist updated: {ArtistId}", id);
                    return RedirectToAction(nameof(Artists));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ArtistExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.Artworks)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null)
            {
                return NotFound();
            }

            try
            {
                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Artist deleted successfully!";
                _logger.LogInformation("Artist deleted: {ArtistId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting artist {ArtistId}", id);
                TempData["Error"] = "Error deleting artist.";
            }

            return RedirectToAction(nameof(Artists));
        }

        private async Task<bool> ArtistExists(int id)
        {
            return await _context.Artists.AnyAsync(a => a.Id == id);
        }

        #endregion

        #region Artwork Management (CRUD Operations)

        public async Task<IActionResult> Artworks(string searchTerm, int? artistId, int? type)
        {
            var query = _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Exhibition)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm) || a.Description.Contains(searchTerm));
            }

            if (artistId.HasValue)
            {
                query = query.Where(a => a.ArtistId == artistId.Value);
            }

            if (type.HasValue)
            {
                query = query.Where(a => (int)a.Type == type.Value);
            }

            var artworks = await query
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();

            ViewBag.Artists = await _context.Artists
                .Where(a => a.Status == ArtistStatus.Active)
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.ArtistId = artistId;
            ViewBag.Type = type;

            return View(artworks);
        }

        public async Task<IActionResult> CreateArtwork()
        {
            ViewBag.Artists = await _context.Artists
                .Where(a => a.Status == ArtistStatus.Active)
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewBag.Exhibitions = await _context.Exhibitions
                .Where(e => e.Status == ExhibitionStatus.Active || e.Status == ExhibitionStatus.Upcoming)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return View(new Artwork());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArtwork(Artwork artwork)
        {
            ModelState.Remove("Artist");
            ModelState.Remove("Exhibition");
            ModelState.Remove("ArtistName");
            ModelState.Remove("ArtistEmail");
            ModelState.Remove("ArtistPhone");

            if (ModelState.IsValid)
            {
                try
                {
                    artwork.CreatedDate = DateTime.Now;
                    artwork.Status = ArtworkStatus.Active;
                    artwork.Likes = 0;

                    _context.Artworks.Add(artwork);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Artwork created successfully!";
                    _logger.LogInformation("Artwork created: {ArtworkTitle}", artwork.Title);
                    return RedirectToAction(nameof(Artworks));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating artwork");
                    ModelState.AddModelError("", "An error occurred while creating the artwork.");
                }
            }

            ViewBag.Artists = await _context.Artists
                .Where(a => a.Status == ArtistStatus.Active)
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewBag.Exhibitions = await _context.Exhibitions
                .Where(e => e.Status == ExhibitionStatus.Active || e.Status == ExhibitionStatus.Upcoming)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return View(artwork);
        }

        public async Task<IActionResult> EditArtwork(int id)
        {
            var artwork = await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Exhibition)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artwork == null)
            {
                return NotFound();
            }

            ViewBag.Artists = await _context.Artists
                .Where(a => a.Status == ArtistStatus.Active)
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewBag.Exhibitions = await _context.Exhibitions
                .Where(e => e.Status == ExhibitionStatus.Active || e.Status == ExhibitionStatus.Upcoming)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return View(artwork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtwork(int id, Artwork artwork)
        {
            if (id != artwork.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Artist");
            ModelState.Remove("Exhibition");
            ModelState.Remove("ArtistName");
            ModelState.Remove("ArtistEmail");
            ModelState.Remove("ArtistPhone");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artwork);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Artwork updated successfully!";
                    _logger.LogInformation("Artwork updated: {ArtworkId}", id);
                    return RedirectToAction(nameof(Artworks));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ArtworkExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.Artists = await _context.Artists
                .Where(a => a.Status == ArtistStatus.Active)
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewBag.Exhibitions = await _context.Exhibitions
                .Where(e => e.Status == ExhibitionStatus.Active || e.Status == ExhibitionStatus.Upcoming)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return View(artwork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArtwork(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);

            if (artwork == null)
            {
                return NotFound();
            }

            try
            {
                _context.Artworks.Remove(artwork);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Artwork deleted successfully!";
                _logger.LogInformation("Artwork deleted: {ArtworkId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting artwork {ArtworkId}", id);
                TempData["Error"] = "Error deleting artwork.";
            }

            return RedirectToAction(nameof(Artworks));
        }

        private async Task<bool> ArtworkExists(int id)
        {
            return await _context.Artworks.AnyAsync(a => a.Id == id);
        }

        #endregion

        #region Exhibition Management (CRUD Operations)

        public async Task<IActionResult> Exhibitions(string searchTerm, int? status)
        {
            var query = _context.Exhibitions.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchTerm) || e.Location.Contains(searchTerm));
            }

            if (status.HasValue)
            {
                query = query.Where(e => (int)e.Status == status.Value);
            }

            var exhibitions = await query
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;
            return View(exhibitions);
        }

        public IActionResult CreateExhibition()
        {
            return View(new Exhibition { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExhibition(Exhibition exhibition)
        {
            ModelState.Remove("FeaturedArtworks");

            if (ModelState.IsValid)
            {
                try
                {
                    exhibition.CreatedDate = DateTime.Now;
                    exhibition.UpdateStatus();

                    _context.Exhibitions.Add(exhibition);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Exhibition created successfully!";
                    _logger.LogInformation("Exhibition created: {ExhibitionName}", exhibition.Name);
                    return RedirectToAction(nameof(Exhibitions));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating exhibition");
                    ModelState.AddModelError("", "An error occurred while creating the exhibition.");
                }
            }
            return View(exhibition);
        }

        public async Task<IActionResult> EditExhibition(int id)
        {
            var exhibition = await _context.Exhibitions.FindAsync(id);

            if (exhibition == null)
            {
                return NotFound();
            }

            return View(exhibition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExhibition(int id, Exhibition exhibition)
        {
            if (id != exhibition.Id)
            {
                return NotFound();
            }

            ModelState.Remove("FeaturedArtworks");

            if (ModelState.IsValid)
            {
                try
                {
                    exhibition.UpdateStatus();
                    _context.Update(exhibition);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Exhibition updated successfully!";
                    _logger.LogInformation("Exhibition updated: {ExhibitionId}", id);
                    return RedirectToAction(nameof(Exhibitions));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ExhibitionExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(exhibition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExhibition(int id)
        {
            var exhibition = await _context.Exhibitions.FindAsync(id);

            if (exhibition == null)
            {
                return NotFound();
            }

            try
            {
                _context.Exhibitions.Remove(exhibition);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Exhibition deleted successfully!";
                _logger.LogInformation("Exhibition deleted: {ExhibitionId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exhibition {ExhibitionId}", id);
                TempData["Error"] = "Error deleting exhibition.";
            }

            return RedirectToAction(nameof(Exhibitions));
        }

        private async Task<bool> ExhibitionExists(int id)
        {
            return await _context.Exhibitions.AnyAsync(e => e.Id == id);
        }

        #endregion
    }

    public class AdminDashboardViewModel
    {
        public int TotalArtists { get; set; }
        public int TotalArtworks { get; set; }
        public int TotalExhibitions { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveArtists { get; set; }
        public int ActiveExhibitions { get; set; }
        public List<Artwork> RecentArtworks { get; set; } = new();
    }
}
