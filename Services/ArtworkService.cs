using projectPart1.Models;

namespace projectPart1.Services
{
    public class ArtworkService
    {
        private readonly DataStore datastore;

        public ArtworkService(DataStore dataStore)
        {
            datastore=dataStore;
        }

        public List<Artwork> GetAllArtworks()
        {
            List<Artwork> artworks=new List<Artwork>(datastore.Artworks);
            SortingHelper.SortByDate(artworks, a => a.CreatedDate);
            return artworks;
        }

        public List<Artwork> GetActiveArtworks()
        {
            List<Artwork> result=new List<Artwork>();
            for(int i=0;i<datastore.Artworks.Count;i++)
            {
                if(datastore.Artworks[i].Status==ArtworkStatus.Active)
                {
                    result.Add(datastore.Artworks[i]);
                }
            }
            SortingHelper.SortByDate(result, a => a.CreatedDate);
            return result;
        }

        public Artwork? GetArtworkById(int id)
        {
            if(id<=0)
            {
                throw new ArgumentException("Invalid artwork ID");
            }
            for(int i=0;i<datastore.Artworks.Count;i++)
            {
                if(datastore.Artworks[i].Id==id)
                {
                    return datastore.Artworks[i];
                }
            }
            return null;
        }

        public void AddArtwork(Artwork artwork)
        {
            if(artwork==null)
            {
                throw new ArgumentNullException(nameof(artwork));
            }
            if(string.IsNullOrWhiteSpace(artwork.Title))
            {
                throw new ArgumentException("Title is required");
            }
            if(string.IsNullOrWhiteSpace(artwork.ArtistName))
            {
                throw new ArgumentException("Artist name is required");
            }
            if(string.IsNullOrWhiteSpace(artwork.Description))
            {
                throw new ArgumentException("Description is required");
            }

            int maxId=0;
            for(int i=0;i<datastore.Artworks.Count;i++)
            {
                if(datastore.Artworks[i].Id>maxId)
                {
                    maxId=datastore.Artworks[i].Id;
                }
            }
            artwork.Id=maxId+1;
            artwork.CreatedDate=DateTime.Now;
            artwork.Likes=0;
            artwork.Status=ArtworkStatus.Active;

            if(string.IsNullOrWhiteSpace(artwork.ImageUrl))
            {
                artwork.ImageUrl="/images/artworks/default.svg";
            }

            datastore.Artworks.Add(artwork);
        }

        public void UpdateArtwork(Artwork artwork)
        {
            if(artwork==null)
            {
                throw new ArgumentNullException(nameof(artwork));
            }
            
            Artwork? existing=GetArtworkById(artwork.Id);
            if(existing==null)
            {
                throw new KeyNotFoundException($"Artwork with ID {artwork.Id} not found");
            }

            existing.Title=artwork.Title;
            existing.ArtistName=artwork.ArtistName;
            existing.Description=artwork.Description;
            existing.ImageUrl=artwork.ImageUrl;
            existing.Type=artwork.Type;
            existing.ArtistEmail=artwork.ArtistEmail;
            existing.ArtistPhone=artwork.ArtistPhone;
            existing.Price=artwork.Price;
            existing.IsForSale=artwork.IsForSale;
            existing.Status=artwork.Status;
        }

        public void DeleteArtwork(int id)
        {
            Artwork? artwork=GetArtworkById(id);
            if(artwork==null)
            {
                throw new KeyNotFoundException($"Artwork with ID {id} not found");
            }
            datastore.Artworks.Remove(artwork);
        }

        public void IncrementLikes(int id)
        {
            Artwork? artwork=GetArtworkById(id);
            if(artwork==null)
            {
                throw new KeyNotFoundException($"Artwork with ID {id} not found");
            }
            if(!artwork.CanBeLiked())
            {
                throw new InvalidOperationException("Cannot like this artwork");
            }
            artwork.IncrementLikes();
        }

        public List<Artwork> SearchArtworks(string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllArtworks();
            }

            List<Artwork> result=new List<Artwork>();
            string searchLower=searchTerm.ToLower();
            
            for(int i=0;i<datastore.Artworks.Count;i++)
            {
                Artwork a=datastore.Artworks[i];
                if(a.Title.ToLower().Contains(searchLower) || 
                   a.ArtistName.ToLower().Contains(searchLower) || 
                   a.Description.ToLower().Contains(searchLower))
                {
                    result.Add(a);
                }
            }
            SortingHelper.SortByDate(result, a => a.CreatedDate);
            return result;
        }

        public List<Artwork> FilterByType(ArtworkType? type)
        {
            if(type==null)
            {
                return GetAllArtworks();
            }

            List<Artwork> result=new List<Artwork>();
            for(int i=0;i<datastore.Artworks.Count;i++)
            {
                if(datastore.Artworks[i].Type==type)
                {
                    result.Add(datastore.Artworks[i]);
                }
            }
            SortingHelper.SortByDate(result, a => a.CreatedDate);
            return result;
        }

        public List<Artwork> FilterByStatus(ArtworkStatus status)
        {
            List<Artwork> result=new List<Artwork>();
            for(int i=0;i<datastore.Artworks.Count;i++)
            {
                if(datastore.Artworks[i].Status==status)
                {
                    result.Add(datastore.Artworks[i]);
                }
            }
            SortingHelper.SortByDate(result, a => a.CreatedDate);
            return result;
        }

        public List<Artwork> GetArtworksByArtist(int artistId)
        {
            List<Artwork> result=new List<Artwork>();
            for(int i=0;i<datastore.Artworks.Count;i++)
            {
                if(datastore.Artworks[i].ArtistId==artistId)
                {
                    result.Add(datastore.Artworks[i]);
                }
            }
            SortingHelper.SortByDate(result, a => a.CreatedDate);
            return result;
        }
    }
}



