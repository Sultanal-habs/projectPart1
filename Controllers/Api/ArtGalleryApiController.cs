using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectPart1.Data;
using projectPart1.Models;
using System.ComponentModel.DataAnnotations;

namespace projectPart1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtGalleryApiController : ControllerBase
    {
        private readonly ArtGalleryDbContext _context;
        private readonly ILogger<ArtGalleryApiController> _logger;

        public ArtGalleryApiController(ArtGalleryDbContext context, ILogger<ArtGalleryApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Artwork API Endpoints

        [HttpGet("artworks")]
        public async Task<ActionResult<ApiResponse<List<ArtworkDto>>>> GetArtworks(
            [FromQuery] string? searchTerm,
            [FromQuery] int? type,
            [FromQuery] int? artistId,
            [FromQuery] bool? forSale,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
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
                    _ => query.OrderByDescending(a => a.CreatedDate)
                };

                var totalCount = await query.CountAsync();

                var artworks = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new ArtworkDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        ImageUrl = a.ImageUrl,
                        Type = a.Type.ToString(),
                        Likes = a.Likes,
                        Price = a.Price,
                        IsForSale = a.IsForSale,
                        CreatedDate = a.CreatedDate,
                        ArtistId = a.ArtistId,
                        ArtistName = a.Artist!.Name
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ArtworkDto>>
                {
                    Success = true,
                    Data = artworks,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting artworks");
                return StatusCode(500, new ApiResponse<List<ArtworkDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving artworks."
                });
            }
        }

        [HttpGet("artworks/{id}")]
        public async Task<ActionResult<ApiResponse<ArtworkDto>>> GetArtwork(int id)
        {
            try
            {
                var artwork = await _context.Artworks
                    .Include(a => a.Artist)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (artwork == null)
                {
                    return NotFound(new ApiResponse<ArtworkDto>
                    {
                        Success = false,
                        Message = "Artwork not found."
                    });
                }

                return Ok(new ApiResponse<ArtworkDto>
                {
                    Success = true,
                    Data = new ArtworkDto
                    {
                        Id = artwork.Id,
                        Title = artwork.Title,
                        Description = artwork.Description,
                        ImageUrl = artwork.ImageUrl,
                        Type = artwork.Type.ToString(),
                        Likes = artwork.Likes,
                        Price = artwork.Price,
                        IsForSale = artwork.IsForSale,
                        CreatedDate = artwork.CreatedDate,
                        ArtistId = artwork.ArtistId,
                        ArtistName = artwork.Artist?.Name ?? "Unknown"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting artwork {ArtworkId}", id);
                return StatusCode(500, new ApiResponse<ArtworkDto>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the artwork."
                });
            }
        }

        [HttpPost("artworks")]
        public async Task<ActionResult<ApiResponse<ArtworkDto>>> CreateArtwork([FromBody] CreateArtworkDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<ArtworkDto>
                    {
                        Success = false,
                        Message = "Invalid data provided.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var artistExists = await _context.Artists.AnyAsync(a => a.Id == dto.ArtistId);
                if (!artistExists)
                {
                    return BadRequest(new ApiResponse<ArtworkDto>
                    {
                        Success = false,
                        Message = "Artist not found."
                    });
                }

                var artwork = new Artwork
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl ?? "",
                    Type = Enum.Parse<ArtworkType>(dto.Type),
                    ArtistId = dto.ArtistId,
                    Price = dto.Price,
                    IsForSale = dto.IsForSale,
                    Status = ArtworkStatus.Active,
                    Likes = 0,
                    CreatedDate = DateTime.Now
                };

                _context.Artworks.Add(artwork);
                await _context.SaveChangesAsync();

                _logger.LogInformation("API: Artwork created: {ArtworkId}", artwork.Id);

                var artist = await _context.Artists.FindAsync(dto.ArtistId);

                return CreatedAtAction(nameof(GetArtwork), new { id = artwork.Id }, new ApiResponse<ArtworkDto>
                {
                    Success = true,
                    Message = "Artwork created successfully.",
                    Data = new ArtworkDto
                    {
                        Id = artwork.Id,
                        Title = artwork.Title,
                        Description = artwork.Description,
                        ImageUrl = artwork.ImageUrl,
                        Type = artwork.Type.ToString(),
                        Likes = artwork.Likes,
                        Price = artwork.Price,
                        IsForSale = artwork.IsForSale,
                        CreatedDate = artwork.CreatedDate,
                        ArtistId = artwork.ArtistId,
                        ArtistName = artist?.Name ?? "Unknown"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating artwork");
                return StatusCode(500, new ApiResponse<ArtworkDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the artwork."
                });
            }
        }

        [HttpPut("artworks/{id}")]
        public async Task<ActionResult<ApiResponse<ArtworkDto>>> UpdateArtwork(int id, [FromBody] UpdateArtworkDto dto)
        {
            try
            {
                var artwork = await _context.Artworks
                    .Include(a => a.Artist)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (artwork == null)
                {
                    return NotFound(new ApiResponse<ArtworkDto>
                    {
                        Success = false,
                        Message = "Artwork not found."
                    });
                }

                if (!string.IsNullOrEmpty(dto.Title))
                    artwork.Title = dto.Title;

                if (!string.IsNullOrEmpty(dto.Description))
                    artwork.Description = dto.Description;

                if (!string.IsNullOrEmpty(dto.ImageUrl))
                    artwork.ImageUrl = dto.ImageUrl;

                if (!string.IsNullOrEmpty(dto.Type) && Enum.TryParse<ArtworkType>(dto.Type, out var type))
                    artwork.Type = type;

                if (dto.Price.HasValue)
                    artwork.Price = dto.Price.Value;

                if (dto.IsForSale.HasValue)
                    artwork.IsForSale = dto.IsForSale.Value;

                _context.Update(artwork);
                await _context.SaveChangesAsync();

                _logger.LogInformation("API: Artwork updated: {ArtworkId}", id);

                return Ok(new ApiResponse<ArtworkDto>
                {
                    Success = true,
                    Message = "Artwork updated successfully.",
                    Data = new ArtworkDto
                    {
                        Id = artwork.Id,
                        Title = artwork.Title,
                        Description = artwork.Description,
                        ImageUrl = artwork.ImageUrl,
                        Type = artwork.Type.ToString(),
                        Likes = artwork.Likes,
                        Price = artwork.Price,
                        IsForSale = artwork.IsForSale,
                        CreatedDate = artwork.CreatedDate,
                        ArtistId = artwork.ArtistId,
                        ArtistName = artwork.Artist?.Name ?? "Unknown"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating artwork {ArtworkId}", id);
                return StatusCode(500, new ApiResponse<ArtworkDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the artwork."
                });
            }
        }

        [HttpDelete("artworks/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteArtwork(int id)
        {
            try
            {
                var artwork = await _context.Artworks.FindAsync(id);

                if (artwork == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Artwork not found."
                    });
                }

                _context.Artworks.Remove(artwork);
                await _context.SaveChangesAsync();

                _logger.LogInformation("API: Artwork deleted: {ArtworkId}", id);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Artwork deleted successfully.",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting artwork {ArtworkId}", id);
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "An error occurred while deleting the artwork."
                });
            }
        }

        [HttpPost("artworks/{id}/like")]
        public async Task<ActionResult<ApiResponse<int>>> LikeArtwork(int id)
        {
            try
            {
                var artwork = await _context.Artworks.FindAsync(id);

                if (artwork == null)
                {
                    return NotFound(new ApiResponse<int>
                    {
                        Success = false,
                        Message = "Artwork not found."
                    });
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var existingLike = await _context.ArtworkLikes
                    .FirstOrDefaultAsync(l => l.ArtworkId == id && l.IpAddress == ipAddress);

                if (existingLike != null)
                {
                    return BadRequest(new ApiResponse<int>
                    {
                        Success = false,
                        Message = "You have already liked this artwork.",
                        Data = artwork.Likes
                    });
                }

                var like = new ArtworkLike
                {
                    ArtworkId = id,
                    IpAddress = ipAddress,
                    LikedDate = DateTime.Now
                };
                _context.ArtworkLikes.Add(like);

                artwork.Likes++;
                _context.Update(artwork);

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Message = "Artwork liked successfully.",
                    Data = artwork.Likes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error liking artwork {ArtworkId}", id);
                return StatusCode(500, new ApiResponse<int>
                {
                    Success = false,
                    Message = "An error occurred while liking the artwork."
                });
            }
        }

        #endregion

        #region Artist API Endpoints

        [HttpGet("artists")]
        public async Task<ActionResult<ApiResponse<List<ArtistDto>>>> GetArtists(
            [FromQuery] string? searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Artists
                    .Include(a => a.Artworks)
                    .Where(a => a.Status == ArtistStatus.Active)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(a => a.Name.Contains(searchTerm) || a.Bio.Contains(searchTerm));
                }

                var totalCount = await query.CountAsync();

                var artists = await query
                    .OrderBy(a => a.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new ArtistDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Email = a.Email,
                        Phone = a.Phone,
                        Bio = a.Bio,
                        ProfileImageUrl = a.ProfileImageUrl,
                        JoinedDate = a.JoinedDate,
                        ArtworkCount = a.Artworks != null ? a.Artworks.Count : 0,
                        TotalLikes = a.Artworks != null ? a.Artworks.Sum(aw => aw.Likes) : 0
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ArtistDto>>
                {
                    Success = true,
                    Data = artists,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting artists");
                return StatusCode(500, new ApiResponse<List<ArtistDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving artists."
                });
            }
        }

        [HttpGet("artists/{id}")]
        public async Task<ActionResult<ApiResponse<ArtistDto>>> GetArtist(int id)
        {
            try
            {
                var artist = await _context.Artists
                    .Include(a => a.Artworks)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (artist == null)
                {
                    return NotFound(new ApiResponse<ArtistDto>
                    {
                        Success = false,
                        Message = "Artist not found."
                    });
                }

                return Ok(new ApiResponse<ArtistDto>
                {
                    Success = true,
                    Data = new ArtistDto
                    {
                        Id = artist.Id,
                        Name = artist.Name,
                        Email = artist.Email,
                        Phone = artist.Phone,
                        Bio = artist.Bio,
                        ProfileImageUrl = artist.ProfileImageUrl,
                        JoinedDate = artist.JoinedDate,
                        ArtworkCount = artist.Artworks?.Count ?? 0,
                        TotalLikes = artist.Artworks?.Sum(a => a.Likes) ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting artist {ArtistId}", id);
                return StatusCode(500, new ApiResponse<ArtistDto>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the artist."
                });
            }
        }

        [HttpPost("artists")]
        public async Task<ActionResult<ApiResponse<ArtistDto>>> CreateArtist([FromBody] CreateArtistDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<ArtistDto>
                    {
                        Success = false,
                        Message = "Invalid data provided.",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var emailExists = await _context.Artists.AnyAsync(a => a.Email == dto.Email);
                if (emailExists)
                {
                    return BadRequest(new ApiResponse<ArtistDto>
                    {
                        Success = false,
                        Message = "An artist with this email already exists."
                    });
                }

                var artist = new Artist
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone ?? "",
                    Bio = dto.Bio ?? "",
                    ProfileImageUrl = dto.ProfileImageUrl ?? "",
                    Status = ArtistStatus.Active,
                    JoinedDate = DateTime.Now
                };

                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();

                _logger.LogInformation("API: Artist created: {ArtistId}", artist.Id);

                return CreatedAtAction(nameof(GetArtist), new { id = artist.Id }, new ApiResponse<ArtistDto>
                {
                    Success = true,
                    Message = "Artist created successfully.",
                    Data = new ArtistDto
                    {
                        Id = artist.Id,
                        Name = artist.Name,
                        Email = artist.Email,
                        Phone = artist.Phone,
                        Bio = artist.Bio,
                        ProfileImageUrl = artist.ProfileImageUrl,
                        JoinedDate = artist.JoinedDate,
                        ArtworkCount = 0,
                        TotalLikes = 0
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating artist");
                return StatusCode(500, new ApiResponse<ArtistDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the artist."
                });
            }
        }

        [HttpPut("artists/{id}")]
        public async Task<ActionResult<ApiResponse<ArtistDto>>> UpdateArtist(int id, [FromBody] UpdateArtistDto dto)
        {
            try
            {
                var artist = await _context.Artists
                    .Include(a => a.Artworks)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (artist == null)
                {
                    return NotFound(new ApiResponse<ArtistDto>
                    {
                        Success = false,
                        Message = "Artist not found."
                    });
                }

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var duplicateEmail = await _context.Artists
                        .AnyAsync(a => a.Email == dto.Email && a.Id != id);

                    if (duplicateEmail)
                    {
                        return BadRequest(new ApiResponse<ArtistDto>
                        {
                            Success = false,
                            Message = "An artist with this email already exists."
                        });
                    }
                    artist.Email = dto.Email;
                }

                if (!string.IsNullOrEmpty(dto.Name))
                    artist.Name = dto.Name;

                if (!string.IsNullOrEmpty(dto.Phone))
                    artist.Phone = dto.Phone;

                if (!string.IsNullOrEmpty(dto.Bio))
                    artist.Bio = dto.Bio;

                if (!string.IsNullOrEmpty(dto.ProfileImageUrl))
                    artist.ProfileImageUrl = dto.ProfileImageUrl;

                _context.Update(artist);
                await _context.SaveChangesAsync();

                _logger.LogInformation("API: Artist updated: {ArtistId}", id);

                return Ok(new ApiResponse<ArtistDto>
                {
                    Success = true,
                    Message = "Artist updated successfully.",
                    Data = new ArtistDto
                    {
                        Id = artist.Id,
                        Name = artist.Name,
                        Email = artist.Email,
                        Phone = artist.Phone,
                        Bio = artist.Bio,
                        ProfileImageUrl = artist.ProfileImageUrl,
                        JoinedDate = artist.JoinedDate,
                        ArtworkCount = artist.Artworks?.Count ?? 0,
                        TotalLikes = artist.Artworks?.Sum(a => a.Likes) ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating artist {ArtistId}", id);
                return StatusCode(500, new ApiResponse<ArtistDto>
                {
                    Success = false,
                    Message = "An error occurred while updating the artist."
                });
            }
        }

        [HttpDelete("artists/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteArtist(int id)
        {
            try
            {
                var artist = await _context.Artists.FindAsync(id);

                if (artist == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Artist not found."
                    });
                }

                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();

                _logger.LogInformation("API: Artist deleted: {ArtistId}", id);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Artist deleted successfully.",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting artist {ArtistId}", id);
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "An error occurred while deleting the artist."
                });
            }
        }

        #endregion

        #region Exhibition API Endpoints

        [HttpGet("exhibitions")]
        public async Task<ActionResult<ApiResponse<List<ExhibitionDto>>>> GetExhibitions(
            [FromQuery] string? searchTerm,
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Exhibitions
                    .Where(e => e.Status != ExhibitionStatus.Cancelled)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(e =>
                        e.Name.Contains(searchTerm) ||
                        e.Description.Contains(searchTerm) ||
                        e.Location.Contains(searchTerm));
                }

                if (status.HasValue)
                {
                    query = query.Where(e => (int)e.Status == status.Value);
                }

                var totalCount = await query.CountAsync();

                var exhibitions = await query
                    .OrderBy(e => e.StartDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => new ExhibitionDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Location = e.Location,
                        MaxArtworks = e.MaxArtworks,
                        Status = e.Status.ToString(),
                        BannerImageUrl = e.BannerImageUrl
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ExhibitionDto>>
                {
                    Success = true,
                    Data = exhibitions,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exhibitions");
                return StatusCode(500, new ApiResponse<List<ExhibitionDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving exhibitions."
                });
            }
        }

        [HttpGet("exhibitions/{id}")]
        public async Task<ActionResult<ApiResponse<ExhibitionDto>>> GetExhibition(int id)
        {
            try
            {
                var exhibition = await _context.Exhibitions.FindAsync(id);

                if (exhibition == null)
                {
                    return NotFound(new ApiResponse<ExhibitionDto>
                    {
                        Success = false,
                        Message = "Exhibition not found."
                    });
                }

                return Ok(new ApiResponse<ExhibitionDto>
                {
                    Success = true,
                    Data = new ExhibitionDto
                    {
                        Id = exhibition.Id,
                        Name = exhibition.Name,
                        Description = exhibition.Description,
                        StartDate = exhibition.StartDate,
                        EndDate = exhibition.EndDate,
                        Location = exhibition.Location,
                        MaxArtworks = exhibition.MaxArtworks,
                        Status = exhibition.Status.ToString(),
                        BannerImageUrl = exhibition.BannerImageUrl
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exhibition {ExhibitionId}", id);
                return StatusCode(500, new ApiResponse<ExhibitionDto>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the exhibition."
                });
            }
        }

        #endregion
    }

    #region API DTOs (Data Transfer Objects)

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<string>? Errors { get; set; }
    }

    public class ArtworkDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Likes { get; set; }
        public decimal? Price { get; set; }
        public bool IsForSale { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
    }

    public class CreateArtworkDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public int ArtistId { get; set; }

        public decimal? Price { get; set; }
        public bool IsForSale { get; set; }
    }

    public class UpdateArtworkDto
    {
        [StringLength(150, MinimumLength = 3)]
        public string? Title { get; set; }

        [StringLength(1000, MinimumLength = 10)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public string? Type { get; set; }
        public decimal? Price { get; set; }
        public bool? IsForSale { get; set; }
    }

    public class ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime JoinedDate { get; set; }
        public int ArtworkCount { get; set; }
        public int TotalLikes { get; set; }
    }

    public class CreateArtistDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public string? ProfileImageUrl { get; set; }
    }

    public class UpdateArtistDto
    {
        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public string? ProfileImageUrl { get; set; }
    }

    public class ExhibitionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int MaxArtworks { get; set; }
        public string Status { get; set; } = string.Empty;
        public string BannerImageUrl { get; set; } = string.Empty;
    }

    #endregion
}
