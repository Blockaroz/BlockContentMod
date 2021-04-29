using BlockContentMod.Content.Dusts;
using BlockContentMod.Drawing;
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

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            default(JellyfishTentacleHelper).DrawLowerHalfTentacles(spriteBatch, NPC.Center, 0, 1f, new Vector2(0.8f, -0.3f));
        }
    }
}