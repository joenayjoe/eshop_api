using eshop_api.DTO;
using eshop_api.Models;
using System.Threading.Tasks;

namespace eshop_api.Auth
{
    public interface IAuthManager
    {
        Task<AppUser> ValidateUser(UserLoginDTO loginDTO);
        Task<string> CreateToken();
    }
}
