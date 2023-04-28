//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Orchestrations.UserTokens.Exceptions
{
    public class InvalidUserPasswordOrchestrationException : Xeption
    {
        public InvalidUserPasswordOrchestrationException()
            : base(
                  "Password is invalid, " +
                  "password must have :" +
                  " minimum 8 characters," +
                  " 2 letters in Upper Case" +
                  "1 Special Character (!@#$&*)" +
                  "2 numerals (0-9)" +
                  "3 letters in Lower Case")
        { }
    }
}
