using System;
using System.Collections.Generic;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// Defines a global context for Lua.
    /// </summary>
    public class Global
    {
        /// <summary>
        /// The default global context, which should include constructors and functions needed to define common objects without
        /// allowing significant state changes.
        /// </summary>
        public static Global Default = new Global();

        /// <summary>
        /// Creates a new Lua state in this global context.
        /// </summary>
        public Lua.lua_State Instantiate()
        {
            return Lua.luaL_newstate();
        }
    }
}
