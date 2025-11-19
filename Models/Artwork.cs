using System.ComponentModel.DataAnnotations;

namespace projectPart1.Models
{
    public class Artwork
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Artist name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Artist name must be between 2 and 100 characters")]
        public string ArtistName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Artwork type is required")]
        public ArtworkType Type { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Likes cannot be negative")]
        public int Likes { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string ArtistEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [RegularExpression(@"^\+968\s?\d{4}\s?\d{4}$", ErrorMessage = "Phone must be in format: +968 9123 4567")]
        public string ArtistPhone { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? ArtistId { get; set; }

        public int? ExhibitionId { get; set; }

        [Range(0, 999999, ErrorMessage = "Price must be between 0 and 999999")]
        public decimal Price { get; set; }

        public bool IsForSale { get; set; }

        public ArtworkStatus Status { get; set; } = ArtworkStatus.Active;

        public bool CanBeLiked()
        {
            return Status == ArtworkStatus.Active;
        }

        public void IncrementLikes()
        {
            if (CanBeLiked())
            {
                Likes++;
            }
        }

        public bool IsNew()
        {
            TimeSpan diff = DateTime.Now - CreatedDate;
            return diff.Days <= 7;
        }

        public string GetDisplayPrice()
        {
            return IsForSale ? $"{Price:C} OMR" : "Not for sale";
        }

        public void MarkAsSold()
        {
            Status = ArtworkStatus.Sold;
            IsForSale = false;
        }
    }

    public enum ArtworkType
    {
        Painting,
        Photography,
        HandmadeCraft,
        Sculpture,
        DigitalArt
    }

    public enum ArtworkStatus
    {
        Active,
        Pending,
        Sold,
        Archived
    }
}
