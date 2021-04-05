using BlockContentMod.Content.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Items
{
    public class TestDummyItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dummy");
            Tooltip.SetDefault("Creates a puppet in midair" +
                "\nRight click to remove a puppet");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 38;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 0, 90, 0);
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 0)
                NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y + 20, (ushort)ModContent.NPCType<TestNPC>(), 0);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .Register();
        }
    }
}