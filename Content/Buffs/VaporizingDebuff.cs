using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Buffs
{
    public class VaporizingDebuff : ModBuff
    {
        private Color ColorMethod(float speed = 1f)
        {
            Color result = Main.hslToRgb(Main.GlobalTimeWrappedHourly * speed % 1f, 0.9f, 0.66f);
            result.A = 200;
            return result;
        }

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Radiation");
            Description.SetDefault("Your body is breaking into waves of energy");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            LongerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            Dust dust = Dust.NewDustDirect(npc.position - new Vector2(7, 7), npc.width + 14, npc.height + 14, DustID.AncientLight, 0, 0, 0, ColorMethod(), 1f);
            dust.noGravity = true;

            Vector2 center = dust.position.DirectionTo(npc.Center) * 2f;
            dust.velocity = center;
            dust.velocity *= 1.1f;
            dust.position += npc.velocity / 1.1f;
            dust.scale *= 0.98f;

            if (npc.lifeRegen < 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 70;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            Dust dust = Main.dust[Dust.NewDust(player.position - new Vector2(7, 7), player.width + 14, player.height + 14, DustID.AncientLight, 0, 0, 0, ColorMethod(), 1f)];
            dust.noGravity = true;

            Vector2 center = dust.position.DirectionTo(player.Center) * 2f;
            dust.velocity = center;
            dust.velocity *= 1.1f;
            dust.position += player.velocity / 1.1f;
            dust.scale *= 0.98f;

            if (player.lifeRegen < 0)
                player.lifeRegen = 0;
            player.lifeRegen -= 30;
        }
    }
}