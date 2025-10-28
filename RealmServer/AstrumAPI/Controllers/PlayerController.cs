#region

using Common.API;
using Common.API.Models.SubscriptionModels;
using Common.API.Requests;
using Common.API.Requests.SubscriptionRequests;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AstrumAPI.Controllers
{
    /// <summary>
    /// Controller for all Loot endpoints.
    /// </summary>
    [ApiController]
    [Route("api/Players")]
    public class PlayerController : Controller
    {
        private static List<APISubscription> subscriptions = new();

        /// <summary>
        /// Subscribe a callback URL to the player death event.
        /// </summary>
        /// <param name="request">Model.</param>
        /// <returns>Ok(200) if successful. Conflict(409) if already subscribed.</returns>
        [HttpPost]
        [Route("DeathSubscribe")]
        public IActionResult SubscribePlayerDeath([FromBody] PlayerDeathSubscriptionRequest request)
        {
            if (subscriptions.Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                return Conflict("Already subscribed to this event.");
            }

            subscriptions.Add(new APISubscription() { CallbackUrl = request.CallbackUrl });
            return Ok($"Subscribed for player deaths!");
        }

        /// <summary>
        /// Unsubscribe a callback URL from the player death event.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Ok(200) if successful. Conflict(409) if wasn't subscribed.</returns>
        [HttpPost]
        [Route("DeathUnsubscribe")]
        public IActionResult UnsubscribePlayerDeath([FromBody] PlayerDeathUnsubscriptionRequest request)
        {
            if (subscriptions.Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                subscriptions.Remove(new APISubscription() { CallbackUrl = request.CallbackUrl });
                return Ok($"Unsubscribed for player deaths");
            }

            return Conflict("Wasn't subscribed to this event.");
        }

        /// <summary>
        /// Call the player death event.
        /// </summary>
        /// <param name="playerDeathRequest">Request.</param>
        /// <param name="auth">Authorization header, must match the bearer token as this is restricted.</param>
        /// <returns>Ok(200) if authorized. BadRequest(400) if not authorized.</returns>
        [HttpPut]
        [Route("Death")]
        public IActionResult DropLoot([FromBody] PlayerDeathRequest playerDeathRequest, [FromHeader(Name = "Authorization")] string auth)
        {
            if (!BearerToken.ValidateAuthorizationHeader(auth))
            {
                return BadRequest("Invalid permissions.");
            }

            SubscriptionPublisher.PublishSubscription(
                subscriptions,
                playerDeathRequest,
                (subscription) => { subscriptions.Remove(subscription); });
            return Ok();
        }
    }
}