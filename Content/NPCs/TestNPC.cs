using BlockContentMod.Content.Dusts;
using BlockContentMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.NPCs
{
    public class TestNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("NPC");
            NPCID.Sets.PositiveNPCTypesExcludedFromDeathTally[NPC.type] = true;
            NPCID.Sets.TeleportationImmune[NPC.type] = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void SetDefaults()
        {
            Player player = Main.player[Main.myPlayer];
            NPC.width = 42;
            NPC.height = 38;
            NPC.aiStyle = 0;
            NPC.lifeMax = 100000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.spriteDirection = NPC.direction;
            NPC.defense = 1;
            NPC.damage = 0;
            NPC.knockBackResist = 0.4f;
            NPC.noGravity = true;
            NPC.chaseable = true;
            NPC.lifeRegen = 90000;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];

            if (NPC.ai[0] < 60)
                NPC.ai[0]++;

            if (NPC.ai[0] == 60)
            {
                NPC.ai[0] = 0;
            }

            if (NPC.life < NPC.lifeMax)
                NPC.life += NPC.lifeMax;

            float length = Vector2.Distance(Main.MouseWorld, NPC.Center);

            if (length < 25 && player.altFunctionUse == 2)
                NPC.active = false;

            if (!NPC.active)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<BlackShadeDust>(), 0, 0, 0, Color.White, 0.66f)];
                    dust.noGravity = true;
                    Vector2 DustSpeed = new Vector2(i + 1f, 0f).RotatedBy(dust.position.AngleTo(NPC.Center));
                    dust.velocity = DustSpeed;
                    dust.position += NPC.velocity;
                }
            }

            Movement(Main.MouseWorld, 2f);
        }

        public void Movement(Vector2 position, float speed = 1f)
        {
            if (Vector2.Distance(NPC.Center, position) > 32)
                NPC.velocity += NPC.DirectionTo(position) * speed;
            else
                SlowMovement();

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.9f;
        }

        public void SlowMovement()
        {
            NPC.velocity *= 0.975f;
            if (NPC.velocity.Length() < 0.005f)
                NPC.velocity = Vector2.Zero;
        }

        public override Color? GetAlpha(Color drawColor) => Color.White;

        public float wobbleCounter { get; set; }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Asset<Texture2D> glowBall = ModContent.GetTexture("BlockContentMod/Assets/GlowBall_" + (short)1);
            Asset<Texture2D> bubbleTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBubble");
            Asset<Texture2D> bubbleGlow = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBubble_Glow");
            Asset<Texture2D> coreTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishCore");
            Asset<Texture2D> coreGlow = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishCore_Glow");

            wobbleCounter++;
            float tick = 90;
            if (wobbleCounter > tick)
                wobbleCounter = 0;
            float wobbleReal = (wobbleCounter * MathHelper.PiOver2) / tick;

            float waveX = (float)Math.Cos(wobbleReal * 8f);
            float waveY = (float)Math.Sin(wobbleReal * 8f);
            Vector2 wobble = new Vector2(1 + (waveX * 0.2f), 1 + (waveY * 0.2f));

            float rotation = NPC.rotation;
            Vector2 bubbleOrigin = bubbleTexture.Size() / 2;
            Vector2 coreOrigin = coreGlow.Size() / 2;

            Vector2 trueScale = Vector2.One;
            Vector2 wobbleScale = wobble * trueScale;
            Vector2 tentacleWobble = new Vector2(waveX, waveY) * trueScale;

            Color color = Color.White;
            color.A = 10;
            Color color2 = Color.White;
            color2.A = 60;

            float waveV = (float)Math.Cos((wobbleReal * 4f) + 1f) * 0.06f;
            Vector2 glowScale0 = (new Vector2(0.89f) + new Vector2(waveV)) * trueScale;
            Vector2 offsetVector = (Vector2.UnitY * trueScale * 20).RotatedBy(NPC.rotation);

            if (NPC.velocity.Length() >= 2)
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            else
                NPC.rotation *= 0.8f;

            spriteBatch.Draw(coreTexture.Value, NPC.Center - Main.screenPosition + offsetVector, null, Color.White, rotation, coreOrigin, trueScale, SpriteEffects.None, 0f);

            default(JellyfishTentacleHelper).DrawLowerHalf(spriteBatch, NPC, tentacleWobble, trueScale.Y);

            for (int i = 0; i < 2; i++)
            {
                float waveW = (float)Math.Cos((wobbleReal * 4f) + (i * MathHelper.PiOver4)) * 0.06f;
                Vector2 glowScale = (new Vector2(0.89f) + new Vector2(waveW)) * trueScale;
                spriteBatch.Draw(coreGlow.Value, NPC.Center - Main.screenPosition + offsetVector, null, color, rotation, coreOrigin, glowScale, SpriteEffects.None, 0f);
            }

            ExtendedUtils.DrawStreak(glowBall, SpriteEffects.None, NPC.Center - Main.screenPosition + offsetVector, glowBall.Size() / 2f, 1.5f, glowScale0.X, glowScale0.Y, rotation, ExtendedColor.JellyOrange, ExtendedColor.JellyOrange);

            Dust coreDust = Dust.NewDustDirect(NPC.Center + offsetVector - new Vector2(30), 60, 60, ModContent.DustType<JellyExplosionDust>(), 0, 0, 128, Color.White, 1.5f);
            coreDust.velocity *= 0.9f;
            coreDust.position += NPC.velocity;

            spriteBatch.Draw(bubbleGlow.Value, NPC.Center - Main.screenPosition, null, color2, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bubbleTexture.Value, NPC.Center - Main.screenPosition, null, Color.White, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0f);

            Lighting.AddLight(NPC.Center + offsetVector, ExtendedColor.JellyOrange.ToVector3() * 0.5f);
        }
    }
}