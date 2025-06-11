using System.Security.Claims;
using Bogus;
using Currency.Data.Contracts;
using Currency.Domain.Login;
using Currency.Domain.Users;
using Currency.Infrastructure.Contracts.JwtBearer;
using Currency.Services.Application;
using Currency.Services.Application.Settings;
using Currency.Services.Tests.Fakes;
using Moq;

namespace Currency.Services.Tests.Application;

public class TokenServiceTests
{
    private Mock<IAuthRepository> _mockAuthRepository;
    private Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;

    [SetUp]
    public void Setup()
    {
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockAuthRepository = new Mock<IAuthRepository>();
    }

    [Test]
    public void GenerateTokens_HappyPath_ShouldReturnTokenModel()
    {
        //Arrange
        _mockJwtTokenGenerator.Setup(x => x.BuildClaims(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<UserRole>()))
            .Returns(new List<Claim>
            {
                new(ClaimTypes.Name, "name")
            });

        _mockJwtTokenGenerator.Setup(x => x.CreateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(new AccessToken { ExpiresAt = DateTime.MaxValue, Token = "111" });

        _mockJwtTokenGenerator.Setup(x => x.CreateRefreshToken(It.IsAny<string>()))
            .Returns("1111");

        var sut = new TokenService(
            new ServicesSettings(),
            _mockJwtTokenGenerator.Object,
            _mockAuthRepository.Object);

        //Act
        var (result, resultClaims) = sut.GenerateTokens(FakeModels.GenerateFakeUser());

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.AccessToken, Is.Not.Null.Or.Empty);
            Assert.That(result.RefreshToken, Is.Not.Null.Or.Empty);
            Assert.That(resultClaims, Is.Not.Null);
        });
    }

    [Test]
    public void GenerateAccessToken_HappyPath_ShouldReturnAccessTokenAndClaims()
    {
        //Arrange
        _mockJwtTokenGenerator.Setup(x => x.BuildClaims(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<UserRole>()))
            .Returns(new List<Claim>
            {
                new(ClaimTypes.Name, "name")
            });

        _mockJwtTokenGenerator.Setup(x => x.CreateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(new AccessToken { ExpiresAt = DateTime.MaxValue, Token = "111" });

        var sut = new TokenService(
            new ServicesSettings(),
            _mockJwtTokenGenerator.Object,
            _mockAuthRepository.Object);

        //Act
        var (result, resultClaims) = sut.GenerateAccessToken(FakeModels.GenerateFakeUser());

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Token, Is.Not.Null.Or.Empty);
            Assert.That(resultClaims, Is.Not.Null);
        });
    }

    [Test]
    public async Task GetRefreshTokenAsync_HappyPath_ShouldReturnRefreshToken()
    {
        //Arrange
        var refreshToken = new Faker().Random.Hash();

        _mockAuthRepository.Setup(x => x.GetRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken { Verified = true });

        var sut = new TokenService(
            new ServicesSettings(),
            _mockJwtTokenGenerator.Object,
            _mockAuthRepository.Object);

        //Act
        var result = await sut.GetRefreshTokenAsync(refreshToken);

        //Assert
        Assert.That(result.Verified, Is.True);
    }

    [Test]
    public async Task AddRefreshTokenAsync_HappyPath_ShouldProcessSuccessfully()
    {
        //Arrange
        _mockAuthRepository.Setup(x => x.AddRefreshToken(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        var sut = new TokenService(
            new ServicesSettings(),
            _mockJwtTokenGenerator.Object,
            _mockAuthRepository.Object);

        var refreshToken = new Faker().Random.Hash();

        //Act & Assert
        Assert.DoesNotThrowAsync(async () => await sut.AddRefreshTokenAsync(refreshToken, "1"));
    }
}