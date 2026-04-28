#region

using System.Collections.Generic;
using Common.Structs;
using GameServerOld.Game.Entities.Types;

#endregion

namespace GameServerOld.Game;

public readonly record struct SearchQuery(string EntityName, IntPoint Pos, float MaxRadius, float MinRadius);

public readonly record struct SearchQueryResult(IEnumerable<CharacterEntity> Entities, CharacterEntity NearestEntity);