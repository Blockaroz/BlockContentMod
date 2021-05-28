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
    public class JellyfishExplosionProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.knockBack = 3f;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 70)
            {
                Projectile.Kill();
            }

            if (Projectile.ai[0] == 1)
                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.Center);

            if (Projectile.ai[0] == 61)
            {
                //resize method is private, so we do this
                Projectile.position = Projectile.Center;
                Projectile.width = 136;
                Projectile.height = 136;
                Projectile.Center = Projectile.position;

                Projectile.hostile = true;

                SoundEngine.PlaySound(SoundID.Item62, Projectile.position);
                Lighting.AddLight(Projectile.Center, EColor.JellyfishOrange.ToVector3());

                for (int i = 0; i < 25; i++)
                {
                    Vector2 speed = Vector2.UnitY.RotatedByRandom(EUtils.GetCircle(i, 25)) * Main.rand.Next(6, 18) * 0.33f;

                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<JellyfishBubbleDust>(), 0, 0, 0, Color.White, 1.5f)];
                    dust.noGravity = true;
                    dust.velocity = speed;
                    dust.noLightEmittence = true;

                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<JellyExplosionDust>(), 0, 0, 128, Color.White, 1.5f)];
                    dust2.noGravity = true;
                    dust2.velocity = speed * 2f;
                    dust2.velocity *= 1.4f;
                    dust2.velocity.Y *= 0.6f;
                }
            }

            Projectile others = Main.projectile[Type];
            if (Projectile.Colliding(Projectile.getRect(), others.getRect()))
                Projectile.velocity += Projectile.DirectionFrom(others.Center) * 2f;
            //no idea if this does what's intended

            float lightStrength = Terraria.Utils.GetLerpValue(0, 80, Projectile.ai[0]);
            Lighting.AddLight(Projectile.Center, EColor.JellyfishOrange.ToVector3() * lightStrength);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Collision.SolidCollision(Projectile.Center, Projectile.width, Projectile.height))
                Projectile.velocity = Vector2.Zero;

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Asset<Texture2D> glowBall = ModContent.GetTexture("BlockContentMod/Assets/GlowBall_" + (short)1);
            Color drawColor = Color.White;
            drawColor.A /= 2;

            for (int i = 0; i < 4; i++)
            {
                float glowScale = EUtils.GetSquareLerp(1, 32, 60, Projectile.ai[0] - (i * 3f)) * 1.5f;
                EUtils.DrawStreak(glowBall, SpriteEffects.None, Projectile.Center - Main.screenPosition, glowBall.Size() / 2f, glowScale, 1.5f + (i / 2), 1.5f + (i / 2), 0, EColor.JellyfishOrange, Color.PaleGoldenrod, 0.2f);
            }
            if (Projectile.ai[0] <= 60)
            {
                float bubbleScale = EUtils.GetSquareLerp(9, 50, 9, Projectile.ai[0]);
                spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White, 0, Projectile.Size / 2f, bubbleScale, SpriteEffects.None, 0);
            }

            float explosionScale = EUtils.GetSquareLerp(55, 58, 70, Projectile.ai[0]) * 1.8f;
            EUtils.DrawStreak(glowBall, SpriteEffects.None, Projectile.Center - Main.screenPosition, glowBall.Size() / 2f, explosionScale, 2f, 2f, 0f, EColor.JellyfishOrange, Color.PaleGoldenrod);

            if (Projectile.ai[0] < 60)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position - (Projectile.Size / 2), Projectile.width * 2, Projectile.height * 2, ModContent.DustType<JellyfishBubbleDust>(), 0, 0, 0, Color.White, 1f)];
                dust.noGravity = true;
                Vector2 speed = dust.position.DirectionTo(Projectile.Center) * 2f;
                dust.velocity = speed;
            }

            return false;
        }
    }
}