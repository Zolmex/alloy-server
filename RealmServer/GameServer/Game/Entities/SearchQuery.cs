#region

using Common;
using GameServer.Game.Entities;
using System.Collections.Generic;
using GameServer.Game.Entities.Types;

#endregion

namespace GameServer.Game;

public readonly record struct SearchQuery(string EntityName, IntPoint Pos, float MaxRadius, float MinRadius);

public readonly record struct SearchQueryResult(IEnumerable<CharacterEntity> Entities, CharacterEntity NearestEntity);