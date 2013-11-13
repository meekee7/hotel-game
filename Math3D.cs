//=============================================
// Downloaded From                            |
// Visual C# Kicks - http://www.vcskicks.com/ |
//=============================================
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Juego_Hotel
{
    class Math3D
    {
        const double PIOVER180 = Math.PI / 180.0;

        public class Vector3D
        {
            public float x;
            public float y;
            public float z;

            public Vector3D(int _x, int _y, int _z)
            {
                x = _x;
                y = _y;
                z = _z;
            }

            public Vector3D(double _x, double _y, double _z)
            {
                x = (float)_x;
                y = (float)_y;
                z = (float)_z;
            }

            public Vector3D(float _x, float _y, float _z)
            {
                x = _x;
                y = _y;
                z = _z;
            }

            public Vector3D()
            {
            }

            public override string ToString()
            {
                return "(" + x + ", " + y + ", " + z + ")";
            }
        }

        internal class Camera
        {
            public Vector3D position = new Vector3D();
        }

        public class Cube
        {
            //Cube face, has four points, 3D and 2D
            internal class Face : IComparable<Face>
            {
                public enum Side
                {
                    Front,
                    Back,
                    Left,
                    Right,
                    Top,
                    Bottom
                }

                public PointF[] Corners2D;
                public Vector3D[] Corners3D;
                public Vector3D Center;
                public Side CubeSide;
                public Brush color;

                public int CompareTo(Face otherFace)
                {
                    return (int)(Center.z - otherFace.Center.z); //In order of which is closest to the screen
                }
            }

            public int width = 0;
            public int height = 0;
            public int depth = 0;

            float xRotation;
            float yRotation;
            float zRotation;

            bool drawWires = true;
            bool fillFront;
            bool fillBack;
            bool fillLeft;
            bool fillRight;
            bool fillTop;
            bool fillBottom;

            readonly Vector3D cubeOrigin;

            public Face[] faces;

            public float RotateX
            {
                get { return xRotation; }
                set
                {
                    //rotate the difference between this rotation and last rotation
                    RotateCubeX(value - xRotation);
                    xRotation = value;
                }
            }

            public float RotateY
            {
                get { return yRotation; }
                set
                {
                    RotateCubeY(value - yRotation);
                    yRotation = value;
                }
            }

            public float RotateZ
            {
                get { return zRotation; }
                set
                {
                    RotateCubeZ(value - zRotation);
                    zRotation = value;
                }
            }

            public bool DrawWires
            {
                get { return drawWires; }
                set { drawWires = value; }
            }
            public bool FillFront
            {
                get { return fillFront; }
                set { fillFront = value; }
            }
            public bool FillBack
            {
                get { return fillBack; }
                set { fillBack = value; }
            }
            public bool FillLeft
            {
                get { return fillLeft; }
                set { fillLeft = value; }
            }
            public bool FillRight
            {
                get { return fillRight; }
                set { fillRight = value; }
            }
            public bool FillTop
            {
                get { return fillTop; }
                set { fillTop = value; }
            }
            public bool FillBottom
            {
                get { return fillBottom; }
                set { fillBottom = value; }
            }


            #region Initializers
            public Cube(int side)
            {
                width = side;
                height = side;
                depth = side;
                cubeOrigin = new Vector3D(width / 2, height / 2, depth / 2);
                InitializeCube();
            }

            public Cube(int side, Vector3D origin)
            {
                width = side;
                height = side;
                depth = side;
                cubeOrigin = origin;

                InitializeCube();
            }

            public Cube(int Width, int Height, int Depth)
            {
                width = Width;
                height = Height;
                depth = Depth;
                cubeOrigin = new Vector3D(width / 2, height / 2, depth / 2);

                InitializeCube();
            }

            public Cube(int Width, int Height, int Depth, Vector3D origin)
            {
                width = Width;
                height = Height;
                depth = Depth;
                cubeOrigin = origin;

                InitializeCube();
            }
            #endregion

            private void InitializeCube()
            {
                //Fill in the cube

                faces = new Face[6]; //cube has 6 faces

                //Front Face --------------------------------------------
                faces[0] = new Face { CubeSide = Face.Side.Front, Corners3D = new Vector3D[4] };
                faces[0].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[0].Corners3D[1] = new Vector3D(0, height, 0);
                faces[0].Corners3D[2] = new Vector3D(width, height, 0);
                faces[0].Corners3D[3] = new Vector3D(width, 0, 0);
                faces[0].Center = new Vector3D(width / 2, height / 2, 0);
                faces[0].color = Brushes.Red;
                // -------------------------------------------------------

                //Back Face --------------------------------------------
                faces[1] = new Face { CubeSide = Face.Side.Back, Corners3D = new Vector3D[4]} ;
                faces[1].Corners3D[0] = new Vector3D(0, 0, depth);
                faces[1].Corners3D[1] = new Vector3D(0, height, depth);
                faces[1].Corners3D[2] = new Vector3D(width, height, depth);
                faces[1].Corners3D[3] = new Vector3D(width, 0, depth);
                faces[1].Center = new Vector3D(width / 2, height / 2, depth);
                faces[1].color = Brushes.Green;
                // -------------------------------------------------------

                //Left Face --------------------------------------------
                faces[2] = new Face { CubeSide = Face.Side.Left, Corners3D = new Vector3D[4] };
                faces[2].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[2].Corners3D[1] = new Vector3D(0, 0, depth);
                faces[2].Corners3D[2] = new Vector3D(0, height, depth);
                faces[2].Corners3D[3] = new Vector3D(0, height, 0);
                faces[2].Center = new Vector3D(0, height / 2, depth / 2);
                faces[2].color = Brushes.Green;
                // -------------------------------------------------------

                //Right Face --------------------------------------------
                faces[3] = new Face { CubeSide = Face.Side.Right, Corners3D = new Vector3D[4]} ;
                faces[3].Corners3D[0] = new Vector3D(width, 0, 0);
                faces[3].Corners3D[1] = new Vector3D(width, 0, depth);
                faces[3].Corners3D[2] = new Vector3D(width, height, depth);
                faces[3].Corners3D[3] = new Vector3D(width, height, 0);
                faces[3].Center = new Vector3D(width, height / 2, depth / 2);
                faces[3].color = Brushes.Green;
                // -------------------------------------------------------

                //Top Face --------------------------------------------
                faces[4] = new Face { CubeSide = Face.Side.Top, Corners3D = new Vector3D[4] };
                faces[4].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[4].Corners3D[1] = new Vector3D(0, 0, depth);
                faces[4].Corners3D[2] = new Vector3D(width, 0, depth);
                faces[4].Corners3D[3] = new Vector3D(width, 0, 0);
                faces[4].Center = new Vector3D(width / 2, 0, depth / 2);
                faces[4].color = Brushes.Yellow;
                // -------------------------------------------------------

                //Bottom Face --------------------------------------------
                faces[5] = new Face { CubeSide = Face.Side.Bottom, Corners3D = new Vector3D[4] };
                faces[5].Corners3D[0] = new Vector3D(0, height, 0);
                faces[5].Corners3D[1] = new Vector3D(0, height, depth);
                faces[5].Corners3D[2] = new Vector3D(width, height, depth);
                faces[5].Corners3D[3] = new Vector3D(width, height, 0);
                faces[5].Center = new Vector3D(width / 2, height, depth / 2);
                faces[5].color = Brushes.Orange;
                // -------------------------------------------------------
            }

            //Calculates the 2D points of each face
            private void Update2DPoints(Point drawOrigin)
            {
                //Update the 2D points of all the faces
                for (int i = 0; i < faces.Length; i++)
                {
                    Update2DPoints(drawOrigin, i);
                }
            }

            private void Update2DPoints(Point drawOrigin, int faceIndex)
            {
                //Calculates the projected coordinates of the 3D points in a cube face
                var point2D = new PointF[4];

                //Convert 3D Points to 2D
                for (int i = 0; i < point2D.Length; i++)
                {
                    Vector3D vec = faces[faceIndex].Corners3D[i];
                    point2D[i] = Get2D(vec, drawOrigin);
                }

                //Update face
                faces[faceIndex].Corners2D = point2D;
            }

            //Rotating methods, has to translate the cube to the rotation point (center), rotate, and translate back

            private void RotateCubeX(float deltaX)
            {
                foreach (Face face in faces)
                {
                    //Apply rotation
                    //------Rotate points
                    var point0 = new Vector3D(0, 0, 0);
                    face.Corners3D = Translate(face.Corners3D, cubeOrigin, point0); //Move corner to origin
                    face.Corners3D = RotateX(face.Corners3D, deltaX);
                    face.Corners3D = Translate(face.Corners3D, point0, cubeOrigin); //Move back

                    //-------Rotate center
                    face.Center = Translate(face.Center, cubeOrigin, point0);
                    face.Center = RotateX(face.Center, deltaX);
                    face.Center = Translate(face.Center, point0, cubeOrigin);
                }
            }

            private void RotateCubeY(float deltaY)
            {
                foreach (Face face in faces)
                {
                    //Apply rotation
                    //------Rotate points
                    var point0 = new Vector3D(0, 0, 0);
                    face.Corners3D = Translate(face.Corners3D, cubeOrigin, point0); //Move corner to origin
                    face.Corners3D = RotateY(face.Corners3D, deltaY);
                    face.Corners3D = Translate(face.Corners3D, point0, cubeOrigin); //Move back

                    //-------Rotate center
                    face.Center = Translate(face.Center, cubeOrigin, point0);
                    face.Center = RotateY(face.Center, deltaY);
                    face.Center = Translate(face.Center, point0, cubeOrigin);
                }
            }

            private void RotateCubeZ(float deltaZ)
            {
                foreach (Face face in faces)
                {
                    //Apply rotation
                    //------Rotate points
                    var point0 = new Vector3D(0, 0, 0);
                    face.Corners3D = Translate(face.Corners3D, cubeOrigin, point0); //Move corner to origin
                    face.Corners3D = RotateZ(face.Corners3D, deltaZ);
                    face.Corners3D = Translate(face.Corners3D, point0, cubeOrigin); //Move back

                    //-------Rotate center
                    face.Center = Translate(face.Center, cubeOrigin, point0);
                    face.Center = RotateZ(face.Center, deltaZ);
                    face.Center = Translate(face.Center, point0, cubeOrigin);
                }
            }

            public Bitmap DrawCube(Point drawOrigin)
            {
                //Get the corresponding 2D
                Update2DPoints(drawOrigin);

                //Get the bounds of the final bitmap
                Rectangle bounds = getDrawingBounds();
                bounds.Width += drawOrigin.X;
                bounds.Height += drawOrigin.Y;

                var finalBmp = new Bitmap(bounds.Width, bounds.Height);
                Graphics g = Graphics.FromImage(finalBmp);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                Array.Sort(faces); //sort faces from closest to farthest
                //message();
                for (int i = faces.Length - 1; i >= 0; i--) //draw faces from back to front
                {
                    switch (faces[i].CubeSide)
                    {
                        case Face.Side.Front:
                            if (fillFront)
                                g.FillPolygon(faces[i].color, GetFrontFace());
                            break;
                        case Face.Side.Back:
                            if (fillBack)
                                g.FillPolygon(faces[i].color, GetBackFace());
                            break;
                        case Face.Side.Left:
                            if (fillLeft)
                                g.FillPolygon(faces[i].color, GetLeftFace());
                            break;
                        case Face.Side.Right:
                            if (fillRight)
                                g.FillPolygon(faces[i].color, GetRightFace());
                            break;
                        case Face.Side.Top:
                            if (fillTop)
                                g.FillPolygon(faces[i].color, GetTopFace());
                            break;
                        case Face.Side.Bottom:
                            if (fillBottom)
                                g.FillPolygon(faces[i].color, GetBottomFace());
                            break;
                    }

                    if (!drawWires)
                        continue;
                    g.DrawLine(Pens.Black, faces[i].Corners2D[0], faces[i].Corners2D[1]);
                    g.DrawLine(Pens.Black, faces[i].Corners2D[1], faces[i].Corners2D[2]);
                    g.DrawLine(Pens.Black, faces[i].Corners2D[2], faces[i].Corners2D[3]);
                    g.DrawLine(Pens.Black, faces[i].Corners2D[3], faces[i].Corners2D[0]);
                }

                g.Dispose();

                return finalBmp;
            }

            //Converts 3D points to 2D points
            private PointF Get2D(Vector3D vec, Point drawOrigin)
            {
                PointF point2D = Get2D(vec);
                return new PointF(point2D.X + drawOrigin.X, point2D.Y + drawOrigin.Y);
            }

            private PointF Get2D(Vector3D vec)
            {
                var returnPoint = new PointF();

                float zoom = Screen.PrimaryScreen.Bounds.Width / 1.5f;
                var tempCam = new Camera
                {
                    position = { x = cubeOrigin.x, y = cubeOrigin.y, z = (cubeOrigin.x*zoom)/cubeOrigin.x }
                };

                float zValue = -vec.z - tempCam.position.z;

                returnPoint.X = (tempCam.position.x - vec.x) / zValue * zoom;
                returnPoint.Y = (tempCam.position.y - vec.y) / zValue * zoom;

                return returnPoint;
            }

            public PointF[] GetFrontFace()
            {
                //Returns the four points corresponding to the front face
                //Get the corresponding 2D
                return getFace(Face.Side.Front).Corners2D;
            }

            public PointF[] GetBackFace()
            {
                return getFace(Face.Side.Back).Corners2D;
            }

            public PointF[] GetRightFace()
            {
                return getFace(Face.Side.Right).Corners2D;
            }

            public PointF[] GetLeftFace()
            {
                return getFace(Face.Side.Left).Corners2D;
            }

            public PointF[] GetTopFace()
            {
                return getFace(Face.Side.Top).Corners2D;
            }

            public PointF[] GetBottomFace()
            {
                return getFace(Face.Side.Bottom).Corners2D;
            }

            private Face getFace(Face.Side side)
            {
                //Find the correct side
                //Since faces are sorted in order of closest to farthest
                //They won't always be in the same index
                return faces.FirstOrDefault(t => t.CubeSide == side);
            }

            private Rectangle getDrawingBounds()
            {
                //Find the farthest most points to calculate the size of the returning bitmap
                float left = float.MaxValue;
                float right = float.MinValue;
                float top = float.MaxValue;
                float bottom = float.MinValue;

                foreach (PointF corner in faces.SelectMany(face => face.Corners2D))
                {
                    if (corner.X < left)
                        left = corner.X;
                    if (corner.X > right)
                        right = corner.X;
                    if (corner.Y < top)
                        top = corner.Y;
                    if (corner.Y > bottom)
                        bottom = corner.Y;
                }

                return new Rectangle(0, 0, (int)Math.Round(right - left), (int)Math.Round(bottom - top));
            }
        }

        public static Vector3D RotateX(Vector3D point3D, float degrees)
        {
            //[ a  b  c ] [ x ]   [ x*a + y*b + z*c ]
            //[ d  e  f ] [ y ] = [ x*d + y*e + z*f ]
            //[ g  h  i ] [ z ]   [ x*g + y*h + z*i ]

            //[ 1    0        0   ]
            //[ 0   cos(x)  sin(x)]
            //[ 0   -sin(x) cos(x)]

            double cDegrees = degrees * PIOVER180;
            double cosDegrees = Math.Cos(cDegrees);
            double sinDegrees = Math.Sin(cDegrees);

            double y = (point3D.y * cosDegrees) + (point3D.z * sinDegrees);
            double z = (point3D.y * -sinDegrees) + (point3D.z * cosDegrees);

            return new Vector3D(point3D.x, y, z);
        }

        public static Vector3D RotateY(Vector3D point3D, float degrees)
        {
            //[ cos(x)   0    sin(x)]
            //[   0      1      0   ]
            //[-sin(x)   0    cos(x)]

            double cDegrees = degrees * PIOVER180;
            double cosDegrees = Math.Cos(cDegrees);
            double sinDegrees = Math.Sin(cDegrees);

            double x = (point3D.x * cosDegrees) + (point3D.z * sinDegrees);
            double z = (point3D.x * -sinDegrees) + (point3D.z * cosDegrees);

            return new Vector3D(x, point3D.y, z);
        }

        public static Vector3D RotateZ(Vector3D point3D, float degrees)
        {
            //[ cos(x)  sin(x) 0]
            //[ -sin(x) cos(x) 0]
            //[    0     0     1]

            double cDegrees = degrees * PIOVER180;
            double cosDegrees = Math.Cos(cDegrees);
            double sinDegrees = Math.Sin(cDegrees);

            double x = (point3D.x * cosDegrees) + (point3D.y * sinDegrees);
            double y = (point3D.x * -sinDegrees) + (point3D.y * cosDegrees);

            return new Vector3D(x, y, point3D.z);
        }

        public static Vector3D Translate(Vector3D points3D, Vector3D oldOrigin, Vector3D newOrigin)
        {
            var difference = new Vector3D(newOrigin.x - oldOrigin.x, newOrigin.y - oldOrigin.y, newOrigin.z - oldOrigin.z);
            points3D.x += difference.x;
            points3D.y += difference.y;
            points3D.z += difference.z;
            return points3D;
        }

        public static Vector3D[] RotateX(Vector3D[] points3D, float degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateX(points3D[i], degrees);
            }
            return points3D;
        }

        public static Vector3D[] RotateY(Vector3D[] points3D, float degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateY(points3D[i], degrees);
            }
            return points3D;
        }

        public static Vector3D[] RotateZ(Vector3D[] points3D, float degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateZ(points3D[i], degrees);
            }
            return points3D;
        }

        public static Vector3D[] Translate(Vector3D[] points3D, Vector3D oldOrigin, Vector3D newOrigin)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = Translate(points3D[i], oldOrigin, newOrigin);
            }
            return points3D;
        }
    }
}