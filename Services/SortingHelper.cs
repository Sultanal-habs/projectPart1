using projectPart1.Models;

namespace projectPart1.Services
{
    public static class SortingHelper
    {
        public static void SortByDate<T>(List<T> items, Func<T, DateTime> dateSelector, bool descending = true)
        {
            for(int i=0;i<items.Count-1;i++)
            {
                for(int j=0;j<items.Count-i-1;j++)
                {
                    DateTime date1=dateSelector(items[j]);
                    DateTime date2=dateSelector(items[j+1]);
                    
                    bool shouldSwap=descending ? date1<date2 : date1>date2;
                    
                    if(shouldSwap)
                    {
                        T temp=items[j];
                        items[j]=items[j+1];
                        items[j+1]=temp;
                    }
                }
            }
        }
    }
}
