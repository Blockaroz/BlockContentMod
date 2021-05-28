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
    public class JellyfishMovingExplosionProj : ModProjectile
    {
        public override string Texture => "BlockContentMod/Content/Projectiles/JellyfishProjs/JellyfishExplosionProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

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
            if (Projectile.ai[0] >= 60)
            {
                Projectile.Kill();
            }

            if (Projectile.ai[0] >= 25)
            {
                Projectile.velocity *= 0.65f;
            }

            if (Projectile.ai[0] == 1)
                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.Center);

            if (Projectile.ai[0] == 51)
            {
                //resize method is private, so we do this
                Projectile.position = Projectile.Center;
                Projectile.width = 140;
                Projectile.height = 140;
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
                float glowScale = EUtils.GetSquareLerp(-5, 30, 50, Projectile.ai[0] - (i * 3f)) * 0.8f;
                EUtils.DrawStreak(glowBall, SpriteEffects.None, Projectile.Center - Main.screenPosition, glowBall.Size() / 2f, glowScale, 1.5f + (i / 2), 1.5f + (i / 2), 0, EColor.JellyfishOrange, Color.PaleGoldenrod, 0.2f);
            }
            if (Projectile.ai[0] <= 50)
            {
                float bubbleScale = EUtils.GetSquareLerp(1, 30, 1, Projectile.ai[0]);
                for (int i = 0; i < 5; i++)
                {
                    float strength = Terraria.Utils.GetLerpValue(10, 0, i, true);
                    Color trailColor = Color.Lerp(Color.Goldenrod, EColor.JellyfishOrange, strength) * strength;
                    trailColor.A /= 2;
                    Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + (Projectile.Size / 2f);
                    Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, pos, null, trailColor, 0, Projectile.Size / 2f, bubbleScale, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, drawColor, 0, Projectile.Size / 2f, bubbleScale, SpriteEffects.None, 0);
            }

            float explosionScale = EUtils.GetSquareLerp(45, 48, 60, Projectile.ai[0]) * 1.8f;
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