using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SeriousGameLib
{
    public enum CollissionType
    {
        NONE,
        EVENTFIRE,
        BLOCK
    }

    public abstract class GameObject3D
    {
        public string UID { get; set; }
        public CollissionType CollissionBehaviour { get; set; }
        public bool FireMouseOverEvent { get; set; }
        public bool FireMouseClickEvent { get; set; }

        public bool IsPlayerControlled { get; set; }

        public MiniGames MiniGameType { get; set; }
        public Vector3 MiniGameAngle { get; set; } // In degrees
        public Vector3 MiniGamePosition { get; set; }

        public static ContentManager Content;
        public GameObject3D()
        {
            UID = string.Empty;
            CollissionBehaviour = CollissionType.NONE;

            MeshRotationX = new Dictionary<string, float>();
            MeshRotationY = new Dictionary<string, float>();
            MeshRotationZ = new Dictionary<string, float>();

            Visible = true;
            Alpha = 1.0f;

            BoundingBoxScale  = new Vector3(1.0f, 1.0f, 1.0f);
            BoundingBoxOffset = Vector3.Zero;

            Scale = new Vector3(1.0f, 1.0f, 1.0f);

            RotateY = 0.0f;
        }

        public virtual Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }

        public float RotateY { get; set; }
        public float RotateZ { get; set; }

        public BoundingBox CombinedBoundingBox { get; set; }

        public Vector3 BoundingBoxScale { get; set; }
        public Vector3 BoundingBoxOffset { get; set; }

        public BoundingBox TransformedCombinedBoundingBox
        {
            get
            {
                List<Vector3> boundingBoxPoints = new List<Vector3>(CombinedBoundingBox.GetCorners());

                for (int i = 0; i < boundingBoxPoints.Count; ++i)
                {
                    boundingBoxPoints[i] = Vector3.Transform(boundingBoxPoints[i], (BoneTransforms!= null ? BoneTransforms[Model.Root.Index] : Matrix.Identity) *
                                                                                   Matrix.CreateScale(BoundingBoxScale) *
                                                                                   Matrix.CreateScale(Scale) *
                                                                                   //Matrix.CreateRotationY(MathHelper.ToRadians(RotateY)) *
                                                                                   //Matrix.CreateRotationZ(MathHelper.ToRadians(RotateZ)) *
                                                                                   Matrix.CreateTranslation(Position + BoundingBoxOffset));
                }

                return BoundingBox.CreateFromPoints(boundingBoxPoints);
            }
        }

        public bool Visible { get; set; }

        public float Alpha { get; set; }

        private Model _model;
        public Model Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                RefreshBoneMatrices();
                SetCombinedBoundingBox();
                RefreshMeshRotations();
            }
        }

        private void RefreshMeshRotations()
        {
            MeshRotationX.Clear();
            MeshRotationY.Clear();
            MeshRotationZ.Clear();

            if (Model != null)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    if (!MeshRotationX.ContainsKey(mesh.Name))
                    {
                        MeshRotationX.Add(mesh.Name, 0.0f);
                        MeshRotationY.Add(mesh.Name, 0.0f);
                        MeshRotationZ.Add(mesh.Name, 0.0f);
                    }
                }
            }
        }

        public void SetMeshRotationX(string meshName, float xDegrees)
        {
            if (MeshRotationX.ContainsKey(meshName))
            {
                MeshRotationX[meshName] = xDegrees;
            }
            else
            {
                throw new Exception("Meshname doesn't exist!");
            }
        }
        public void SetMeshRotationY(string meshName, float yDegrees)
        {
            if (MeshRotationY.ContainsKey(meshName))
            {
                MeshRotationY[meshName] = yDegrees;
            }
            else
            {
                throw new Exception("Meshname doesn't exist!");
            }
        }
        public void SetMeshRotationZ(string meshName, float zDegrees)
        {
            if (MeshRotationZ.ContainsKey(meshName))
            {
                MeshRotationZ[meshName] = zDegrees;
            }
            else
            {
                throw new Exception("Meshname doesn't exist!");
            }
        }

        private void SetCombinedBoundingBox()
        {
            BoundingBox? result = null;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                if (result != null)
                {
                    BoundingBox.CreateMerged((BoundingBox)result, BoundingBox.CreateFromSphere(mesh.BoundingSphere));
                }
                else
                {
                    result = BoundingBox.CreateFromSphere(mesh.BoundingSphere);
                }
            }

            CombinedBoundingBox = (BoundingBox)result;
        }

        public Matrix[] BoneTransforms { get; set; }
        private void RefreshBoneMatrices()
        {
            if (_model != null)
            {
                BoneTransforms = new Matrix[_model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(BoneTransforms);
            }
        }

        public bool Intersects(GameObject3D other)
        {
            if (TransformedCombinedBoundingBox.Intersects(other.TransformedCombinedBoundingBox))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool PositionIntersects(Vector3 position, float width, float height)
        {
            BoundingBox intersectBox = new BoundingBox(new Vector3(position.X - width / 2, position.Y - height / 2, position.Z - width / 2),
                                                       new Vector3(position.X + width / 2, position.Y + height / 2, position.Z + width / 2));

            if (TransformedCombinedBoundingBox.Intersects(intersectBox))
            {
                return true;
            }

            return false;
        }

        //mesh name + X/Y/Z rotation in degrees
        public Dictionary<string, float> MeshRotationX { get; set; }
        public Dictionary<string, float> MeshRotationY { get; set; }
        public Dictionary<string, float> MeshRotationZ { get; set; }
        
    }
 }
