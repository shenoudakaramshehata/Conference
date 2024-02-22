﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Conference.Models
{
    [Table("SubscriptionActivity")]
    public partial class SubscriptionActivity
    {
        [Key]
        public int SubscriptionActivityId { get; set; }
        public int EventSubscriptionId { get; set; }
        public int EventActivityId { get; set; }

        [ForeignKey("EventActivityId")]
        [InverseProperty("SubscriptionActivities")]
        public virtual EventActivity_Single EventActivity { get; set; }
        [ForeignKey("EventSubscriptionId")]
        [InverseProperty("SubscriptionActivities")]
        public virtual EventSubscription EventSubscription { get; set; }
    }
}