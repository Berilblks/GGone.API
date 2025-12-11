using GGone.API.Models.Auth;
using GGone.API.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGone.API.Models.Addiction
{
    public class Addiction
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public DateTime QuitDate { get; set; }  // Kullanıcının temiz kalmaya başladığı asıl tarih.
        public double DailyConsumption { get; set; }  // Günlük Tüketim Miktarı (Örn: Sigara için paket, Alkol için ünite)
        public double UnitPrice { get; set; } // Birim Fiyat (Örn: Sigara paketi fiyatı, Alkol birim fiyatı)
        public DateTime LastConsumptionDate { get; set; } = DateTime.UtcNow; // Frontend'deki "I RELAPSED / RESET PROGRESS" butonuna basıldığında bu kayıt güncellenmeli.
        public AddictionType AddictionType { get; set; }

    }
}
