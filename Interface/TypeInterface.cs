using System;
using System.Collections.Generic;
using System.Reflection;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// Contains methods that allow interfacing with an object of a certain type.
    /// </summary>
    public interface ITypeInterface<T>
    {
        /// <summary>
        /// Pushes a Lua representation of the given object to the Lua stack.
        /// </summary>
        void Push(Lua.lua_State State, T Object);

        /// <summary>
        /// Converts the item at the given location on the stack to an object.
        /// </summary>
        T To(Lua.lua_State State, int Index);

        /// <summary>
        /// Gets the Lua code (in the global scope) needed to create the given object, or null
        /// if not possible.
        /// </summary>
        string Create(T Object);

        /// <summary>
        /// Gets the Lua code (in the global scope) needed to mutate the object "From" (with the given name) to 
        /// be equivalent to "To", or returns false if not possible.
        /// </summary>
        bool Mutate(string Name, T From, T To, List<string> Code);
    }

    /// <summary>
    /// Stores type interfaces for various types.
    /// </summary>
    public static class TypeInterface
    {
        /// <summary>
        /// Sets the interface for the given type.
        /// </summary>
        public static void Set<T>(ITypeInterface<T> Interface)
        {
            Set(typeof(T), new _Wrapper<T>(Interface));
        }

        /// <summary>
        /// Sets the interface for the given type.
        /// </summary>
        public static void Set(Type Type, ITypeInterface<object> Interface)
        {
            _Interfaces[Type] = Interface;
        }

        /// <summary>
        /// Gets the interface for the given type, or null if none exists.
        /// </summary>
        public static ITypeInterface<object> Get(Type Type)
        {
            ITypeInterface<object> i;
            if (_Interfaces.TryGetValue(Type, out i))
                return i;
            else
                return null;
        }

        /// <summary>
        /// Gets the interface for the given type, or null if none exists.
        /// </summary>
        public static ITypeInterface<T> Get<T>()
        {
            ITypeInterface<object> i = Get(typeof(T));
            if (i == null) return null;
            _Wrapper<T> wrapper = i as _Wrapper<T>;
            if (wrapper != null) return wrapper.Source;
            return (i as ITypeInterface<T>);
        }

        /// <summary>
        /// The known type interfaces. 
        /// </summary>
        private static readonly Dictionary<Type, ITypeInterface<object>> _Interfaces = 
            new Dictionary<Type, ITypeInterface<object>>();

        /// <summary>
        /// A wrapper for a type interface that allows it to work on a general object type.
        /// </summary>
        private class _Wrapper<T> : ITypeInterface<object>
        {
            public _Wrapper(ITypeInterface<T> Source)
            {
                this.Source = Source;
            }

            /// <summary>
            /// The source type interface for this wrapper.
            /// </summary>
            public readonly ITypeInterface<T> Source;

            void ITypeInterface<object>.Push(Lua.lua_State State, object Object)
            {
                this.Source.Push(State, (T)Object);
            }

            object ITypeInterface<object>.To(Lua.lua_State State, int Index)
            {
                return (object)this.Source.To(State, Index);
            }

            string ITypeInterface<object>.Create(object Object)
            {
                return this.Source.Create((T)Object);
            }

            bool ITypeInterface<object>.Mutate(string Name, object From, object To, List<string> Code)
            {
                return this.Source.Mutate(Name, (T)From, (T)To, Code);
            }
        }
    }
}
