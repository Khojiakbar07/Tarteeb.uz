//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Orchestrations.UserTokens.Exceptions;

namespace Tarteeb.Api.Services.Orchestrations
{
    public partial class UserSecurityOrchestrationService
    {
        private static void ValidateEmailAndPassword(string email, string password)
        {
            Validate(
                (Rule: IsInvalid(email), Parameter: nameof(User.Email)),
                (Rule: IsInvalid(password), Parameter: nameof(User.Password)));
        }

        private void ValidateUser(User user)
        {
            ValidateUserExists(user);

            Validate(
                (Rule: IsUnverified(user.IsVerified), Parameter: nameof(User.IsVerified)));
        }

        private void ValidateUserExists(User user)
        {
            if (user is null)
            {
                throw new NotFoundUserException();
            }
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Value is required"
        };

        private static dynamic IsUnverified(bool isVerified) => new
        {
            Condition = isVerified is false,
            Message = "User is not verified"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserCreadentialOrchestrationException =
                new InvalidUserCredentialOrchestrationException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserCreadentialOrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserCreadentialOrchestrationException.ThrowIfContainsErrors();
        }
    }
}
