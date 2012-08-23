using System;
using System.Collections.Generic;
using System.Reflection;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// A type interface for several primitive types.
    /// </summary>
    public class PrimitiveTypeInterface : ITypeInterface<double>, ITypeInterface<int>, ITypeInterface<string>
    {
        private PrimitiveTypeInterface()
        {

        }

        /// <summary>
        /// The only instance of this class.
        /// </summary>
        public static readonly PrimitiveTypeInterface Instance = new PrimitiveTypeInterface();

        /// <summary>
        /// Registers the primitive type interface.
        /// </summary>
        public static void Register()
        {
            TypeInterface.Set<double>(PrimitiveTypeInterface.Instance);
            TypeInterface.Set<int>(PrimitiveTypeInterface.Instance);
            TypeInterface.Set<string>(PrimitiveTypeInterface.Instance);
        }

        void ITypeInterface<double>.Push(Lua.lua_State State, double Object)
        {
            Lua.lua_pushnumber(State, Object);
        }

        double ITypeInterface<double>.To(Lua.lua_State State, int Index)
        {
            if (Lua.lua_isnumber(State, Index) > 0)
                return Lua.lua_tonumber(State, Index);
            else
                throw new Exception(String.Format("Expected number (Got {0}).", Lua.lua_typename(State, Index).ToString()));
        }

        string ITypeInterface<double>.Create(double Object)
        {
            return Object.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        bool ITypeInterface<double>.Mutate(string Name, double From, double To, List<string> Code)
        {
            if (From != To) Code.Add(Name + " = " + To.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return true;
        }

        void ITypeInterface<int>.Push(Lua.lua_State State, int Object)
        {
            Lua.lua_pushinteger(State, Object);
        }

        int ITypeInterface<int>.To(Lua.lua_State State, int Index)
        {
            if (Lua.lua_isnumber(State, Index) > 0)
                return (int)Lua.lua_tonumber(State, Index);
            else
                throw new Exception(String.Format("Expected integer (Got {0}).", Lua.lua_typename(State, Index).ToString()));
        }

        string ITypeInterface<int>.Create(int Object)
        {
            return Object.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        bool ITypeInterface<int>.Mutate(string Name, int From, int To, List<string> Code)
        {
            if (From != To) Code.Add(Name + " = " + To.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return true;
        }

        void ITypeInterface<string>.Push(Lua.lua_State State, string Object)
        {
            Lua.lua_pushstring(State, Object);
        }

        string ITypeInterface<string>.To(Lua.lua_State State, int Index)
        {
            if (Lua.lua_isstring(State, Index) > 0)
                return Lua.lua_tostring(State, Index).ToString();
            else
                throw new Exception(String.Format("Expected string (Got {0}).", Lua.lua_typename(State, Index).ToString()));
        }

        string ITypeInterface<string>.Create(string Object)
        {
            return Object;
        }

        bool ITypeInterface<string>.Mutate(string Name, string From, string To, List<string> Code)
        {
            if (From != To) Code.Add(Name + " = " + To);
            return true;
        }
    }
}
