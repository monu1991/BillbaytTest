using Bilbayt.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilbayt.Services
{
    public class LoginService : ILoginService
    {
        private readonly IMongoCollection<Login> _login;
        private readonly IConfiguration _configuration;

        public LoginService(IMongoDbSettings settings, IConfiguration configuration)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _configuration = configuration;

            _login = database.GetCollection<Login>(settings.CollectionName);
        }

        public async Task<Login> Get(string id) => await _login.Find<Login>(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Login> GetByUserName(string userName) => await _login.Find<Login>(x=>x.UserName.Equals(userName.ToLower())).FirstOrDefaultAsync();

        public async Task<Login> Create(Login login)
        {
            await _login.InsertOneAsync(login);
            return login;
        }

        public string GenerateJSONWebToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["JWT:ValidIssuer"],
              _configuration["JWT:ValidIssuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
