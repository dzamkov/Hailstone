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
        public Stone(Vector Position, uint Value)
        {
            this.Position = Position;
            this.Value = Value;
        }

        /// <summary>
        /// The value this stone represents.
        /// </summary>
        public readonly uint Value;

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
    }
}
