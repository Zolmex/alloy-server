#region

using Common.API;
using Common.API.Models.SubscriptionModels;
using Common.API.Requests;
using Common.API.Requests.SubscriptionRequests;
using Common.Enums;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace AstrumAPI.Controllers
{
    /// <summary>
    /// Controller for all Loot endpoints.
    /// </summary>
    [ApiController]
    [Route("api/Loot")]
    public class LootController : Controller
    {
        private static Dictionary<LootDropRarity, List<LootDropSubscription>> lootDropSubscriptions = new() { { LootDropRarity.All, new List<LootDropSubscription>() }, { LootDropRarity.Legendary, new List<LootDropSubscription>() }, { LootDropRarity.Primal, new List<LootDropSubscription>() } };

        /// <summary>
        /// Subscribe a callback URL to the loot drop event.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Ok(200) if successful. Conflict(409) if already subscribed.</returns>
        [HttpPost]
        [Route("LootDropSubscribe")]
        public IActionResult SubscribeLootDrop([FromBody] LootDropSubscriptionRequest request)
        {
            if (lootDropSubscriptions[request.LootDropRarity].Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                return Conflict("Already subscribed to this event.");
            }
            else if (lootDropSubscriptions[LootDropRarity.All].Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                return Conflict("Already subscribed to all events.");
            }

            lootDropSubscriptions[request.LootDropRarity].Add(new LootDropSubscription(request));
            return Ok($"Subscribed for {request.LootDropRarity} loot drops!");
        }

        /// <summary>
        /// Unsubscribe a callback URL from the loot drop event.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Ok(200) if successful. Conflict(409) if wasn't subscribed.</returns>
        [HttpPost]
        [Route("LootDropUnsubscribe")]
        public IActionResult UnsubscribeLootDrop([FromBody] LootDropUnsubscriptionRequest request)
        {
            if (lootDropSubscriptions[request.LootDropRarity].Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                lootDropSubscriptions[request.LootDropRarity].Remove(new LootDropSubscription(request));
                return Ok($"Unsubscribed for {request.LootDropRarity} loot drops!");
            }

            return Conflict("Wasn't subscribed to this event.");
        }

        /// <summary>
        /// Call the loot drop event.
        /// </summary>
        /// <param name="dropLootRequest">Request.</param>
        /// <param name="auth">Authorization header, must match the bearer token as this is restricted.</param>
        /// <returns>Ok(200) if authorized. BadRequest(400) if not authorized.</returns>
        [HttpPut]
        [Route("DropLoot")]
        public IActionResult DropLoot([FromBody] DropLootRequest dropLootRequest, [FromHeader(Name = "Authorization")] string auth)
        {
            if (!BearerToken.ValidateAuthorizationHeader(auth))
            {
                return BadRequest("Invalid permissions.");
            }

            SubscriptionPublisher.PublishSubscription(
                lootDropSubscriptions[dropLootRequest.Rarity].Union(lootDropSubscriptions[LootDropRarity.All]),
                dropLootRequest,
                (subscription) =>
                {
                    var lootDropSubscription = subscription as LootDropSubscription;
                    if (lootDropSubscription != null)
                    {
                        lootDropSubscriptions[lootDropSubscription.Rarity].Remove(lootDropSubscription);
                    }
                });
            return Ok();
        }
    }
}