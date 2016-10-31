using prompto.error;
using prompto.runtime;
using System;
using prompto.parser;
using prompto.expression;
using prompto.type;
using prompto.utils;
using prompto.value;
using prompto.grammar;

namespace prompto.instance
{

    public class VariableInstance : IAssignableInstance
    {

        String name;

        public VariableInstance(String i1)
        {
            this.name = i1;
        }

        public String getName()
        {
            return name;
        }

		public void ToDialect(CodeWriter writer, IExpression expression)
        {
			if(expression!=null) try {
				IType type = expression.check(writer.getContext());
				INamed actual = writer.getContext().getRegisteredValue<INamed>(name);
				if(actual==null)
					writer.getContext().registerValue(new Variable(name, type));
			} catch(SyntaxError) {
				// TODO warning
			}
			writer.append(name);
        }

		public IType checkAssignValue(Context context, IType valueType)
        {
            INamed actual = context.getRegisteredValue<INamed>(name);
            if (actual == null)
            {
				context.registerValue(new Variable(name, valueType));
            }
            else
            {
                // need to check type compatibility
                IType actualType = actual.GetIType(context);
				valueType.checkAssignableTo(context, actualType);
				valueType = actualType;
            }
			return valueType;
        }

		public IType checkAssignMember(Context context, String memberName, IType valueType)
        {
			INamed actual = context.getRegisteredValue<INamed>(name);
			if(actual==null) 
				throw new SyntaxError("Unknown variable:" + this.name);
			IType parentType = actual.GetIType(context);
			return parentType.checkMember(context, memberName);
		}



		public IType checkAssignItem(Context context, IType itemType, IType valueType)
        {
			INamed actual = context.getRegisteredValue<INamed>(name);
			if(actual==null) 
				throw new SyntaxError("Unknown variable:" + this.name);
			IType parentType = actual.GetIType(context);
			return parentType.checkItem(context, itemType);
        }

        public void assign(Context context, IExpression expression)
        {
            IValue value = expression.interpret(context);
			if (context.getRegisteredValue<INamed>(name) == null)
			{
				IType type = value != NullValue.Instance ? value.GetIType() : expression.check(context);
				context.registerValue(new Variable(name, type));
			}
            context.setValue(name, value);
        }

        public IValue interpret(Context context)
        {
            return context.getValue(name);
        }

    }

}