
namespace GGone.API.Models
{
    public class BaseResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
        public int? ErrorCode { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(T data, string message = "İşlem başarıyla tamamlandı.")
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public static BaseResponse<T> Ok(T data, string message = "Başarılı")
        {
            return new BaseResponse<T>(data, message);
        }

        public static BaseResponse<T> Fail(string error, int? errorCode = null)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Error = error,
                Message = error,
                ErrorCode = errorCode
            };
        }
    }
}