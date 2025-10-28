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
    /// Controller for all Event endpoints.
    /// </summary>
    [ApiController]
    [Route("api/Events")]
    public class EventController : Controller
    {
        #region ugly constructors

        private static Dictionary<SubscribableEvent, List<EventLifecycleSubscription>> eventSpawnSubscriptions = new()
        {
            { SubscribableEvent.All, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.Avatar, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.LordoftheLostLands, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.GrandSphinx, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.SkullShrine, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.CubeGod, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.Pentaract, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.HermitGod, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.GhostShip, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.EpicHive, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.RockDragon, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.LostSentry, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.MountainTemple, new List<EventLifecycleSubscription>() }
        };

        private static Dictionary<SubscribableEvent, List<EventLifecycleSubscription>> eventDeathSubscriptions = new()
        {
            { SubscribableEvent.All, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.Avatar, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.LordoftheLostLands, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.GrandSphinx, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.SkullShrine, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.CubeGod, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.Pentaract, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.HermitGod, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.GhostShip, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.EpicHive, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.RockDragon, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.LostSentry, new List<EventLifecycleSubscription>() },
            { SubscribableEvent.MountainTemple, new List<EventLifecycleSubscription>() }
        };

        #endregion

        /// <summary>
        /// Subscribe a callback URL to the event spawn event.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Ok(200) if successful. Conflict(409) if already subscribed.</returns>
        [HttpPost]
        [Route("EventSpawnSubscribe")]
        public IActionResult SubscribeEventSpawn([FromBody] EventSpawnSubscriptionRequest request)
        {
            if (eventSpawnSubscriptions[request.Event].Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                return Conflict("Already subscribed to this event.");
            }
            else if (eventSpawnSubscriptions[SubscribableEvent.All].Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                return Conflict("Already subscribed to all events.");
            }

            eventSpawnSubscriptions[request.Event].Add(new EventLifecycleSubscription(request));
            return Ok($"Subscribed for {request.Event} event spawns!");
        }

        /// <summary>
        /// Subscribe a callback URL to the event death event.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Ok(200) if successful. Conflict(409) if already subscribed.</returns>
        [HttpPost]
        [Route("EventDeathSubscribe")]
        public IActionResult SubscribeEventDeath([FromBody] EventDeathSubscriptionRequest request)
        {
            if (eventDeathSubscriptions[request.Event].Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                return Conflict("Already subscribed to this event.");
            }
            else if (eventDeathSubscriptions[SubscribableEvent.All].Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                return Conflict("Already subscribed to all events.");
            }

            eventDeathSubscriptions[request.Event].Add(new EventLifecycleSubscription(request));
            return Ok($"Subscribed for {request.Event} event deaths!");
        }

        /// <summary>
        /// Unsubscribe a callback URL from the event spawn event.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Ok(200) if successful. Conflict(409) if wasn't subscribed.</returns>
        [HttpPost]
        [Route("EventSpawnUnsubscribe")]
        public IActionResult UnsubscribeEventSpawn([FromBody] EventSpawnUnsubscriptionRequest request)
        {
            if (eventSpawnSubscriptions[request.Event].Where(x => x.CallbackUrl == request.CallbackUrl).Any())
            {
                eventSpawnSubscriptions[request.Event].Remove(new EventLifecycleSubscription(request));
                return Ok($"Unsubscribed for {request.Event} event spawns!");
            }

            return Conflict("Wasn't subscribed to this event.");
        }

        /// <summary>
        /// Call the event spawn event.
        /// </summary>
        /// <param name="eventSpawnRequest">Request.</param>
        /// <param name="auth">Authorization header, must match the bearer token as this is restricted.</param>
        /// <returns>Ok(200) if authorized. BadRequest(400) if not authorized.</returns>
        [HttpPut]
        [Route("EventSpawn")]
        public IActionResult EventSpawn([FromBody] EventSpawnRequest eventSpawnRequest, [FromHeader(Name = "Authorization")] string auth)
        {
            if (!BearerToken.ValidateAuthorizationHeader(auth))
            {
                return BadRequest("Invalid permissions.");
            }

            SubscriptionPublisher.PublishSubscription(
                eventSpawnSubscriptions[eventSpawnRequest.Event].Union(eventSpawnSubscriptions[SubscribableEvent.All]),
                eventSpawnRequest,
                (subscription) =>
                {
                    var eventLifecycleSubscription = subscription as EventLifecycleSubscription;
                    if (eventLifecycleSubscription != null)
                    {
                        eventSpawnSubscriptions[eventLifecycleSubscription.Event].Remove(eventLifecycleSubscription);
                    }
                });
            return Ok();
        }

        /// <summary>
        /// Call the event death event.
        /// </summary>
        /// <param name="eventDeathRequest">Request.</param>
        /// <param name="auth">Authorization header, must match the bearer token as this is restricted.</param>
        /// <returns>Ok(200) if authorized. BadRequest(400) if not authorized.</returns>
        [HttpPut]
        [Route("EventDeath")]
        public IActionResult EventDeath([FromBody] EventDeathRequest eventDeathRequest, [FromHeader(Name = "Authorization")] string auth)
        {
            if (!BearerToken.ValidateAuthorizationHeader(auth))
            {
                return BadRequest("Invalid permissions.");
            }

            SubscriptionPublisher.PublishSubscription(
                eventDeathSubscriptions[eventDeathRequest.Event].Union(eventDeathSubscriptions[SubscribableEvent.All]),
                eventDeathRequest,
                (subscription) =>
                {
                    var eventLifecycleSubscription = subscription as EventLifecycleSubscription;
                    if (eventLifecycleSubscription != null)
                    {
                        eventDeathSubscriptions[eventLifecycleSubscription.Event].Remove(eventLifecycleSubscription);
                    }
                });
            return Ok();
        }
    }
}