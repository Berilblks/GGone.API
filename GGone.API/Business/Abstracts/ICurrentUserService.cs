namespace GGone.API.Business.Abstracts
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        bool IsAuthenticated { get; }
    }
}
