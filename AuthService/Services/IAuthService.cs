using System.Threading.Tasks;
using AuthService.Models;

namespace AuthService.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(string email, string password,string role);
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<AuthResponse> GoogleLoginAsync(string email, string googleId, string name);
    }
}
