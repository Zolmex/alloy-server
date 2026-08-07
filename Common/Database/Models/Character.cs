using System;
using System.Collections.Generic;

namespace Common.Database.Models;

public class Character {
    public int CharId { get; set; }
    public ushort ObjectType { get; set; }
    public int Level { get; set; }
    public int CurrentFame { get; set; }
    public int XpPoints { get; set; }
    public ushort SkinType { get; set; }
    public ushort TextureOne { get; set; }
    public ushort TextureTwo { get; set; }
    public ushort PetType { get; set; }
    public int HealthPotions { get; set; }
    public int MagicPotions { get; set; }
    public bool IsDead { get; set; }
    public bool IsDeleted { get; set; }
    public bool HasBackpack { get; set; }
    public DateTime CreatedAt { get; set; }
    public int[] ItemTypes { get; set; }
    public byte[] ItemDatas { get; set; }
    public CharacterStats Stats { get; set; }
    public CombatStats CombatStats { get; set; }
    public DungeonStats DungeonStats { get; set; }
    public ExplorationStats ExplorationStats { get; set; }
    public KillStats KillStats { get; set; }

}