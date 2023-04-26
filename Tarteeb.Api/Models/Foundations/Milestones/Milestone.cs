//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Tarteeb.Api.Models.Foundations.Tickets;
using Tarteeb.Api.Models.Foundations.Users;

namespace Tarteeb.Api.Models.Foundations.Milestones
{
    public class Milestone
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public Guid AssigneeId { get; set; }
        [JsonIgnore]
        public User Assignee { get; set; }
        [JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; }
    }
}