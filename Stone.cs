using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hailstone
{
    /// <summary>
    /// A visual representation of a value in a sequence (what else am I supposed to call these things?).
    /// </summary>
    public class Stone
    {
        public Stone(uint Value)
        {
            this.Number = Value;
            this.Radius = Math.Log10((double)Value + 100.0) * 0.25;
        }

        /// <summary>
        /// The size of the numbers on a stone.
        /// </summary>
        public static readonly double NumberSize = 0.3;

        /// <summary>
        /// The width of the links between stones.
        /// </summary>
        public static readonly double LinkWidth = 0.07;

        /// <summary>
        /// The minimum length between stones before an arrow link appears.
        /// </summary>
        public static readonly double LinkArrowLength = 0.8;

        /// <summary>
        /// The target length for a link.
        /// </summary>
        public static readonly double LinkTargetLength = 1.7;

        /// <summary>
        /// The force per unit length applied by a link.
        /// </summary>
        public static readonly double LinkForce = 25.0;

        /// <summary>
        /// The repulsion force applied by nearby stones.
        /// </summary>
        public static readonly double RepelForce = 60.0;

        /// <summary>
        /// The drag force for stones.
        /// </summary>
        public static readonly double DragForce = 10.0;

        /// <summary>
        /// The number this stone represents.
        /// </summary>
        public readonly uint Number;

        /// <summary>
        /// The radius of this stone.
        /// </summary>
        public readonly double Radius;

        /// <summary>
        /// Gets the mass of this stone.
        /// </summary>
        public double Mass
        {
            get
            {
                return this.Radius;
            }
        }

        /// <summary>
        /// The position of this stone in world-space.
        /// </summary>
        public Vector Position;

        /// <summary>
        /// The velocity of this stone.
        /// </summary>
        public Vector Velocity;

        /// <summary>
        /// The next stone after this one in the sequence, or null if does not exist.
        /// </summary>
        public Stone Next;

        /// <summary>
        /// Applies an impulse to this stone.
        /// </summary>
        public void Impluse(Vector Impulse)
        {
            this.Velocity += Impulse * (1.0 / this.Mass);
        }

        /// <summary>
        /// Handles the interaction between two unique stones.
        /// </summary>
        public static void Interact(Stone A, Stone B, double RepelImpulse)
        {
            Vector dif = B.Position - A.Position;
            double len = Math.Max(dif.Length, A.Radius + B.Radius);
            A.Impluse(dif * -RepelImpulse / (len * len * len));
            B.Impluse(dif * RepelImpulse / (len * len * len));
        }

        /// <summary>
        /// Updates a stone.
        /// </summary>
        public void Update(double LinkImpulse, double Time)
        {
            Stone next = this.Next;
            if (next != null && this != next)
            {
                Vector dif = next.Position - this.Position;
                double len = dif.Length;
                this.Impluse(dif * LinkImpulse * (len - Stone.LinkTargetLength));
                next.Impluse(dif * -LinkImpulse * (len - Stone.LinkTargetLength));
            }

            this.Position += this.Velocity * Time;

            double speed = this.Velocity.Length;
            double drag = Stone.DragForce * this.Radius / this.Mass;
            if (speed > 0.001) this.Velocity *= (Math.Sqrt(1.0 + 4.0 * speed * drag * Time) - 1.0) / (2.0 * speed * drag * Time);
        }

        /// <summary>
        /// Gets or sets the currently-selected chain.
        /// </summary>
        public static Chain Selection
        {
            get
            {
                return _Selection;
            }
            set
            {
                if (_Selection != value)
                {
                    if (_Selection != null)
                    {
                        foreach (Stone stone in _Selection.Stones)
                        {
                            stone._SelectionIndex = uint.MaxValue;
                        }
                    }
                    if (value != null)
                    {
                        for (int t = 0; t < value.Stones.Count; t++)
                        {
                            value.Stones[t]._SelectionIndex = (uint)t;
                        }
                    }
                    _Selection = value;
                }
            }
        }

        /// <summary>
        /// The current glow phase (in radians) for the selected stones.
        /// </summary>
        public static double SelectionGlowPhase = 0.0;

        /// <summary>
        /// Gets the selection index for the given stone.
        /// </summary>
        public static uint GetSelectionIndex(Stone Stone)
        {
            return Stone._SelectionIndex;
        }

        private uint _SelectionIndex = uint.MaxValue;
        private static Chain _Selection;
    }

    /// <summary>
    /// Identifies a chain of stones.
    /// </summary>
    public class Chain
    {
        public Chain(Stone Start, Stone End)
        {
            Stone cur = Start;
            List<Stone> stones = new List<Stone>();
            stones.Add(cur);
            while (cur != End)
            {
                cur = cur.Next;
                stones.Add(cur);
            }
            this.Length = stones.Count - 1;
            this.Stones = stones;
        }

        /// <summary>
        /// The amount of steps in this chain.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The stones in this chain, in the order in which they appear. Note that the size of the list is one larger than 
        /// the length of the chain.
        /// </summary>
        public readonly List<Stone> Stones;

        /// <summary>
        /// Gets the stone at the start of this chain.
        /// </summary>
        public Stone Start
        {
            get
            {
                return this.Stones[0];
            }
        }

        /// <summary>
        /// Gets the stone at the end of this chain.
        /// </summary>
        public Stone End
        {
            get
            {
                return this.Stones[this.Length];
            }
        }
    }
}
