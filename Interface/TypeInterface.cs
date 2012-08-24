using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// Stores type interfaces for various types.
    /// </summary>
    public abstract class TypeInterface
    {
        public TypeInterface(Type Type, string Name)
        {
            this.Type = Type;
            this.Name = Name;
        }

        /// <summary>
        /// The type this type interface works on.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// A user-friendly name of this type. This does not have to be unique.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Pushes a Lua representation of the given object to the Lua stack.
        /// </summary>
        public abstract void Push(Lua.lua_State State, object Object);

        /// <summary>
        /// Converts the item at the given location on the stack to an object.
        /// </summary>
        public abstract object To(Lua.lua_State State, int Index);

        /// <summary>
        /// Gets the Lua code (in the given global context) needed to create the given object, or returns null
        /// if not possible.
        /// </summary>
        public virtual string Create(Global Global, object Object)
        {
            return null;
        }

        /// <summary>
        /// Gets the Lua code (in the given global) needed to mutate the object "From" (with the given name) to 
        /// be equivalent to "To", or returns false if not possible.
        /// </summary>
        public virtual bool Mutate(Global Global, string Name, object From, object To, List<string> Code)
        {
            if (From.Equals(To)) return true;
            string create = this.Create(Global, To);
            if (create != null)
            {
                Code.Add(Name + " = " + create);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the change between two objects of this type to the given stream.
        /// </summary>
        public void Save(Global Global, object From, object To, Stream Stream)
        {
            List<string> code = new List<string>();
            if (this.Mutate(Global, this.Name, From, To, code))
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
        /// Loads the changes from the given stream and applies them to the given object.
        /// </summary>
        public void Load(Global Global, ref object Object, Stream Stream)
        {
            Lua.lua_State state = Global.Default.Instantiate();
            this.Push(state, Object);
            Lua.lua_setglobal(state, this.Name);

            string code;
            using (TextReader txt = new StreamReader(Stream))
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

            Lua.lua_getglobal(state, this.Name);
            Object = this.To(state, -1);
        }

        /// <summary>
        /// Registers this type interface.
        /// </summary>
        public void Register()
        {
            _Interfaces[this.Type] = this;
        }

        /// <summary>
        /// Gets the interface for the given type, or null if none exists.
        /// </summary>
        public static TypeInterface Get(Type Type)
        {
            TypeInterface i;
            if (_Interfaces.TryGetValue(Type, out i))
                return i;
            else
                return null;
        }

        /// <summary>
        /// The known type interfaces. 
        /// </summary>
        private static readonly Dictionary<Type, TypeInterface> _Interfaces = 
            new Dictionary<Type, TypeInterface>();
    }
}
