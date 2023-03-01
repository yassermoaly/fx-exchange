using System.ComponentModel;

namespace Models.DTOs.General
{
    public record FilterRequest<T>
    {
        public T? SearchData { get; set; }
        public int Start { get; set; }
        [DefaultValue(10)]
        public int PageLength { get; set; }
    }
}
