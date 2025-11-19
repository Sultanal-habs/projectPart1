using System.ComponentModel.DataAnnotations;

namespace projectPart1.Models
{
    public class Artist
    {
        public int Id{get;set;}

        [Required(ErrorMessage="Artist name is required")]
        [StringLength(100,MinimumLength=2,ErrorMessage ="Artist name must be between 2 and 100 characters")]
        public string Name { get; set; }=string.Empty;

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage="Invalid email address")]
        public string Email{get;set;}=string.Empty;

        [Phone(ErrorMessage ="Invalid phone number")]
        [RegularExpression(@"^\+968\s?\d{4}\s?\d{4}$", ErrorMessage = "Phone must be in format: +968 9123 4567")]
        public string Phone { get; set; }= string.Empty;

        [StringLength(500,ErrorMessage="Bio cannot exceed 500 characters")]
        public string Bio{get;set;}=string.Empty;

        public string ProfileImageUrl{get;set;}=string.Empty;
        public ArtistStatus Status{get;set;}=ArtistStatus.Active;
        public DateTime JoinedDate { get; set; } =DateTime.Now;

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<Artwork> Artworks{get;set;}=new List<Artwork>();


        public int GetTotalArtworks()
        {
            return Artworks.Count;
        }

        public int GetTotalLikes()
        {
            int total=0;
            for(int i=0;i<Artworks.Count;i++)
            {
                total+=Artworks[i].Likes;
            }
            return total;
        }

        public bool IsActive()
        {
            return Status==ArtistStatus.Active;
        }

        public void Deactivate()
        {
            Status=ArtistStatus.Inactive;
        }

        public void Activate()
        {
            Status=ArtistStatus.Active;
        }
    }

    public enum ArtistStatus
    {
        Active,
        Inactive,
        Suspended,
        PendingApproval
    }
}
