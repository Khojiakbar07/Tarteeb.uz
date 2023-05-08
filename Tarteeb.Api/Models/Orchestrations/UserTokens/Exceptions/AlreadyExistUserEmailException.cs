//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using Xeptions;

namespace Tarteeb.Api.Models.Orchestrations.UserTokens.Exceptions
{
    public class AlreadyExistUserEmailException : Xeption
    {
        public AlreadyExistUserEmailException(Exception innerException)
            : base(message: "Email alreadry exist.", innerException)
        {
            
        }
    }
}
