using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
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

            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser([Required] string userId)
        {
            var response = await _subscriptionService.GetByUserIdAsync(userId);

            if (response == null)
                return NoContent();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([Required] CreateSubscriptionRequestDto request, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var response = await _subscriptionService.CreateAsync(request, userId);

            response.Url = $"{Request.Scheme}://{Request.Host}{Request.Path}/user/{userId}/subscription/{response.SubscriptionId}";

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync([Required] string id, [Required] CreateSubscriptionRequestDto request, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var response = await _subscriptionService.UpdateAsync(request, id, userId);

            response.Url = $"{Request.Scheme}://{Request.Host}{Request.Path}/user/{userId}/subscription/{response.SubscriptionId}";

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            await _subscriptionService.DeleteAsync(id, userId);

            return Ok();
        }
    }
}