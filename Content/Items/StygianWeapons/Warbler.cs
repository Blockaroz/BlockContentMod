using BlockContentMod.Content.Projectiles.StygianWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlockContentMod.Content.Items.StygianWeapons
{
    public class Warbler : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Tooltip.SetDefault("Releases a semi-controllable burst of ink missiles");
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.width = 28;
            Item.height = 34;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.reuseDelay = 15;
            Item.value = 10000;
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<WarblerMissile>();
            Item.shootSpeed = 16f;
            Item.channel = true;
            Item.UseSound = SoundID.Item74;
            Item.mana = 4;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.holdStyle = ItemHoldStyleID.HoldGuitar;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float scale = 1f - (Main.rand.NextFloat() * .2f);
            float angle = MathHelper.ToRadians(18);
            Vector2 perturbedSpeed = velocity.RotatedByRandom(angle);
            perturbedSpeed *= scale;
            velocity = perturbedSpeed;
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
}