using BlockContentMod.Content.Dusts;
using BlockContentMod.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Projectiles.StygianWeapons
{
    public class WarblerMissile : ModProjectile
    {
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 18;
            ProjectileID.Sets.CountsAsHoming[Type] = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 14;
            Projectile.height = 26;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 1;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.rand.Next(4) < 1)
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BlackShadeDust>(), null, 0, Color.White, 1f);

            bool isClose = Vector2.Distance(Main.MouseWorld, Projectile.Center) <= 36;
            if (Projectile.ai[0] == 0 && player.channel && !isClose)
            {
                Projectile.velocity += Projectile.DirectionTo(Main.MouseWorld);

                if (isClose)
                    Projectile.direction += Main.rand.Next();
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Main.npc[i];
                bool distanceCheck = Vector2.Distance(Projectile.Center, Target.Center) < 400;
                if (Target.CanBeChasedBy() && distanceCheck)
                {
                    Projectile.velocity += Projectile.DirectionTo(Target.Center) * 2f;
                    Projectile.ai[0] = 1;
                }
            }
            if (Projectile.velocity.Length() > 9.5f)
                Projectile.velocity *= 0.97f;

            if (Projectile.velocity.Length() < 2f && Projectile.ai[1] < 241)
                Projectile.velocity *= 1.2f;

            if (!player.channel)
                Projectile.ai[0] = 1;

            Projectile.ai[1]++;
            if (Projectile.ai[1] > 240)
            {
                Projectile.velocity *= 0.94f;
                Projectile.scale *= 0.95f;
                Projectile.ai[0] = 1;
            }

            if (Projectile.velocity.Length() < 0.01f)
            {
                Projectile.Kill();
                return;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -Projectile.oldVelocity.X;

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -Projectile.oldVelocity.Y;

            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = new Vector2(3, 0).RotatedByRandom((MathHelper.PiOver2 / i) * 15);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BlackShadeDust>(), speed, 0, Color.White, 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            default(WarblerDrawer).Draw(Projectile);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Extra[98];
            ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.Center - Main.screenPosition, texture.Size() / 2f, Projectile.scale, 0.8f, 1.2f, Projectile.rotation, ExtendedColor.ShadeColor, Color.Black, 1, 250);
        }
    }
}