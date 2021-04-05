using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using System.Text;

namespace Dubizzle.SavedSearch.Service
{
    public class EmailHtmlTemplateService : ITemplateService<(InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult)>
    {
        public string GenerateTemplate((InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult) obj)
        {

            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"Hi {obj.message.Email}<br /><br />");

            stringBuilder.Append($"Here are some products matching your search criteria");
            
            stringBuilder.Append($"<table>");

            stringBuilder.Append($"<tr><td>Title</td><td>Description</td><td>Milage</td><td>Price</td></tr>");
            
            foreach (var result in obj.searchResult.Result)
            {
                stringBuilder.Append($"<tr><td>{result.Title}</td><td>{result.Description}</td><td>{result.Milage}</td><td>{result.Price}</td></tr>");
            }

            stringBuilder.Append($"</table><br /><br />");

            stringBuilder.Append($"<br /><br />");

            stringBuilder.Append($"Happy Searching on Dubizzle!<br /><br />");

            stringBuilder.Append($"Thanks,<br />");

            stringBuilder.Append($"Dubizzle Team");

            return stringBuilder.ToString();
        }
    }
}