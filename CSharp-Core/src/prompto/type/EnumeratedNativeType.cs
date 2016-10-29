
using prompto.runtime;
using System;
using prompto.declaration;
using prompto.value;
using prompto.error;

namespace prompto.type
{

    public class EnumeratedNativeType : BaseType
    {

        NativeType derivedFrom;
		String typeName;

        public EnumeratedNativeType(String typeName, NativeType derivedFrom)
			: base(TypeFamily.ENUMERATED)
         {
           this.derivedFrom = derivedFrom;
			this.typeName = typeName;
        }

		public override string GetTypeName()
		{
			return typeName;
		}

        
		public override IType checkMember(Context context, String name)
        {
            if ("symbols" == name)
			    return new ListType(derivedFrom);
		    else if ("value" == name)
                return this;
            else if ("name" == name)
                return TextType.Instance;
            else
                return base.checkMember(context, name);
        }

        override
        public IValue getMember(Context context, String name)
        {
            IDeclaration decl = context.getRegisteredDeclaration<IDeclaration>(this.typeName);
            if (!(decl is EnumeratedNativeDeclaration))
                throw new SyntaxError(name + " is not an enumerated type!");
            if ("symbols" == name)
                return ((EnumeratedNativeDeclaration)decl).getSymbols();
            else
                throw new SyntaxError("Unknown member:" + name);
        }
	    

        public NativeType getDerivedFrom()
        {
            return derivedFrom;
        }

        override
        public System.Type ToCSharpType()
        {
            // TODO Auto-generated method stub
            return null;
        }

        override
        public void checkUnique(Context context)
        {
            // TODO Auto-generated method stub

        }

        override
        public void checkExists(Context context)
        {
            // TODO Auto-generated method stub

        }

        override
        public bool isAssignableTo(Context context, IType other)
        {
            // TODO Auto-generated method stub
            return false;
        }

        override
        public bool isMoreSpecificThan(Context context, IType other)
        {
            // TODO Auto-generated method stub
            return false;
        }

    }

}