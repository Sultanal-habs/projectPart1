using projectPart1.Models;

namespace projectPart1.Services
{
    public class DataStore
    {
        public List<Artwork> Artworks{get;set;}=new List<Artwork>();
        public List<Artist> Artists{get;set;}=new List<Artist>();
        public List<Exhibition> Exhibitions{get;set;}=new List<Exhibition>();

        public DataStore()
        {
            SeedData();
        }

        private void SeedData()
        {
            Artist artist1=new Artist();
            artist1.Id=1;
            artist1.Name="Mohammed Al Harthi";
            artist1.Email="mohammed@example.com";
            artist1.Phone="+968 9123 4567";
            artist1.Bio="Traditional Omani artist specializing in desert landscapes and cultural heritage";
            artist1.ProfileImageUrl= "/images/MohammedAlHarthi.jpg";
            artist1.Status=ArtistStatus.Active;
            artist1.JoinedDate=DateTime.Now.AddMonths(-6);
            Artists.Add(artist1);

            Artist artist2=new Artist();
            artist2.Id=2;
            artist2.Name="Fatima Al Saidi";
            artist2.Email="fatima@example.com";
            artist2.Phone="+968 9234 5678";
            artist2.Bio="Contemporary photographer capturing modern Omani life and architecture";
            artist2.ProfileImageUrl= "/images/FatimaAlSaidi.jpg";
            artist2.Status=ArtistStatus.Active;
            artist2.JoinedDate=DateTime.Now.AddMonths(-3);
            Artists.Add(artist2);

            Artist artist3=new Artist();
            artist3.Id=3;
            artist3.Name="Ahmed Al Balushi";
            artist3.Email="ahmed@example.com";
            artist3.Phone="+968 9345 6789";
            artist3.Bio="Master craftsman known for traditional Omani silverwork and khanjars";
            artist3.ProfileImageUrl= "/images/AhmedAlBalushi.jpg";
            artist3.Status=ArtistStatus.Active;
            artist3.JoinedDate=DateTime.Now.AddMonths(-12);
            Artists.Add(artist3);


            Artwork art1=new Artwork();
            art1.Id=1;
            art1.Title="the nothing";
            art1.ArtistName="Mohammed Al Harthi";
            art1.Description="nothing";
            art1.ImageUrl= "/images/painting1.jpg";
            art1.Type=ArtworkType.Painting;
            art1.Likes=45;
            art1.ArtistEmail="mohammed@example.com";
            art1.ArtistPhone="+968 9123 4567";
            art1.CreatedDate=DateTime.Now.AddDays(-30);
            art1.ArtistId=1;
            art1.Price=250.00m;
            art1.IsForSale=true;
            art1.Status=ArtworkStatus.Active;
            Artworks.Add(art1);

            Artwork art2=new Artwork();
            art2.Id=2;
            art2.Title= "Ghost Pier";
            art2.ArtistName="Fatima Al Saidi";
            art2.Description= "A hauntingly beautiful monochrome long-exposure photograph capturing the skeletal remains of a pier dissolving into the misty, ethereal sea";
            art2.ImageUrl= "/images/photograph1.jpg";
            art2.Type=ArtworkType.Photography;
            art2.Likes=67;
            art2.ArtistEmail="fatima@example.com";
            art2.ArtistPhone="+968 9234 5678";
            art2.CreatedDate=DateTime.Now.AddDays(-15);
            art2.ArtistId=2;
            art2.Price=180.00m;
            art2.IsForSale=true;
            art2.Status=ArtworkStatus.Active;
            Artworks.Add(art2);

            Artwork art3 =new Artwork();
            art3.Id=3;
            art3.Title= "Woven Warmth";
            art3.ArtistName="Ahmed Al Balushi";
            art3.Description = "A charming display of handcrafted rattan baskets, coasters, and homewares, bathed in soft light, evoking a cozy, natural, and artisanal aesthetic";
            art3.ImageUrl= "/images/handmadecrafts1.jpg";
            art3.Type=ArtworkType.HandmadeCraft;
            art3.Likes=89;
            art3.ArtistEmail ="ahmed@example.com";
            art3.ArtistPhone ="+968 9345 6789";
            art3.CreatedDate=DateTime.Now.AddDays(-7);
            art3.ArtistId =3;
            art3.Price=500.00m;
            art3.IsForSale=true;
            art3.Status=ArtworkStatus.Active;
            Artworks.Add(art3);

            Artwork art4=new Artwork();
            art4.Id=4;
            art4.Title = "Echoes of the Rails";
            art4.ArtistName="Fatima Al Saidi";
            art4.Description= "A powerful black-and-white portrait of an elderly woman, her face etched with time, holding a photograph of a train, suggesting a poignant connection to a past journey or memory";
            art4.ImageUrl= "/images/photograph2.jpg";
            art4.Type=ArtworkType.Photography;
            art4.Likes=52;
            art4.ArtistEmail="fatima@example.com";
            art4.ArtistPhone ="+968 9234 5678";
            art4.CreatedDate=DateTime.Now.AddDays(-20);
            art4.ArtistId=2;
            art4.Price=200.00m;
            art4.IsForSale =true;
            art4.Status=ArtworkStatus.Active;
            Artworks.Add(art4);

            Artwork art5=new Artwork();
            art5.Id =5;
            art5.Title= "Chromatic Storm";
            art5.ArtistName="Mohammed Al Harthi";
            art5.Description = "An energetic and vibrant abstract painting where thick, impasto layers of blue, purple, orange, and red clash and swirl, creating a powerful visual explosion of color and texture";
            art5.ImageUrl= "/images/painting2.jpg";
            art5.Type =ArtworkType.Painting;
            art5.Likes=73;
            art5.ArtistEmail="mohammed@example.com";
            art5.ArtistPhone="+968 9123 4567";
            art5.CreatedDate =DateTime.Now.AddDays(-5);
            art5.ArtistId=1;
            art5.Price =350.00m;
            art5.IsForSale=true;
            art5.Status=ArtworkStatus.Active;
            Artworks.Add(art5);

            Artwork art6=new Artwork();
            art6.Id=6;
            art6.Title = "Whisker Whisper";
            art6.ArtistName="Ahmed Al Balushi";
            art6.Description= "A stunningly detailed embroidery of a tabby cat's face, held gently in an embroidery hoop, showcasing intricate stitching that brings its expressive green eyes to life";
            art6.ImageUrl= "/images/handmadecrafts2.jpg";
            art6.Type=ArtworkType.HandmadeCraft;
            art6.Likes =41;
            art6.ArtistEmail="ahmed@example.com";
            art6.ArtistPhone="+968 9345 6789";
            art6.CreatedDate =DateTime.Now.AddDays(-12);
            art6.ArtistId=3;
            art6.Price=120.00m;
            art6.IsForSale =true;
            art6.Status=ArtworkStatus.Active;
            Artworks.Add(art6);


            Exhibition ex1=new Exhibition();
            ex1.Id=1;
            ex1.Name ="Contemporary Omani Art";
            ex1.Description="A showcase of modern and traditional Omani art featuring local artists and their interpretations of our rich culture";
            ex1.StartDate =DateTime.Today.AddDays(-10);
            ex1.EndDate=DateTime.Today.AddDays(20);
            ex1.Location ="Muscat Art Gallery";
            ex1.MaxArtworks=50;
            ex1.Status =ExhibitionStatus.Active;
            ex1.BannerImageUrl= "/images/ContemporaryOmaniArt.jpg";
            ex1.CreatedDate=DateTime.Now.AddDays(-20);
            Exhibitions.Add(ex1);

            Exhibition ex2 =new Exhibition();
            ex2.Id=2;
            ex2.Name="Photography Week";
            ex2.Description ="Annual photography exhibition celebrating Omani culture, landscapes, and the beauty of everyday life";
            ex2.StartDate=DateTime.Today.AddDays(15);
            ex2.EndDate =DateTime.Today.AddDays(22);
            ex2.Location="Royal Opera House";
            ex2.MaxArtworks =75;
            ex2.Status=ExhibitionStatus.Upcoming;
            ex2.BannerImageUrl= "/images/PhotographyWeek.jpg";
            ex2.CreatedDate=DateTime.Now.AddDays(-5);
            Exhibitions.Add(ex2);

            Exhibition ex3=new Exhibition();
            ex3.Id =3;
            ex3.Name="Heritage Crafts Fair";
            ex3.Description="Exhibition showcasing traditional Omani craftsmanship including silverwork, pottery, and textiles";
            ex3.StartDate=DateTime.Today.AddDays(30);
            ex3.EndDate=DateTime.Today.AddDays(45);
            ex3.Location ="Nizwa Fort";
            ex3.MaxArtworks=60;
            ex3.Status=ExhibitionStatus.Upcoming;
            ex3.BannerImageUrl= "/images/HeritageCraftsFair.jpg";
            ex3.CreatedDate =DateTime.Now.AddDays(-3);
            Exhibitions.Add(ex3);
        }
    }
}
