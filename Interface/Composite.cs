using System;
using System.Collections.Generic;
using System.Reflection;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// A type interface for a composite type.
    /// </summary>
    public class CompositeTypeInterface : TypeInterface
    {
        public CompositeTypeInterface(IEnumerable<Member> Members, Type Type)
            : base(Type)
        {
            this._Members = new Dictionary<string, Member>();
            foreach (Member member in Members)
            {
                this._Members[member.Name] = member;
            }
        }

        /// <summary>
        /// Gets all of the members in this composite type interface.
        /// </summary>
        public IEnumerable<Member> Members
        {
            get
            {
                return this._Members.Values;
            }
        }

        /// <summary>
        /// Gets the member of this composite type interface with the given name, or null if it does not exist.
        /// </summary>
        public Member this[string Name]
        {
            get
            {
                Member member;
                this._Members.TryGetValue(Name, out member);
                return member;
            }
        }

        /// <summary>
        /// Gets the full name of a member.
        /// </summary>
        public static string FullName(string ObjectName, string MemberName)
        {
            return ObjectName + "." + MemberName;
        }

        public override void Push(Lua.lua_State State, object Object)
        {
            Lua.lua_newtable(State);
            int tableindex = Lua.lua_gettop(State);
            Lua.lua_newtable(State);
            int metatableindex = Lua.lua_gettop(State);
            Lua.lua_pushcfunction(State, this._IndexFunction(Object));
            Lua.lua_setfield(State, metatableindex, "__index");
            Lua.lua_pushcfunction(State, this._NewindexFunction(Object));
            Lua.lua_setfield(State, metatableindex, "__newindex");
            Lua.lua_pushlightuserdata(State, Object);
            Lua.lua_setfield(State, metatableindex, "__ref");
            Lua.lua_setmetatable(State, tableindex);
        }

        /// <summary>
        /// Creates a Lua CFunction to index the given object.
        /// </summary>
        private Lua.lua_CFunction _IndexFunction(object Object)
        {
            return delegate(Lua.lua_State State)
            {
                try
                {
                    if (Lua.lua_isstring(State, 2) > 0)
                    {
                        string key = Lua.lua_tostring(State, 2).ToString();
                        Member member = this[key];
                        if (member != null)
                        {
                            switch (member.Tag)
                            {
                                case MemberTag.Property:
                                    Property property = (Property)member;
                                    property.Type.Push(State, property.Get(Object));
                                    return 1;
                                case MemberTag.Method:
                                    Method method = (Method)member;
                                    Lua.lua_pushcfunction(State, _CallFunction(method, Object));
                                    return 1;
                            }
                        }
                        throw new Exception(String.Format("{0} does not exist and can not be retrieved.", key));
                    }
                    throw new Exception("The given index does not exist and can not be retrieved.");
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
        private Lua.lua_CFunction _NewindexFunction(object Object)
        {
            return delegate(Lua.lua_State State)
            {
                try
                {
                    if (Lua.lua_isstring(State, 2) > 0)
                    {
                        string key = Lua.lua_tostring(State, 2).ToString();
                        Member member = this[key];
                        if (member != null)
                        {
                            switch (member.Tag)
                            {
                                case MemberTag.Property:
                                    Property property = (Property)member;
                                    property.Set(Object, property.Type.To(State, 3));
                                    return 0;
                                case MemberTag.Method:
                                    throw new Exception(String.Format("{0} is a method and can not be set", member.Name));
                            }
                        }
                        throw new Exception(String.Format("{0} does not exist and can not be set.", key));
                    }
                    throw new Exception("The given index does not exist and can not be set.");
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
        private static Lua.lua_CFunction _CallFunction(Method Method, object Object)
        {
            return delegate(Lua.lua_State State)
            {
                try
                {
                    int args = Lua.lua_gettop(State);
                    TypeInterface[] argtypes = Method.ArgumentTypes;
                    if (args != argtypes.Length)
                    {
                        throw new Exception(String.Format("Incorrect number of arguments to {0} ({1} expected, got {2}).",
                            Method.Name, argtypes.Length, args));
                    }
                    object[] parametervalues = new object[argtypes.Length];
                    for (int t = 0; t < argtypes.Length; t++)
                    {
                        parametervalues[t] = argtypes[t].To(State, t + 1);
                    }
                    object res = Method.Call(Object, parametervalues);
                    if (Method.Type == null)
                    {
                        return 0;
                    }
                    else
                    {
                        Method.Type.Push(State, res);
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

        public override object To(Lua.lua_State State, int Index)
        {
            if (Lua.lua_getmetatable(State, Index) > 0)
            {
                Lua.lua_getfield(State, -1, "__ref");
                object obj = Lua.lua_touserdata(State, Index);
                Lua.lua_pop(State, 2);
                if (obj.GetType() == Type)
                    return obj;
                else
                    throw new Exception(String.Format("Could not convert object of type {0} to type {1}.", obj.GetType(), Type));
            }
            throw new Exception("Invalid object.");
        }

        public override bool Mutate(Global Global, string Name, object From, object To, List<string> Code)
        {
            if (From.Equals(To)) return true;
            string create = this.Create(Global, To);
            if (create != null)
            {
                Code.Add(Name + " = " + create);
                return true;
            }
            foreach (Member member in this.Members)
            {
                if (member.Tag == MemberTag.Property)
                {
                    Property property = (Property)member;
                    if (!property.Type.Mutate(Global, FullName(Name, property.Name), property.Get(From), property.Get(To), Code))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private Dictionary<string, Member> _Members;
    }

    /// <summary>
    /// Gives information about a member of a type.
    /// </summary>
    public abstract class Member
    {
        public Member(string Name, TypeInterface Type, MemberTag Tag)
        {
            this.Name = Name;
            this.Type = Type;
            this.Tag = Tag;
        }

        /// <summary>
        /// The name of this member.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The type interface for the primary type of this member (the return type for methods).
        /// </summary>
        public readonly TypeInterface Type;

        /// <summary>
        /// Identifies the type of this member.
        /// </summary>
        public readonly MemberTag Tag;
    }

    /// <summary>
    /// A property member.
    /// </summary>
    public abstract class Property : Member
    {
        public Property(string Name, TypeInterface Type)
            : base(Name, Type, MemberTag.Property)
        {

        }

        /// <summary>
        /// Sets the value of this property on the given object.
        /// </summary>
        public abstract void Set(object Object, object Value);

        /// <summary>
        /// Gets the value of this property on the given object.
        /// </summary>
        public abstract object Get(object Object);
    }

    /// <summary>
    /// A method member.
    /// </summary>
    public abstract class Method : Member
    {
        public Method(string Name, TypeInterface Type, TypeInterface[] ArgumentTypes)
            : base(Name, Type, MemberTag.Method)
        {

        }

        /// <summary>
        /// Calls this method on the given object.
        /// </summary>
        public abstract object Call(object Object, object[] Arguments);

        /// <summary>
        /// The type interfaces for the argument types of this member, or null if the member is not a method.
        /// </summary>
        public readonly TypeInterface[] ArgumentTypes;
    }

    /// <summary>
    /// Identifies a type of member.
    /// </summary>
    public enum MemberTag
    {
        Property,
        Method
    }
}
