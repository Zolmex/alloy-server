#region

using System;

#endregion

namespace GameServer.Utilities
{
    public static class Easing
    {
        public static void EaseVal(Ease ease, ref float val)
        {
            switch (ease)
            {
                case Ease.InSine:
                    val = 1 - MathF.Cos(val * MathF.PI / 2);
                    break;
                case Ease.OutSine:
                    val = MathF.Sin(val * MathF.PI / 2);
                    break;
                case Ease.InCubic:
                    val = val * val * val;
                    break;
                case Ease.OutCubic:
                    val = 1 - MathF.Pow(1 - val, 3);
                    break;
                case Ease.InOutBounce:
                    val = val < 0.5
                        ? (1 - _easeOutBounce(1 - (2 * val))) / 2
                        : (1 + _easeOutBounce((2 * val) - 1)) / 2;
                    break;
            }
        }

        private const float n1 = 7.5625f;
        private const float d1 = 2.75f;

        private static float _easeOutBounce(float x)
        {
            if (x < 1f / d1)
                return n1 * x * x;
            else if (x < 2f / d1)
                return (n1 * (x -= 1.5f / d1) * x) + 0.75f;
            else if (x < 2.5f / d1)
                return (n1 * (x -= 2.25f / d1) * x) + 0.9375f;
            else
                return (n1 * (x -= 2.625f / d1) * x) + 0.984375f;
        }
    }

    public enum Ease
    {
        None,
        InSine,
        OutSine,
        InCubic,
        OutCubic,
        InOutBounce
    }
}