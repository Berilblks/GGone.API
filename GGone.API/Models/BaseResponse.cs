namespace GGone.API.Models
{
    public class BaseResponse<T>
    {
        public string? Error { get; set; }
        public int? ErrorCode { get; set; }
        public T? Data { get; set; }
    }
}
