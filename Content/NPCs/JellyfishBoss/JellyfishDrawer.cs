using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BlockContentMod.Content.NPCs.JellyfishBoss
{
    public struct JellyfishDrawer
    {
        private void DrawSingleTentacle(int segmentCount, List<Vector2> points, float glowScale)
        {
            Vector2 position = points[0];

            Asset<Texture2D> tentacleTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBossTentacle");
            Asset<Texture2D> glowBall = ModContent.GetTexture("BlockContentMod/Assets/GlowBall_" + (short)1);

            for (int i = 1; i < segmentCount + 1; i++)
            {
                Vector2 pointDifference = points[i + 1] - points[i];
                float pointRotation = pointDifference.ToRotation() - MathHelper.PiOver2;

                Vector2 scale;
                Rectangle rectangle;
                Vector2 origin = new Vector2(9, 2);
                float length = 0.5f + (pointDifference.Length() / 8);

                if (i == segmentCount)
                {
                    scale = Vector2.One;
                    rectangle = new Rectangle(0, 40, 18, 22);
                    origin = new Vector2(9, 4);
                }
                else if (i == segmentCount - 1)
                {
                    scale = new Vector2(1, length);
                    rectangle = new Rectangle(0, 32, 18, 8);
                    origin = new Vector2(9, 4);
                }
                else if (i > segmentCount * 0.8f)
                {
                    scale = new Vector2(1, length);
                    rectangle = new Rectangle(0, 24, 18, 8);
                }
                else if (i > segmentCount * 0.65f)
                {
                    scale = new Vector2(1, length);
                    rectangle = new Rectangle(0, 16, 18, 8);
                    origin = new Vector2(9, 2);
                }
                else if (i > segmentCount * 0.4f)
                {
                    scale = new Vector2(1, length);
                    rectangle = new Rectangle(0, 8, 18, 8);
                }
                else
                {
                    scale = new Vector2(1, length);
                    rectangle = new Rectangle(0, 0, 18, 8);
                }

                Main.spriteBatch.Draw(tentacleTexture.Value, position - Main.screenPosition, rectangle, Color.White, pointRotation, origin, scale, SpriteEffects.None, 0);

                if (i == segmentCount)
                {
                    EUtils.DrawStreak(glowBall, SpriteEffects.None, position + new Vector2(0, 3).RotatedBy(pointRotation) - Main.screenPosition, glowBall.Size() / 2f, 0.75f, glowScale * 1.3f, glowScale, pointRotation, EColor.JellyfishRed * 0.1f, EColor.JellyfishOrange * 0.05f);
                    Lighting.AddLight(position, EColor.JellyfishRed.ToVector3() * 0.1f);
                }


                position += pointDifference;
            }
        }

        public void DrawMainTentacles(float counter, Vector2 center, float rotation, float scale)
        {
            const int points = 5;
            const int segmentCount = 24;
            for (int i = 0; i < points; i++)
            {
                float topMotion = (float)Math.Cos((counter * 2f) + 0.5f);
                float midMotion = (float)Math.Cos((counter * 2f));
                float endMotion = (float)Math.Sin((counter * 2f) + 0.3f);

                float topRadius = (10 + (topMotion * 10)) * scale;
                float midRadius = (120 + (midMotion * 120)) * scale;
                float endRadius = (90 + (endMotion * 90)) * scale;

                Vector2 startPoint = center;

                if (i == 0)
                    startPoint = center + (new Vector2(32, 20).RotatedBy(rotation) * scale);

                if (i == 1)
                    startPoint = center + (new Vector2(20, 32).RotatedBy(rotation) * scale);

                if (i == 2)
                    startPoint = center + (new Vector2(0, 36).RotatedBy(rotation) * scale);

                if (i == 3)
                    startPoint = center + (new Vector2(-20, 32).RotatedBy(rotation) * scale);

                if (i == 4)
                    startPoint = center + (new Vector2(-32, 20).RotatedBy(rotation) * scale);

                Vector2 topMid = startPoint + new Vector2(0, 96 + (topMotion * 8)).RotatedBy(MathHelper.ToRadians((topRadius / points * i) - (topRadius / points * 2)) + rotation);

                Vector2 bottomMid = topMid + new Vector2(0, 64 + (midMotion * 24))
                    .RotatedBy(MathHelper.ToRadians((midRadius / points * i) - (midRadius / points * 2)) + rotation);

                Vector2 endPoint = topMid + new Vector2(0, 96 + (endMotion * 16))
                   .RotatedBy(MathHelper.ToRadians((endRadius / points * i) - (endRadius / points * 2)) + rotation);


                List<Vector2> pointList = EUtils.CubicBezierPoints(startPoint, topMid, bottomMid, endPoint, segmentCount);

                DrawSingleTentacle(segmentCount, pointList, 1f + (endMotion * 0.5f)); 
            }
        }

        public void DrawSideTentacles(float counter, Vector2 center, float rotation, Vector2 scale)
        {

        }

        public void DrawCore(float counter, Vector2 center, float rotation, Vector2 scale)
        {
            Asset<Texture2D> coreTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBossCore");
            Asset<Texture2D> coreGlow = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBossCore_Glow");
            Asset<Texture2D> glowBall = ModContent.GetTexture("BlockContentMod/Assets/GlowBall_" + (short)1);

            Vector2 coreOrigin = coreGlow.Size() / 2;

            Color alphaZero = Color.White;
            alphaZero.A = 0;
            Color alphaHalf = Color.White;
            alphaHalf.A /= 2;

            DrawMainTentacles(counter, center, rotation, 1);

            Main.spriteBatch.Draw(coreTexture.Value, center - Main.screenPosition, null, Color.White, rotation, coreOrigin, scale, SpriteEffects.None, 0);
            Lighting.AddLight(center, EColor.JellyfishRed.ToVector3() * 0.2f);

            for (int i = 0; i < 6; i++)
            {
                float opacityWave = (float)(Math.Sin(counter * 2) + 1f) * 0.5f;
                Vector2 glowOffset = new Vector2(0, (opacityWave * 2f)).RotatedBy(EUtils.GetCircle(i, 6));
                Main.spriteBatch.Draw(coreGlow.Value, center + glowOffset - Main.screenPosition, null, alphaHalf * 0.3f, rotation, coreOrigin, scale, SpriteEffects.None, 0);
                if (i > 2)
                    EUtils.DrawStreak(glowBall, SpriteEffects.None, center + new Vector2(0, -12).RotatedBy(rotation) - Main.screenPosition, glowBall.Size() / 2f, 2f + (glowOffset.Y * 0.5f), 1.1f * scale.X, 1f * scale.Y, rotation, EColor.JellyfishRed * 0.2f, EColor.JellyfishOrange * 0.3f);
            }
        }

        public void DrawJellyfish(float counter, float tpCounter, float drawPhase, Vector2 center, float rotation)
        {
            Asset<Texture2D> bubbleTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBossBubble");
            Asset<Texture2D> bubbleGlow = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBossBubble_Glow");

            float wobbleStep = 0;
            float wobbleSpeed = MathHelper.Lerp(4, 8, wobbleStep);
            float waveX = (float)Math.Cos(counter * wobbleSpeed) * 0.5f;
            float waveY = (float)Math.Sin(counter * wobbleSpeed) * 0.5f;
            Vector2 wobble = new Vector2(1 + (waveX * 0.2f), 1 + (waveY * 0.2f));

            Vector2 bubbleOrigin = bubbleTexture.Size() / 2;

            Vector2 trueScale = Vector2.One;
            Vector2 wobbleScale = trueScale;
            Vector2 offsetVector = (Vector2.UnitY * trueScale * 24).RotatedBy(rotation);

            if (drawPhase == -1)//teleport
            {
                trueScale = Vector2.One * Utils.GetLerpValue(0f, 1f, tpCounter * 1.1f, true);
            }
            else if (drawPhase == -3)//more intense wobble
            {
                if (wobbleStep < 1)
                    wobbleStep += 0.2f;

                Vector2 wobble2 = new Vector2(1 + (waveX * 0.25f), 1 + (waveY * 0.25f));

                wobbleScale = wobble2 * trueScale;
            }
            else if (drawPhase == 0)//default
            {
                if (wobbleStep > 0)
                    wobbleStep -= 0.2f;
                    
                wobbleScale = wobble * trueScale;
            }

            if (wobbleStep < 0)
                wobbleStep = 0;

            if (wobbleStep > 1)
                wobbleStep = 1;

            Color alphaZero = Color.White;
            alphaZero.A = 0;
            Color alphaHalf = Color.White;
            alphaHalf.A /= 2;

            DrawCore(counter, center, rotation, trueScale);

            Main.spriteBatch.Draw(bubbleGlow.Value, center - offsetVector - Main.screenPosition, null, alphaZero, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(bubbleTexture.Value, center - offsetVector - Main.screenPosition, null, alphaHalf, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0);
        }
    }
}