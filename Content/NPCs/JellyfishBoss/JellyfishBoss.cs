using BlockContentMod.Content.Dusts;
using BlockContentMod.Content.Projectiles.JellyfishProjs;
using BlockContentMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.NPCs.JellyfishBoss
{
    [AutoloadBossHead]
    public class JellyfishBoss : ModNPC
    {
        public ref float Phase => ref NPC.ai[0];
        public ref float PhaseTimer => ref NPC.ai[1];
        public ref float OverallPhase => ref NPC.ai[2];
        public ref float DrawPhase => ref NPC.ai[3];
        public bool TeleportNext = false;
        public ref float LocalCounter => ref NPC.localAI[0];
        public ref float ExtraCounter => ref NPC.localAI[1];

        public static float PhaseTimerMax = 60;
        public static float TeleportTime { get; set; }
        public static float BaseAttackTime = 100;
        public static float TrailAttackTime = 20;
        public static float TrailAttackTimeMax = 140;

        public static int BubbleDust = ModContent.DustType<JellyfishBubbleDust>();
        public static int JellyExplodeDust = ModContent.DustType<JellyExplosionDust>();
        public static int CoreOffset = 40;

        public override string Texture => "BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBubble";

        public override string BossHeadTexture => "BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBoss_Head";

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position -= new Vector2(0, 18);
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement("This jellyfish has grown to multitudinous sizes and holds volatile material within itself. It is a surprise that it has kept its form for so long.")
            });
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jopojelly");

            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;

            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.OnFire,
                    BuffID.Frostburn,
                    BuffID.Bleeding
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "BlockContentMod/Content/Bestiary/JellyfishPortrait",
                PortraitPositionYOverride = -6f,
                PortraitPositionXOverride = 18f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.width = 158;
            NPC.height = 150;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.DD2_LightningBugZap;
            NPC.DeathSound = SoundID.DD2_LightningBugDeath;

            Music = MusicID.Boss2;
            NPC.lifeMax = 5000;
            NPC.lifeRegen = 1;
            NPC.defense = 20;
            NPC.value = Item.buyPrice(gold: 4);
            NPC.SpawnWithHigherTime(8);
            NPC.knockBackResist = 0.2f;
            NPC.damage = NPC.GetAttackDamage_ScaledByStrength(15);
        }

        public override bool PreKill()
        {
            for (int i = 0; i < 4; i++)
            {
                Dust orangeDust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, JellyExplodeDust, 0, 0, 128, Color.White, 1f);
                Vector2 orangeSpeed = orangeDust.position.DirectionFrom(NPC.Center);
                orangeDust.velocity = orangeSpeed;
            }
            Dust bubbleDust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, JellyExplodeDust, 0, 0, 128, Color.White, 1f);
            Vector2 bubbleSpeed = bubbleDust.position.DirectionFrom(NPC.Center);
            bubbleDust.velocity = bubbleSpeed;

            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(TeleportNext);
            writer.Write(TeleportTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            TeleportNext = reader.ReadBoolean();
            TeleportTime = reader.Read();
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                NPC.velocity.Y -= 0.7f;

                NPC.EncourageDespawn(12);
                return;
            }

            PhaseCheck();

            if (OverallPhase <= 2)
            {
                TeleportTime = 50;
                PhaseTimer++;
                if (PhaseTimer > 50)
                    PhaseTimer = 50;

                if (Phase <= 0)
                    Phase++;

                if (PhaseTimer == 50)
                {
                    if (Phase == 1 && TeleportNext == false)
                        BaseAttack(player);
                    else if (Phase == 1 && TeleportNext == true)
                        SpinBaseAttack(player);
                    else if (Phase == 2)
                        ExplosionAttack(player, true, OverallPhase == 2);
                    else if (Phase == 3 && OverallPhase == 2)
                        LightningCircleAttack(player);
                    else if (Phase == 4)
                        ResetAI(true);
                }
                else if (TeleportNext == true)
                    Teleport(player);
                else
                {
                    DrawPhase = 0;
                    Movement(player.MountedCenter, 0.55f, 10);
                }
            }
            else if (OverallPhase == 3)
            {
                NPC.immortal = true;
                Slow(0.8f);
                DrawPhase = -3;

                DustHelper(true, 1f, BubbleDust);
                DustHelper(false, 1.2f, JellyExplodeDust);

                PhaseTimer++;
                for (int i = 0; i < 180; i++)
                {
                    if (PhaseTimer == i * 2)
                    {
                        TransitionAttack(player);
                    }
                }
                if (PhaseTimer >= 180)
                {
                    //SoundEngine.PlaySound(SoundID., NPC.Center);

                    DrawPhase = 0;
                    NPC.immortal = false;
                    PhaseTimer = 0;
                    OverallPhase = 4;
                    ResetAI(true);

                    DustHelper(true, 1f, JellyExplodeDust);
                    for (int i = 0; i < 3; i++)
                        DustHelper(false, 1f, JellyExplodeDust);
                }
            }
            else if (OverallPhase == 4)
            {
                NPC.defense = 60;
                TeleportTime = 24;
                PhaseTimer++;
                if (PhaseTimer > 25)
                    PhaseTimer = 25;

                if (Phase <= 0)
                    Phase++;

                if (PhaseTimer == 25)
                {
                    if (Phase == 1 && TeleportNext == false)
                    {
                        if (Main.expertMode)
                            BaseAttackExpert(player);
                        else
                            BaseAttack(player);
                    }
                    else if (Phase == 1 && TeleportNext == true)
                        LightningCircleAttack(player);
                    else if (Phase == 2)
                        ExplosionAttackTwo(player);
                    else if (Phase == 3)
                        ExplosionAttack(player, true, false, true);
                    else if (Phase == 4)
                        ResetAI(true);
                }
                else if (TeleportNext == true)
                    Teleport(player);
                else if (TeleportNext == false)
                    Movement(player.MountedCenter, 0.66f, 20);
            }
        }

        public static int GetScaledNumbers(int count)
        {
            if (Main.expertMode)
                count += 15;

            if (Main.getGoodWorld)
                count += 15;

            return count;
        }

        public Vector2 PredictedPosition(Player player, float intensity) => player.Center + (player.velocity * 2f * intensity);

        public void PhaseCheck()
        {
            if (OverallPhase == 4)
            {
                return;
            }
            else if (NPC.GetLifePercent() < 0.3f)
            {
                OverallPhase = 3;
                return;
            }
            else if (NPC.GetLifePercent() < 0.6f)
            {
                OverallPhase = 2;
                return;
            }
            else
            {
                OverallPhase = 1;
                return;
            }
        }

        public void ResetAI(bool resetPhase)
        {
            if (resetPhase)
                Phase = -1;
            else
                Phase++;
            PhaseTimer = 0;
            LocalCounter = 0;
        }

        public void Movement(Vector2 position, float speed, float minDistance)
        {
            if (Vector2.Distance(NPC.Center, position) >= minDistance)
                NPC.velocity += NPC.DirectionTo(position) * speed;
            else
                Slow(0.97f);

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.93f;
        }

        public void Slow(float harshness = 0.933f)
        {
            NPC.velocity *= harshness;
            if (NPC.velocity.Length() < 0.0044f)
                NPC.velocity = Vector2.Zero;
        }

        public void Teleport(Player player)
        {
            DrawPhase = -1;

            Slow();

            Vector2 pos = ExtendedUtils.GetPositionAroundTarget(player.MountedCenter, Main.rand.Next(370, 460), true);

            ExtraCounter++;

            NPC.position += player.velocity / 10f;

            if (ExtraCounter == (TeleportTime / 2))
            {
                NPC.position = pos;
            }
            if (ExtraCounter >= TeleportTime)
            {
                ExtraCounter = 0;
                DrawPhase = 0;
                TeleportNext = false;
                return;
            }
        }

        public void StraightDash(Vector2 vector, float speed, bool instant = false)
        {
            DrawPhase = -2;

            NPC.knockBackResist = 0f;

            if (instant == false)
            {
                ExtraCounter++;
                if (ExtraCounter < 15)
                {
                    Slow(0.75f);
                    NPC.velocity -= NPC.DirectionTo(vector) * (speed / 5);
                    DustHelper(true, 1f, BubbleDust);
                }
                if (ExtraCounter == 25)
                {
                    NPC.velocity += NPC.DirectionTo(vector) * speed;
                    NPC.velocity.Normalize();
                    NPC.velocity *= speed;
                }
            }
            else
            {
                NPC.velocity += NPC.DirectionTo(vector) * speed;
                NPC.velocity.Normalize();
                NPC.velocity *= speed;
                return;
            }

            if (ExtraCounter >= 27)
            {
                ExtraCounter = 0;
                DrawPhase = 0;
                NPC.knockBackResist = 0.7f;
                Slow(0.84f);
                return;
            }
        }

        public void BaseAttack(Player player)
        {
            Movement(player.MountedCenter, 0.33f, 30);

            LocalCounter++;

            for (int i = 0; i < (BaseAttackTime / 10); i++)
            {
                if (LocalCounter == 10 * i && LocalCounter > 20)
                {
                    float rotation = NPC.Center.DirectionTo(player.MountedCenter).ToRotation();
                    Vector2 offset = new Vector2(CoreOffset, 0).RotatedBy(rotation);
                    Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center + offset, NPC.Center.DirectionTo(PredictedPosition(player, 1.1f)).RotatedByRandom(0.1f) * (15f + i), ModContent.ProjectileType<JellyfishBoltProj>(), NPC.GetAttackDamage_ScaledByStrength(10), 0);
                }
            }
            if (LocalCounter >= BaseAttackTime)
            {
                ResetAI(false);
                return;
            }
        }

        public void BaseAttackExpert(Player player)
        {
            Movement(player.MountedCenter, 0.13f, 30);

            LocalCounter++;

            for (int i = 0; i < (63 / 7); i++)
            {
                if (LocalCounter == 7 * i && LocalCounter >= 21)
                {
                    SoundEngine.PlaySound(SoundID.Item122, NPC.Center);

                    float rotation = NPC.Center.DirectionTo(player.MountedCenter).ToRotation();
                    Vector2 offset = new Vector2(CoreOffset, 0).RotatedBy(rotation);
                    Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center + offset, NPC.Center.DirectionTo(player.MountedCenter).RotatedBy(MathHelper.PiOver2 / 3) * 6f, ModContent.ProjectileType<JellyfishLightningProj>(), NPC.GetAttackDamage_ScaledByStrength(10), 0);
                }
            }
            if (LocalCounter >= 84)
            {
                ResetAI(false);
                return;
            }
        }

        public void SpinBaseAttack(Player player)
        {
            LocalCounter++;
            int TotalProjectiles = GetScaledNumbers(30);

            for (int i = 0; i < TotalProjectiles; i++)
            {
                if (LocalCounter == (i * 3))
                {
                    float rotation = ExtendedUtils.GetCircle(i * 3, TotalProjectiles) + (Main.rand.NextFloat(-3, 3) / 30);
                    Vector2 offset = new Vector2(0, CoreOffset).RotatedBy(rotation);
                    Vector2 speed = new Vector2(0, Main.rand.Next(15, 18)).RotatedBy(rotation);
                    Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center + offset, speed, ModContent.ProjectileType<JellyfishBoltProj>(), NPC.GetAttackDamage_ScaledByStrength(10), 0);
                }
            }

            if (LocalCounter > 50)
            {
                DrawPhase = 0;
                Movement(player.MountedCenter, 1f, 30);
            }
            else
                DrawPhase = -3;

            if (LocalCounter >= 60)
            {
                ResetAI(false);
                return;
            }
        }

        public void ExplosionAttack(Player player, bool aroundPlayer, bool onPlayer, bool shootOtherProjectiles = false)
        {
            LocalCounter++;
            if (LocalCounter > 15)
                Movement(player.MountedCenter, 0.15f, 70);
            else
                Slow();

            if (LocalCounter == 1)
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);

            int playerCount = Main.player.Count(x => x.active);
            for (int i = 0; i < playerCount; i++)
            {
                Player target = Main.player[i];
                for (int j = 0; j < TrailAttackTimeMax; j++)
                {
                    float multiplier;
                    if (Main.expertMode)
                        multiplier = j * 0.5f;
                    else if (Main.getGoodWorld)
                        multiplier = 1f;
                    else
                        multiplier = j;

                    if (LocalCounter == TrailAttackTime * multiplier && aroundPlayer == true)
                    {
                        Vector2 pos1 = ExtendedUtils.GetPositionAroundTarget(target.MountedCenter, Main.rand.Next(60, 720), true);
                        Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), pos1, pos1.DirectionTo(player.MountedCenter), ModContent.ProjectileType<JellyfishExplosionProj>(), NPC.GetAttackDamage_ScaledByStrength(20), 0);
                    }

                    if (LocalCounter == (TrailAttackTime * multiplier * 2f) + (TrailAttackTime * 0.5f) && onPlayer == true)
                    {
                        Vector2 pos2 = ExtendedUtils.GetPositionAroundTarget(target.MountedCenter, Main.rand.Next(5, 60), false);
                        Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), pos2, pos2.DirectionTo(target.MountedCenter), ModContent.ProjectileType<JellyfishExplosionProj>(), NPC.GetAttackDamage_ScaledByStrength(20), 0);
                    }

                    if (LocalCounter == (TrailAttackTime * multiplier * 1.5f) && shootOtherProjectiles == true)
                    {
                        Vector2 pos3 = ExtendedUtils.GetPositionAroundTarget(target.MountedCenter, Main.rand.Next(900, 1200), false);
                        Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), pos3, pos3.DirectionTo(target.MountedCenter).RotatedByRandom(0.1f) * Main.rand.Next(12, 15), ModContent.ProjectileType<JellyfishBoltProj>(), NPC.GetAttackDamage_ScaledByStrength(15), 0);
                    }
                }
            }

            if (LocalCounter >= TrailAttackTimeMax)
            {
                if (OverallPhase == 1 || OverallPhase == 4)
                {
                    if (Main.rand.Next(2) == 0)
                        TeleportNext = true;
                }

                if (OverallPhase > 1)
                    ResetAI(false);
                else
                    ResetAI(true);

                return;
            }
        }

        public void ExplosionAttackTwo(Player player)
        {
            LocalCounter++;
            int TotalProjectiles = GetScaledNumbers(10);

            if (LocalCounter == 1)
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);

            for (int i = 0; i < TotalProjectiles; i++)
            {
                if (LocalCounter == (i * 3) && LocalCounter >= 10)
                {
                    float rotation = ExtendedUtils.GetCircle(i * 3, TotalProjectiles) + (Main.rand.NextFloat(-3, 3) / 30);
                    Vector2 offset = new Vector2(0, CoreOffset).RotatedBy(rotation);
                    Vector2 speed = new Vector2(0, Main.rand.Next(11, 15)).RotatedBy(rotation);
                    Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center + offset, speed, ModContent.ProjectileType<JellyfishMovingExplosionProj>(), NPC.GetAttackDamage_ScaledByStrength(20), 0);
                }

                if (LocalCounter == (i * 6))
                {
                    Vector2 speed = NPC.Center.DirectionTo(player.MountedCenter);
                    Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), ExtendedUtils.GetPositionAroundTarget(NPC.Center, Main.rand.Next(60, 720), true), speed, ModContent.ProjectileType<JellyfishMovingExplosionProj>(), NPC.GetAttackDamage_ScaledByStrength(20), 0);
                }
            }

            if (LocalCounter > 70)
            {
                DrawPhase = 0;
                Movement(player.MountedCenter, 0.89f, 30);
            }
            else
                DrawPhase = -3;

            if (LocalCounter <= 30)
                Movement(player.MountedCenter, 2f, 250);
            else if (LocalCounter <= 70)
                Slow(0.85f);

            if (LocalCounter >= 80)
            {
                if (Main.rand.Next(2) == 0)
                    TeleportNext = true;

                ResetAI(false);
                return;
            }
        }

        public void LightningCircleAttack(Player player)
        {
            int TotalExplosionProjectiles = GetScaledNumbers(10);

            DrawPhase = -3;

            Slow(0.8f);

            LocalCounter++;

            if (LocalCounter < 25)
            {
                DustHelper(true, 1f, BubbleDust);
                DustHelper(true, 1.2f, JellyExplodeDust);
            }
            if (LocalCounter == 30)
            {
                SoundEngine.PlaySound(SoundID.Item122, NPC.Center);

                for (int i = 0; i < TotalExplosionProjectiles; i++)
                {
                    DustHelper(false, 1f, BubbleDust);
                    Vector2 speed = new Vector2(0, Main.rand.Next(5, 6)).RotatedBy(ExtendedUtils.GetCircle(i, TotalExplosionProjectiles)).RotatedByRandom(0.3f);
                    Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center, speed, ModContent.ProjectileType<JellyfishLightningProj>(), NPC.GetAttackDamage_ScaledByStrength(15), 0);
                }
            }

            if (LocalCounter >= 50)
            {
                DrawPhase = 0;
                Movement(player.MountedCenter, 0.7f, 30);
            }
            if (LocalCounter >= 70)
            {
                if (Main.rand.Next(2) == 0 && OverallPhase == 2)
                    TeleportNext = true;

                ResetAI(false);
                return;
            }
        }

        public void TransitionAttack(Player player)
        {
            LocalCounter++;

            for (int i = 0; i < TrailAttackTimeMax; i++)
            {
                if (LocalCounter == (TrailAttackTime * i) + (TrailAttackTime * 0.5f))
                {
                    Vector2 pos = ExtendedUtils.GetPositionAroundTarget(NPC.Center, Main.rand.Next(50, 500), true);
                    Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), pos, Vector2.Zero, ModContent.ProjectileType<JellyfishExplosionProj>(), NPC.GetAttackDamage_ScaledByStrength(20), 0);
                }

                if (LocalCounter == (TrailAttackTime * i * 0.5f) + (TrailAttackTime * 0.5f))
                {
                    SoundEngine.PlaySound(SoundID.Item122, NPC.Center);
                    Vector2 velocity = NPC.DirectionTo(player.MountedCenter).RotatedByRandom(MathHelper.PiOver2) * Main.rand.Next(6, 7);
                    Projectile.NewProjectileDirect(NPC.GetProjectileSpawnSource(), NPC.Center, velocity, ModContent.ProjectileType<JellyfishLightningProj>(), NPC.GetAttackDamage_ScaledByStrength(15), 0);
                }
            }
        }

        public void DustHelper(bool inward, float scale, int type)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 DustSpeed;
                Vector2 DustPosition = NPC.position - new Vector2(10, 10);
                Dust dust = Main.dust[Dust.NewDust(DustPosition, NPC.width + 20, NPC.height + 20, type, 0, 0, 128, Color.White, scale)];
                if (inward)
                {
                    DustSpeed = new Vector2(i + 3, 0).RotatedBy(dust.position.AngleTo(NPC.Center + new Vector2(0, 4)));
                }
                else
                {
                    DustSpeed = new Vector2(i + 3, 0).RotatedBy(dust.position.AngleFrom(NPC.Center + new Vector2(0, 4)));
                }
                dust.noGravity = true;
                dust.velocity = DustSpeed;
                dust.position += NPC.velocity;
            }
        }

        public override Color? GetAlpha(Color drawColor) => Color.White;

        public float WobbleCounter { get; set; }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Asset<Texture2D> glowBall = ModContent.GetTexture("BlockContentMod/Assets/GlowBall_" + (short)1);
            Asset<Texture2D> bubbleTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBubble");
            Asset<Texture2D> bubbleGlow = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishBubble_Glow");
            Asset<Texture2D> coreTexture = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishCore");
            Asset<Texture2D> coreGlow = ModContent.GetTexture("BlockContentMod/Content/NPCs/JellyfishBoss/JellyfishCore_Glow");

            WobbleCounter++;
            float tick = 90;
            if (WobbleCounter > tick)
                WobbleCounter = 0;
            float wobbleReal = (WobbleCounter * MathHelper.Pi) / tick;

            if (NPC.velocity.Length() >= 3f)
            {
                const short rotateFactor = 9;
                float rotationValue = (NPC.velocity.X * 0.05f) + (NPC.velocity.Y * 0.05f);
                NPC.rotation = (NPC.rotation * (rotateFactor - 1f) + rotationValue) / rotateFactor;
            }
            else
            {
                NPC.rotation *= 0.95f;
                if (Math.Abs(NPC.rotation) < 0.02f)
                {
                    NPC.rotation = 0;
                }
            }

            float waveX = (float)Math.Cos(wobbleReal * 4f);
            float waveY = (float)Math.Sin(wobbleReal * 4f);
            Vector2 wobble = new Vector2(1 + (waveX * 0.2f), 1 + (waveY * 0.2f));

            float rotation = NPC.rotation;
            Vector2 bubbleOrigin = bubbleTexture.Size() / 2;
            Vector2 coreOrigin = coreGlow.Size() / 2;

            Vector2 trueScale = Vector2.One;
            Vector2 wobbleScale = trueScale;
            Vector2 tentacleWobble = trueScale;
            Vector2 offsetVector = (Vector2.UnitY * trueScale * 20).RotatedBy(NPC.rotation);

            Color color = Color.White;
            color.A = 10;
            Color color2 = Color.White;
            color2.A = 40;

            if (DrawPhase == -1)//teleport
            {
                float t = (float)Math.Abs(Math.Cos(ExtraCounter / (TeleportTime / MathHelper.Pi)));

                trueScale = Vector2.One * Utils.GetLerpValue(0f, 1f, t * 1.1f, true);
                wobbleScale = Vector2.One * Utils.GetLerpValue(0f, 1f, t * 1.1f, true);
                tentacleWobble = new Vector2(waveX, waveY) * Utils.GetLerpValue(0f, 1f, t * 1.1f, true);

                float scaleFactor = Utils.GetLerpValue(1.5f, 0f, t * 1.1f, true) + 0.54f;
                bool dir = ExtraCounter > (TeleportTime / 2);
                DustHelper(dir, scaleFactor, BubbleDust);
            }
            else if (DrawPhase == -3)//more intense wobble
            {
                float waveX2 = (float)Math.Cos(wobbleReal * 8f);
                float waveY2 = (float)Math.Sin(wobbleReal * 8f);
                Vector2 wobble2 = new Vector2(1 + (waveX2 * 0.25f), 1 + (waveY2 * 0.25f));

                wobbleScale = wobble2 * trueScale;
                tentacleWobble = new Vector2(waveX2, waveY2) * trueScale;
            }
            else if (DrawPhase == 0)//default
            {
                wobbleScale = wobble * trueScale;
                tentacleWobble = new Vector2(waveX, waveY) * trueScale;
            }
            
            spriteBatch.Draw(coreTexture.Value, NPC.Center - Main.screenPosition, null, Color.White, rotation, coreOrigin, trueScale, SpriteEffects.None, 0f);

            default(JellyfishTentacleHelper).DrawLowerHalf(spriteBatch, NPC.Center, NPC.rotation, trueScale.Y, tentacleWobble);

            spriteBatch.Draw(bubbleGlow.Value, NPC.Center - offsetVector - Main.screenPosition, null, color2, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0f);

            Dust coreDust = Dust.NewDustDirect(NPC.Center - new Vector2(30), 60, 60, ModContent.DustType<JellyExplosionDust>(), 0, 0, 128, Color.White, 1.5f);
            coreDust.velocity *= 0.9f;
            coreDust.position += NPC.velocity;

            for (int i = 0; i < 2; i++)
            {
                float waveW = (float)Math.Cos(wobbleReal + (i * MathHelper.PiOver4)) * 0.06f;
                Vector2 glowScale = (new Vector2(0.89f) + new Vector2(waveW)) * trueScale;
                spriteBatch.Draw(coreGlow.Value, NPC.Center - Main.screenPosition, null, color2 * 0.5f, rotation, coreOrigin, glowScale, SpriteEffects.None, 0f);
            }

            float waveV = (float)Math.Cos(wobbleReal + 1f) * 0.06f;
            Vector2 glowScale0 = (new Vector2(0.89f) + new Vector2(waveV)) * trueScale;
            ExtendedUtils.DrawStreak(glowBall, SpriteEffects.None, NPC.Center - Main.screenPosition, glowBall.Size() / 2f, 1.5f, glowScale0.X, glowScale0.Y, rotation, ExtendedColor.JellyRed, ExtendedColor.JellyOrange * 0.7f);

            spriteBatch.Draw(bubbleTexture.Value, NPC.Center - offsetVector - Main.screenPosition, null, Color.White, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0f);

            Lighting.AddLight(NPC.Center, ExtendedColor.JellyRed.ToVector3() * 0.1f);

            return false;
        }
    }
}