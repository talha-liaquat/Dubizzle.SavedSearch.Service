using System.ComponentModel.DataAnnotations;

namespace Dubizzle.SavedSearch.Dto
{
    public class SubscriptionDetailDto
    {
        [Required]
        public string Key { get; set; }
        [Required] 
        public string Value { get; set; }
        [Required] 
        public string Operator { get; set; }
    }
}
