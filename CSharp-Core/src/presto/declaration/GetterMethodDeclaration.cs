using presto.runtime;
using System;
using presto.statement;
using presto.expression;
using presto.grammar;
using presto.type;
using presto.utils;
using presto.parser;
using presto.value;


namespace presto.declaration
{

	public class GetterMethodDeclaration : ConcreteMethodDeclaration, IExpression
    {

		public GetterMethodDeclaration(String name, StatementList statements)
			: base(name, null, null, statements)
        {
        }

        override
        public void check(ConcreteCategoryDeclaration category, Context context)
        {
            // TODO Auto-generated method stub

        }

        override
        public IType check(Context context)
        {
            // TODO Auto-generated method stub
            return null;
        }

		protected override void toODialect(CodeWriter writer) {
			writer.append("getter ");
			writer.append(name);
			writer.append(" {\n");
			writer.indent();
			statements.ToDialect(writer);
			writer.dedent();
			writer.append("}\n");
		}

		protected override void toEDialect(CodeWriter writer) {
			writer.append("define ");
			writer.append(name);
			writer.append(" getter doing:\n");
			writer.indent();
			statements.ToDialect(writer);
			writer.dedent();
		}

		protected override void toSDialect(CodeWriter writer) {
			writer.append("def ");
			writer.append(name);
			writer.append(" getter():\n");
			writer.indent();
			statements.ToDialect(writer);
			writer.dedent();
		}
    }
}
