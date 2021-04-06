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
            dust.scale *= 1.1f;
        }

        public override bool Update(Dust dust)
        {
            dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;
            dust.velocity *= 1.08f;

            Lighting.AddLight(dust.position, ExtendedColor.JellyRed.ToVector3() * dust.scale * 0.3f);

            return true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color result = Color.White;
            result.A /= 4;
            return result;
        }
    }
}