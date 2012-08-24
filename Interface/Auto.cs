using System;
using System.Collections.Generic;
using System.Reflection;
using KopiLua;

namespace Hailstone.Interface
{
    /// <summary>
    /// An automatically generated type interface for a type made up of public fields, properties and methods.
    /// </summary>
    public class AutoTypeInterface : CompositeTypeInterface
    {
        public AutoTypeInterface(Type Type)
            : base(GetMembers(Type), Type)
        {

        }

        /// <summary>
        /// Uses reflection to find the members of the given type.
        /// </summary>
        public static IEnumerable<Member> GetMembers(Type Type)
        {
            foreach (FieldInfo field in Type.GetFields(_Mask))
            {
                yield return new _FieldProperty(field);
            }
            foreach (PropertyInfo property in Type.GetProperties(_Mask))
            {
                yield return new _Property(property);
            }
            foreach (MethodInfo method in Type.GetMethods(_Mask))
            {
                yield return new _Method(method);
            }
        }

        /// <summary>
        /// A property representation of a field.
        /// </summary>
        private class _FieldProperty : Property
        {
            public _FieldProperty(FieldInfo Field)
                : base(Field.Name, TypeInterface.Get(Field.FieldType))
            {
                this.Field = Field;
            }

            /// <summary>
            /// The field for this property.
            /// </summary>
            public readonly FieldInfo Field;

            public override void Set(object Object, object Value)
            {
                this.Field.SetValue(Object, Value);
            }

            public override object Get(object Object)
            {
                return this.Field.GetValue(Object);
            }
        }

        /// <summary>
        /// A property representation of a property.
        /// </summary>
        private class _Property : Property
        {
            public _Property(PropertyInfo Property)
                : base(Property.Name, TypeInterface.Get(Property.PropertyType))
            {
                this.Property = Property;
            }

            /// <summary>
            /// The source property for this property.
            /// </summary>
            public readonly PropertyInfo Property;

            public override void Set(object Object, object Value)
            {
                this.Property.SetValue(Object, Value, null);
            }

            public override object Get(object Object)
            {
                return this.Property.GetValue(Object, null);
            }
        }

        /// <summary>
        /// A method representation of a method.
        /// </summary>
        private class _Method : Method
        {
            public _Method(MethodInfo Method)
                : base(Method.Name, 
                Method.ReturnType == typeof(void) ? null : TypeInterface.Get(Method.ReturnType), 
                _Interfaces(Method.GetParameters()))
            {
                this.Method = Method;
            }

            /// <summary>
            /// Gets type interfaces for the given types.
            /// </summary>
            private static TypeInterface[] _Interfaces(ParameterInfo[] Parameters)
            {
                TypeInterface[] interfaces = new TypeInterface[Parameters.Length];
                for(int t = 0; t < interfaces.Length; t++)
                {
                    interfaces[t] = TypeInterface.Get(Parameters[t].ParameterType);
                }
                return interfaces;
            }

            /// <summary>
            /// The source method for this method.
            /// </summary>
            public readonly MethodInfo Method;

            public override object Call(object Object, object[] Arguments)
            {
                return this.Method.Invoke(Object, Arguments);
            }
        }

        /// <summary>
        /// The mask that determines which properties are used in the interface.
        /// </summary>
        private static readonly BindingFlags _Mask = BindingFlags.Instance | BindingFlags.Public;
    }
}
