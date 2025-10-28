#region

using Common.API.Helpers;
using Common.API.Models.SubscriptionModels;
using Common.API.Requests;

#endregion

namespace AstrumAPI
{
    /// <summary>
    /// Class for handling the publishing of subscriptions.
    /// </summary>
    public static class SubscriptionPublisher
    {
        /// <summary>
        /// Publish a set of subscriptions, and return any failed subscriptions.
        /// </summary>
        /// <param name="subscriptions">Subscription list to publish to.</param>
        /// <param name="request">Request to send out.</param>
        /// <returns>Colletion of failed subscriptions.</returns>
        public static async void PublishSubscription(
            IEnumerable<APISubscription> subscriptions,
            IAPIRequest request,
            Action<APISubscription> onFailedSend)
        {
            var tasks = subscriptions.Select(async subscription =>
            {
                try
                {
                    var response = await APIHelper.SendCallbackRequest(subscription.CallbackUrl, request);
                    var success = response != null && response.IsSuccessStatusCode;
                    return new { Subscription = subscription, Success = success };
                }
                catch
                {
                    return new { Subscription = subscription, Success = false };
                }
            });

            var results = await Task.WhenAll(tasks);
            var failedResults = results.Where(result => !result.Success).Select(x => x.Subscription);
            foreach (var failiure in failedResults)
            {
                onFailedSend(failiure);
            }
        }
    }
}