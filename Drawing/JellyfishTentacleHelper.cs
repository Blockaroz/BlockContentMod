using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BlockContentMod.Drawing
{
    public struct JellyfishTentacleHelper
    {
        public void DrawTentacle(SpriteBatch spriteBatch, int segmentCount, Vector2 startPoint, Vector2 midPoint, Vector2 endPoint, float glowScale)
        {
            List<Vector2> points = ExtendedUtils.QuadraticBezierPoints(startPoint, midPoint, endPoint, segmentCount);
            Vector2 position = points[0];

            Asset<Texture2D> tentacleTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishTentacle");
            Asset<Texture2D> glowBall = ModContent.GetTexture("BlockContentMod/Assets/GlowBall_" + (short)1);

            for (int i = 2; i < segmentCount + 1; i++)
            {
                Vector2 pointDifference = points[i + 1] - points[i];
                float pointRotation = pointDifference.ToRotation() - MathHelper.PiOver2;

                Vector2 scale;
                Rectangle rectangle;
                Vector2 origin;

                if (i == 16)//head
                {
                    scale = Vector2.One;
                    rectangle = new Rectangle(0, 52, 18, 18);
                    origin = new Vector2(9, 6);
                }
                else if (i > 12)
                {
                    scale = new Vector2(1, 0.5f + (pointDifference.Length() / 8));
                    rectangle = new Rectangle(0, 36, 18, 18);
                    origin = new Vector2(9, 6);
                }
                else if (i > 10)
                {
                    scale = new Vector2(1, 0.5f + (pointDifference.Length() / 8));
                    rectangle = new Rectangle(0, 24, 18, 18);
                    origin = new Vector2(9, 6);
                }
                else if (i > 7)
                {
                    scale = new Vector2(1, 0.5f + (pointDifference.Length() / 8));
                    rectangle = new Rectangle(0, 12, 18, 18);
                    origin = new Vector2(9, 6);
                }
                else
                {
                    scale = new Vector2(1, 0.5f + (pointDifference.Length() / 8));
                    rectangle = new Rectangle(0, 0, 18, 18);
                    origin = new Vector2(9, 6);
                }

                spriteBatch.Draw(tentacleTexture.Value, position - Main.screenPosition, rectangle, Color.White, pointRotation, origin, scale, SpriteEffects.None, 0);

                if (i == 16)
                {
                    ExtendedUtils.DrawStreak(glowBall, SpriteEffects.None, position + new Vector2(0, 3).RotatedBy(pointRotation) - Main.screenPosition, glowBall.Size() / 2f, 0.5f, glowScale, glowScale, pointRotation, ExtendedColor.JellyRed * 0.2f, ExtendedColor.JellyOrange * 0.2f);
                    Lighting.AddLight(position, ExtendedColor.JellyRed.ToVector3() * 0.1f);
                }


                position += pointDifference;
            }
        }


        public void DrawLowerHalfTentacles(SpriteBatch spriteBatch, Vector2 center, float rotation, float scale, Vector2 wobbleVector)
        {
            int points = 5;
            for (int i = 0; i < points; i += 1)
            {
                float wave = wobbleVector.X;
                float wave2 = wobbleVector.Y;

                Vector2 endCenterPoint = center + (new Vector2(0, 50 + wave)).RotatedBy(rotation);

                float angleRadius = (100f + (wave * 80f)) * scale;
                float distance = 64f * scale;
                Vector2 segmentEnd = endCenterPoint + new Vector2(0f, distance)
                    .RotatedBy(MathHelper.ToRadians((angleRadius / points * i) - (angleRadius / points * 2)) + rotation);

                Vector2 centralControlPoint = center + (new Vector2(0, 70 + (wave2 * 36))).RotatedBy(rotation) * scale;
                float segmentLerp = 12 - (i * 6);
                Vector2 segmentStart = center + (new Vector2(segmentLerp, 34) * scale).RotatedBy(rotation);

                DrawTentacle(spriteBatch, 16, segmentStart, centralControlPoint, segmentEnd, 1 + (wobbleVector.Y * 0.3f));
            }
        }
    }
}