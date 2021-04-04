using Dubizzle.SavedSearch.Contracts;
using System;
using System.Collections.Generic;

namespace Dubizzle.SavedSearch.Entity
{
    public class Subscription : IEntity
    {
        public string Id { get; set; }
        public string SubscriptionId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public IEnumerable<SubscriptionDetail> Details { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}