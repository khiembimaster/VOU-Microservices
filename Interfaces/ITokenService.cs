using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userapi.Mappers;

namespace userapi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}