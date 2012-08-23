using System;
using System.Collections.Generic;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// Contains information about the global functions and values that are part of the interface.
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Creates a new Lua state with all globals included.
        /// </summary>
        public static Lua.lua_State Initialize()
        {
            return Lua.luaL_newstate();
        }
    }
}
