using System;
using System.Collections.Generic;
using System.Text;

namespace Hailstone
{
    /// <summary>
    /// A visual representation of a entry in a domain.
    /// </summary>
    public class Stone
    {
        public Stone(Entry Entry)
        {
            this.Entry = Entry;
            this.Radius = Math.Log10((double)Entry.Value + 100.0) * 0.25;
        }

        /// <summary>
        /// The entry this stone represents.
        /// </summary>
        public readonly Entry Entry;

        /// <summary>s
        /// The radius of this stone.
        /// </summary>
        public double Radius;

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
        /// Gets the target link length for this stone.
        /// </summary>
        public double TargetLinkLength
        {
            get
            {
                return this.Radius * Settings.Current.LinkTargetLength;
            }
        }

        /// <summary>
        /// Handles the interaction between two unique stones.
        /// </summary>
        public static void Interact(Stone A, Stone B, double Time)
        {
            Vector dif = B.Position - A.Position;
            double len = Math.Max(dif.Length, A.Radius + B.Radius);
            Vector force = dif * (Time * Settings.Current.StoneRepelForce / (len * len * len));
            A.Velocity -= force;
            B.Velocity += force;
        }

        /// <summary>
        /// Updates a stone.
        /// </summary>
        public void Update(double Time, double Damping)
        {
            Stone next = this.Next;
            if (next != null && this != next)
            {
                Vector dif = next.Position - this.Position;
                double len = dif.Length;
                double power = len - this.TargetLinkLength;
                power = Math.Min(10.0, Math.Abs(power) * power);
                Vector force = dif * (Time * Settings.Current.StoneLinkForce * power / len);
                this.Velocity += force;
                next.Velocity -= force;
            }

            this.Position += this.Velocity * Time;

            if (double.IsNaN(this.Position.X) || double.IsNaN(this.Position.Y))
            {
                this.Position = Vector.Zero;
                this.Velocity = Vector.Zero;
            }
            else
            {
                this.Velocity *= Damping;
            }
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
        /// The current pulse phase (in radians) for the selected stones.
        /// </summary>
        public static double SelectionPulsePhase = 0.0;

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
