using prompto.runtime;
using System;
using prompto.parser;
using prompto.error;
using prompto.type;
using prompto.declaration;
using prompto.utils;
using prompto.expression;
using prompto.value;
using prompto.grammar;

namespace prompto.argument
{

    public class UnresolvedArgument : BaseArgument, INamedArgument, IDialectElement
    {

        INamedArgument resolved = null;

        public UnresolvedArgument(String name)
			: base(name)
        {
        }

        public override String getSignature(Dialect dialect)
        {
			return GetName();
        }
			
		public override void ToDialect(CodeWriter writer)
        {
			writer.append(name);
			if(DefaultValue!=null) {
				writer.append(" = ");
				DefaultValue.ToDialect(writer);
			}
       }

		public override void check(Context context)
        {
            resolveAndCheck(context);
        }

		public override String getProto(Context context)
        {
            return name;
        }

		public override IType GetIType(Context context)
        {
            resolveAndCheck(context);
            return resolved.GetIType(context);
        }

		public override void register(Context context)
        {
            resolveAndCheck(context);
            resolved.register(context);
        }

		public override IValue checkValue(Context context, IExpression expression)
        {
            resolveAndCheck(context);
			return resolved.checkValue(context, expression);
        }

        private void resolveAndCheck(Context context)
        {
            if (resolved != null)
                return;
            IDeclaration named = context.getRegisteredDeclaration<IDeclaration>(name);
            if (named is AttributeDeclaration)
                resolved = new AttributeArgument(name);
            else if (named is MethodDeclarationMap)
                resolved = new MethodArgument(name);
            else
                throw new SyntaxError("Unknown identifier:" + name);
        }

    }

}