using System.Collections;
using System.Collections.Generic;

namespace Dubizzle.SavedSearch.Dto
{
    public class ProductSearchResponseDto
    {
        public IEnumerable<ProductSearchDto> Result { get; set; }
    }
}
