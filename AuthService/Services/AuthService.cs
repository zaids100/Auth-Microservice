using AuthService.Models;
using AuthService.Repositories;
using System;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;

        public AuthService(IUserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        // Normal email/password registration
        public async Task<AuthResponse> RegisterAsync(string email, string password,string role)
        {
            if (await _userRepository.EmailExistsAsync(email))
                throw new Exception("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid().ToString("N"), // Or custom ID
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Provider = "local",
                Role = role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            var token = _tokenService.GenerateAccessToken(user);
            var refresh = _tokenService.GenerateRefreshToken();

            return new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refresh,
                Email = user.Email,
                Role = user.Role
            };
        }

        // Normal email/password login
        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("Invalid credentials");

            if (user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var token = _tokenService.GenerateAccessToken(user);
            var refresh = _tokenService.GenerateRefreshToken();

            return new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refresh,
                Email = user.Email,
                Role = user.Role
            };
        }

        // Google login / auto signup
        public async Task<AuthResponse> GoogleLoginAsync(string email, string googleId, string name)
        {
            // Check by GoogleId first
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                // Create new user if not exists
                user = new User
                {
                    Id = Guid.NewGuid().ToString("N"), // Or custom ID
                    Email = email,
                    GoogleId = googleId,
                    Provider = "google",
                    Role = "user",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
            }
            else if (user.Provider != "google")
            {
                // Optional: if email exists as local, block or link accounts
                throw new Exception("Email already registered with local login");
            }

            var token = _tokenService.GenerateAccessToken(user);
            var refresh = _tokenService.GenerateRefreshToken();

            return new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refresh,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
