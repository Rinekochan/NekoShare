﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using server.Entities;
using server.Services;

namespace server.Tests;

[TestFixture]
public class TokenServiceTests
{
    private IConfiguration _configuration;
    private UserManager<AppUser> _userManager;
    private AppUser _user;
    

    [SetUp]
    public void Setup()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {
                "TokenKey", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
            }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var userStoreMock = new Mock<IUserStore<AppUser>>();
        var optionsMock = new Mock<IOptions<IdentityOptions>>();
        var passwordHasherMock = new Mock<IPasswordHasher<AppUser>>();
        var userValidators = new List<IUserValidator<AppUser>>();
        var passwordValidators = new List<IPasswordValidator<AppUser>>();
        var keyNormalizerMock = new Mock<ILookupNormalizer>();
        var errorsMock = new Mock<IdentityErrorDescriber>();
        var servicesMock = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<UserManager<AppUser>>>();

        _userManager = new UserManager<AppUser>(
            userStoreMock.Object,
            optionsMock.Object,
            passwordHasherMock.Object,
            userValidators,
            passwordValidators,
            keyNormalizerMock.Object,
            errorsMock.Object,
            servicesMock.Object,
            loggerMock.Object
        );

        _user = new AppUser
        {
            Id = 1,
            UserName = "test",
            // PasswordHash = new byte[64],
            // PasswordSalt = new byte[64],
            City = "test",
            Country = "test",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
            KnownAs = "test",
            Created = DateTime.Now,
            Gender = "test"
        };
    }

    [Test]
    public void CreateValidToken()
    {
        var token = new TokenService(_configuration, _userManager).CreateToken(_user);

        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.InstanceOf(typeof(string)));
    }

    [Test]
    public void CreateTokenWithInvalidKey()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {
                "TokenKey", "a"
            }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        Assert.Throws<Exception>(() => new TokenService(_configuration, _userManager).CreateToken(_user));
    }
}