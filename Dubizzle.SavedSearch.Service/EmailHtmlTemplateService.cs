using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Dubizzle.SavedSearch.Service
{
    public class EmailHtmlTemplateService : ITemplateService<(InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult)>
    {
        public string GenerateTemplate((InternalMessageEnvelopDto message, ProductSearchResponseDto searchResult) obj)
        {

            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"Hi <b>{obj.message.Email}</b><br /><br />");
        
            stringBuilder.Append("<p style='color: blue;'>");
            stringBuilder.Append(string.Join(" <i>and</i> ", obj.message.Items.Select(x => $"<b>{x.Key} {x.Operator} {x.Value}</b>")));
            stringBuilder.Append("</p>");

            stringBuilder.Append($"<br /><table style='border: 1px solid black; border-collapse: collapse'>");

            stringBuilder.Append($"<tr style='border: 1px solid black; padding: 10px; background-color: darkseagreen;'>" +
                $"<td style='border: 1px solid black; padding: 10px'>Title</td>" +
                $"<td style='border: 1px solid black; padding: 10px'>Milage</td>" +
                $"<td style='border: 1px solid black; padding: 10px'>Price</td></tr>");

            var keys = obj.message.Items.Select(x => x.Key.ToLower()).ToList();
            var hasTitle = keys.Contains("title");
            var hasMilage = keys.Contains("milage");
            var hasPrice = keys.Contains("price");

            foreach (var result in obj.searchResult.Result)
            {
                stringBuilder.Append($"<tr style='border: 1px solid black; padding: 10px;'>" +
                    $"<td style='border: 1px solid black;padding: 10px; {(hasTitle ? "color: blue;" : string.Empty)}'>{result.Title}</td>" +
                    $"<td style='border: 1px solid black;padding: 10px; text-align:right; {(hasMilage ? "color: blue;" : string.Empty)}'>{result.Milage} KM</td>" +
                    $"<td style='border: 1px solid black;padding: 10px; text-align:right; {(hasPrice ? "color: blue;" : string.Empty)}'>{result.Price} AED</td></tr>");
            }

            stringBuilder.Append($"</table><br />");

            stringBuilder.Append($"Happy Searching on Dubizzle!<br /><br />");

            stringBuilder.Append($"Thanks,<br />");

            stringBuilder.Append($"Dubizzle Team<br /><br />");
            
            stringBuilder.Append($"<img src='https://cdn.freelogovectors.net/wp-content/uploads/2019/01/dubizzle_logo.png' width='150' height='50'></img>");

            return stringBuilder.ToString();
        }
    }
}