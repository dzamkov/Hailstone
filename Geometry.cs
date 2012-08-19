using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hailstone
{
    /// <summary>
    /// Represents a point or offset in two-dimensional space.
    /// </summary>
    public struct Vector
    {
        public Vector(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// The zero vector.
        /// </summary>
        public static readonly Vector Zero = new Vector(0.0, 0.0);

        /// <summary>
        /// The horizontal coordinate of this vector, ascending to the right.
        /// </summary>
        public double X;

        /// <summary>
        /// The vertical coordinate of this vector, ascending upwards.
        /// </summary>
        public double Y;

        /// <summary>
        /// Gets the sum of two vectors.
        /// </summary>
        public static Vector operator +(Vector A, Vector B)
        {
            return new Vector(A.X + B.X, A.Y + B.Y);
        }

        /// <summary>
        /// Gets the difference of two vectors.
        /// </summary>
        public static Vector operator -(Vector A, Vector B)
        {
            return new Vector(A.X - B.X, A.Y - B.Y);
        }

        /// <summary>
        /// Gets the inverse of a vector.
        /// </summary>
        public static Vector operator -(Vector A)
        {
            return new Vector(-A.X, -A.Y);
        }

        /// <summary>
        /// Gets the product of a vector and a scalar.
        /// </summary>
        public static Vector operator *(Vector A, double B)
        {
            return new Vector(A.X * B, A.Y * B);
        }

        /// <summary>
        /// Gets the inverse product of a vector and a scalar.
        /// </summary>
        public static Vector operator /(Vector A, double B)
        {
            return new Vector(A.X / B, A.Y / B);
        }

        /// <summary>
        /// Gets the dot product of two vectors.
        /// </summary>
        public static double Dot(Vector A, Vector B)
        {
            return A.X * B.X + A.Y * B.Y;
        }

        public static implicit operator OpenTK.Vector2d(Vector Source)
        {
            return new OpenTK.Vector2d(Source.X, Source.Y);
        }

        public static implicit operator OpenTK.Vector3d(Vector Source)
        {
            return new OpenTK.Vector3d(Source.X, Source.Y, 0.0);
        }

        /// <summary>
        /// Gets the length of this vector.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(this.X * this.X + this.Y * this.Y);
            }
        }

        /// <summary>
        /// Gets the normal form of this vector.
        /// </summary>
        public Vector Normal
        {
            get
            {
                return this / this.Length;
            }
        }

        /// <summary>
        /// Gets the cross product of a vector (equivalent to rotating the vector 90 degrees counter-clockwise).
        /// </summary>
        public Vector Cross
        {
            get
            {
                return new Vector(-this.Y, this.X);
            }
        }
    }

    /// <summary>
    /// An axis-aligned rectangle in two-dimensional space.
    /// </summary>
    public struct Rectangle
    {
        public Rectangle(double Left, double Top, double Right, double Bottom)
        {
            this.Left = Left;
            this.Top = Top;
            this.Right = Right;
            this.Bottom = Bottom;
        }

        public Rectangle(Vector TopLeft, Vector BottomRight)
        {
            this.Left = TopLeft.X;
            this.Top = TopLeft.Y;
            this.Right = BottomRight.X;
            this.Bottom = BottomRight.Y;
        }

        /// <summary>
        /// The horizontal component of the left edge of the rectangle.
        /// </summary>
        public double Left;

        /// <summary>
        /// The vertical component of the top edge of the rectangle.
        /// </summary>
        public double Top;

        /// <summary>
        /// The horizontal component of the right edge of the rectangle.
        /// </summary>
        public double Right;

        /// <summary>
        /// The vertical component of the bottom edge of the rectangle.
        /// </summary>
        public double Bottom;

        /// <summary>
        /// Gets the top-left corner of this rectangle.
        /// </summary>
        public Vector TopLeft
        {
            get
            {
                return new Vector(this.Left, this.Top);
            }
        }

        /// <summary>
        /// Gets the top-right corner of this rectangle.
        /// </summary>
        public Vector TopRight
        {
            get
            {
                return new Vector(this.Right, this.Top);
            }
        }

        /// <summary>
        /// Gets the left-left corner of this rectangle.
        /// </summary>
        public Vector  BottomLeft
        {
            get
            {
                return new Vector(this.Left, this.Bottom);
            }
        }

        /// <summary>
        /// Gets the bottom-right corner of this rectangle.
        /// </summary>
        public Vector BottomRight
        {
            get
            {
                return new Vector(this.Right, this.Bottom);
            }
        }
    }

    /// <summary>
    /// An affline transform in two-dimensional space.
    /// </summary>
    public struct Transform
    {
        public Transform(Vector Offset, Vector Right, Vector Up)
        {
            this.Offset = Offset;
            this.Right = Right;
            this.Up = Up;
        }

        /// <summary>
        /// The identity transform.
        /// </summary>
        public static readonly Transform Identity = new Transform(Vector.Zero, new Vector(1.0, 0.0), new Vector(0.0, 1.0));

        /// <summary>
        /// The offset of this transform.
        /// </summary>
        public Vector Offset;

        /// <summary>
        /// The right vector in this transform.
        /// </summary>
        public Vector Right;

        /// <summary>
        /// The up vector in this transform.
        /// </summary>
        public Vector Up;

        /// <summary>
        /// Applies this transform to a vector.
        /// </summary>
        public Vector Project(Vector Source)
        {
            return this.Offset + this.Right * Source.X + this.Up * Source.Y;
        }

        /// <summary>
        /// Applies this transform to a vector.
        /// </summary>
        public Vector Project(double X, double Y)
        {
            return this.Offset + this.Right * X + this.Up * Y;
        }

        /// <summary>
        /// Composes two transforms such that projection in the returned transform is equivalent to
        /// projection in A followed by projection in B.
        /// </summary>
        public static Transform Compose(Transform A, Transform B)
        {
            return new Transform(
                B.Project(A.Offset),
                B.Right * A.Right.X + B.Up * A.Right.Y,
                B.Right * A.Up.X + B.Up * A.Up.Y);
        }

        /// <summary>
        /// Create a translation transform.
        /// </summary>
        public static Transform Translate(Vector Amount)
        {
            return new Transform(Amount, new Vector(1.0, 0.0), new Vector(0.0, 1.0));
        }

        /// <summary>
        /// Create a scale transform.
        /// </summary>
        public static Transform Scale(double Amount)
        {
            return new Transform(Vector.Zero, new Vector(Amount, 0.0), new Vector(0.0, Amount));
        }

        /// <summary>
        /// Create a scale transform.
        /// </summary>
        public static Transform Scale(double X, double Y)
        {
            return new Transform(Vector.Zero, new Vector(X, 0.0), new Vector(0.0, Y));
        }

        /// <summary>
        /// Creates a rotation transform that rotates by the given radian angle counter-clockwise.
        /// </summary>
        public static Transform Rotate(double Angle)
        {
            double cos = Math.Cos(Angle);
            double sin = Math.Sin(Angle);
            return new Transform(Vector.Zero, new Vector(cos, sin), new Vector(-sin, cos));
        }

        public static Transform operator *(Transform A, Transform B)
        {
            return Transform.Compose(A, B);
        }

        public static Vector operator *(Transform A, Vector B)
        {
            return A.Project(B);
        }

        public static implicit operator OpenTK.Matrix4d(Transform Source)
        {
            double a = Source.Right.X;
            double b = Source.Right.Y;
            double c = Source.Up.X;
            double d = Source.Up.Y;
            double e = Source.Offset.X;
            double f = Source.Offset.Y;
            return new OpenTK.Matrix4d(a, b, 0.0, 0.0, c, d, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, e, f, 0.0, 1.0);
        }
           
        /// <summary>
        /// Gets the inverse of this transform.
        /// </summary>
        public Transform Inverse
        {
            get
            {
                double det = 1.0 / (this.Right.X * this.Up.Y - this.Right.Y * this.Up.X);
                return new Transform(
                    new Vector(
                        (this.Up.X * this.Offset.Y - this.Up.Y * this.Offset.X) * det,
                        (this.Right.Y * this.Offset.X - this.Right.X * this.Offset.Y) * det),
                    new Vector(this.Up.Y * det, this.Right.Y * -det),
                    new Vector(this.Up.X * -det, this.Right.X * det));
            }
        }
    }
}
