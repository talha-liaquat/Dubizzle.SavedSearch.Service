using System;
using System.Collections.Generic;
using System.Text;

namespace Dubizzle.SavedSearch.Dto
{
    public class ProductSearchRequestDto
    {
        public IList<ProductSearchRequestParamDto> Params { get; set; }
    }
}
