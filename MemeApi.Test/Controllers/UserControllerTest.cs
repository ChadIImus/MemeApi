using System.Threading.Tasks;
using FluentAssertions;
using MemeApi.Controllers;
using MemeApi.Models.DTO;
using MemeApi.Models.Entity;
using MemeApi.Test.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace MemeApi.Test.Controllers
{
    public class UserControllerTest
    {

        [Fact]
        public async Task GIVEN_DummyUser_WHEN_CreatingUser_THEN_UserIsCreatedWithProperValues()
        {
            using (var context = ContextUtils.CreateMemeTestContext())
            {
                var controller = new UsersController(context, new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

                // given

                var userDTO = new UserCreationDTO
                {
                    Username = "Test",
                    Email = "Test",
                    Password = "Test"
                };

                // When

                var createResult = (await controller.CreateUser(userDTO)).Result;


                // Then
                createResult.Should().NotBeNull();
                createResult.Should().BeOfType<CreatedAtActionResult>();
                var createdUser = ((CreatedAtActionResult)createResult).Value as User;  

                (await context.Users.CountAsync()).Should().Be(1);

                createdUser.Username.Should().Be(userDTO.Username);
                createdUser.Email.Should().Be(userDTO.Email);
                createdUser.PasswordHash.Should().NotBe(userDTO.Password);
            }
        }

        [Fact]
        public async Task GIVEN_CreatedDummyUser_WHEN_GettingUser_THEN_UserHasProperValues()
        {
            using (var context = ContextUtils.CreateMemeTestContext())
            {
                var controller = new UsersController(context, new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

                // given

                var userDTO = new UserCreationDTO
                {
                    Username = "Test",
                    Email = "Test",
                    Password = "Test"
                };

                // When

                var createResult = (await controller.CreateUser(userDTO)).Result;
                var createdUser = ((CreatedAtActionResult)createResult).Value as User;  

                // Then
                var result = (await controller.GetUser(createdUser.Id)).Result;

                result.Should().NotBeNull();
                result.Should().BeOfType<OkObjectResult>();
                var foundUser = ((OkObjectResult)result).Value as User;   

                foundUser.Username.Should().Be(userDTO.Username);
                foundUser.Email.Should().Be(userDTO.Email);
                foundUser.PasswordHash.Should().NotBe(userDTO.Password);
            }
        }

        [Fact]
        public async Task GIVEN_CreatedDummyUser_WHEN_Updating_THEN_UserIsUpdatedWithGivenValues()
        {
            using (var context = ContextUtils.CreateMemeTestContext())
            {
                var controller = new UsersController(context, new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

                // given

                var userDTO = new UserCreationDTO
                {
                    Username = "Test",
                    Email = "Test",
                    Password = "Test"
                };

                var updateDto = new UserUpdateDTO
                {
                    NewUsername = "Test2",
                    NewEmail = "Test2",
                    NewPassword = "Test2",
                };


                var createResult = (await controller.CreateUser(userDTO)).Result;
                var createdUser = ((CreatedAtActionResult)createResult).Value as User;

                // When
                await controller.UpdateUser(createdUser.Id, updateDto);

                // Then

                var result = (await controller.GetUser(createdUser.Id)).Result;
                result.Should().BeOfType<OkObjectResult>();
                var foundUser = ((OkObjectResult)result).Value as User;

                foundUser.Username.Should().Be(updateDto.NewUsername);
                foundUser.Email.Should().Be(updateDto.NewEmail);
                foundUser.PasswordHash.Should().NotBe(updateDto.NewPassword);
            }
        }

        [Fact]
        public async Task GIVEN_CreatedDummyUser_WHEN_Deleting_THEN_UserIsDeleted()
        {
            using (var context = ContextUtils.CreateMemeTestContext())
            {
                var controller = new UsersController(context, new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

                // given

                var userDTO = new UserCreationDTO
                {
                    Username = "Test",
                    Email = "Test",
                    Password = "Test"
                };


                var createResult = (await controller.CreateUser(userDTO)).Result;
                var createdUser = ((CreatedAtActionResult)createResult).Value as User;

                // When
                var result = await controller.DeleteUser(createdUser.Id);

                // Then

                result.Should().NotBeNull();
                result.Should().BeOfType<NoContentResult>();
                (await context.Users.CountAsync()).Should().Be(0);
            }
        }

        [Fact]
        public async Task GIVEN_CreatedDummyUser_WHEN_LoggingIn_THEN_UserIsLoggedIn()
        {
            using (var context = ContextUtils.CreateMemeTestContext())
            {
                var controller = new UsersController(context, new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

                // given

                var userDTO = new UserCreationDTO
                {
                    Username = "Test",
                    Email = "Test",
                    Password = "Test"
                };

                var loginDTO = new UserLoginDTO
                {
                    Username = "Test",
                    password = "Test"
                };
                await controller.CreateUser(userDTO);

                // When
                var result = controller.Login(loginDTO);

                // Then
                result.Should().NotBeNull();
                result.Should().BeOfType<OkObjectResult>();
            }
        }
    }
}