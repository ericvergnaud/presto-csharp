using System;
using prompto.grammar;
using prompto.type;
using prompto.expression;
using prompto.statement;
using prompto.runtime;
using prompto.utils;
using prompto.param;

namespace prompto.declaration
{
	public class OperatorMethodDeclaration : ConcreteMethodDeclaration, IExpression 
	{

		Operator oper;

		public OperatorMethodDeclaration(Operator oper, IParameter arg, IType returnType, StatementList stmts)
			: base("operator_" + oper.ToString(), new ParameterList(arg), returnType, stmts)
		{
			this.oper = oper;
		}


		protected override void ToMDialect(CodeWriter writer) {
			writer.append("def operator ");
			writer.append(Enums.OperatorToString(oper));
			writer.append(" (");
			parameters.ToDialect(writer);
			writer.append(")");
			if(returnType!=null && returnType!=VoidType.Instance) {
				writer.append("->");
				returnType.ToDialect(writer);
			}
			writer.append(":\n");
			writer.indent();
			statements.ToDialect(writer);
			writer.dedent();
		}

		protected override void ToEDialect(CodeWriter writer) {
			writer.append("define ");
			writer.append(Enums.OperatorToString(oper));
			writer.append(" as operator ");
			parameters.ToDialect(writer);
			if(returnType!=null && returnType!=VoidType.Instance) {
				writer.append("returning ");
				returnType.ToDialect(writer);
				writer.append(" ");
			}
			writer.append("doing:\n");
			writer.indent();
			statements.ToDialect(writer);
			writer.dedent();
		}

		protected override void ToODialect(CodeWriter writer) {
			if(returnType!=null && returnType!=VoidType.Instance) {
				returnType.ToDialect(writer);
				writer.append(" ");
			}
			writer.append("operator ");
			writer.append(Enums.OperatorToString(oper));
			writer.append(" (");
			parameters.ToDialect(writer);
			writer.append(") {\n");
			writer.indent();
			statements.ToDialect(writer);
			writer.dedent();
			writer.append("}\n");
		}
			
	}

}

