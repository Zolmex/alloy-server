#region

using System.Collections.Generic;

#endregion

namespace Common.Enums
{
    /// <summary>
    /// Class for mapping the internal entity name to the <see cref="SubscribableEvent"/> enum.
    /// </summary>
    public static class SubscribableEventMapping
    {
        /// <summary>
        /// Maps the internal entity name to the <see cref="SubscribableEvent"/> enum.
        /// </summary>
        public static Dictionary<string, SubscribableEvent> NameToSubscribableEvent = new()
        {
            { "Avatar of the Forgotten King", SubscribableEvent.Avatar },
            { "Lord of the Lost Lands", SubscribableEvent.LordoftheLostLands },
            { "Grand Sphinx", SubscribableEvent.GrandSphinx },
            { "Skull Shrine", SubscribableEvent.SkullShrine },
            { "Cube God", SubscribableEvent.CubeGod },
            { "Pentaract", SubscribableEvent.Pentaract },
            { "Hermit God", SubscribableEvent.HermitGod },
            { "Ghost Ship Whirlpool", SubscribableEvent.GhostShip },
            { "EH Event Hive Summoner", SubscribableEvent.EpicHive },
            { "Rock Dragon", SubscribableEvent.RockDragon },
            { "Lost Sentry", SubscribableEvent.LostSentry },
            { "Temple Encounter", SubscribableEvent.MountainTemple }
        };

        /// <summary>
        /// Is a name a subscribable event.
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <returns>Whether or not its a subscribable event.</returns>
        public static bool IsSubscribableEvent(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            return NameToSubscribableEvent.ContainsKey(name);
        }
    }
}