using BankDirectoryApi.Infrastructure.Services.ThirdParties.Verification;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Writers;
using Moq;

namespace BankDirectoryApi.UnitTests;

public class TwilioEmailVerificationTests
{
    private readonly TwilioEmailVerificationService _twilioEmailVerificationService;

    public TwilioEmailVerificationTests()
    {
        var mockConfig = new Mock<IConfiguration>();
        var mockLogger = new Mock<ILogger<TwilioEmailVerificationService>>();

        // Properly set all required config keys
        void MockConfigValue(string key, string value)
        {
            mockConfig.Setup(x => x[key]).Returns(value);

            var section = new Mock<IConfigurationSection>();
            section.Setup(x => x.Value).Returns(value);
            mockConfig.Setup(x => x.GetSection(key)).Returns(section.Object);
        }

        MockConfigValue("Sms:Twilio:AccountSid", "AC---");
        MockConfigValue("Sms:Twilio:AuthToken", "--");
        MockConfigValue("Verification:Twilio:ServiceSid", "VA--");

        // Create the service
        _twilioEmailVerificationService = new TwilioEmailVerificationService(
            mockConfig.Object,
            mockLogger.Object
        );
    }
    [Fact(Skip = "Unit test — requires real Twilio email")]
    //[Fact]
    public async Task SendVerificationCodeAsync_ShouldReturnSuccess_WhenPhoneNumberIsValid()
    {
        // Arrange
        string email = "a@b.com"; // Example phone number
        // Act
        var result = await _twilioEmailVerificationService.SendVerificationCodeAsync(email);
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    [Fact(Skip = "Unit test — requires real Twilio email and valid OTP code")]
    //[Fact]
    public async Task VerifyCodeAsync_ShouldReturnSuccess_WhenCodeIsValid()
    {
        // Arrange
        string email = "a@b.com"; // Example phone number
        string code = "1111"; // Example verification code
        // Act
        var result = await _twilioEmailVerificationService.VerifyCodeAsync(email, code);
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
