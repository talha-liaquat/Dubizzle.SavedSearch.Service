using Dubizzle.SavedSearch.Contracts;
using System;

namespace Dubizzle.SavedSearch.Entity
{
    public class SubscriptionDetail : IEntity
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
        public bool IsDeleted { get; set; }
    }
}