#region

using Common;
using GameServer.Game.Entities;
using System.Collections.Generic;

#endregion

namespace GameServer.Game
{
    public readonly record struct SearchQuery(string EntityName, IntPoint Pos, float MaxRadius, float MinRadius);

    public readonly record struct SearchQueryResult(IEnumerable<Character> Entities, Character NearestEntity);
}