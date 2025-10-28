#region

using Common;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Numerics;

#endregion

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        private Ability _ability;

        private Ability ResolveAbilityController(string objId)
        {
            return objId switch
            {
                "Rogue" => new RogueCloakAbility(this),
                "Archer" => new ArcherQuiverAbility(this),
                "Wizard" => new WizardSpellAbility(this),
                // "Priest" => new PriestTomeController(this),
                "Warrior" => new WarriorHelmAbility(this),
                // "Knight" => new KnightShieldController(this),
                "Paladin" => new PaladinSealAbility(this),
                "Assassin" => new AssassinPoisonAbility(this),
                // "Necromancer" => new NecroSkullController(this),
                // "Huntress" => new HuntressTrapController(this),
                // "Mystic" => new MysticOrbController(this),
                // "Trickster" => new TricksterPrismController(this),
                // "Sorcerer" => new SorcScepterController(this),
                "Ninja" => new NinjaSheathAbility(this),
                _ => null
            };
        }
        
        public void UseItem(SlotObjectData slot, WorldPosData usePos, int clientTime) // TODO: validate client time
        {
            var item = Inventory[slot.SlotId];
            if (!ValidateUseItem(item, slot, usePos))
            {
                _log.Debug("test");
                InvResult.Write(User.Network, 1);
                return;
            }

            if (!item.Usable) // Random inventory items
            {
                var result = HandleActivateEffect(item, usePos);
                Inventory[slot.SlotId] = result;
            }
            else // Ability slot item
                _ability?.Use(item, usePos, clientTime);

            InvResult.Write(User.Network, 0);
        }

        private bool ValidateUseItem(Item item, SlotObjectData slot, WorldPosData usePos)
        {
            if (item == null || this.DistSqr(usePos.X, usePos.Y) > SIGHT_RADIUS_SQR)
                return false;

            if (!World.Entities.TryGetValue(slot.ObjectId, out var en))
                return false;

            if ((en is Container container && container.OwnerId != Id) || (en is Player plr && plr.Id != Id))
                return false;

            if (item.Usable && !(_ability?.Validate(item, en) ?? false))
                return false;

            return true;
        }

        public Item HandleActivateEffect(Item item, WorldPosData usePos)
        {
            var ret = item;

            if (item.Consumable)
                ret = null;

            foreach (var ae in item.ActivateEffects)
                switch (ae.AEIndex)
                {
                    case ActivateEffectIndex.Heal:
                        AEHeal(ae);
                        break;
                    case ActivateEffectIndex.Create:
                        ret = AECreate(item, ae);
                        break;
                    case ActivateEffectIndex.ConditionEffectAura:
                        AECondEffAura(ae);
                        break;
                    case ActivateEffectIndex.ConditionEffectSelf:
                        AECondEffSelf(ae);
                        break;
                    case ActivateEffectIndex.Shoot:
                        AEShoot(item, ae, usePos);
                        break;
                    case ActivateEffectIndex.HealNova:
                        AEHealNova(ae);
                        break;
                    default:
                        ret = item;
                        SendError($"Activate Effect not supported: {ae.AEIndex}");
                        break;
                }

            return ret;
        }

        private void AEHeal(ActivateEffectDesc ae)
        {
            Heal(ae.Amount);
        }

        private void AEHealNova(ActivateEffectDesc ae)
        {
            foreach (var plr in World.GetAllPlayersWithin(Position.X, Position.Y, ae.Range))
                plr.Heal(ae.Amount);

            ShowEffect.Write(User.Network,
                (byte)ShowEffectIndex.Nova,
                Id,
                0xFFFFFF,
                ae.Range,
                new WorldPosData(Position.X, Position.Y),
                new WorldPosData());
        }

        private Item AECreate(Item item, ActivateEffectDesc ae)
        {
            var objId = ae.ObjectId;
            var objDesc = XmlLibrary.Id2Object(objId);
            if (string.IsNullOrWhiteSpace(objId) || objDesc == null)
                return item;

            if (objDesc.Class == "Portal")
            {
                var en = Resolve(objDesc.ObjectType);
                en.Move(Position.X, Position.Y);
                en.EnterWorld(World);
            }

            return null;
        }

        private void AECondEffAura(ActivateEffectDesc ae)
        {
            var color = 0xFFFFFFFF;

            if (ae.EffectDesc.Effect == ConditionEffectIndex.Damaging)
                color = 0xFFFF0000;

            if (ae.Color != 0)
                color = ae.Color;

            foreach (var plr in World.GetAllPlayersWithin(Position.X, Position.Y, ae.Range))
            {
                plr.ApplyConditionEffect(ae.EffectDesc.Effect, ae.DurationMS);
                ShowEffect.Write(plr.User.Network,
                    (byte)ShowEffectIndex.Nova,
                    Id,
                    (int)color,
                    ae.Range,
                    new WorldPosData(Position.X, Position.Y),
                    new WorldPosData());
            }
        }

        private void AECondEffSelf(ActivateEffectDesc ae)
        {
            var color = 0xFFFFFF;

            if (ae.EffectDesc.Effect == ConditionEffectIndex.Damaging)
                color = 0xFF0000;

            if (ae.Color != 0)
                color = (int)ae.Color;

            ApplyConditionEffect(ae.EffectDesc.Effect, ae.DurationMS);
            ShowEffect.Write(User.Network,
                (byte)ShowEffectIndex.Nova,
                Id,
                (int)color,
                1,
                new WorldPosData(Position.X, Position.Y),
                new WorldPosData());
        }

        private void AEShoot(Item item, ActivateEffectDesc ae, WorldPosData usePos)
        {
            var angleInc = (float)(item.ArcGap * Math.PI / 180);
            var startAngle = (float)Math.Atan2(usePos.Y - Position.Y, usePos.X - Position.X) -
                             ((int)((item.NumProjectiles - 1) / 2f) * angleInc);

            ServerShoot(item.Projectiles[0], item.NumProjectiles, startAngle.Rad2Deg(), angleInc.Rad2Deg(),
                new Vector2(Position.X, Position.Y), item.ObjectType);
        }
    }
}