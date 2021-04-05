using BlockContentMod.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Projectiles
{
    public class AestheticusBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.CountsAsHoming[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 14;
            Projectile.height = 28;
            Projectile.timeLeft = 300;
            Projectile.idStaticNPCHitCooldown = 4;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.tileCollide = true;
            Projectile.netUpdate = true;
        }

        private int DustType = DustID.AncientLight;
        private float MaxDistance = 800f;

        private Color ColorMethod(float shift = 1f, float speed = 1f, float H = 1f, float S = 1f, float L = 0.5f)
        {
            Color result = Main.hslToRgb((Main.GlobalTimeWrappedHourly + shift) * speed % H, S, L);
            result.A = 200;
            return result;
        }

        private void SpinDust(int dustCount, float size, bool randomized)
        {
            for (int i = 0; i < dustCount + 1; i++)
            {
                Vector2 circle;
                if (randomized == true)
                    circle = new Vector2(0, size).RotatedByRandom(ExtendedUtils.GetCircle(i, dustCount));
                else
                    circle = new Vector2(0, size).RotatedBy((MathHelper.TwoPi / dustCount) * i);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType, circle, 0, ColorMethod(speed: 2), 1.2f);
                dust.noGravity = true;
            }
        }

        private void Split(int frequency, Vector2 position, Vector2 velocity, float degree, float ai0)
        {
            for (int i = 0; i < frequency; i++)
            {
                Vector2 velocityB = velocity.RotatedByRandom(MathHelper.ToRadians(degree));
                velocityB.Normalize();
                velocityB *= 16f + 2 * i;

                Projectile.NewProjectileDirect(Projectile.GetProjectileSource_FromThis(), position, velocityB, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai0);
            }
        }

        private void HomeIn(NPC Target, float speed = 20f)
        {
            Vector2 v = Target.Center - Projectile.Center;
            float d = Projectile.Distance(Target.Center);
            float lerp1 = Terraria.Utils.GetLerpValue(0f, 100f, d, clamped: true) * Terraria.Utils.GetLerpValue(600f, 400f, d, clamped: true);
            float amount = MathHelper.Lerp(0f, 0.3f, Terraria.Utils.GetLerpValue(500f, 10f, 1f - lerp1, clamped: true));

            Vector2 v2 = v.SafeNormalize(Vector2.Zero);
            float scaleFactor = Math.Min(speed, v.Length());
            Vector2 v3 = v2 * scaleFactor;
            if (Projectile.velocity.Length() < 8f)
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.TwoPi).SafeNormalize(Vector2.Zero) * 8f;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, v3, amount / 2.4f);
        }

        public override bool PreAI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 10)
                Projectile.ai[1] = 10;

            bool canHome;

            if (Projectile.ai[0] == 1 || Projectile.ai[1] >= 10)
                canHome = true;
            else
                canHome = false;

            Player player = Main.player[Projectile.owner];
            if (player.HasMinionAttackTargetNPC)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                bool distanceCheck = Vector2.Distance(Projectile.Center, Target.Center) < MaxDistance;
                if (Target.CanBeChasedBy() && distanceCheck && canHome)
                {
                    HomeIn(Target);
                }
            }
            else for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC Target = Main.npc[n];
                    bool distanceCheck = Vector2.Distance(Projectile.Center, Target.Center) < MaxDistance;
                    if (Target.CanBeChasedBy() && distanceCheck && canHome)
                    {
                        HomeIn(Target);
                    }
                }

            Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.velocity.ToRotation() + MathHelper.ToRadians(90), (float)Math.PI / 4f);

            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[0] == 0)
            {
                Split(4, Projectile.Center, Projectile.velocity, 30, 1);
            }

            SpinDust(10, 5, true);

            target.AddBuff(BuffID.Slow, 300);
            target.AddBuff(ModContent.BuffType<VaporizingDebuff>(), 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 bounceVector = Projectile.velocity;
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                bounceVector.X = -Projectile.oldVelocity.X;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                bounceVector.Y = -Projectile.oldVelocity.Y;

            if (Projectile.ai[0] == 0)
            {
                Split(4, Projectile.Center, bounceVector, 50, 1);
            }

            SpinDust(10, 4, false);
            return Projectile.tileCollide;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, ColorMethod(speed: 0.5f).ToVector3() * 0.25f);
            for (int i = 0; i < 7; i++)
            {
                Vector2 xFactor = Projectile.Center - Projectile.velocity * i / 6;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType, Vector2.Zero, 255, ColorMethod(speed: 1.77f, L: 0.6f), 0.7f);
                dust.noGravity = true;
                dust.position = xFactor;
            }
            Projectile.localAI[0] = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 4.5f) * 0.05f;
            Projectile.localAI[1] = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 4.5f) * 0.05f;

            Asset<Texture2D> texture = TextureAssets.Extra[98];
            ExtendedUtils.DrawSparkle(texture, SpriteEffects.None, Projectile.Center - Main.screenPosition, texture.Size() / 2f, 0.5f + Projectile.localAI[0], 0.5f, 1.5f + Projectile.localAI[1], 1.5f + Projectile.localAI[1], 0f, ColorMethod(speed: 1.77f, L: 0.6f), Color.White, 1f);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
    }
}