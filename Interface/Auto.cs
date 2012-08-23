using System;
using System.Collections.Generic;
using System.Reflection;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// An automatically generated type interface for a class made up of public fields, properties and methods.
    /// </summary>
    public class AutoTypeInterface<T> : ITypeInterface<T>
        where T : class
    {
        private AutoTypeInterface()
        {

        }

        /// <summary>
        /// The only instance of this type.
        /// </summary>
        public static readonly AutoTypeInterface<T> Instance = new AutoTypeInterface<T>();

        /// <summary>
        /// Registers this type interface.
        /// </summary>
        public static void Register()
        {
            TypeInterface.Set(AutoTypeInterface<T>.Instance);
        }

        void ITypeInterface<T>.Push(Lua.lua_State State, T Object)
        {
            Type type = typeof(T);
            Lua.lua_newtable(State);
            int tableindex = Lua.lua_gettop(State);
            Lua.lua_newtable(State);
            int metatableindex = Lua.lua_gettop(State);
            Lua.lua_pushcfunction(State, _IndexFunction(Object));
            Lua.lua_setfield(State, metatableindex, "__index");
            Lua.lua_pushcfunction(State, _NewindexFunction(Object));
            Lua.lua_setfield(State, metatableindex, "__newindex");
            Lua.lua_pushlightuserdata(State, Object);
            Lua.lua_setfield(State, metatableindex, "__ref");
            Lua.lua_setmetatable(State, tableindex);
        }

        /// <summary>
        /// Creates a Lua CFunction to index the given object.
        /// </summary>
        private static Lua.lua_CFunction _IndexFunction(T Object)
        {
            return delegate(Lua.lua_State State)
            {
                try
                {
                    if (Lua.lua_isstring(State, 2) == 0) return 0;
                    string key = Lua.lua_tostring(State, 2).ToString();
                    MethodInfo method = typeof(T).GetMethod(key);
                    if (method != null)
                    {
                        Lua.lua_pushcfunction(State, _CallFunction(Object, method));
                        return 1;
                    }
                    PropertyInfo property = typeof(T).GetProperty(key);
                    if (property != null)
                    {
                        TypeInterface.Get(property.PropertyType).Push(State, property.GetValue(Object, null));
                        return 1;
                    }
                    FieldInfo field = typeof(T).GetField(key);
                    if (field != null)
                    {
                        TypeInterface.Get(field.FieldType).Push(State, field.GetValue(Object));
                        return 1;
                    }
                    throw new Exception(String.Format("{0} does not exist and can not be retrieved.", key));
                }
                catch (Exception e)
                {
                    Lua.lua_pushstring(State, e.Message);
                    Lua.lua_error(State);
                    return 1;
                }
            };
        }

        /// <summary>
        /// Creates a Lua CFunction to change a property on the given object.
        /// </summary>
        private static Lua.lua_CFunction _NewindexFunction(T Object)
        {
            return delegate(Lua.lua_State State)
            {
                try
                {
                    if (Lua.lua_isstring(State, 2) == 0) return 0;
                    string key = Lua.lua_tostring(State, 2).ToString();
                    MethodInfo method = typeof(T).GetMethod(key);
                    if (method != null)
                    {
                        throw new Exception(String.Format("{0} is a method and can not be set.", method.Name));
                    }
                    PropertyInfo property = typeof(T).GetProperty(key);
                    if (property != null)
                    {
                        property.SetValue(Object, TypeInterface.Get(property.PropertyType).To(State, 3), null);
                        return 0;
                    }
                    FieldInfo field = typeof(T).GetField(key);
                    if (field != null)
                    {
                        field.SetValue(Object, TypeInterface.Get(field.FieldType).To(State, 3));
                        return 0;
                    }
                    throw new Exception(String.Format("{0} does not exist and can not be set.", key));
                }
                catch (Exception e)
                {
                    Lua.lua_pushstring(State, e.Message);
                    Lua.lua_error(State);
                    return 1;
                }
            };
        }

        /// <summary>
        /// Creates a Lua CFunction for a method call of the given object.
        /// </summary>
        private static Lua.lua_CFunction _CallFunction(T Object, MethodInfo Method)
        {
            return delegate(Lua.lua_State State)
            {
                try
                {
                    int args = Lua.lua_gettop(State);
                    ParameterInfo[] parameters = Method.GetParameters();
                    if (args != parameters.Length)
                    {
                        throw new Exception(String.Format("Incorrect number of arguments to {0} ({1} expected, got {2}).",
                            Method.Name, parameters.Length, args));
                    }
                    object[] parametervalues = new object[parameters.Length];
                    for (int t = 0; t < parametervalues.Length; t++)
                    {
                        parametervalues[t] = TypeInterface.Get(parameters[t].ParameterType).To(State, t + 1);
                    }
                    object res = Method.Invoke(Object, parametervalues);
                    if (Method.ReturnType == typeof(void))
                    {
                        return 0;
                    }
                    else
                    {
                        TypeInterface.Get(Method.ReturnType).Push(State, res);
                        return 1;
                    }
                }
                catch (Exception e)
                {
                    Lua.lua_pushstring(State, e.Message);
                    Lua.lua_error(State);
                    return 1;
                }
            };
        }

        T ITypeInterface<T>.To(Lua.lua_State State, int Index)
        {
            if (Lua.lua_getmetatable(State, Index) > 0)
            {
                Lua.lua_getfield(State, -1, "__ref");
                object obj = Lua.lua_touserdata(State, Index);
                Lua.lua_pop(State, 2);
                if (obj.GetType() == typeof(T))
                    return (T)obj;
                else
                    throw new Exception(String.Format("Could not convert object of type {0} to type {1}.", obj.GetType(), typeof(T).Name));
            }
            throw new Exception("Invalid object.");
        }

        string ITypeInterface<T>.Create(T Object)
        {
            return null;
        }

        bool ITypeInterface<T>.Mutate(string Name, T From, T To, List<string> Code)
        {
            if (!From.Equals(To))
            {
                foreach (FieldInfo field in typeof(T).GetFields())
                {
                    if (!TypeInterface.Get(field.FieldType).Mutate(_FieldName(Name, field.Name), field.GetValue(From), field.GetValue(To), Code))
                        return false;
                }
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    if (!TypeInterface.Get(property.PropertyType).Mutate(_FieldName(Name, property.Name), property.GetValue(From, null), property.GetValue(To, null), Code))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the name for a field, given the name for the object.
        /// </summary>
        private static string _FieldName(string Name, string Field)
        {
            return Name + "." + Field;
        }
    }
}
