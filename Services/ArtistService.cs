using projectPart1.Models;

namespace projectPart1.Services
{
    public class ArtistService
    {
        private readonly DataStore datastore;

        public ArtistService(DataStore dataStore)
        {
            datastore=dataStore;
        }

        public List<Artist> GetAllArtists()
        {
            List<Artist> artists=new List<Artist>(datastore.Artists);
            SortingHelper.SortByDate(artists, a => a.JoinedDate);
            return artists;
        }

        public List<Artist> GetActiveArtists()
        {
            List<Artist> result=new List<Artist>();
            for(int i=0;i<datastore.Artists.Count;i++)
            {
                Artist a=datastore.Artists[i];
                if(a.Status==ArtistStatus.Active)
                {
                    result.Add(a);
                }
            }
            SortingHelper.SortByDate(result, a => a.JoinedDate);
            return result;
        }

        public Artist? GetArtistById(int id)
        {
            if(id<=0)
            {
                throw new ArgumentException("Invalid artist ID");
            }
            for(int i=0;i<datastore.Artists.Count;i++)
            {
                if(datastore.Artists[i].Id==id)
                {
                    return datastore.Artists[i];
                }
            }
            return null;
        }

        public void AddArtist(Artist artist)
        {
            if(artist==null)
            {
                throw new ArgumentNullException(nameof(artist));
            }
            if(string.IsNullOrWhiteSpace(artist.Name))
            {
                throw new ArgumentException("Artist name is required");
            }
            if(string.IsNullOrWhiteSpace(artist.Email))
            {
                throw new ArgumentException("Email is required");
            }

            string newEmail=artist.Email.ToLower();
            for(int i=0;i<datastore.Artists.Count;i++)
            {
                if(datastore.Artists[i].Email.ToLower()==newEmail)
                {
                    throw new InvalidOperationException("An artist with this email already exists");
                }
            }

            int maxId=0;
            for(int i=0;i<datastore.Artists.Count;i++)
            {
                if(datastore.Artists[i].Id>maxId)
                {
                    maxId=datastore.Artists[i].Id;
                }
            }
            artist.Id=maxId+1;
            artist.JoinedDate=DateTime.Now;
            artist.Status=ArtistStatus.Active;

            if(string.IsNullOrWhiteSpace(artist.ProfileImageUrl))
            {
                artist.ProfileImageUrl="/images/artists/default.svg";
            }

            datastore.Artists.Add(artist);
        }

        public void UpdateArtist(Artist artist)
        {
            if(artist==null)
            {
                throw new ArgumentNullException(nameof(artist));
            }
            
            Artist? existing=GetArtistById(artist.Id);
            if(existing==null)
            {
                throw new KeyNotFoundException($"Artist with ID {artist.Id} not found");
            }

            string newEmail=artist.Email.ToLower();
            for(int i=0;i<datastore.Artists.Count;i++)
            {
                if(datastore.Artists[i].Id!=artist.Id && datastore.Artists[i].Email.ToLower()==newEmail)
                {
                    throw new InvalidOperationException("Another artist with this email already exists");
                }
            }

            existing.Name=artist.Name;
            existing.Email=artist.Email;
            existing.Phone=artist.Phone;
            existing.Bio=artist.Bio;
            existing.ProfileImageUrl=artist.ProfileImageUrl;
            existing.Status=artist.Status;
        }

        public void DeleteArtist(int id)
        {
            Artist? artist=GetArtistById(id);
            if(artist==null)
            {
                throw new KeyNotFoundException($"Artist with ID {id} not found");
            }
            datastore.Artists.Remove(artist);
        }

        public List<Artist> SearchArtists(string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllArtists();
            }

            List<Artist> result=new List<Artist>();
            string searchLower=searchTerm.ToLower();
            
            for(int i=0;i<datastore.Artists.Count;i++)
            {
                Artist a=datastore.Artists[i];
                if(a.Name.ToLower().Contains(searchLower) || 
                   a.Email.ToLower().Contains(searchLower) || 
                   a.Bio.ToLower().Contains(searchLower))
                {
                    result.Add(a);
                }
            }
            SortingHelper.SortByDate(result, a => a.JoinedDate);
            return result;
        }
    }
}


