//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

namespace Tarteeb.Api.Models.Foundations.Users
{
    public class EmailModel
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public EmailModel(string to, string subject, string content)
        {
            To = to;
            Subject = subject;
            Content = content;
        }

    }
}
