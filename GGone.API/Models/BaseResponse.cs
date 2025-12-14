using GGone.API.Models.Exercises; 

namespace GGone.API.Models
{
    public class BaseResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }

        // Hata kodları veya detayları
        public string? Error { get; set; }
        public int? ErrorCode { get; set; }

        public BaseResponse()
        {
            Success = true;
        }

        public BaseResponse(T data)
        {
            Success = true;
            Data = data;
            Message = "İşlem başarıyla tamamlandı.";
        }

       
        public BaseResponse(bool success, string message)
        {
            Success = success;
            Message = message;

            if (!success)
            {
                Error = message; 
            }
        }
    }
}