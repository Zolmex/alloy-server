#region

using System.Collections.Generic;
using Common.Structs;
using GameServer.Game.Entities.Types;

#endregion

namespace GameServer.Game;

public readonly record struct SearchQuery(string EntityName, IntPoint Pos, float MaxRadius, float MinRadius);

public readonly record struct SearchQueryResult(IEnumerable<CharacterEntity> Entities, CharacterEntity NearestEntity);