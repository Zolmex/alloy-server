namespace GameServer.Game.Entities.Loot
{
    public class CharacterLoot
    {
        public readonly ItemLoot[] Loots;

        public CharacterLoot(params ItemLoot[] loots)
        {
            Loots = loots;
        }
    }
}