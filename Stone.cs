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
        public static readonly double LinkWidth = 0.05;

        /// <summary>
        /// The minimum length between stones before an arrow link appears.
        /// </summary>
        public static readonly double LinkArrowLength = 1.0;

        /// <summary>
        /// The force per unit length applied by a link.
        /// </summary>
        public static readonly double LinkForce = 0.6;

        /// <summary>
        /// The repulsion force applied by nearby stones.
        /// </summary>
        public static readonly double RepelForce = 20.0;

        /// <summary>
        /// The drag force for stones.
        /// </summary>
        public static readonly double DragForce = 1.0;

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
                this.Impluse(dif * LinkImpulse * len);
                next.Impluse(dif * -LinkImpulse * len);
            }

            this.Position += this.Velocity * Time;

            double speed = this.Velocity.Length;
            double drag = Stone.DragForce * this.Radius / this.Mass;
            if (speed > 0.001) this.Velocity *= (Math.Sqrt(1.0 + 4.0 * speed * drag * Time) - 1.0) / (2.0 * speed * drag * Time);
        }
    }

    /// <summary>
    /// An organized collection of stones.
    /// </summary>
    public class World
    {
        public World(Sequence Sequence)
        {
            this.Sequence = Sequence;
            this.Stones = new Dictionary<uint, Stone>();
            this.Unlinked = new Dictionary<uint, List<Stone>>();
        }

        /// <summary>
        /// The sequence that defines the relationships between stones.
        /// </summary>
        public readonly Sequence Sequence;

        /// <summary>
        /// A mapping from integer values to their corresponding stone in the world.
        /// </summary>
        public Dictionary<uint, Stone> Stones;

        /// <summary>
        /// A mapping from integer values to unlinked stones that have that as their next value.
        /// </summary>
        public Dictionary<uint, List<Stone>> Unlinked;

        /// <summary>
        /// Tries inserting a stone for the given number into this world. If one already exists, it will be
        /// returned.
        /// </summary>
        public Stone Insert(uint Number)
        {
            Stone stone;
            if (this.Stones.TryGetValue(Number, out stone)) return stone;
            this.Stones[Number] = stone = new Stone(Number);
            int influences = 0;

            List<Stone> unlinked;
            if (this.Unlinked.TryGetValue(Number, out unlinked))
            {
                influences += unlinked.Count;
                foreach (Stone s in unlinked)
                {
                    stone.Position += s.Position;
                    s.Next = stone;
                }
                this.Unlinked.Remove(Number);
            }

            uint next = this.Sequence.Next(Number);
            Stone nexts;
            if (this.Stones.TryGetValue(next, out nexts))
            {
                stone.Position += nexts.Position;
                stone.Next = nexts;
                influences++;
            }
            else
            {
                if (Number == next)
                    stone.Next = stone;
                else
                {
                    if (!this.Unlinked.TryGetValue(next, out unlinked))
                        this.Unlinked[next] = unlinked = new List<Stone>();
                    unlinked.Add(stone);
                }
            }

            if (influences > 0)
            {
                stone.Position /= (double)influences;
                stone.Velocity /= (double)influences;
                stone.Position *= 1.00001;
                stone.Velocity *= 1.3;
            }
            else
            {
                double phase = Math.Sqrt((double)Number * 10.0);
                stone.Position = new Vector(Math.Cos(phase), Math.Sin(phase)) * phase;
            }

            return stone;
        }

        /// <summary>
        /// Updates the state of the world by the given time in seconds.
        /// </summary>
        public void Update(double Time)
        {
            double repelimpulse = Stone.RepelForce * Time;
            foreach (Stone a in this.Stones.Values)
            {
                foreach (Stone b in this.Stones.Values)
                {
                    if (a != b && a.Number > b.Number)
                    {
                        Stone.Interact(a, b, repelimpulse);
                    }
                }
            }

            double linkimpulse = Stone.LinkForce * Time;
            foreach (Stone stone in this.Stones.Values)
            {
                stone.Update(linkimpulse, Time);
            }
        }

        /// <summary>
        /// Renders this world to the current context.
        /// </summary>
        public void Render(double Extent)
        {
            Render r;
            Atlas.Begin(out r);
            foreach (Stone stone in this.Stones.Values)
            {
                Atlas.DrawStone(r, stone, Extent);
            }
            Atlas.End(r);
        }
    }
}
