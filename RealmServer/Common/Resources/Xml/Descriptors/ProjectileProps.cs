using Common.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Common.Resources.Xml.Descriptors;

public class ProjectileProps
{
    public readonly XElement Root;
    public readonly byte BulletId;
    public readonly string ObjectId;
    public readonly int LifetimeMS;
    public readonly float Speed;
    public readonly int Damage;
    public readonly int MinDamage;
    public readonly int MaxDamage;
    public readonly (ConditionEffectIndex, int)[] Effects;
    public readonly bool MultiHit;
    public readonly bool PassesCover;
    public readonly bool ArmorPiercing;
    public readonly bool Wavy;
    public readonly bool Parametric;
    public readonly bool Boomerang;
    public readonly float Amplitude;
    public readonly float Frequency;
    public readonly float Magnitude;
    public readonly int Size;
    public readonly XElement PathXML;
    public readonly PathType PathType;

    public ushort ObjectType; // To be set by Shoot behavior

    public ProjectileProps(XElement e)
    {
        Root = e;
        BulletId = (byte)e.GetAttribute<int>("id");
        ObjectId = e.GetValue<string>("ObjectId");
        LifetimeMS = (int)e.GetValue<float>("LifetimeMS");
        Speed = e.GetValue<float>("Speed") / 10;
        Damage = e.GetValue<int>("Damage");
        MinDamage = e.GetValue<int>("MinDamage", Damage);
        MaxDamage = e.GetValue<int>("MaxDamage", Damage);

        Effects = e.Elements("ConditionEffect")
            .Select(i => ((ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), i.Value.Replace(" ", "")),
                (int)(i.GetAttribute<float>("duration") * 1000)))
            .ToArray();

        MultiHit = e.HasElement("MultiHit");
        PassesCover = e.HasElement("PassesCover");
        ArmorPiercing = e.HasElement("ArmorPiercing");
        Size = e.GetValue<int>("Size", 0);
        Wavy = e.HasElement("Wavy");
        Parametric = e.HasElement("Parametric");
        Boomerang = e.HasElement("Boomerang");

        Amplitude = e.GetValue<float>("Amplitude", 0);
        Frequency = e.GetValue<float>("Frequency", 1);
        Magnitude = e.GetValue<float>("Magnitude", 3);

        if (e.Element("Path") != null)
            PathXML = e.Element("Path");
        else
        {
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

    public ProjectileProps(byte bulletId, string objectId, int lifetimeMs, float speed, int damage = -1, int minDamage = -1,
        int maxDamage = -1, (ConditionEffectIndex, int)[] effects = null, bool multiHit = false, bool passesCover = false, bool armorPiercing = false,
        bool wavy = false, bool parametric = false, bool boomerang = false, float amplitude = 0, float frequency = 1, float magnitude = 3, int size = 100)
    {
        BulletId = bulletId;
        ObjectId = objectId;
        LifetimeMS = lifetimeMs;
        Speed = speed;
        Damage = damage;
        if (damage == -1)
        {
            MinDamage = minDamage;
            MaxDamage = maxDamage;
        }
        else
        {
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
}