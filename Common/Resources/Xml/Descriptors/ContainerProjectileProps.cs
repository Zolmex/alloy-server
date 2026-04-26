using System;
using System.Linq;
using System.Xml.Linq;
using Common.Utilities;

namespace Common.Resources.Xml.Descriptors;

public class ContainerProjectileProps {
    public readonly byte ProjectileIndex;
    public readonly ProjectileProps Props;

    public ContainerProjectileProps(XElement e) {
        ProjectileIndex = e.GetAttribute<byte>("id");
        Props = new ProjectileProps(e);
    }

    public ContainerProjectileProps(byte projectileIndex, ProjectileProps props) {
        ProjectileIndex = projectileIndex;
        Props = props;
    }
}

public record ProjectileProps {
    public readonly float Amplitude;
    public readonly bool ArmorPiercing;
    public readonly bool Boomerang;
    public readonly int Damage;
    public readonly (ConditionEffectIndex, int)[] Effects;
    public readonly float Frequency;
    public readonly int LifetimeMS;
    public readonly float Magnitude;
    public readonly int MaxDamage;
    public readonly int MinDamage;
    public readonly bool MultiHit;
    public readonly string ObjectId;
    public readonly bool Parametric;
    public readonly bool PassesCover;
    public readonly PathType PathType;
    public readonly int Size;
    public readonly float Speed;
    public readonly bool Wavy;

    public ProjectileProps(XElement e) {
        ObjectId = e.GetValue<string>("ObjectId");
        LifetimeMS = (int)e.GetValue<float>("LifetimeMS");
        Speed = e.GetValue<float>("Speed") / 10;
        Damage = e.GetValue<int>("Damage");
        MinDamage = e.GetValue("MinDamage", Damage);
        MaxDamage = e.GetValue("MaxDamage", Damage);

        Effects = e.Elements("ConditionEffect")
            .Select(i => ((ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), i.Value.Replace(" ", "")),
                (int)(i.GetAttribute<float>("duration") * 1000)))
            .ToArray();

        MultiHit = e.HasElement("MultiHit");
        PassesCover = e.HasElement("PassesCover");
        ArmorPiercing = e.HasElement("ArmorPiercing");
        Size = e.GetValue<int>("Size");
        Wavy = e.HasElement("Wavy");
        Parametric = e.HasElement("Parametric");
        Boomerang = e.HasElement("Boomerang");

        Amplitude = e.GetValue<float>("Amplitude");
        Frequency = e.GetValue<float>("Frequency", 1);
        Magnitude = e.GetValue<float>("Magnitude", 3);

        if (e.Element("Path") == null) {
            if (e.HasElement("Amplitude") || e.HasElement("Frequency"))
                PathType = PathType.AmplitudePath;
            // else if (Parametric)
            //     PathType = PathType.ParametricPath;
            else if (Wavy)
                PathType = PathType.WavyPath;
            else if (Boomerang)
                PathType = PathType.BoomerangPath;
            else PathType = PathType.LinePath;
        }
    }

    public ProjectileProps(string objectId, int lifetimeMs, float speed, int damage = -1, int minDamage = -1,
        int maxDamage = -1, (ConditionEffectIndex, int)[] effects = null, bool multiHit = false,
        bool passesCover = false, bool armorPiercing = false,
        bool wavy = false, bool parametric = false, bool boomerang = false, float amplitude = 0, float frequency = 1,
        float magnitude = 3, int size = 100) {
        ObjectId = objectId;
        LifetimeMS = lifetimeMs;
        Speed = speed;
        Damage = damage;
        if (damage == -1) {
            MinDamage = minDamage;
            MaxDamage = maxDamage;
        }
        else {
            MinDamage = damage;
            MaxDamage = damage;
        }

        Effects = effects;
        MultiHit = multiHit;
        PassesCover = passesCover;
        ArmorPiercing = armorPiercing;
        Wavy = wavy;
        Parametric = parametric;
        Boomerang = boomerang;
        Amplitude = amplitude;
        Frequency = frequency;
        Magnitude = magnitude;
        Size = size;
        if (Amplitude != 0 || Frequency != 1)
            PathType = PathType.AmplitudePath;
        // else if (Parametric)
        //     PathType = PathType.ParametricPath;
        else if (Wavy)
            PathType = PathType.WavyPath;
        else if (Boomerang)
            PathType = PathType.BoomerangPath;
        else PathType = PathType.LinePath;
    }

    public virtual bool Equals(ProjectileProps other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Amplitude == other.Amplitude
               && ArmorPiercing == other.ArmorPiercing
               && Boomerang == other.Boomerang
               && Damage == other.Damage
               && Frequency == other.Frequency
               && LifetimeMS == other.LifetimeMS
               && Magnitude == other.Magnitude
               && MaxDamage == other.MaxDamage
               && MinDamage == other.MinDamage
               && MultiHit == other.MultiHit
               && ObjectId == other.ObjectId
               && Parametric == other.Parametric
               && PassesCover == other.PassesCover
               && PathType == other.PathType
               && Size == other.Size
               && Speed == other.Speed
               && Wavy == other.Wavy
               && (Effects == other.Effects ||
                   (Effects != null && other.Effects != null && Effects.SequenceEqual(other.Effects)));
    }

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(Amplitude);
        hash.Add(ArmorPiercing);
        hash.Add(Boomerang);
        hash.Add(Damage);
        hash.Add(Frequency);
        hash.Add(LifetimeMS);
        hash.Add(Magnitude);
        hash.Add(MaxDamage);
        hash.Add(MinDamage);
        hash.Add(MultiHit);
        hash.Add(ObjectId);
        hash.Add(Parametric);
        hash.Add(PassesCover);
        hash.Add(PathType);
        hash.Add(Size);
        hash.Add(Speed);
        hash.Add(Wavy);

        // Fold each element into the hash so order matters
        if (Effects != null)
            foreach (var effect in Effects)
                hash.Add(effect);

        return hash.ToHashCode();
    }
}