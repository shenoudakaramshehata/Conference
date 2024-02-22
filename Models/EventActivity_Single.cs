﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Conference.Models
{
    [Table("EventActivity_Single")]
    public partial class EventActivity_Single
    {
        public EventActivity_Single()
        {
            SubscriptionActivities = new HashSet<SubscriptionActivity>();
        }

        [Key]
        public int EventActivityId { get; set; }
        public int EventId { get; set; }
        public string EventActivityTitle { get; set; }
        public string EventActivityDescription { get; set; }
        public int? EventActivityType { get; set; }

        [ForeignKey("EventId")]
        [InverseProperty("EventActivity_Singles")]
        public virtual Event Event { get; set; }
        [InverseProperty("EventActivity")]
        public virtual ICollection<SubscriptionActivity> SubscriptionActivities { get; set; }
    }
}