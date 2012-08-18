using System;
using System.Collections.Generic;

namespace Hailstone
{
    /// <summary>
    /// Defines an iterative sequence.
    /// </summary>
    public abstract class Sequence
    {
        /// <summary>
        /// Gets the integer after the given integer in the sequence. An overflow exception should be thrown
        /// if the next value can not be represented by a uint.
        /// </summary>
        public abstract uint Next(uint Value);

        /// <summary>
        /// Gets a set of integers that includes all integers that have the given integer as their next
        /// value in the sequence.
        /// </summary>
        public abstract IEnumerable<uint> PreviousBound(uint Value);

        /// <summary>
        /// Gets the set of all integer that have the given integer as their next value in the sequence.
        /// </summary>
        public IEnumerable<uint> Previous(uint Value)
        {
            foreach (uint possible in this.PreviousBound(Value))
            {
                if (this.Next(possible) == Value)
                    yield return possible;
            }
        }
    }

    /// <summary>
    /// Defines the hailstone sequence.
    /// </summary>
    public sealed class HailstoneSequence : Sequence
    {
        private HailstoneSequence()
        {

        }

        /// <summary>
        /// The only instance of this class.
        /// </summary>
        public static readonly HailstoneSequence Instance = new HailstoneSequence();

        public override uint Next(uint Value)
        {
            if (Value % 2 == 0)
                return Value / 2;
            else
                return checked (Value * 3 + 1);
        }

        public override IEnumerable<uint> PreviousBound(uint Value)
        {
            return new uint[] { Value * 2, (Value - 1) / 3 };
        }
    }
}
