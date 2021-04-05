using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Dusts
{
    public class BlackShadeDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 255;
            dust.scale *= 1.5f;
        }

        public override bool Update(Dust dust)
        {
            dust.rotation *= 0.8f;
            dust.velocity *= 0.99f;

            dust.alpha--;
            if (dust.alpha < 0)
                dust.active = false;

            return true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White;
        }
    }
}