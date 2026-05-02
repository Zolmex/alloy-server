using System.Collections.Generic;
using Common.Game;
using Common.Game.Components;
using Common.Game.Systems;
using Common.Utilities.Collections;

namespace Common.Game;

public class EntityManager(int capacity) : ManagerBase<Entity>(capacity) {
    // Nothing
}