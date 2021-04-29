using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BlockContentMod
{
    public class JellyfishMenu : ModMenu
    {
        private void DrawTentacle(SpriteBatch spriteBatch, int segmentCount, Vector2 startPoint, Vector2 midPoint, Vector2 endPoint, float glowScale)
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

                spriteBatch.Draw(tentacleTexture.Value, position, rectangle, Color.White, pointRotation, origin, scale, SpriteEffects.None, 0);

                if (i == 16)
                {
                    ExtendedUtils.DrawStreak(glowBall, SpriteEffects.None, position + new Vector2(0, 3).RotatedBy(pointRotation), glowBall.Size() / 2f, 0.5f, glowScale, glowScale, pointRotation, ExtendedColor.JellyRed * 0.15f, ExtendedColor.JellyOrange * 0.15f);
                }


                position += pointDifference;
            }
        }


        private void DrawLowerHalf(SpriteBatch spriteBatch, Vector2 center, float rotation, float scale, Vector2 wobbleVector)
        {
            int points = 5;
            for (int i = 0; i < points; i += 1)
            {
                float wave = wobbleVector.X;
                float wave2 = wobbleVector.Y;

                Vector2 endCenterPoint = center + (new Vector2(0, 50 + wave)).RotatedBy(rotation);

                float angleRadius = (90f + (wave * 72f)) * scale;
                float distance = 64f * scale;
                Vector2 segmentEnd = endCenterPoint + new Vector2(0f, distance)
                    .RotatedBy(MathHelper.ToRadians((angleRadius / points * i) - (angleRadius / points * 2)) + rotation);

                Vector2 centralControlPoint = center + (new Vector2(0, 74 + (wave2 * 30))).RotatedBy(rotation) * scale;
                float segmentLerp = 12 - (i * 6);
                Vector2 segmentStart = center + (new Vector2(segmentLerp, 34) * scale).RotatedBy(rotation);

                DrawTentacle(spriteBatch, 16, segmentStart, centralControlPoint, segmentEnd, 1 + (wobbleVector.Y * 0.3f));
            }
        }

        public override string DisplayName => "Jellyfish Logo";

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            Asset<Texture2D> glowBall = ModContent.GetTexture("BlockContentMod/Assets/GlowBall_" + (short)1);
            Asset<Texture2D> bubbleTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBubble");
            Asset<Texture2D> bubbleGlow = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBubble_Glow");
            Asset<Texture2D> coreTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishCore");
            Asset<Texture2D> coreGlow = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishCore_Glow");

            float wobbleReal = Main.GlobalTimeWrappedHourly * MathHelper.Pi;
            float waveX = (float)Math.Cos(wobbleReal);
            float waveY = (float)Math.Sin(wobbleReal);
            Vector2 wobble = new Vector2(1 + (waveX * 0.2f), 1 + (waveY * 0.2f));

            float rotation = MathHelper.PiOver4;

            Vector2 bubbleOrigin = bubbleTexture.Size() / 2;
            Vector2 coreOrigin = coreGlow.Size() / 2;

            Vector2 trueScale = Vector2.One;
            Vector2 wobbleScale = wobble * trueScale;
            Vector2 tentacleWobble = new Vector2(waveX, waveY) * trueScale;
            Vector2 offsetVector = (Vector2.UnitY * trueScale * 20).RotatedBy(rotation);

            logoDrawCenter = new Vector2(Main.screenWidth / 5f + (tentacleWobble.X * 8), Main.screenHeight / 2.2f + (tentacleWobble.Y * 8));

            Color color = Color.White;
            color.A = 10;
            Color color2 = Color.White;
            color2.A = 40;
           
            spriteBatch.Draw(coreTexture.Value, logoDrawCenter, null, Color.White, rotation, coreOrigin, trueScale, SpriteEffects.None, 0f);

            DrawLowerHalf(spriteBatch, logoDrawCenter, MathHelper.PiOver4, trueScale.Y, tentacleWobble);

            spriteBatch.Draw(bubbleGlow.Value, logoDrawCenter - offsetVector, null, color2, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0f);

            for (int i = 0; i < 2; i++)
            {
                float waveW = (float)Math.Cos((wobbleReal) + (i * MathHelper.PiOver4)) * 0.06f;
                Vector2 glowScale = (new Vector2(0.89f) + new Vector2(waveW)) * trueScale;
                spriteBatch.Draw(coreGlow.Value, logoDrawCenter, null, color2 * 0.5f, rotation, coreOrigin, glowScale, SpriteEffects.None, 0f);
            }

            float waveV = (float)Math.Cos((wobbleReal) + 1f) * 0.06f;
            Vector2 glowScale0 = (new Vector2(0.89f) + new Vector2(waveV)) * trueScale;
            ExtendedUtils.DrawStreak(glowBall, SpriteEffects.None, logoDrawCenter, glowBall.Size() / 2f, 1.5f, glowScale0.X, glowScale0.Y, rotation, ExtendedColor.JellyRed, ExtendedColor.JellyOrange * 0.7f);

            spriteBatch.Draw(bubbleTexture.Value, logoDrawCenter - offsetVector, null, Color.White, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
