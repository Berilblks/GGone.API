using GGone.API.Models.Enum;

namespace GGone.API.Models.Addictions
{
    public class GetCounterRequest
    {
        // GetDependencyCounterAsync için kullanılacak.
        public int UserId { get; set; }
        public int AddictionType { get; set; }
        public AddictionType Type { get; internal set; }
    }
}
