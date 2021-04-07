using BlockContentMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Projectiles.JellyfishProjs
{
    public class JellyfishLightningProj : ModProjectile
    {
        public override string Texture => "BlockContentMod/Content/Projectiles/JellyfishProjs/JellyfishBoltProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 80;
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
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 901;
        }

        private float random = 1f;

        public override void AI()
        {
            if (Projectile.timeLeft <= 120)
                Projectile.velocity = Vector2.Zero;

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] >= Main.rand.Next(3, 36) && Projectile.velocity != Vector2.Zero)
            {
                if (random != 0)
                    random *= -1f;
                Projectile.localAI[0] = 0;
                Projectile.velocity = Vector2.Zero;

                Projectile.velocity = Projectile.oldVelocity.RotatedBy(random);
                Projectile.rotation = Projectile.velocity.ToRotation();
                return;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Collision.SolidCollision(Projectile.Center, Projectile.width, Projectile.height))
                Projectile.velocity = Vector2.Zero;

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < 55; i++)
            {
                if (targetHitbox.Contains(Projectile.oldPos[i].ToPoint()))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.GetTexture("BlockContentMod/Assets/Streak_" + (short)1);

            Color alphaOrange = ExtendedColor.JellyOrange;
            alphaOrange.A = 0;

            ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.Center - Main.screenPosition, texture.Size() / 2f, Projectile.scale * 0.3f, 1f, 1.2f, Projectile.rotation, alphaOrange, Color.White);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float strength = Projectile.scale * 0.3f * MathHelper.Lerp(0.5f, 1.5f, Terraria.Utils.GetLerpValue(60, 30, i, true));
                float strength2 = Projectile.scale * 0.3f * MathHelper.Lerp(1f, 2f, Terraria.Utils.GetLerpValue(60, 1, i, true));
                float length = Projectile.scale * MathHelper.Lerp(1.5f, 1f, Terraria.Utils.GetLerpValue(60, 1, i, true));
                Color lerpColor = Color.Lerp(Color.Goldenrod, Color.White, strength);

                ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition, texture.Size() / 2f, strength2, 0.9f, length, Projectile.oldRot[i], alphaOrange, lerpColor);

                Lighting.AddLight(Projectile.Center, ExtendedColor.JellyOrange.ToVector3() * 0.3f * strength);
            }

            if (Projectile.timeLeft >= 130)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<JellyExplosionDust>(), Projectile.velocity.X, Projectile.velocity.Y, 128, Color.White, 1.3f)];
                dust.noGravity = true;
            }
        }
    }
}