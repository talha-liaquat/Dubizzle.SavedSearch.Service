using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dubizzle.SavedSearch.Dto
{
    public class SubscriptionResponseDto
    {
        public string SubscriptionId { get; set; }

        public string Url { get; set; }

        public string Email { get; set; }
        
        public IEnumerable<SubscriptionDetailDto> Details { get; set; }
    }
}