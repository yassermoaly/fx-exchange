namespace Models.DTOs.General
{
    public class FilterResponse<T>
    {
        public FilterResponse()
        {
            Data = new HashSet<T>();
        }
        public int TotalCount { get; set; }
        public ICollection<T> Data { get; set; }
    }
}
