//using BankDirectoryApi.Infrastructure.Services.ThirdParties.Verification;
//using FluentAssertions;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Microsoft.OpenApi.Writers;
//using Moq;

//namespace BankDirectoryApi.UnitTests;

//public class TwilioSmsVerificationTests
//{
//    private readonly TwilioSmsVerificationService _twilioSmsVerificationService;

//    public TwilioSmsVerificationTests()
//    {
//        var mockConfig = new Mock<IConfiguration>();
//        var mockLogger = new Mock<ILogger<TwilioSmsVerificationService>>();

//        // Properly set all required config keys
//        void MockConfigValue(string key, string value)
//        {
//            mockConfig.Setup(x => x[key]).Returns(value);
//            var section = new Mock<IConfigurationSection>();
//            section.Setup(x => x.Value).Returns(value);
//            mockConfig.Setup(x => x.GetSection(key)).Returns(section.Object);
//        }

//        MockConfigValue("Sms:Twilio:AccountSid", "AC--");
//        MockConfigValue("Sms:Twilio:AuthToken", "--");
//        MockConfigValue("Verification:Twilio:ServiceSid", "VA--");

//        // Create the service
//        _twilioSmsVerificationService = new TwilioSmsVerificationService(
//            mockConfig.Object,
//            mockLogger.Object
//        );
//    }
//    [Fact(Skip = "Unit test — requires real Twilio number")]

//    public async Task SendVerificationCodeAsync_ShouldReturnSuccess_WhenPhoneNumberIsValid()
//    {
//        // Arrange
//        string phoneNumber = "+9647714076925"; // Example phone number
//        // Act
//        var result = await _twilioSmsVerificationService.SendVerificationCodeAsync(phoneNumber);
//        // Assert
//        result.IsSuccess.Should().BeTrue();
//    }
//    [Fact(Skip = "Unit test — requires real Twilio number and valid OTP code")]

//    public async Task VerifyCodeAsync_ShouldReturnSuccess_WhenCodeIsValid()
//    {
//        // Arrange
//        string phoneNumber = "+9647714076925"; // Example phone number
//        string code = "437012"; // Example verification code
//        // Act
//        var result = await _twilioSmsVerificationService.VerifyCodeAsync(phoneNumber, code);
//        // Assert
//        result.IsSuccess.Should().BeTrue();
//    }
//}
