using AutoMapper;
using GGone.API.Models.BMI;

namespace GGone.API.Mapping
{
    public class BMIProfile : Profile
    {
        public BMIProfile() 
        {
            // Request -> Entity: Kullanıcıdan gelen veriyi tablo modeline çevir
            CreateMap<CreateBmiRequest, UserHealthRecord>();

            // Entity -> Response: Veritabanındaki veriyi kullanıcıya dönecek modele çevir
            CreateMap<UserHealthRecord, BmiResponse>()
                .ForMember(dest => dest.CalculationDate, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
