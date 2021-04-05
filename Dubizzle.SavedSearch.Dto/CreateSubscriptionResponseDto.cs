using System.ComponentModel.DataAnnotations;

namespace Dubizzle.SavedSearch.Dto
{
    public class CreateSubscriptionResponseDto
    {
        [Required]
        public string SubscriptionId { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}