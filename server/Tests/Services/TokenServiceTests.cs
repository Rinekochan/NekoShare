using NUnit.Framework;
using server.Entities;
using server.Services;

namespace server.Tests;

[TestFixture]
public class TokenServiceTests
{
    private IConfiguration _configuration;
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

        _user = new AppUser
        {
            Id = 1,
            UserName = "test",
            PasswordHash = new byte[64],
            PasswordSalt = new byte[64],
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
        var token = new TokenService(_configuration).CreateToken(_user);

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

        Assert.Throws<Exception>(() => new TokenService(_configuration).CreateToken(_user));
    }
}