using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Rarities
{
    public class DarkRedRarity : ModRarity
    {
        private Color ColorMethod()
        {
            float t = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 0.21f));
            float lerp = EUtils.GetSquareLerp(0.2f, 0.35f, 0.5f, t);
            Color result = Color.Lerp(EColor.LightRed, EColor.ShadeColor, lerp);
            return result;
        }

        public override Color RarityColor => ColorMethod();
    }
}