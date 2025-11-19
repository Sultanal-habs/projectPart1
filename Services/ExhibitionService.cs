using projectPart1.Models;

namespace projectPart1.Services
{
    public class ExhibitionService
    {
        private readonly DataStore datastore;

        public ExhibitionService(DataStore dataStore)
        {
            datastore=dataStore;
        }

        public List<Exhibition> GetAllExhibitions()
        {
            for(int i=0;i<datastore.Exhibitions.Count;i++)
            {
                datastore.Exhibitions[i].UpdateStatus();
            }
            
            List<Exhibition> exhibitions=new List<Exhibition>(datastore.Exhibitions);
            SortingHelper.SortByDate(exhibitions, e => e.StartDate);
            return exhibitions;
        }

        public List<Exhibition> GetActiveExhibitions()
        {
            List<Exhibition> result=new List<Exhibition>();
            for(int i=0;i<datastore.Exhibitions.Count;i++)
            {
                if(datastore.Exhibitions[i].IsActive())
                {
                    result.Add(datastore.Exhibitions[i]);
                }
            }
            SortingHelper.SortByDate(result, e => e.EndDate, false);
            return result;
        }

        public List<Exhibition> GetUpcomingExhibitions()
        {
            List<Exhibition> result=new List<Exhibition>();
            for(int i=0;i<datastore.Exhibitions.Count;i++)
            {
                if(datastore.Exhibitions[i].IsUpcoming())
                {
                    result.Add(datastore.Exhibitions[i]);
                }
            }
            SortingHelper.SortByDate(result, e => e.StartDate, false);
            return result;
        }

        public Exhibition? GetExhibitionById(int id)
        {
            if(id<=0)
            {
                throw new ArgumentException("Invalid exhibition ID");
            }

            for(int i=0;i<datastore.Exhibitions.Count;i++)
            {
                if(datastore.Exhibitions[i].Id==id)
                {
                    Exhibition exhibition=datastore.Exhibitions[i];
                    exhibition.UpdateStatus();
                    return exhibition;
                }
            }
            return null;
        }

        public void AddExhibition(Exhibition exhibition)
        {
            if(exhibition==null)
            {
                throw new ArgumentNullException(nameof(exhibition));
            }
            if(string.IsNullOrWhiteSpace(exhibition.Name))
            {
                throw new ArgumentException("Exhibition name is required");
            }
            if(exhibition.StartDate<DateTime.Today)
            {
                throw new ArgumentException("Start date must be in the future");
            }
            if(exhibition.EndDate<=exhibition.StartDate)
            {
                throw new ArgumentException("End date must be after start date");
            }
            if(exhibition.MaxArtworks<=0)
            {
                throw new ArgumentException("Max artworks must be greater than zero");
            }

            int maxId=0;
            for(int i=0;i<datastore.Exhibitions.Count;i++)
            {
                if(datastore.Exhibitions[i].Id>maxId)
                {
                    maxId=datastore.Exhibitions[i].Id;
                }
            }
            exhibition.Id=maxId+1;
            exhibition.CreatedDate=DateTime.Now;
            exhibition.UpdateStatus();

            if(string.IsNullOrWhiteSpace(exhibition.BannerImageUrl))
            {
                exhibition.BannerImageUrl="/images/exhibitions/default.svg";
            }

            datastore.Exhibitions.Add(exhibition);
        }

        public void UpdateExhibition(Exhibition exhibition)
        {
            if(exhibition==null)
            {
                throw new ArgumentNullException(nameof(exhibition));
            }

            Exhibition? existing=GetExhibitionById(exhibition.Id);
            if(existing==null)
            {
                throw new KeyNotFoundException($"Exhibition {exhibition.Id} not found");
            }
            if(exhibition.EndDate<=exhibition.StartDate)
            {
                throw new ArgumentException("End date must be after start date");
            }

            existing.Name=exhibition.Name;
            existing.Description=exhibition.Description;
            existing.StartDate=exhibition.StartDate;
            existing.EndDate=exhibition.EndDate;
            existing.Location=exhibition.Location;
            existing.MaxArtworks=exhibition.MaxArtworks;
            existing.BannerImageUrl=exhibition.BannerImageUrl;
            existing.UpdateStatus();
        }

        public void DeleteExhibition(int id)
        {
            Exhibition? exhibition=GetExhibitionById(id);
            if(exhibition==null)
            {
                throw new KeyNotFoundException($"Exhibition {id} not found");
            }
            datastore.Exhibitions.Remove(exhibition);
        }

        public List<Exhibition> SearchExhibitions(string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllExhibitions();
            }

            List<Exhibition> result=new List<Exhibition>();
            string searchLower=searchTerm.ToLower();
            
            for(int i=0;i<datastore.Exhibitions.Count;i++)
            {
                Exhibition e=datastore.Exhibitions[i];
                if(e.Name.ToLower().Contains(searchLower) || 
                   e.Description.ToLower().Contains(searchLower) || 
                   e.Location.ToLower().Contains(searchLower))
                {
                    result.Add(e);
                }
            }
            SortingHelper.SortByDate(result, e => e.StartDate);
            return result;
        }

        public List<Exhibition> FilterByStatus(ExhibitionStatus status)
        {
            List<Exhibition> result=new List<Exhibition>();
            for(int i=0;i<datastore.Exhibitions.Count;i++)
            {
                if(datastore.Exhibitions[i].Status==status)
                {
                    result.Add(datastore.Exhibitions[i]);
                }
            }
            SortingHelper.SortByDate(result, e => e.StartDate);
            return result;
        }
    }
}
