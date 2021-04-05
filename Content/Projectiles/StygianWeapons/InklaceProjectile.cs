using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Projectiles.StygianWeapons
{
    public class InklaceProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            ProjectileID.Sets.IsAWhip[Projectile.type] = true;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 165;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Asset<Texture2D> tex = TextureAssets.FishingLine;
            List<Vector2> points = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, points);
            Vector2 position = points[0];
            Vector2 origin = new Vector2(tex.Width() / 2, 2f);
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 pointDifference = points[i + 1] - points[i];
                float pointRotation = pointDifference.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(points[i].ToTileCoordinates(), ExtendedColor.ShadeColor);
                Vector2 scale = new Vector2(1, (pointDifference.Length() + 2f) / (float)tex.Frame().Height);
                spriteBatch.Draw(tex.Value, position - Main.screenPosition, tex.Frame(), color, pointRotation, origin, scale, SpriteEffects.None, 0);
                position += pointDifference;
            }
            position = Main.DrawWhip_WhipBland(Projectile, points);

            return false;
        }
    }
}