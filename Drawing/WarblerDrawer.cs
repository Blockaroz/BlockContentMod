using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace BlockContentMod.Drawing
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct WarblerDrawer
    {
        private static VertexStrip _vertexStrip = new VertexStrip();
        private float widthFactor;

        public void Draw(Projectile proj)
        {
            MiscShaderData miscShaderData = GameShaders.Misc["Warbler"];
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            widthFactor = proj.scale;
            _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        private Color StripColors(float progressOnStrip) => ExtendedColor.ShadeColor;

        private float StripWidth(float progressOnStrip)
        {
            float num = 1f;
            float lerpValue = Terraria.Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 13f, num) * widthFactor;
        }
    }
}