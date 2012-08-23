using System;
using System.Collections.Generic;
using System.Text;

using Hailstone.Interface;

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
    /// A visual representation for a subset of a domain, with stones representing entries.
    /// </summary>
    public class World
    {
        public World(Domain Domain)
        {
            this.Domain = Domain;
            this._Stones = new Dictionary<Entry, Stone>();
        }

        /// <summary>
        /// The edge-length of a zone in a world.
        /// </summary>
        public static readonly double ZoneSize = 7.0;

        /// <summary>
        /// Gets the domain this world is associated with.
        /// </summary>
        public readonly Domain Domain;

        /// <summary>
        /// Gets the stone for the given entry, or null if it is not in the world.
        /// </summary>
        public Stone this[Entry Entry]
        {
            get
            {
                Stone stone;
                this._Stones.TryGetValue(Entry, out stone);
                return stone;
            }
        }

        /// <summary>
        /// Tries to get a stone for the given entry. Returns false if none exists in this world.
        /// </summary>
        public bool TryGetStone(Entry Entry, out Stone Stone)
        {
            return this._Stones.TryGetValue(Entry, out Stone);
        }

        /// <summary>
        /// Determines whether this world contains a stone for the given entry.
        /// </summary>
        public bool ContainsEntry(Entry Entry)
        {
            return this._Stones.ContainsKey(Entry);
        }

        /// <summary>
        /// Determines whether this world contains the given stone.
        /// </summary>
        public bool ContainsStone(Stone Stone)
        {
            return this._Stones.ContainsValue(Stone);
        }

        /// <summary>
        /// Gets the amount of stones in this world.
        /// </summary>
        public int Count
        {
            get
            {
                return this._Stones.Count;
            }
        }

        /// <summary>
        /// Inserts a stone for the given entry. There must not be any existing stones for the entry.
        /// </summary>
        public Stone Insert(Entry Entry, Vector Position, Vector Velocity)
        {
            Stone stone = new Stone(Entry);
            stone.Position = Position;
            stone.Velocity = Velocity;
            this._Introduce(stone, Entry);
            return stone;
        }

        /// <summary>
        /// Inserts a stone for the given entry. There must not be any existing stones for the entry.
        /// </summary>
        public void Insert(Stone Stone)
        {
            this._Introduce(Stone, Stone.Entry);
        }

        /// <summary>
        /// Introduces a stone to the world.
        /// </summary>
        private void _Introduce(Stone Stone, Entry Entry)
        {
            foreach (Entry p in Entry.Previous)
            {
                Stone prev;
                if (this._Stones.TryGetValue(p, out prev))
                    prev.Next = Stone;
            }
            Entry n = Entry.Next;
            Stone next;
            if (n != null && this._Stones.TryGetValue(n, out next))
                Stone.Next = next;
            this._Stones[Entry] = Stone;
            this._Insert(Stone, _GetZone(Stone.Position));
            this.MakeLeader(Stone);
        }

        /// <summary>
        /// Updates the set of leader stones in response to a change in the associated domain.
        /// </summary>
        public void UpdateLeaders()
        {
            foreach (Stone stone in this._Stones.Values)
                this.MakeLeader(stone);
        }

        /// <summary>
        /// Makes the given stone in this world a leader stone, allowing it to admit new stones for
        /// adjacent entries in the associated domain.
        /// </summary>
        public void MakeLeader(Stone Stone)
        {
            _Leader leader = new _Leader();
            if (leader.Reset(this, Stone)) this._Leaders.Add(Stone, leader);
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
        private void _Interact(Stone Stone, ZoneIndex Index, double Time)
        {
            List<Stone> zone;
            if (this._Zones.TryGetValue(Index, out zone))
            {
                foreach (Stone other in zone)
                {
                    Stone.Interact(Stone, other, Time);
                }
            }
        }

        /// <summary>
        /// Updates the state of the world by the given time in seconds.
        /// </summary>
        public void Update(double Time)
        {
            foreach (var kvp in this._Zones)
            {
                ZoneIndex index = kvp.Key;
                List<Stone> zone = kvp.Value;
                foreach (Stone a in zone)
                {
                    foreach (Stone b in zone)
                    {
                        if (a != b && a.GetHashCode() > b.GetHashCode())
                        {
                            Stone.Interact(a, b, Time);
                        }
                    }
                    this._Interact(a, new ZoneIndex(index.X + 1, index.Y), Time);
                    this._Interact(a, new ZoneIndex(index.X + 1, index.Y + 1), Time);
                    this._Interact(a, new ZoneIndex(index.X, index.Y + 1), Time);
                    this._Interact(a, new ZoneIndex(index.X - 1, index.Y), Time);
                }
            }

            List<Action> changes = new List<Action>();
            double damping = Math.Pow(Settings.Current.StoneDamping, Time);
            foreach (var kvp in this._Zones)
            {
                ZoneIndex index = kvp.Key;
                List<Stone> zone = kvp.Value;
                foreach (Stone stone in zone)
                {
                    stone.Update(Time, damping);
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
            
            double pressuredamping = Math.Pow(Settings.Current.StoneIntroductionPressureExpansion, Time);
            foreach (var kvp in this._Leaders)
            {
                Stone s = kvp.Key;
                _Leader l = kvp.Value;
                Stone i;
                if (l.Update(this, s, Time, pressuredamping, out i))
                {
                    if (!l.Reset(this, s)) changes.Add(delegate { this._Leaders.Remove(s); });
                    if (i != null) changes.Add(delegate { if (!this.ContainsEntry(i.Entry)) this.Insert(i); });
                }
                else
                {
                    if (l.Timeout <= 0.0) changes.Add(delegate { this._Leaders.Remove(s); });
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
        public void Render(Rectangle Bounds, double Extent)
        {
            ZoneIndex bl = _GetZone(Bounds.BottomLeft);
            ZoneIndex tr = _GetZone(Bounds.TopRight);
            bl.X -= 1; tr.X += 1;
            bl.Y -= 1; tr.Y += 1;
            int zonecount = (tr.X - bl.X) * (tr.Y - bl.Y);

            Render r;
            Atlas.Begin(out r);
            List<Stone> selected = new List<Stone>();
            if (zonecount < this._Zones.Count)
            {
                for (int x = bl.X; x <= tr.X; x++)
                    for (int y = bl.Y; y <= tr.Y; y++)
                    {
                        List<Stone> zone;
                        if (this._Zones.TryGetValue(new ZoneIndex(x, y), out zone))
                            this._DrawZone(r, zone, Extent, selected);
                    }
            }
            else
            {
                foreach (var kvp in this._Zones)
                {
                    ZoneIndex index = kvp.Key;
                    int x = index.X;
                    int y = index.Y;
                    if (x >= bl.X && x <= tr.X && y >= bl.Y && y <= tr.Y)
                        this._DrawZone(r, kvp.Value, Extent, selected);
                }
            }
            foreach (Stone stone in selected)
                Atlas.DrawStone(r, stone, Extent);
            Atlas.End(r);
        }

        /// <summary>
        /// Draws all the stones in the given zone.
        /// </summary>
        private void _DrawZone(Render Render, List<Stone> Zone, double Extent, List<Stone> Selected)
        {
            foreach (Stone stone in Zone)
            {
                if (Stone.GetSelectionIndex(stone) != uint.MaxValue)
                    Selected.Add(stone);
                else
                    Atlas.DrawStone(Render, stone, Extent);
            }
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

        /// <summary>
        /// Gets the repulsive pressure felt by the given stone.
        /// </summary>
        private double _Pressure(Stone Stone, out Vector Direction)
        {
            double pressure = 0.0;
            Direction = Vector.Zero;
            ZoneIndex centerindex = _GetZone(Stone.Position);
            for (int x = centerindex.X - 1; x <= centerindex.X + 1; x++)
                for (int y = centerindex.Y - 1; y <= centerindex.Y + 1; y++)
                {
                    List<Stone> zone;
                    if (this._Zones.TryGetValue(new ZoneIndex(x, y), out zone))
                        foreach (Stone other in zone)
                            if (other != Stone)
                            {
                                Vector dif = Stone.Position - other.Position;
                                double len = Math.Max(dif.Length, other.Radius);
                                double mag = 1.0 / (len * len);
                                Vector force = dif * (mag / len);
                                Direction += force;
                                pressure += mag;
                            }
                }
            return pressure;
        }

        /// <summary>
        /// Contains information about a leader stone, a stone that will introduce new entries.
        /// </summary>
        private class _Leader
        {
            /// <summary>
            /// The maximum pressure for the leader stone before the next stone is introduced. This will
            /// decrease over time to insure the new stone will be admitted eventually.
            /// </summary>
            public double PressureThreshold;

            /// <summary>
            /// The idle time before this leader is discarded.
            /// </summary>
            public double Timeout;

            /// <summary>
            /// The next entry to be introduced by this leader.
            /// </summary>
            public Entry Next;

            /// <summary>
            /// Prepares this leader for the next entry. Returns false if there are no entries left to introduce.
            /// </summary>
            public bool Reset(World World, Stone Stone)
            {
                this.PressureThreshold = Settings.Current.StoneIntroductionPressureThreshold;
                this.Timeout = Settings.Current.StoneIntroductionTimeout;
                this.Next = GetNext(World, Stone);
                return this.Next != null;
            }

            /// <summary>
            /// Updates this leader state by the given amount of time. Returns true if a new stone was introduced.
            /// </summary>
            public bool Update(World World, Stone Stone, double Time, double Expansion, out Stone Introduced)
            {
                Vector dir;
                if (World._Pressure(Stone, out dir) < this.PressureThreshold)
                {
                    if (World.ContainsEntry(this.Next))
                    {
                        this.Next = GetNext(World, Stone);
                        if (this.Next == null)
                        {
                            Introduced = null;
                            return true;
                        }
                    }

                    Introduced = new Stone(this.Next);
                    Introduced.Position = Stone.Position;
                    double dirlen = dir.Length;
                    if (dirlen > 0.001)
                    {
                        dir = dir.Normal;
                        Introduced.Velocity = Stone.Velocity + dir * Settings.Current.StoneIntroductionSpeed;
                        Introduced.Position += dir * Stone.Radius;
                    }
                    _Tweak(this.Next, ref Introduced.Position);
                    return true;
                }
                else
                {
                    this.PressureThreshold *= Expansion;
                    this.Timeout -= Time;
                    Introduced = null;
                    return false;
                }
            }

            /// <summary>
            /// Changes a vector slightly to make it distinct from other vectors.
            /// </summary>
            private static void _Tweak(object Object, ref Vector Vector)
            {
                int h = Object.GetHashCode();
                int x = h & 0xFF;
                int y = (h & 0xFF00) >> 8;
                Vector.X += (x - 127.0) * 0.0001;
                Vector.Y += (y - 127.0) * 0.0001;
            }

            /// <summary>
            /// Gets the next entry to be introduced by the given stone, or null if there are none left.
            /// </summary>
            public static Entry GetNext(World World, Stone Stone)
            {
                Entry entry = Stone.Entry;
                Entry n = entry.Next;
                if (n != null && !World.ContainsEntry(n))
                    return n;
                uint maxweight = 0;
                Entry next = null;
                foreach (Entry p in entry.Previous)
                    if (!World.ContainsEntry(p) && p.Weight > maxweight)
                    {
                        maxweight = p.Weight;
                        next = p;
                    }
                return next;
            }
        }

        private Dictionary<ZoneIndex, List<Stone>> _Zones = new Dictionary<ZoneIndex, List<Stone>>();
        private Dictionary<Entry, Stone> _Stones = new Dictionary<Entry, Stone>();
        private Dictionary<Stone, _Leader> _Leaders = new Dictionary<Stone, _Leader>();
    }
}
