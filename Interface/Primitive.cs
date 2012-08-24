using System;
using System.Collections.Generic;
using System.Reflection;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// A type interface for a double.
    /// </summary>
    public class DoubleTypeInterface : TypeInterface
    {
        public DoubleTypeInterface()
            : base(typeof(double), "Number")
        {

        }

        public override void Push(Lua.lua_State State, object Object)
        {
            Lua.lua_pushnumber(State, (double)Object);
        }

        public override object To(Lua.lua_State State, int Index)
        {
            if (Lua.lua_isnumber(State, Index) > 0)
                return Lua.lua_tonumber(State, Index);
            else
                throw new Exception(String.Format("Expected number (Got {0}).", Lua.lua_typename(State, Index).ToString()));
        }

        public override string Create(Global Global, object Object)
        {
            return ((double)Object).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// A type interface for an integer.
    /// </summary>
    public class IntegerTypeInterface : TypeInterface
    {
        public IntegerTypeInterface()
            : base(typeof(int), "Integer")
        {

        }

        public override void Push(Lua.lua_State State, object Object)
        {
            Lua.lua_pushinteger(State, (int)Object);
        }

        public override object To(Lua.lua_State State, int Index)
        {
            if (Lua.lua_isnumber(State, Index) > 0)
                return (int)Lua.lua_tonumber(State, Index);
            else
                throw new Exception(String.Format("Expected integer (Got {0}).", Lua.lua_typename(State, Index).ToString()));
        }

        public override string Create(Global Global, object Object)
        {
            return Object.ToString();
        }
    }

    /// <summary>
    /// A type interface for a string.
    /// </summary>
    public class StringTypeInterface : TypeInterface
    {
        public StringTypeInterface()
            : base(typeof(string), "String")
        {

        }

        public override void Push(Lua.lua_State State, object Object)
        {
            Lua.lua_pushstring(State, (string)Object);
        }

        public override object To(Lua.lua_State State, int Index)
        {
            if (Lua.lua_isstring(State, Index) > 0)
                return Lua.lua_tostring(State, Index).ToString();
            else
                throw new Exception(String.Format("Expected string (Got {0}).", Lua.lua_typename(State, Index).ToString()));
        }

        public override string Create(Global Global, object Object)
        {
            return String.Format("\"{0}\"", Object);
        }
    }
}
