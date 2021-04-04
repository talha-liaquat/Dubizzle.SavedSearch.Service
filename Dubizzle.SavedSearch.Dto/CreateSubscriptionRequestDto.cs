using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dubizzle.SavedSearch.Dto
{
    public class CreateSubscriptionRequestDto
    {        
        [Required]
        public string DbContext { get; set; }
        [Required]
        public string Email { get; set; }
        public string CronExpression { get; set; }
        [Required] 
        public IEnumerable<SubscriptionDetailDto> Details { get; set; }
    }
}