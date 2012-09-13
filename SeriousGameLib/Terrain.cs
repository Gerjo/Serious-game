using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace SeriousGameLib
{
    public class Terrain : DrawableGameComponent
    {

        public struct VertexMultitextured
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector4 TextureCoordinate;
            public Vector4 TexWeights;

            public static int SizeInBytes = (3 + 3 + 4 + 4) * sizeof(float);

            public static VertexElement[]
                VertexElements = new[]
                                     {
                                         new VertexElement(0, VertexElementFormat.Vector3,
                                                           VertexElementUsage.Position, 0),
                                         new VertexElement(sizeof (float)*3,
                                                           VertexElementFormat.Vector3,
                                                           VertexElementUsage.TextureCoordinate, 0),
                                         new VertexElement(sizeof (float)*6,
                                                           VertexElementFormat.Vector4,
                                                           VertexElementUsage.TextureCoordinate, 1),
                                         new VertexElement(sizeof (float)*10,
                                                           VertexElementFormat.Vector4,
                                                           VertexElementUsage.TextureCoordinate, 2),
                                     };
        }

        public short Width { get; private set; }
        public short Height { get; private set; }

        private Effect effect;
        private Texture2D bitmap;
        private Texture2D[] textures;
        private string asset;
        private string[] textureAssets;

        private float[,] data;
        private int minheight;
        private int maxheight;
        private VertexMultitextured[] vertices;
        private short[] indices;
        private VertexDeclaration vertexDeclaration;

        private Matrix world;

        private Camera3D Camera { get; set; }

        public Terrain(Game game, string heightmap, string[] textures, Camera3D camera)
            : base(game)
        {
            if (textures.Length < 4)
                throw new ArgumentException("Need four terrain textures.");
            this.asset = heightmap;
            this.textureAssets = textures;
            this.Camera = camera;
        }

        protected override void LoadContent()
        {
            this.effect = Game.Content.Load<Effect>("Effects/terrain");
            this.bitmap = Game.Content.Load<Texture2D>(this.asset);

            this.textures = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                textures[i] = Game.Content.Load<Texture2D>(this.textureAssets[i]);

            this.LoadHeightData(); // Width & Height are available from this point.
            this.SetUpVertices();
            this.SetUpIndices();
            this.InitializeNormals();

            world = Matrix.CreateTranslation(-Width / 2.0f, 0, Height / 2.0f);

            //KXNAEindopdracht.Console.WriteLine("Terrain: heightmap '" + this.asset + "' loaded.");

            base.LoadContent();
        }

        private void LoadHeightData()
        {
            Width = (short)bitmap.Width;
            Height = (short)bitmap.Height;
            Color[] pixels = new Color[Width * Height];
            bitmap.GetData(pixels);

            data = new float[Width, Height];

            minheight = int.MaxValue;
            maxheight = int.MinValue;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    data[x, y] = pixels[x + y * Width].R / 20.0f; // using red channel only.
                    minheight = (int)Math.Min(data[x, y], minheight);
                    maxheight = (int)Math.Max(data[x, y], maxheight);
                }
            }

        }

        private void SetUpVertices()
        {
            if (data == null)
                throw new InvalidOperationException("Call LoadHeightData() first!");

            float step = (maxheight - minheight) / 3;

            vertices = new VertexMultitextured[Width * Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    vertices[x + y * Width].Position = new Vector3(x * 25, data[x, y]*100, -y*25);
                    vertices[x + y * Width].TextureCoordinate.X = x;
                    vertices[x + y * Width].TextureCoordinate.Y = y;

                    vertices[x + y * Width].TexWeights = Vector4.Zero;

                    vertices[x + y * Width].TexWeights.X =
                        MathHelper.Clamp(1.0f - Math.Abs(data[x, y]) / step, 0, 1);
                    vertices[x + y * Width].TexWeights.Y =
                        MathHelper.Clamp(1.0f - Math.Abs(data[x, y] - step) / step, 0, 1);
                    vertices[x + y * Width].TexWeights.Z =
                        MathHelper.Clamp(1.0f - Math.Abs(data[x, y] - 2 * step) / step, 0, 1);
                    vertices[x + y * Width].TexWeights.W =
                        MathHelper.Clamp(1.0f - Math.Abs(data[x, y] - 3 * step) / step, 0, 1);

                    float total = vertices[x + y * Width].TexWeights.X;
                    total += vertices[x + y * Width].TexWeights.Y;
                    total += vertices[x + y * Width].TexWeights.Z;
                    total += vertices[x + y * Width].TexWeights.W;

                    vertices[x + y * Width].TexWeights.X /= total;
                    vertices[x + y * Width].TexWeights.Y /= total;
                    vertices[x + y * Width].TexWeights.Z /= total;
                    vertices[x + y * Width].TexWeights.W /= total;
                }
            }

            vertexDeclaration = new VertexDeclaration(VertexMultitextured.VertexElements);
        }

        private void SetUpIndices()
        {
            if (vertices == null)
                throw new InvalidOperationException("Call SetUpVertices() first!");
            indices = new short[(Width - 1) * (Height - 1) * 6];
            int counter = 0;
            for (int y = 0; y < Height - 1; y++)
            {
                for (int x = 0; x < Width - 1; x++)
                {
                    short lowerLeft = (short)(x + y * Width);
                    short lowerRight = (short)((x + 1) + y * Width);
                    short topLeft = (short)(x + (y + 1) * Width);
                    short topRight = (short)((x + 1) + (y + 1) * Width);

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }

        private void InitializeNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = Vector3.Zero;

            for (int i = 0; i < indices.Length / 3; i++)
            {
                short index0 = indices[i * 3];
                short index1 = indices[i * 3 + 1];
                short index2 = indices[i * 3 + 2];

                Vector3 side0 = vertices[index0].Position - vertices[index2].Position;
                Vector3 side1 = vertices[index0].Position - vertices[index1].Position;
                Vector3 normal = Vector3.Cross(side0, side1);

                vertices[index0].Normal += normal;
                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            effect.CurrentTechnique = effect.Techniques["Multitextured"];
            effect.Parameters["World"].SetValue(this.world);

            effect.Parameters["View"].SetValue(Camera.View);
            effect.Parameters["Projection"].SetValue(Camera.Projection);

            effect.Parameters["LightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));
            effect.Parameters["Ambient"].SetValue(0.4f);

            for (int i = 0; i < 4; i++)
                effect.Parameters["Texture" + i].SetValue(this.textures[i]);

            //*/
            Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //*/

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, this.vertexDeclaration);
            }
            base.Draw(gameTime);
        }
    }
}
