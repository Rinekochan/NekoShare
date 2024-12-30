using NekoShare.Entities;

namespace NekoShare.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}