//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Securities.Exceptions;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Securities
{
    public partial class SecurityServiceTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldThrowValidationExceptionOnHashPasswordIfPasswordIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            string invalidPassword = invalidString;

            var invalidPasswordException = new InsecurePasswordException();

            invalidPasswordException.AddData(
                key: "ValueComplexity",
                values: "Text is required");

            invalidPasswordException.AddData(
                key: "LengthComplexity",
                values: "At least 8 characters is required");

            invalidPasswordException.AddData(
                key: "UppercaseComplexity",
                values: "At least one capital letter is required");

            invalidPasswordException.AddData(
                key: "SymbolComplexity",
                values: "At least one symbol is required");

            invalidPasswordException.AddData(
                key: "DigitComplexity",
                values: "At least one digit is required");

            var expectedUserValidationException = new UserValidationException(
                invalidPasswordException);

            //when
            Func<string> hashPasswordFunc = () =>
                this.securityService.HashPassword(invalidPassword);

            UserValidationException actualUserValidationException =
                Assert.Throws<UserValidationException>(hashPasswordFunc);

            //then
            actualUserValidationException.Should().BeEquivalentTo(
                expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogError(It.Is(SameExceptionAs(
                 expectedUserValidationException))), Times.Once);

            this.tokenBrokerMock.Verify(broker =>
              broker.HashToken(It.IsAny<string>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.tokenBrokerMock.VerifyNoOtherCalls();
        }
    }
}
