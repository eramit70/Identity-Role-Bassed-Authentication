using RoleBassedAuthentication.DTO;

namespace RoleBassedAuthentication.interfaces
{
    public interface IAuthencticationUser
    {
        Task<Status> LoginAsync(LoginUser model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(RegisterUser model);
        Task<Status> ChangePasswordAsync(ChangePassword model, string username);
    }
}
