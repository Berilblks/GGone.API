using GGone.API.Models.Enum;

namespace GGone.API.Models.Addictions
{
    public class QuitDateRequest
    {
        public AddictionType Type { get; set; }
        public bool DidConsume { get; set; }
    }
}
