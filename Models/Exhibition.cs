using System.ComponentModel.DataAnnotations;

namespace projectPart1.Models
{
    public class Exhibition
    {
        public int Id{get;set;}

        [Required(ErrorMessage ="Exhibition name is required")]
        [StringLength(200, MinimumLength =3,ErrorMessage="Exhibition name must be between 3 and 200 characters")]
        public string Name { get; set; }=string.Empty;

        [Required(ErrorMessage ="Description is required")]
        [StringLength(1000,MinimumLength=10, ErrorMessage="Description must be between 10 and 1000 characters")]
        public string Description{get;set;}=string.Empty;

        [Required(ErrorMessage="Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage ="End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate{get;set;}

        [StringLength(200, ErrorMessage ="Location cannot exceed 200 characters")]
        public string Location { get; set; }=string.Empty;

        [Range(0,1000, ErrorMessage="Max artworks must be between 0 and 1000")]
        public int MaxArtworks{get;set;}=50;

        public ExhibitionStatus Status { get; set; }=ExhibitionStatus.Upcoming;
        public string BannerImageUrl{get;set;}=string.Empty;
        public DateTime CreatedDate { get; set; }=DateTime.Now;

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<Artwork> FeaturedArtworks{get;set;}=new List<Artwork>();


        public bool IsActive()
        {
            DateTime today=DateTime.Today;
            return today>=StartDate&&today<=EndDate&&Status==ExhibitionStatus.Active;
        }

        public bool IsUpcoming()
        {
            DateTime today=DateTime.Today;
            return today<StartDate&&Status==ExhibitionStatus.Upcoming;
        }

        public bool IsEnded()
        {
            DateTime today=DateTime.Today;
            return today>EndDate||Status==ExhibitionStatus.Ended;
        }

        public int GetDaysRemaining()
        {
            if(IsEnded())
            {
                return 0;
            }
            TimeSpan diff=EndDate-DateTime.Today;
            return diff.Days;
        }

        public int GetTotalDays()
        {
            TimeSpan diff=EndDate-StartDate;
            return diff.Days;
        }

        public bool CanAddArtwork()
        {
            return FeaturedArtworks.Count<MaxArtworks&&IsActive();
        }

        public void UpdateStatus()
        {
            DateTime today=DateTime.Today;
            if(today<StartDate)
            {
                Status=ExhibitionStatus.Upcoming;
            }
            else if(today>=StartDate&&today<=EndDate)
            {
                Status=ExhibitionStatus.Active;
            }
            else
            {
                Status=ExhibitionStatus.Ended;
            }
        }
    }

    public enum ExhibitionStatus
    {
        Upcoming,
        Active,
        Ended,
        Cancelled
    }
}
