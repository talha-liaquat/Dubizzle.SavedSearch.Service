using System;
using System.Collections.Generic;
using System.Text;

namespace Dubizzle.SavedSearch.Contracts
{
    public interface IProductService<T, K>
    {
        K Search(T param);
    }
}