using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dubizzle.SavedSearch.Dto
{
    public class SubscriptionResponseDto
    {
        public string Id { get; set; }

        public string Email { get; set; }
        
        public IEnumerable<SubscriptionDetailDto> Details { get; set; }
    }
}