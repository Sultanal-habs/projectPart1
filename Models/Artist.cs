using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace projectPart1.Models
{
    [Table("Artists")]
    public class Artist
    {
        [Column("ArtistId")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Artist name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Artist name must be between 2 and 100 characters")]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^\+968\s?\d{4}\s?\d{4}$", ErrorMessage = "Phone must be in format: +968 9123 4567")]
        public string Phone { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string Bio { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public ArtistStatus Status { get; set; } = ArtistStatus.Active;
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public virtual ICollection<Artwork>? Artworks { get; set; }
        public int GetTotalArtworks()
        {
            return Artworks?.Count ?? 0;
        }
        public int GetTotalLikes()
        {
            return Artworks?.Sum(a => a.Likes) ?? 0;
        }
        public bool IsActive()
        {
            return Status == ArtistStatus.Active;
        }
        public void Deactivate()
        {
            Status = ArtistStatus.Inactive;
        }
        public void Activate()
        {
            Status = ArtistStatus.Active;
        }
    }
    public enum ArtistStatus
    {
        Active = 0,
        Inactive = 1,
        Suspended = 2,
        PendingApproval = 3
    }
}