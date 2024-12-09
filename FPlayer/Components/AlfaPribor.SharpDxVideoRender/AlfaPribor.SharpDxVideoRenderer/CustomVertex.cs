using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.Windows;
using SharpDX.Direct3D9;

namespace AlfaPribor.SharpDXVideoRenderer
{

    public class CustomVertex
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionColored
        {
            #region Data Members (7)

            /// <summary>Create the Vertex Element Array</summary>
            public static VertexElement[] elements = new VertexElement[]
            {
                new VertexElement(0, 0, DeclarationType.Float3,
                                        DeclarationMethod.Default,
                                        DeclarationUsage.Position, 0),
                                        
                new VertexElement(0, 12, DeclarationType.Color,
                                         DeclarationMethod.Default,
                                         DeclarationUsage.Color, 0),

                VertexElement.VertexDeclarationEnd
            };

            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture0;

            public int Color
            {
                get { return color; }
                set { color = value; }
            }

            public Vector3 Position
            {
                get { return position; }
                set { position = value; }
            }

            public float X
            {
                get { return this.position.X; }
                set { this.position.X = value; }
            }

            public float Y
            {
                get { return this.position.Y; }
                set { this.position.Y = value; }
            }

            public float Z
            {
                get { return this.position.Z; }
                set { this.position.Z = value; }
            }

            #endregion Data Members

            #region Methods (2)

            public PositionColored(Vector3 position, int Color)
            {
                this.position = position;
                this.color = Color;
            }

            public static int SizeInBytes() { return Marshal.SizeOf(typeof(PositionColored)); }

            #endregion Methods

            #region vertexData

            Vector3 position;
            int color;

            #endregion
        }

        public struct PositionColoredTextured
        {
            #region Data Members (5)

            public static readonly VertexFormat Format = VertexFormat.Diffuse | VertexFormat.Position | VertexFormat.Texture0;
            
            public int Color
            {
                get { return color; }
                set { color = value; }
            }

            public Vector3 Position
            {
                get { return position; }
                set { position = value; }
            }

            public float Tu
            {
                get { return tu; }
                set { tu = value; }
            }

            public float Tv
            {
                get { return tv; }
                set { tv = value; }
            }

            #endregion Data Members

            #region Methods (1)

            internal static int SizeInBytes() { return Marshal.SizeOf(typeof(PositionColoredTextured)); }

            #endregion Methods

            #region vertexData

            Vector3 position;
            int color;
            float tu;
            float tv;

            #endregion
        }

        public struct PositionNormalColored
        {
            #region Data Members (1)

            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse;

            #endregion Data Members

            #region vertexData

            public Vector3 Position;
            public Vector3 Normal;
            public int Color;

            #endregion
        }

        public struct TransformedColored
        {
            #region Data Members (3)

            public static readonly VertexFormat Format = VertexFormat.Diffuse | VertexFormat.PositionRhw;
            public int Color
            {
                get { return color; }
                set { color = value; }
            }

            public Vector4 Position
            {
                get { return position; }
                set { position = value; }
            }

            #endregion Data Members

            #region Methods (1)

            public int SizeInBytes() { return Marshal.SizeOf(typeof(TransformedColored)); }

            #endregion Methods

            #region vertexData

            Vector4 position;
            int color;

            #endregion
        }

        public struct PositionTextured
        {
            #region Data Members (3)

            public static readonly VertexFormat Format = VertexFormat.Diffuse | VertexFormat.Texture0;
            public Vector3 Position
            {
                get { return position; }
                set { position = value; }
            }

            public Vector2 UVtex
            {
                get { return textureCoordinates; }
                set { textureCoordinates = value; }
            }

            #endregion Data Members

            #region Methods (2)

            public PositionTextured(Vector3 position, Vector2 UVCoordinates)
            {
                this.position = position;
                this.textureCoordinates = UVCoordinates;
            }

            public int SizeInBytes() { return Marshal.SizeOf(typeof(PositionTextured)); }

            #endregion Methods

            #region vertexData

            Vector3 position;
            Vector2 textureCoordinates;

            #endregion
        }

        public struct PositionNormalTextured
        {
            #region Data Members (9)

            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;
            public float Nx
            {
                get { return this.Normal.X; }
                set { this.Normal.X = value; }
            }

            public float Ny
            {
                get { return this.Normal.Y; }
                set { this.Normal.Y = value; }
            }

            public float Nz
            {
                get { return this.Normal.Z; }
                set { this.Normal.Z = value; }
            }

            public float Tu
            {
                get { return this.UV.X; }
                set { this.UV.X = value; }
            }

            public float Tv
            {
                get { return this.UV.Y; }
                set { this.UV.Y = value; }
            }

            public float X
            {
                get { return this.Position.X; }
                set { this.Position.X = value; }
            }

            public float Y
            {
                get { return this.Position.Y; }
                set { this.Position.Y = value; }
            }

            public float Z
            {
                get { return this.Position.Z; }
                set { this.Position.Z = value; }
            }

            #endregion Data Members

            #region Methods (2)

            public PositionNormalTextured(Vector3 position, Vector3 normal, float Tu, float Tv)
            {
                this.Position = position;
                this.Normal = normal;
                this.UV = new Vector2(Tu, Tv);
            }

            internal static int SizeInBytes()
            {
                return Marshal.SizeOf(typeof(PositionNormalTextured));
            }

            #endregion Methods

            #region vertexData

            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 UV;

            #endregion
        }
    }

}
