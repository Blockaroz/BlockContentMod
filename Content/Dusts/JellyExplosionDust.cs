using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Dusts
{
    public class JellyExplosionDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, ExtendedColor.JellyOrange.ToVector3() * dust.scale * 0.6f);
            dust.velocity *= 1.08f;
            dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;
            return true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color result = Color.White;
            result.A /= 3;
            return result;
        }
    }
}