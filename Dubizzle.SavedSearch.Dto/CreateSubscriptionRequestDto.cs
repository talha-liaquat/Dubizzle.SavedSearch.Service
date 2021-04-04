using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dubizzle.SavedSearch.Dto
{
    public class CreateSubscriptionRequestDto
    {        
        [Required]
        public string Email { get; set; }
        [Required] 
        public IEnumerable<SubscriptionDetailDto> Details { get; set; }
    }
}