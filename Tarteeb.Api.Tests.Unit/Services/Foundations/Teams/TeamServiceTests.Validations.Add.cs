﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Teams;
using Tarteeb.Api.Models.Teams.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Foundations.Teams
{
    public partial class TeamServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            //given
            Team noTeam = null;
            var nullTeamException = new NullTeamException();

            var expectedTeamValidationException =
                new TeamValidationException(nullTeamException);

            //when
            ValueTask<Team> addTeamTask =
                this.teamService.AddTeamAsync(noTeam);

            TeamValidationException actualTeamValidationException =
               await Assert.ThrowsAsync<TeamValidationException>(addTeamTask.AsTask);

            //then
            actualTeamValidationException.Should()
                .BeEquivalentTo(expectedTeamValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedTeamValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeamAsync(It.IsAny<Team>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfTeamIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given 
            var invalidTeam = new Team
            {
                TeamName = invalidString
            };

            var invalidTeamException = new InvalidTeamException();

            invalidTeamException.AddData(
                key: nameof(Team.Id),
                values: "Id is required");

            invalidTeamException.AddData(
                key: nameof(Team.TeamName),
                    values: "Text is required");

            invalidTeamException.AddData(
                key: nameof(Team.CreatedDate),
                values: "Value is required");

            invalidTeamException.AddData(
                key: nameof(Team.UpdatedDate),
                values: "Value is required");

            var expectedTeamValidationException = 
                new TeamValidationException(invalidTeamException);

            //when

            ValueTask<Team> addTeamTask = this.teamService.AddTeamAsync(invalidTeam);

            TeamValidationException actualTeamValidationException =
                await Assert.ThrowsAsync<TeamValidationException>(addTeamTask.AsTask);

            //then
            actualTeamValidationException.Should().BeEquivalentTo(expectedTeamValidationException);

            this.loggingBrokerMock.Verify(broker=>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeamValidationException))),Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeamAsync(It.IsAny<Team>()),Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}