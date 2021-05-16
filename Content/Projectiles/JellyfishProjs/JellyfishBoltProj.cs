using BlockContentMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Projectiles.JellyfishProjs
{
    public class JellyfishBoltProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 720;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 720)
                SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot, Projectile.Center);

            if (Projectile.timeLeft <= 360)
            {
                Projectile.velocity *= 0.88f;
                Projectile.scale *= 0.97f;
            }

            if (Projectile.velocity.Length() < 0.01f)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Vector2 origin = new Vector2(11, 2);

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 15)
                Projectile.frameCounter = 0;

            float t = Projectile.velocity.X >= 0 ? Utils.GetLerpValue(0, 60, Projectile.frameCounter) * MathHelper.TwoPi : Utils.GetLerpValue(60, 0, Projectile.frameCounter) * MathHelper.TwoPi;

            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float strength = Utils.GetLerpValue(ProjectileID.Sets.TrailCacheLength[Type], 0, i, true);
                ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition, origin, strength, 1f, 2f, Projectile.oldRot[i], ExtendedColor.JellyOrange * 0.1f, ExtendedColor.JellyOrange * 0.7f);
            }

            for (int i = 0; i < 4; i++)
            {
                float length = Utils.GetLerpValue(720, 705, Projectile.timeLeft, true) * 2.2f;
                float rotation = t + (MathHelper.PiOver2 * i);
                Vector2 offset = new Vector2(0, 5).RotatedBy(rotation);
                ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.oldPos[1] + (Projectile.Size / 2f) - offset - Main.screenPosition, origin, 1.5f, 1f, length, Projectile.rotation, ExtendedColor.JellyOrange * 0.1f, ExtendedColor.JellyOrange * 0.7f);
            }

            ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.Center - Main.screenPosition, origin, 1f, 1f, 1f, Projectile.rotation, ExtendedColor.JellyOrange, Color.LightGoldenrodYellow);

            Lighting.AddLight(Projectile.Center, ExtendedColor.JellyOrange.ToVector3() * 0.3f);
            Dust dust = Main.dust[Dust.NewDust(Projectile.oldPos[1] + (Projectile.Size / 2f), 0, 0, ModContent.DustType<JellyExplosionDust>(), 0, 0, 128, Color.White, 1.3f)];
            dust.noGravity = true;

            return false;
        }
    }
}