using System;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface IEntity
    {
        string Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
