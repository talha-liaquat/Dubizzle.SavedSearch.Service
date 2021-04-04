using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;


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
        public IActionResult Get([Required] string id, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            var response = _subscriptionService.Get(id, userId);

            if (response == null)
                return NoContent();

            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(SubscriptionResponseDto), StatusCodes.Status200OK)]
        public IActionResult GetByUser([Required] string userId)
        {
            var response = _subscriptionService.GetByUserId(userId);

            if (response == null)
                return NoContent();

            return Ok(response);
        }

        [HttpPost]
        public IActionResult PostAsync([Required] CreateSubscriptionRequestDto request, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var response = _subscriptionService.Create(request, userId);

            response.Url = $"{Request.Scheme}://{Request.Host}{Request.Path}/user/{userId}/subscription/{response.SubscriptionId}";

            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Put([Required] string id, [Required] CreateSubscriptionRequestDto request, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var response = _subscriptionService.Update(request, id, userId);

            response.Url = $"{Request.Scheme}://{Request.Host}{Request.Path}/user/{userId}/subscription/{response.SubscriptionId}";

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id, [Required][FromHeader(Name = "User-Id")] string userId)
        {
            _subscriptionService.Delete(id, userId);

            return Ok();
        }
    }
}