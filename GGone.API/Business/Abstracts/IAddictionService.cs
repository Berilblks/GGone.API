using GGone.API.Models;
using GGone.API.Models.Addiction;

namespace GGone.API.Business.Abstracts
{
    public interface IAddictionService
    {
        //Bağımlılık ekleme
        Task<BaseResponse<AddictionResponse>> AddAddiction(AddictionRequest request);
        //Bağımlılık listeleme
        Task<BaseResponse<List<AddictionResponse>>> GetAllAddictions(AddictionRequest request);
        //Kullanıcı Id'sine göre bağımlılık bilgisi getirme
        Task<BaseResponse<AddictionResponse>> GetAddictionByUserId(int userId);
        //Bağımlılık bilgisi güncelleme
        Task<BaseResponse<AddictionResponse>> UpdateAddiction(int userId, AddictionRequest request);
    }
}
