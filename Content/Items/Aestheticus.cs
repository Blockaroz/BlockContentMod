using BlockContentMod.Content.Projectiles;
using BlockContentMod.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Items
{
    public class Aestheticus : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Summon;
            Item.damage = 28;
            Item.width = 58;
            Item.height = 64;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            //figoure out sounds
            Item.shoot = ModContent.ProjectileType<AestheticusBolt>();
            Item.shootSpeed = 15f;
            Item.mana = 4;
            ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[Type] = true;
            Item.rare = ModContent.RarityType<LightRainbowRarity>();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AddRecipes()
        {
            CreateRecipe()
                //.AddIngredient(ModContent.ItemType<Biteki>())
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.SoulofLight, 18)
                .AddIngredient(ItemID.SoulofNight, 18)
                .AddIngredient(ItemID.Gel, 30)
                .AddIngredient(ItemID.FallenStar, 50)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}