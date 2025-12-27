using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectPart1.Data;
using projectPart1.Models;

namespace projectPart1.Controllers
{
    public class UserController : Controller
    {
        private readonly ArtGalleryDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ArtGalleryDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Home & Browse

        public async Task<IActionResult> Index()
        {
            var viewModel = new UserHomeViewModel
            {
                FeaturedArtworks = await _context.Artworks
                    .Include(a => a.Artist)
                    .Where(a => a.Status == ArtworkStatus.Active)
                    .OrderByDescending(a => a.Likes)
                    .Take(6)
                    .ToListAsync(),

                RecentArtworks = await _context.Artworks
                    .Include(a => a.Artist)
                    .Where(a => a.Status == ArtworkStatus.Active)
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(6)
                    .ToListAsync(),

                UpcomingExhibitions = await _context.Exhibitions
                    .Where(e => e.Status == ExhibitionStatus.Upcoming || e.Status == ExhibitionStatus.Active)
                    .OrderBy(e => e.StartDate)
                    .Take(3)
                    .ToListAsync(),

                FeaturedArtists = await _context.Artists
                    .Include(a => a.Artworks)
                    .Where(a => a.Status == ArtistStatus.Active)
                    .OrderByDescending(a => a.Artworks!.Count)
                    .Take(4)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        #endregion

        #region Artwork Operations (Search, View, Like)

        public async Task<IActionResult> Artworks(string searchTerm, int? type, int? artistId, string sortBy, bool? forSale)
        {
            var query = _context.Artworks
                .Include(a => a.Artist)
                .Where(a => a.Status == ArtworkStatus.Active)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => 
                    a.Title.Contains(searchTerm) || 
                    a.Description.Contains(searchTerm) ||
                    a.Artist!.Name.Contains(searchTerm));
            }

            if (type.HasValue)
            {
                query = query.Where(a => (int)a.Type == type.Value);
            }

            if (artistId.HasValue)
            {
                query = query.Where(a => a.ArtistId == artistId.Value);
            }

            if (forSale.HasValue && forSale.Value)
            {
                query = query.Where(a => a.IsForSale);
            }

            query = sortBy switch
            {
                "newest" => query.OrderByDescending(a => a.CreatedDate),
                "oldest" => query.OrderBy(a => a.CreatedDate),
                "mostLiked" => query.OrderByDescending(a => a.Likes),
                "title" => query.OrderBy(a => a.Title),
                "priceAsc" => query.OrderBy(a => a.Price ?? 0),
                "priceDesc" => query.OrderByDescending(a => a.Price ?? 0),
                _ => query.OrderByDescending(a => a.CreatedDate)
            };

            var artworks = await query.ToListAsync();

            ViewBag.Artists = await _context.Artists
                .Where(a => a.Status == ArtistStatus.Active)
                .OrderBy(a => a.Name)
                .Select(a => new { a.Id, a.Name })
                .ToListAsync();

            ViewBag.TypeCounts = await _context.Artworks
                .Where(a => a.Status == ArtworkStatus.Active)
                .GroupBy(a => a.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Type = type;
            ViewBag.ArtistId = artistId;
            ViewBag.SortBy = sortBy;
            ViewBag.ForSale = forSale;
            ViewBag.TotalCount = artworks.Count;

            return View(artworks);
        }

        public async Task<IActionResult> ArtworkDetails(int id)
        {
            var artwork = await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Exhibition)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artwork == null)
            {
                return NotFound();
            }

            var relatedArtworks = await _context.Artworks
                .Include(a => a.Artist)
                .Where(a => a.Id != id && 
                           a.Status == ArtworkStatus.Active &&
                           (a.ArtistId == artwork.ArtistId || a.Type == artwork.Type))
                .OrderByDescending(a => a.Likes)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedArtworks = relatedArtworks;

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                ViewBag.HasLiked = await _context.ArtworkLikes
                    .AnyAsync(l => l.ArtworkId == id && l.UserId == userId.Value);
            }

            return View(artwork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeArtwork(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);

            if (artwork == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var existingLike = await _context.ArtworkLikes
                .FirstOrDefaultAsync(l => l.ArtworkId == id && 
                    (l.UserId == userId || (userId == null && l.IpAddress == ipAddress)));

            if (existingLike != null)
            {
                TempData["Info"] = "You have already liked this artwork.";
                return RedirectToAction(nameof(ArtworkDetails), new { id });
            }

            var like = new ArtworkLike
            {
                ArtworkId = id,
                UserId = userId,
                IpAddress = ipAddress,
                LikedDate = DateTime.Now
            };
            _context.ArtworkLikes.Add(like);

            artwork.Likes++;
            _context.Update(artwork);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Artwork liked: {ArtworkId} by User: {UserId}", id, userId);
            TempData["Success"] = "Thank you for liking this artwork!";

            return RedirectToAction(nameof(ArtworkDetails), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlikeArtwork(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var existingLike = await _context.ArtworkLikes
                .FirstOrDefaultAsync(l => l.ArtworkId == id && 
                    (l.UserId == userId || (userId == null && l.IpAddress == ipAddress)));

            if (existingLike == null)
            {
                return RedirectToAction(nameof(ArtworkDetails), new { id });
            }

            _context.ArtworkLikes.Remove(existingLike);

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork != null && artwork.Likes > 0)
            {
                artwork.Likes--;
                _context.Update(artwork);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Artwork unliked: {ArtworkId} by User: {UserId}", id, userId);

            return RedirectToAction(nameof(ArtworkDetails), new { id });
        }

        #endregion

        #region Artist Operations (Browse, View)

        public async Task<IActionResult> Artists(string searchTerm, string sortBy)
        {
            var query = _context.Artists
                .Include(a => a.Artworks)
                .Where(a => a.Status == ArtistStatus.Active)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Name.Contains(searchTerm) || a.Bio.Contains(searchTerm));
            }

            query = sortBy switch
            {
                "name" => query.OrderBy(a => a.Name),
                "newest" => query.OrderByDescending(a => a.JoinedDate),
                "mostArtworks" => query.OrderByDescending(a => a.Artworks!.Count),
                "mostLikes" => query.OrderByDescending(a => a.Artworks!.Sum(aw => aw.Likes)),
                _ => query.OrderBy(a => a.Name)
            };

            var artists = await query.ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;

            return View(artists);
        }

        public async Task<IActionResult> ArtistDetails(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.Artworks!.Where(aw => aw.Status == ArtworkStatus.Active))
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null || artist.Status != ArtistStatus.Active)
            {
                return NotFound();
            }

            ViewBag.TotalLikes = artist.Artworks?.Sum(a => a.Likes) ?? 0;
            ViewBag.ArtworksForSale = artist.Artworks?.Count(a => a.IsForSale) ?? 0;

            return View(artist);
        }

        #endregion

        #region Exhibition Operations (Browse, View)

        public async Task<IActionResult> Exhibitions(string searchTerm, int? status)
        {
            var query = _context.Exhibitions.AsQueryable();

            query = query.Where(e => e.Status != ExhibitionStatus.Cancelled);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchTerm) || 
                                        e.Description.Contains(searchTerm) ||
                                        e.Location.Contains(searchTerm));
            }

            if (status.HasValue)
            {
                query = query.Where(e => (int)e.Status == status.Value);
            }

            var exhibitions = await query
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;

            return View(exhibitions);
        }

        public async Task<IActionResult> ExhibitionDetails(int id)
        {
            var exhibition = await _context.Exhibitions.FindAsync(id);

            if (exhibition == null || exhibition.Status == ExhibitionStatus.Cancelled)
            {
                return NotFound();
            }

            var exhibitionArtworks = await _context.ExhibitionArtworks
                .Where(ea => ea.ExhibitionId == id)
                .Join(_context.Artworks.Include(a => a.Artist),
                    ea => ea.ArtworkId,
                    a => a.Id,
                    (ea, a) => new { Artwork = a, ea.IsFeatured, ea.DisplayOrder })
                .OrderByDescending(x => x.IsFeatured)
                .ThenBy(x => x.DisplayOrder)
                .Select(x => x.Artwork)
                .ToListAsync();

            ViewBag.ExhibitionArtworks = exhibitionArtworks;

            return View(exhibition);
        }

        #endregion

        #region User Profile

        public async Task<IActionResult> MyLikes()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var likedArtworks = await _context.ArtworkLikes
                .Where(l => l.UserId == userId.Value)
                .Join(_context.Artworks.Include(a => a.Artist),
                    l => l.ArtworkId,
                    a => a.Id,
                    (l, a) => new { Artwork = a, LikedDate = l.LikedDate })
                .OrderByDescending(x => x.LikedDate)
                .Select(x => x.Artwork)
                .ToListAsync();

            return View(likedArtworks);
        }

        #endregion
    }

    public class UserHomeViewModel
    {
        public List<Artwork> FeaturedArtworks { get; set; } = new();
        public List<Artwork> RecentArtworks { get; set; } = new();
        public List<Exhibition> UpcomingExhibitions { get; set; } = new();
        public List<Artist> FeaturedArtists { get; set; } = new();
    }
}
