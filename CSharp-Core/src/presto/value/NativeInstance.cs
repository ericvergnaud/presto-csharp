using presto.error;
using System;
using presto.grammar;
using System.Collections.Generic;
using System.Reflection;
using presto.value;
using presto.csharp;
using presto.declaration;
using presto.type;
using presto.runtime;
using presto.expression;

namespace presto.value
{

    public class NativeInstance : BaseValue, IInstance
    {

        NativeCategoryDeclaration declaration;
        protected Object instance;
		bool mutable = false;

        public NativeInstance(NativeCategoryDeclaration declaration)
			: base(new CategoryType(declaration.getName()))
		{
            this.declaration = declaration;
            this.instance = makeInstance();
        }

		public NativeInstance(NativeCategoryDeclaration declaration, Object instance)
			: base(new CategoryType(declaration.getName()))
		{
			this.declaration = declaration;
			this.instance = instance;
		}

		public bool setMutable(bool set)
		{
			bool result = mutable;
			mutable = set;
			return result;
		}

		public bool isMutable()
		{
			return mutable;
		}

       public Object getInstance()
        {
            return instance;
        }

        private Object makeInstance()
        {
            Type mapped = declaration.getMappedClass();
            return Activator.CreateInstance(mapped);
        }

        public CategoryType getType()
        {
			return (CategoryType)this.type;
        }

        public ICollection<String> getAttributeNames()
        {
            // TODO Auto-generated method stub
            return null;
        }

        override
        public IValue GetMember(Context context, String attrName)
        {
            Object value = getPropertyOrField(attrName);
            CSharpClassType ct = new CSharpClassType(value.GetType());
            return ct.convertSystemValueToPrestoValue(value, null);
        }

        private Object getPropertyOrField(String attrName)
        {
            Object value = null;
            if (TryGetProperty(attrName, out value))
                return value;
            if (TryGetField(attrName, out value))
                return value;
            throw new SyntaxError("Missing property or field:" + attrName);
        }

        private bool TryGetField(string attrName, out object value)
        {
            FieldInfo field = instance.GetType().GetField(attrName);
            if (field != null)
                value = field.GetValue(instance);
            else
                value = null;
            return field != null;
        }

        private bool TryGetProperty(String attrName, out Object value)
        {
            PropertyInfo property = instance.GetType().GetProperty(attrName);
            if (property != null)
                value = property.GetValue(instance, null);
            else
                value = null;
            return property != null;
        }
	
   
        public void set(Context context, String attrName, IValue value)
        {
            Object o = value;
            if (o is IExpression)
                o = ((IExpression)value).interpret(context);
            if(o is IValue)
                setPropertyOrField(value, attrName);
            else
                throw new InternalError("Not a value:" + o.GetType().Name);
        }

        private void setPropertyOrField(IValue value, String attrName)
        {
            if (setProperty(value, attrName))
                return;
            if (setField(value, attrName))
                return;
            throw new SyntaxError("Missing property or field:" + attrName);
        }

        private bool setField(IValue value, String attrName)
        {
            FieldInfo field = instance.GetType().GetField(attrName);
            if (field != null)
                field.SetValue(instance, value.ConvertTo(field.FieldType));
            return field != null;
        }

        private bool setProperty(IValue value, String attrName)
        {
 	        PropertyInfo property = instance.GetType().GetProperty(attrName);
            if(property!=null)
                property.SetValue(instance, value.ConvertTo(property.PropertyType), null);
            return property!=null;
        }


   
    }

}
