using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dubizzle.SavedSearch.Service
{
    public class ProductService : IProductService<ProductSearchRequestDto, ProductSearchResponseDto>
    {
        public ProductSearchResponseDto Search(ProductSearchRequestDto param)
        {
            //TODO: For assignment mock response. Can be replaced by calling Product Microservice
            var mockResponse = new List<ProductSearchDto>
            {
                new ProductSearchDto{ Description = "Sample Description 1",     Milage = 100000,    Price = 12120M,     Title = "Sample Title 1" },
                new ProductSearchDto{ Description = "Sample Description 2",     Milage = 20000,     Price = 21364M,     Title = "Sample Title 2" },
                new ProductSearchDto{ Description = "Sample Description 3",     Milage = 1030000,   Price = 123120M,    Title = "Sample Title 3" },
                new ProductSearchDto{ Description = "Sample Description 4",     Milage = 40000,     Price = 44320M,     Title = "Sample Title 4" },
                new ProductSearchDto{ Description = "Sample Description 5",     Milage = 10000,     Price = 12310M,     Title = "Sample Title 5" },
                new ProductSearchDto{ Description = "Sample Description 6",     Milage = 4000,      Price = 43430M,     Title = "Sample Title 6" },
                new ProductSearchDto{ Description = "Sample Description 7",     Milage = 10100,     Price = 123220M,    Title = "Sample Title 7" },
                new ProductSearchDto{ Description = "Sample Description 8",     Milage = 250000,    Price = 12100M,     Title = "Sample Title 8" },
                new ProductSearchDto{ Description = "Sample Description 9",     Milage = 3000,      Price = 12100M,     Title = "Sample Title 9" },
                new ProductSearchDto{ Description = "Sample Description 10",    Milage = 890200,    Price = 7430M,      Title = "Sample Title 10" },
                new ProductSearchDto{ Description = "Sample Description 11",    Milage = 124000,    Price = 12110M,     Title = "Sample Title 11" },
                new ProductSearchDto{ Description = "Sample Description 12",    Milage = 1566000,   Price = 1230M,      Title = "Sample Title 12" },
                new ProductSearchDto{ Description = "Sample Description 13",    Milage = 158000,    Price = 75420M,     Title = "Sample Title 13" },
                new ProductSearchDto{ Description = "Sample Description 14",    Milage = 15700,     Price = 1120M,      Title = "Sample Title 14" },
            };

            //Pick Random top 10 results from above mock list
            return new ProductSearchResponseDto { Result = mockResponse.OrderBy(o => Guid.NewGuid()).Take(10) };
        }
    }
}
