namespace Models.DTOs.General
{
    public class GResponse<T>
    {
        public T? Data { get; set; }
        public GResponseStatus Status { get; set; }
        public string? StatusMessage { get; set; }
        public static GResponse<T> CreateSuccess(T data)
        {
            return new GResponse<T>()
            {
                Data = data,
                Status = GResponseStatus.Success
            };
        }
    }
}
