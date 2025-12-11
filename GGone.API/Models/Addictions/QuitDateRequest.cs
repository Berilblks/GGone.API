using GGone.API.Models.Enum;

namespace GGone.API.Models.Addictions
{
    public class QuitDateRequest
    {
        public int UserId { get; set; }
        public AddictionType Type { get; set; }
        public bool DidConsume { get; set; }
    }
}
