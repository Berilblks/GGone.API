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
        public DateTime QuitDate { get; set; }
        public AddictionType AddictionType { get; set; }

        //kaydedilen başarılar
        public int MilestonesDays { get; set; }

    }
}
