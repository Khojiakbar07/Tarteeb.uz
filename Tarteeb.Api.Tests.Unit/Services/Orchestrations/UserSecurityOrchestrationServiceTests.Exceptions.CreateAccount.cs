//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tarteeb.Api.Models.Foundations.Emails;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Orchestrations.UserTokens.Exceptions;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Orchestrations
{
    public partial class UserSecurityOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(UserEmailDependencyExceptions))]
        public async Task ShoudThrowDependencyExceptionOnCreateAccountIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            User someUser = CreateRandomUser();
            string someUrl = GetRandomString();

            var expectedUserOrchestrationDependencyException =
                new UserOrchestrationDependencyException(dependencyException.InnerException as Xeption);

            this.userServiceMock.Setup(service => service.AddUserAsync(
                It.IsAny<User>())).ThrowsAsync(dependencyException);

            // when
            ValueTask<User> createUserAccountTask = this.userSecurityOrchestrationService
                .CreateUserAccountAsync(someUser, someUrl);

            UserOrchestrationDependencyException actualUserOrchestrationDependencyException =
                await Assert.ThrowsAsync<UserOrchestrationDependencyException>(createUserAccountTask.AsTask);

            // then
            actualUserOrchestrationDependencyException.Should().BeEquivalentTo(
                expectedUserOrchestrationDependencyException);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()), Times.Once);

            this.loggingBrokerMock.Verify(
                broker => broker.LogError(It.Is(SameExceptionAs(
                    expectedUserOrchestrationDependencyException))), Times.Once);

            this.emailServiceMock.Verify(service =>
                service.SendEmailAsync(It.IsAny<Email>()), Times.Never);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.emailServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(UserEmailDependencyValidationExceptions))]
        public async Task ShoudThrowDependencyValidationExceptionOnCreateAccountIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            User someUser = CreateRandomUser();
            string someUrl = GetRandomString();

            var expectedUserOrchestrationDependencyValidationException =
                new UserOrchestrationDependencyValidationException(dependencyException.InnerException as Xeption);

            this.userServiceMock.Setup(service => service.AddUserAsync(
                It.IsAny<User>())).ThrowsAsync(dependencyException);

            // when
            ValueTask<User> createUserAccountTask = this.userSecurityOrchestrationService
                .CreateUserAccountAsync(someUser, someUrl);

            UserOrchestrationDependencyValidationException actualUserOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<UserOrchestrationDependencyValidationException>(createUserAccountTask.AsTask);

            // then
            actualUserOrchestrationDependencyValidationException.Should().BeEquivalentTo(
                expectedUserOrchestrationDependencyValidationException);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()), Times.Once);

            this.loggingBrokerMock.Verify(
                broker => broker.LogError(It.Is(SameExceptionAs(
                    expectedUserOrchestrationDependencyValidationException))), Times.Once);

            this.emailServiceMock.Verify(service =>
                service.SendEmailAsync(It.IsAny<Email>()), Times.Never);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.emailServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudThrowServiceExceptionOnCreateAccountIfServiceErrorOccursAndLogItAsync()
        {
            // given
            User someUser = CreateRandomUser();
            string someUrl = GetRandomString();

            var serviceException = new Exception();

            var failedUserTokenOrchestrationException = new
                FailedUserOrchestrationException(serviceException);

            var expectedUserOrchestrationServiceException =
                new UserOrchestrationServiceException(failedUserTokenOrchestrationException);

            this.userServiceMock.Setup(service => service.AddUserAsync(
                It.IsAny<User>())).ThrowsAsync(serviceException);

            // when
            ValueTask<User> createUserAccountTask = this.userSecurityOrchestrationService
                .CreateUserAccountAsync(someUser, someUrl);

            UserOrchestrationServiceException actualUserOrchestrationServiceException =
                await Assert.ThrowsAsync<UserOrchestrationServiceException>(createUserAccountTask.AsTask);

            // then
            actualUserOrchestrationServiceException.Should().BeEquivalentTo(
                expectedUserOrchestrationServiceException);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()), Times.Once);

            this.loggingBrokerMock.Verify(
                broker => broker.LogError(It.Is(SameExceptionAs(
                    expectedUserOrchestrationServiceException))), Times.Once);

            this.emailServiceMock.Verify(service =>
                service.SendEmailAsync(It.IsAny<Email>()), Times.Never);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.emailServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnCreateAccountWhenDuplicateKeyErrorOccursAndLogItAsync()
        {
            // given
            string someEmail = GetRandomString();
            string randomUrl = CreateRandomUrl();
            User randomUser = CreateRandomUser();
            randomUser.Email = someEmail;
            User inputUser = randomUser.DeepClone();

            var dbKeyException = new DuplicateKeyException(someEmail);
            var expectedAlreadyExistUserEmailException = new AlreadyExistUserEmailException(dbKeyException);

            var userEmailDependencyValidationException =
                new UserOrchestrationDependencyValidationException(expectedAlreadyExistUserEmailException);

            // when
            ValueTask<User> addUserEmailTask = 
                this.userSecurityOrchestrationService.CreateUserAccountAsync(inputUser, randomUrl);

            AlreadyExistUserEmailException actualAlreadyExistUserEmailException =
                await Assert.ThrowsAsync<AlreadyExistUserEmailException>(addUserEmailTask.AsTask);

            // then
            actualAlreadyExistUserEmailException.Should().BeEquivalentTo(expectedAlreadyExistUserEmailException);

            this.userServiceMock.Verify(service =>
                service.AddUserAsync(It.IsAny<User>()), Times.Once);

            this.loggingBrokerMock.Verify(
                broker => broker.LogError(It.Is(SameExceptionAs(
                    expectedAlreadyExistUserEmailException))), Times.Once);

            this.emailServiceMock.Verify(service =>
                service.SendEmailAsync(It.IsAny<Email>()), Times.Never);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.emailServiceMock.VerifyNoOtherCalls();
        }
    }
}
