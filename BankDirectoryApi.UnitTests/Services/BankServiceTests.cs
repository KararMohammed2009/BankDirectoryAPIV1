using BankDirectoryApi.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BankDirectoryApi.Domain.Entities;
using FluentAssertions;
using BankDirectoryApi.Application.Services.Main;
using FluentResults;
using BankDirectoryApi.Application.DTOs.Core.Bank;
namespace BankDirectoryApi.UnitTests.Services
{
    public class BankServiceTests
    {
        private readonly Mock<IBankRepository> _bankRepositoryMock;
        private readonly BankService _bankService;
        private readonly Mock<IMapper> _iMapperMock;
        public BankServiceTests()
        {
            _bankRepositoryMock = new Mock<IBankRepository>();
            _iMapperMock = new Mock<IMapper>();
            _bankService = new BankService(_bankRepositoryMock.Object,_iMapperMock.Object);
        }
        [Fact]
        public async Task GetBankByIdAsync_ShouldReturnBankDto_WhenBankExists()
        {
            // Arrange
            var bank = new Bank { Id = 1, Name = "Test Bank" };
            var bankDto = new BankDTO { Id = 1, Name = "Test Bank" };

            _bankRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                               .ReturnsAsync(bank);
            _iMapperMock.Setup(m => m.Map<BankDTO>(bank))
                .Returns(bankDto);  // Ensure AutoMapper returns the expected DTO

            // Act
            var result = await _bankService.GetBankByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Test Bank");
        }
        public async Task GetBanksAsync_ShouldReturnIEnumerableOfBankDTO_WhenBanksExists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            Result<IEnumerable<Bank>> banks = new List<Bank> { new Bank { Id = 1, Name = "Test Bank 1" },
            new Bank { Id = 2, Name = "Test Bank 2" }};
            IEnumerable<BankDTO> banksDtos = new List<BankDTO> {
                new BankDTO { Id = 1, Name = "Test Bank 1" }, new BankDTO { Id = 2, Name = "Test Bank 2" } };

            _bankRepositoryMock.Setup(repo => repo.GetAllAsync(cancellationToken))
                               .ReturnsAsync(banks);
            _iMapperMock.Setup(m => m.Map<IEnumerable<BankDTO>>(banks))
                .Returns(banksDtos);  // Ensure AutoMapper returns the expected DTO

            // Act
            var result = await _bankService.GetAllBanksAsync(cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(banksDtos);
        }
        [Theory]
        [InlineData(1,"bank 1")]  
        [InlineData(2,"bank 2")]
        //[InlineData(null, "bank 2")]
        public async Task GetBankByIdAsync_ShouldReturnBankDto_WhenBankExists2(int id,string name)
        {
            // Arrange
            var bank = new Bank { Id = id, Name = name };
            var bankDto = new BankDTO { Id = id, Name = name };

            _bankRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                               .ReturnsAsync(bank);
            _iMapperMock.Setup(m => m.Map<BankDTO>(bank))
                .Returns(bankDto);  // Ensure AutoMapper returns the expected DTO

            // Act
            var result = await _bankService.GetBankByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Name.Should().Be(name);
        }
    }
}
