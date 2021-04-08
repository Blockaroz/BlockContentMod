using Microsoft.Xna.Framework;

namespace BlockContentMod
{
    public static class ExtendedColor
    {
        public static readonly Color ShadeColor = new Color(6, 7, 12);

        public static readonly Color LightRed = Color.Lerp(Color.Red, Color.Coral, 0.36f);

        public static readonly Color JellyOrange = new Color(255, 90, 30);

        public static readonly Color JellyRed = new Color(186, 48, 40);

        public static readonly Color RiftMagenta = new Color(229, 141, 211);

        public static readonly Color ZenithGreen = new Color(178, 255, 180);
    }
}