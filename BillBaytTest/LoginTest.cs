using Bilbayt.Models;
using Bilbayt.Services;
using Moq;
using System;
using Xunit;
using MongoDB.Bson;
using Bilbayt.Controllers;
using Microsoft.Extensions.Configuration;
using Bilbayt.RequestModels;
using System.Threading.Tasks;
using Shouldly;
using Microsoft.AspNetCore.Mvc;

namespace BillBaytTest
{
    public class LoginTest
    {
        public Mock<ILoginService> mock = new Mock<ILoginService>();
        public Mock<IConfiguration> config = new Mock<IConfiguration>();
        [Fact]
        public async Task Login_Should_Return_Token_For_Correct_Credentials()
        {
            
            Login login = new Login();
            login.FirstName = "test";
            login.Id = new BsonObjectId(new ObjectId("60ed4a50a17f6d195d1878d6")).ToString();
            login.UserName = "testUserName";
            login.LastName = "testLastNme";
            login.PasswordHash = "testingPasswordHash";
            mock.Setup(x => x.GetByUserName(It.IsAny<string>())).ReturnsAsync(login);

            string token = "tokenTest";
            mock.Setup(x => x.GenerateJSONWebToken()).Returns(token);

            LoginModel loginModel = new LoginModel();
            loginModel.Password = "password";
            loginModel.UserName = "testUserName";

            LoginController controller = new LoginController(mock.Object, config.Object);

            var result = new LoginResultModel();
            var res = (LoginResultModel)((OkObjectResult)await controller.Login(loginModel)).Value;

            res.ShouldNotBeNull();
            res.FirstName.ShouldBe(login.FirstName);
            res.LastName.ShouldBe(login.LastName);
            res.Token.ShouldBe(token);
            
        }
    }
}
