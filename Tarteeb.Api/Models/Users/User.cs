//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
<<<<<<< Updated upstream:Tarteeb.Api/Models/Users/User.cs
=======
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Tarteeb.Api.Models.Foundations.Teams;
using Tarteeb.Api.Models.Foundations.Tickets;
>>>>>>> Stashed changes:Tarteeb.Api/Models/Foundations/Users/User.cs

namespace Tarteeb.Api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public Guid? ManagerId { get; set; }
        public string Password { get; set; }
<<<<<<< Updated upstream:Tarteeb.Api/Models/Users/User.cs
=======
        public bool IsActive { get; set; }
        public bool IsVerififed { get; set; }
        public string GitHubUsername { get; set; }
        public string TelegramUsername { get; set; }

        public Guid TeamId { get; set; }
        [JsonIgnore]
        public Team Team { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
>>>>>>> Stashed changes:Tarteeb.Api/Models/Foundations/Users/User.cs
    }
}
