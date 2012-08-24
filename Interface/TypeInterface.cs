using System;
using System.Collections.Generic;
using System.Reflection;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// Stores type interfaces for various types.
    /// </summary>
    public abstract class TypeInterface
    {
        public TypeInterface(Type Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// The type this type interface works on.
        /// </summary>
        public readonly Type Type;

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
