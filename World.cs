using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hailstone
{
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
        /// Gets a stone at the given location, or null if there's none there.
        /// </summary>
        public Stone Pick(Vector Point)
        {
            foreach (Stone stone in this.Stones.Values)
            {
                if ((Point - stone.Position).Length <= stone.Radius)
                    return stone;
            }
            return null;
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
