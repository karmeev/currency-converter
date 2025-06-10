using System.Security.Claims;
using Currency.Domain.Login;
using Currency.Facades.Contracts.Requests;
using Currency.Facades.Tests.Fakes;
using Currency.Facades.Validators;
using Currency.Facades.Validators.Results;
using Currency.Services.Contracts.Application;
using Moq;

namespace Currency.Facades.Tests;

[TestFixture]
public class AuthFacadeTests
{
    private Mock<IAuthValidator> _authValidator;
    private Mock<IUserService> _userService;
    private Mock<ICacheService> _cacheService;
    private Mock<ITokenService> _tokenService;
    
    [SetUp]
    public void Setup()
    {
        _authValidator = new Mock<IAuthValidator>();
        _userService = new Mock<IUserService>();
        _cacheService = new Mock<ICacheService>();
        _tokenService = new Mock<ITokenService>();
    }

    [Test]
    public async Task LoginAsync_HappyPath_ShouldReturnSuccess()
    {
        //Arrange
        var request = FakeRequests.GenerateLoginRequest();

        _authValidator.Setup(x => x.Validate(request.Username, request.Password))
            .Returns(ValidationResult.Success);
        
        _userService.Setup(x => x.CheckUser(It.IsAny<LoginModel>()))
            .ReturnsAsync(true);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, request.Username),
            new(ClaimTypes.Role, "user")
        };
        
        _tokenService.Setup(x => x.GetClaims(It.IsAny<LoginModel>())).Returns(claims);
        _tokenService.Setup(x => x.GenerateAccessToken(claims)).Returns(FakeResults.GenerateAccessToken());
        _tokenService.Setup(x => x.GenerateRefreshToken(It.IsAny<LoginModel>()))
            .Returns(FakeResults.GenerateRefreshToken());
        
        _cacheService.Setup(x => x.InsertNewRefreshToken(It.IsAny<LoginModel>()));
        
        var sut = new AuthFacade(_authValidator.Object, _userService.Object, _cacheService.Object, 
            _tokenService.Object);
        
        //Act
        var result = await sut.LoginAsync(request);
        
        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Empty);
            Assert.That(result.Claims, Is.Not.Null);
            Assert.That(result.Claims.Count, Is.EqualTo(2));
            Assert.That(result.Claims.First().Value, Is.EqualTo(request.Username));
            Assert.That(result.AccessToken, Is.Not.Null.Or.Empty);
            Assert.That(result.RefreshToken, Is.Not.Null.Or.Empty);
        });
    }
}