using Bilbayt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bilbayt.Services
{
    public interface ILoginService
    {
        Task<Login> Get(string id);

        Task<Login> GetByUserName(string userName);

        Task<Login> Create(Login login);

        string GenerateJSONWebToken();
    }
}
