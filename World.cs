using System;
using System.Collections.Generic;
using System.Text;

namespace Hailstone
{
    /// <summary>
    /// An identifier for a zone in a world.
    /// </summary>
    public struct ZoneIndex
    {
        public ZoneIndex(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// The X coordinate of the zone.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate of the zone.
        /// </summary>
        public int Y;

        public static bool operator ==(ZoneIndex A, ZoneIndex B)
        {
            return A.X == B.X && A.Y == B.Y;
        }

        public static bool operator !=(ZoneIndex A, ZoneIndex B)
        {
            return A.X != B.X || A.Y != B.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is ZoneIndex)
                return this == (ZoneIndex)obj;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.X * 12289 + this.Y * 196613;
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
        }

        /// <summary>
        /// The edge-length of a zone in a world.
        /// </summary>
        public static readonly double ZoneSize = 7.0;

        /// <summary>
        /// The sequence that defines the relationships between stones.
        /// </summary>
        public readonly Sequence Sequence;

        /// <summary>
        /// Gets a stone with the given number, or null if it is not in the world.
        /// </summary>
        public Stone this[uint Number]
        {
            get
            {
                Stone stone;
                this._Stones.TryGetValue(Number, out stone);
                return stone;
            }
        }

        /// <summary>
        /// Gets the amount of stones in this world.
        /// </summary>
        public int StoneCount
        {
            get
            {
                return this._Stones.Count;
            }
        }

        /// <summary>
        /// Tries inserting a stone for the given number into this world. If one already exists, it will be
        /// returned.
        /// </summary>
        public Stone Insert(uint Number)
        {
            Stone stone;
            if (this._Stones.TryGetValue(Number, out stone)) return stone;
            this._Stones[Number] = stone = new Stone(Number);
            int influences = 0;

            List<Stone> unlinked;
            if (this._Unlinked.TryGetValue(Number, out unlinked))
            {
                influences += unlinked.Count;
                foreach (Stone s in unlinked)
                {
                    stone.Position += s.Position;
                    s.Next = stone;
                }
                this._Unlinked.Remove(Number);
            }

            uint next = this.Sequence.Next(Number);
            Stone nexts;
            if (this._Stones.TryGetValue(next, out nexts))
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
                    if (!this._Unlinked.TryGetValue(next, out unlinked))
                        this._Unlinked[next] = unlinked = new List<Stone>();
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

            this._Insert(stone, _GetZone(stone.Position));
            return stone;
        }

        /// <summary>
        /// Gets a stone at the given location, or null if there's none there.
        /// </summary>
        public Stone Pick(Vector Point)
        {
            foreach (Stone stone in this._Stones.Values)
            {
                if ((Point - stone.Position).Length <= stone.Radius)
                    return stone;
            }
            return null;
        }

        /// <summary>
        /// Allows a stone to interact with all other stones in the zone with the given index.
        /// </summary>
        private void _Interact(Stone Stone, ZoneIndex Index, double RepelImpulse)
        {
            List<Stone> zone;
            if (this._Zones.TryGetValue(Index, out zone))
            {
                foreach (Stone other in zone)
                {
                    Stone.Interact(Stone, other, RepelImpulse);
                }
            }
        }

        /// <summary>
        /// Updates the state of the world by the given time in seconds.
        /// </summary>
        public void Update(double Time)
        {
            double repelimpulse = Stone.RepelForce * Time;
            foreach (var kvp in this._Zones)
            {
                ZoneIndex index = kvp.Key;
                List<Stone> zone = kvp.Value;
                foreach (Stone a in zone)
                {
                    foreach (Stone b in zone)
                    {
                        if (a != b && a.Number > b.Number)
                        {
                            Stone.Interact(a, b, repelimpulse);
                        }
                    }
                    this._Interact(a, new ZoneIndex(index.X + 1, index.Y), repelimpulse);
                    this._Interact(a, new ZoneIndex(index.X + 1, index.Y + 1), repelimpulse);
                    this._Interact(a, new ZoneIndex(index.X, index.Y + 1), repelimpulse);
                    this._Interact(a, new ZoneIndex(index.X - 1, index.Y), repelimpulse);
                }
            }

            List<Action> changes = new List<Action>();
            double linkimpulse = Stone.LinkForce * Time;
            foreach (var kvp in this._Zones)
            {
                ZoneIndex index = kvp.Key;
                List<Stone> zone = kvp.Value;
                foreach (Stone stone in zone)
                {
                    stone.Update(linkimpulse, Time);
                    ZoneIndex nextindex = _GetZone(stone.Position);
                    if (nextindex != index) 
                    {
                        List<Stone> z = zone;
                        Stone s = stone;
                        ZoneIndex f = index;
                        ZoneIndex t = nextindex;
                        changes.Add(delegate
                        {
                            this._Remove(s, f, z);
                            this._Insert(s, t);
                        });
                    }
                }
            }
            foreach (Action change in changes)
            {
                change();
            }
        }

        /// <summary>
        /// Renders this world to the current context.
        /// </summary>
        public void Render(double Extent)
        {
            Render r;
            Atlas.Begin(out r);
            foreach (Stone stone in this._Stones.Values)
            {
                Atlas.DrawStone(r, stone, Extent);
            }
            Atlas.End(r);
        }

        /// <summary>
        /// Gets the zone for the given position.
        /// </summary>
        private static ZoneIndex _GetZone(Vector Position)
        {
            return new ZoneIndex((int)Math.Floor(Position.X / ZoneSize), (int)Math.Floor(Position.Y / ZoneSize));
        }

        /// <summary>
        /// Inserts a STONE into a ZONE.
        /// </summary>
        private void _Insert(Stone Stone, ZoneIndex Index)
        {
            List<Stone> zone;
            if (!this._Zones.TryGetValue(Index, out zone))
                this._Zones[Index] = zone = new List<Stone>();
            zone.Add(Stone);
        }

        /// <summary>
        /// Removes a stone from a zone.
        /// </summary>
        private void _Remove(Stone Stone, ZoneIndex Index, List<Stone> Zone)
        {
            Zone.Remove(Stone);
            if (Zone.Count == 0) this._Zones.Remove(Index);
        }

        private Dictionary<ZoneIndex, List<Stone>> _Zones = new Dictionary<ZoneIndex, List<Stone>>();
        private Dictionary<uint, Stone> _Stones = new Dictionary<uint, Stone>();
        private Dictionary<uint, List<Stone>> _Unlinked = new Dictionary<uint, List<Stone>>();
    }
}
