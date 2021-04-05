using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace BlockContentMod
{
    public class BlockContentMod : Mod
    {
        public override void Load()
        {
            LoadShaders();
        }

        public void LoadShaders()
        {
            Ref<Effect> vertexPixelShaderRef = Main.VertexPixelShaderRef;

            GameShaders.Misc["Warbler"] = new MiscShaderData(vertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(doUse: true);
            GameShaders.Misc["Warbler"].UseImage0("Images/Extra_" + (short)190);
            GameShaders.Misc["Warbler"].UseImage1("Images/Extra_" + (short)197);
            GameShaders.Misc["Warbler"].UseImage2("Images/Extra_" + (short)193);

            GameShaders.Misc["Nodachi"] = new MiscShaderData(vertexPixelShaderRef, "FinalFractalVertex").UseProjectionMatrix(doUse: true);
            GameShaders.Misc["Nodachi"].UseImage0("Images/Extra_" + (short)209);
            GameShaders.Misc["Nodachi"].UseImage1("Images/Misc/Noise");
            GameShaders.Misc["Nodachi"].UseImage2("Images/Extra_" + (short)193);
        }
    }
}