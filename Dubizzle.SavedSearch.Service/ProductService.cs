using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dubizzle.SavedSearch.Service
{
    public class ProductService : IProductService<ProductSearchRequestDto, ProductSearchResponseDto>
    {
        static List<ProductSearchDto> mockProducts = null;
        public ProductSearchResponseDto Search(ProductSearchRequestDto param)
        {
            //TODO: For assignment mock response. Can be replaced by calling Product Microservice
            if(mockProducts == null)
                mockProducts = JsonConvert.DeserializeObject<List<ProductSearchDto>>(File.ReadAllText("CarsTestData.json"));
                
            var reqMilage = Convert.ToInt32(param.Params.FirstOrDefault(x => x.Key.Equals("milage", StringComparison.OrdinalIgnoreCase))?.Value);
            var reqMilageOperator = param.Params.FirstOrDefault(x => x.Key.Equals("milage", StringComparison.OrdinalIgnoreCase))?.Operator;

            var reqPrice = Convert.ToInt32(param.Params.FirstOrDefault(x => x.Key.Equals("price", StringComparison.OrdinalIgnoreCase))?.Value);
            var reqPriceOperator = param.Params.FirstOrDefault(x => x.Key.Equals("price", StringComparison.OrdinalIgnoreCase))?.Operator;

            var reqTitle = param.Params.FirstOrDefault(x => x.Key.Equals("title", StringComparison.OrdinalIgnoreCase))?.Value;

            var filteredMockProducts = mockProducts

                .Where(x => x.Title.Equals(reqTitle ?? x.Title, StringComparison.OrdinalIgnoreCase))

                .Where(x => reqMilageOperator == null ||
                            ((reqMilageOperator == "<") && x.Milage < reqMilage) ||
                            ((reqMilageOperator == "=") && x.Milage == reqMilage) ||
                            ((reqMilageOperator == ">") && x.Milage > reqMilage))

                .Where(x => reqPriceOperator == null ||
                            ((reqPriceOperator == "<") && x.Price < reqPrice) ||
                            ((reqPriceOperator == "=") && x.Price == reqPrice) ||
                            ((reqPriceOperator == ">") && x.Price > reqPrice));
            
            //Pick Random top 10 results from above mock list
            return new ProductSearchResponseDto { Result = filteredMockProducts.OrderBy(o => Guid.NewGuid()).Take(10) };
        }
    }
}












