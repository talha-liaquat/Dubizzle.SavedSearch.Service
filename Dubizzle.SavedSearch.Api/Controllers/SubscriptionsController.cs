using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync([Required] string id, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            var response = await _subscriptionService.GetAsync(id, userId);

            if (response == null)
                return NoContent();

            response.Url = GetUrl(response.SubscriptionId);

            return Ok(response);
        }

        [HttpGet()]
        [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser([Required][FromHeader(Name = "User-Id")] string userId)
        {
            var subscriptions = (await _subscriptionService.GetByUserIdAsync(userId)).ToList();

            if (subscriptions == null)
                return NoContent();

            foreach(var subscription in subscriptions)
            {
                subscription.Url = GetUrl(subscription.SubscriptionId);
            }

            return Ok(subscriptions);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([Required] CreateSubscriptionRequestDto request, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var response = await _subscriptionService.CreateAsync(request, userId);

            response.Url = GetUrl(response.SubscriptionId);
            response.UserId = userId;

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync([Required] string id, [Required] CreateSubscriptionRequestDto request, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var response = await _subscriptionService.UpdateAsync(request, id, userId);

            if (response == null)
                return BadRequest();

            response.Url = GetUrl(response.SubscriptionId);
            response.UserId = userId;

            return Ok(response);
        }

        private string GetUrl(string subscriptionId)
        {
            return $"{Request.Scheme}://{Request.Host}/api/subscriptions/{subscriptionId}";
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            var isDeleted = await _subscriptionService.DeleteAsync(id, userId);

            return !isDeleted ? BadRequest() : (IActionResult)Ok();
        }
    }
}