using GGone.API.Business.Abstracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GGone.API.Business.Services.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier);

                return claim != null ? int.Parse(claim.Value) : 0;
            }
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}