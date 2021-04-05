using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace BlockContentMod.Effects
{
    public struct JellyfishTentacleHelper
    {
        public void DrawTentacle(SpriteBatch spriteBatch, int segmentCount, Vector2 startPoint, Vector2 midPoint, Vector2 endPoint, float rotation)
        {
            List<Vector2> points = ExtendedUtils.QuadraticBezierPoints(startPoint, midPoint, endPoint, segmentCount);
            Vector2 position = points[0];

            Asset<Texture2D> texF = TextureAssets.FishingLine;
            Vector2 originF = new Vector2(texF.Width() / 2, 2f);
            for (int i = 2; i < segmentCount + 1; i++)
            {
                Vector2 pointDifference = points[i + 1] - points[i];
                float pointRotation = pointDifference.ToRotation() - MathHelper.PiOver2;

                Vector2 scale;
                Color color;
                Rectangle srcRectangle;
                Vector2 origin;

                if (i == 15)
                {
                    scale = new Vector2(2f, 1f);
                    color = Color.White;
                    srcRectangle = texF.Frame(1, 3, 3);
                }
                else if (i == 14)
                {
                    scale = new Vector2(2f, 1f);
                    color = ExtendedColor.JellyOrange;
                    srcRectangle = texF.Frame(1, 3, 2);
                }
                else
                {
                    scale = new Vector2(1, 0.5f + (pointDifference.Length() / 16));
                    color = ExtendedColor.ShadeColor;
                    srcRectangle = texF.Frame(1, 3, 1);
                }

                spriteBatch.Draw(texF.Value, position - Main.screenPosition, texF.Frame(), color, pointRotation, originF, scale, SpriteEffects.None, 0);
                position += pointDifference;
            }
        }

        public void DrawLowerHalf(SpriteBatch spriteBatch, NPC npc, Vector2 wobbleVector, float scale)
        {
            int points = 5;
            for (int i = 0; i < points; i += 1)
            {
                float wave = wobbleVector.X;
                float wave2 = wobbleVector.Y;

                Vector2 endCenterPoint = npc.Center + (new Vector2(0, 48 + (wave * npc.direction))).RotatedBy(npc.rotation);

                float angleRadius = (90f + (wave * 72f)) * scale;
                float distance = 60f * scale;
                Vector2 segmentEnd = endCenterPoint + new Vector2(0f, distance)
                    .RotatedBy(MathHelper.ToRadians((angleRadius / points * i) - (angleRadius / points * 2)) + npc.rotation);

                Vector2 centralControlPoint = npc.Center + (new Vector2(0, 70 + (wave2 * 30))).RotatedBy(npc.rotation) * scale;
                float segmentLerp = 12 - (i * 6);
                Vector2 segmentStart = npc.Center + (new Vector2(segmentLerp, 54) * scale).RotatedBy(npc.rotation);

                DrawTentacle(spriteBatch, 15, segmentStart, centralControlPoint, segmentEnd, npc.rotation);
            }
        }
    }
}