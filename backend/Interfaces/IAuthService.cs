using backend.DTOs;

namespace backend.Interfaces
{
    public interface IAuthService
    {
        public Task Register(RegisterRequest request);
        public Task<string> Login(LoginRequest request);
    }
}
