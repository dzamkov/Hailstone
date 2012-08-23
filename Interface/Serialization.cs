using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// Implements object serialization using Lua.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Saves the delta between the given objects to the given stream.
        /// </summary>
        public static void Save<T>(string Name, T Initial, T Object, Stream Stream)
        {
            List<string> code = new List<string>();
            if (TypeInterface.Get<T>().Mutate(Name, Initial, Object, code))
            {
                using (TextWriter txt = new StreamWriter(Stream))
                {
                    foreach (string line in code)
                        txt.WriteLine(line);
                }
            }
            else
            {
                throw new Exception("Could not serialize object.");
            }
        }

        /// <summary>
        /// Loads an object from the given stream, applying it to initial.
        /// </summary>
        public static void Load<T>(string Name, ref T Initial, Stream Stream)
        {
            ITypeInterface<T> ti = TypeInterface.Get<T>();
            Lua.lua_State state = Global.Initialize();
            ti.Push(state, Initial);
            Lua.lua_setglobal(state, Name);

            string code;
            using(TextReader txt = new StreamReader(Stream))
            {
                code = txt.ReadToEnd();
            }

            int compileerr = Lua.luaL_loadstring(state, code);
            if (compileerr != 0)
            {
                string error = Lua.lua_tostring(state, -1).ToString();
                throw new Exception(error);
            }

            int runerr = Lua.lua_pcall(state, 0, 0, 0);
            if (runerr != 0)
            {
                string error = Lua.lua_tostring(state, -1).ToString();
                throw new Exception(error);
            }

            Lua.lua_getglobal(state, Name);
            Initial = ti.To(state, -1);
        }
    }
}
