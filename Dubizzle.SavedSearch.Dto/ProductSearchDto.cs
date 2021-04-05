using System;
using System.Collections.Generic;
using System.Text;

namespace Dubizzle.SavedSearch.Dto
{
    public class ProductSearchDto
    {
        public string Title { get; set; }
        public string TitleLocale { get; set; }
        public int Milage { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
