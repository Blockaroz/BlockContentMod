using BlockContentMod.Content.Projectiles.StygianWeapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Items.StygianWeapons
{
    public class Inklace : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.width = 18;
            Item.height = 18;
            Item.shoot = ModContent.ProjectileType<InklaceProjectile>();
            Item.UseSound = SoundID.Item152;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.noUseGraphic = true;
            Item.damage = 90;
            Item.knockBack = 15;
            Item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
}