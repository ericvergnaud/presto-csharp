using System;
using System.Collections.Generic;
using prompto.runtime;
using prompto.error;
using prompto.value;
using prompto.grammar;
using prompto.expression;
using prompto.declaration;
using prompto.statement;
using prompto.utils;
using prompto.store;
using prompto.parser;
using prompto.type.category;

namespace prompto.type
{

	public class CategoryType : BaseType
	{

		String typeName;

		public CategoryType(String typeName)
			: base(TypeFamily.CATEGORY)
		{
			this.typeName = typeName;
		}

		public CategoryType(TypeFamily family, String typeName)
	 		: base(family)
		{
			this.typeName = typeName;
		}

		public CategoryType(CategoryType source, bool mutable)
			: base(source.GetFamily())
		{
			this.typeName = source.typeName;
			this.Mutable = mutable;
		}

		public bool Mutable { get; set; }

		public override void ToDialect(CodeWriter writer)
		{
			if (Mutable)
				writer.append("mutable ");
			writer.append(typeName);
		}

		public override string GetTypeName()
		{
			return typeName;
		}

		public override IType checkMember(Context context, String name)
		{
			IDeclaration dd = context.getRegisteredDeclaration<IDeclaration>(GetTypeName());
			if (dd == null)
				throw new SyntaxError("Unknown category:" + GetTypeName());
			if (dd is EnumeratedNativeDeclaration)
				return dd.GetIType(context).checkMember(context, name);
			else if (dd is CategoryDeclaration)
			{
				CategoryDeclaration cd = (CategoryDeclaration)dd;
				if (cd.Storable && "dbId" == name)
					return AnyType.Instance;
				else if (cd.hasAttribute(context, name))
				{
					AttributeDeclaration ad = context.getRegisteredDeclaration<AttributeDeclaration>(name);
					if (ad == null)
						throw new SyntaxError("Unknown atttribute:" + name);
					else
						return ad.GetIType(context);
				}
				else if ("text" == name)
					return TextType.Instance;
				else if (cd.hasMethod(context, name))
				{
					IMethodDeclaration method = cd.getMemberMethods(context, name).GetFirst();
					return new MethodType(method);
				}
				else
					throw new SyntaxError("No attribute:" + name + " in category:" + GetTypeName());
			}
			else
				throw new SyntaxError("Not a category:" + GetTypeName());
		}

		override
		public Type ToCSharpType()
		{
			return typeof(Object);
		}

		override
		public bool Equals(Object obj)
		{
			if (obj == this)
				return true;
			if (obj == null)
				return false;
			if (!(obj is CategoryType))
				return false;
			CategoryType other = (CategoryType)obj;
			return this.GetTypeName().Equals(other.GetTypeName());
		}

		override
		public void checkUnique(Context context)
		{
			IDeclaration actual = context.getRegisteredDeclaration<IDeclaration>(typeName);
			if (actual != null)
				throw new SyntaxError("Duplicate name: \"" + typeName + "\"");
		}

		IDeclaration getDeclaration(Context context)
		{
			IDeclaration actual = context.getRegisteredDeclaration<CategoryDeclaration>(typeName);
			if (actual == null)
				actual = context.getRegisteredDeclaration<EnumeratedNativeDeclaration>(typeName);
			if (actual == null)
				throw new SyntaxError("Unknown category: \"" + typeName + "\"");
			return actual;
		}

		public override IType checkMultiply(Context context, IType other, bool tryReverse)
		{
			IType type = checkOperator(context, other, tryReverse, Operator.MULTIPLY);
			if (type != null)
				return type;
			else
				return base.checkMultiply(context, other, tryReverse);
		}

		public override IType checkDivide(Context context, IType other)
		{
			IType type = checkOperator(context, other, false, Operator.DIVIDE);
			if (type != null)
				return type;
			else
				return base.checkDivide(context, other);
		}

		public override IType checkIntDivide(Context context, IType other)
		{
			IType type = checkOperator(context, other, false, Operator.IDIVIDE);
			if (type != null)
				return type;
			else
				return base.checkIntDivide(context, other);
		}

		public override IType checkModulo(Context context, IType other)
		{
			IType type = checkOperator(context, other, false, Operator.MODULO);
			if (type != null)
				return type;
			else
				return base.checkModulo(context, other);
		}

		public override IType checkAdd(Context context, IType other, bool tryReverse)
		{
			IType type = checkOperator(context, other, tryReverse, Operator.PLUS);
			if (type != null)
				return type;
			else
				return base.checkAdd(context, other, tryReverse);
		}

		public override IType checkSubstract(Context context, IType other)
		{
			IType type = checkOperator(context, other, false, Operator.MINUS);
			if (type != null)
				return type;
			else
				return base.checkSubstract(context, other);
		}

		private IType checkOperator(Context context, IType other, bool tryReverse, Operator oper)
		{
			IDeclaration actual = getDeclaration(context);
			if (actual is ConcreteCategoryDeclaration) try
				{
					IMethodDeclaration method = ((ConcreteCategoryDeclaration)actual).findOperator(context, oper, other);
					if (method == null)
						return null;
					context = context.newInstanceContext(this, false);
					Context local = context.newLocalContext();
					method.registerArguments(local);
					return method.check(local);
				}
				catch (SyntaxError)
				{
					// ok to pass, will try reverse
				}
			if (tryReverse)
				return null;
			else
				throw new SyntaxError("Unsupported operation: " + this.typeName + " " + Enums.OperatorToString(oper) + " " + other.GetTypeName());
		}

		override
		public void checkExists(Context context)
		{
			getDeclaration(context);
		}

		public override ISet<IMethodDeclaration> getMemberMethods(Context context, string name)
		{
			IDeclaration cd = getDeclaration(context);
			if (!(cd is ConcreteCategoryDeclaration))
				throw new SyntaxError("Unknown category:" + this.GetTypeName());
			ICollection<IMethodDeclaration> decls = ((ConcreteCategoryDeclaration)cd).getMemberMethods(context, name).Values;
			return new HashSet<IMethodDeclaration>(decls);
		}


		public override bool isAssignableFrom(Context context, IType other)
		{
			return base.isAssignableFrom(context, other)
					   || (other is CategoryType
						   && isAssignableFrom(context, (CategoryType)other));
		}

		public bool isAssignableFrom(Context context, CategoryType other)
		{
			return "Any" == this.typeName
				|| other.isDerivedFrom(context, this)
				|| other.isDerivedFromAnonymous(context, this);
		}

		public bool isDerivedFrom(Context context, IType other)
		{
			if (!(other is CategoryType))
				return false;
			return isDerivedFrom(context, (CategoryType)other);
		}

		public bool isDerivedFrom(Context context, CategoryType other)
		{
			try
			{
				IDeclaration thisDecl = getDeclaration(context);
				if (thisDecl is CategoryDeclaration)
					return isDerivedFrom(context, (CategoryDeclaration)thisDecl, other);
			}
			catch (SyntaxError)
			{
			}
			return false; // TODO
		}


		public bool isDerivedFrom(Context context, CategoryDeclaration decl, CategoryType other)
		{
			if (decl.getDerivedFrom() == null)
				return false;
			foreach (String derived in decl.getDerivedFrom())
			{
				CategoryType ct = new CategoryType(derived);
				if (ct.Equals(other) || ct.isDerivedFrom(context, other))
					return true;
			}
			return false;
		}


		public bool isAnonymous()
		{
			return Char.IsLower(typeName[0]); // since it's the name of the argument
		}


		public bool isDerivedFromAnonymous(Context context, IType other)
		{
			if (!(other is CategoryType))
				return false;
			return isDerivedFromAnonymous(context, (CategoryType)other);
		}

		public bool isDerivedFromAnonymous(Context context, CategoryType other)
		{
			if (!other.isAnonymous())
				return false;
			try
			{
				IDeclaration thisDecl = getDeclaration(context);
				if (thisDecl is CategoryDeclaration)
					return isDerivedFromAnonymous(context, (CategoryDeclaration)thisDecl, other);
			}
			catch (SyntaxError)
			{
			}
			return false; // TODO
		}


		public bool isDerivedFromAnonymous(Context context, CategoryDeclaration thisDecl, CategoryType other)
		{
			if (!other.isAnonymous())
				return false;
			try
			{
				IDeclaration otherDecl = other.getDeclaration(context);
				if (otherDecl is CategoryDeclaration)
					return isDerivedFromAnonymous(context, thisDecl, (CategoryDeclaration)otherDecl);
			}
			catch (SyntaxError)
			{
			}
			return false; // TODO
		}

		public bool isDerivedFromAnonymous(Context context, CategoryDeclaration thisDecl, CategoryDeclaration otherDecl)
		{
			// an anonymous category extends 1 and only 1 category
			String baseName = otherDecl.getDerivedFrom()[0];
			// check we derive from root category (if not extending 'any')
			if (!"any".Equals(baseName) && !thisDecl.isDerivedFrom(context, new CategoryType(baseName)))
				return false;
			foreach (String attribute in otherDecl.GetAllAttributes(context))
			{
				if (!thisDecl.hasAttribute(context, attribute))
					return false;
			}
			return true;
		}



		public override bool isMoreSpecificThan(Context context, IType other)
		{
			if (other is NullType || other is AnyType || other is MissingType)
				return true;
			if (!(other is CategoryType))
				return false;
			CategoryType otherCat = (CategoryType)other;
			if (otherCat.isAnonymous())
				return true;
			CategoryDeclaration thisDecl = context.getRegisteredDeclaration<CategoryDeclaration>(this.GetTypeName());
			if (thisDecl.isDerivedFrom(context, otherCat))
				return true;
			return false;
		}

		public Score scoreMostSpecific(Context context, CategoryType t1, CategoryType t2)
		{
			if (t1.Equals(t2))
				return Score.SIMILAR;
			if (this.Equals(t1))
				return Score.BETTER;
			if (this.Equals(t2))
				return Score.WORSE;
			// since this derives from both t1 and t2, return the most specific of t1 and t2
			if (t1.isMoreSpecificThan(context, t2))
				return Score.BETTER;
			if (t2.isMoreSpecificThan(context, t1))
				return Score.WORSE;
			return Score.SIMILAR; // should never happen
		}

		public IInstance newInstance(Context context)
		{
			CategoryDeclaration decl = context.getRegisteredDeclaration<CategoryDeclaration>(this.GetTypeName());
			return decl.newInstance(context);
		}

		public IInstance newInstance(Context context, IStored stored)
		{
			CategoryDeclaration decl = context.getRegisteredDeclaration<CategoryDeclaration>(this.GetTypeName());
			return decl.newInstance(context, stored);
		}

		public override IValue ConvertCSharpValueToIValue(Context context, object value)
		{
			IDeclaration decl = getDeclaration(context);
			if (decl is EnumeratedNativeDeclaration || decl is EnumeratedCategoryDeclaration)
				return LoadEnumValue(context, decl, value);
			else if (decl is CategoryDeclaration)
				return ConvertCSharpValueToIValue(context, (CategoryDeclaration)decl, value);
			else
				return base.ConvertCSharpValueToIValue(context, value);
		}

		IValue LoadEnumValue(Context context, IDeclaration decl, object value)
		{
			return context.getValue(value.ToString(), () =>
					context.getRegisteredValue<Symbol>(value.ToString())
			   );
		}

		private IValue ConvertCSharpValueToIValue(Context context, CategoryDeclaration decl, Object value)
		{
			if (DataStore.Instance.GetDbIdType().IsInstanceOfType(value))
				value = DataStore.Instance.FetchUnique(value);
			if (value is IStored)
				return decl.newInstance(context, (IStored)value);
			else
				return base.ConvertCSharpValueToIValue(context, value);
		}

		public override Comparer<IValue> getComparer(Context context, IExpression key, bool descending)
		{
			if (key == null)
				key = new UnresolvedIdentifier("key", Dialect.E);
			IDeclaration d = getDeclaration(context);
			if (d is CategoryDeclaration)
			{
				CategoryDeclaration decl = (CategoryDeclaration)d;
				if (decl.hasAttribute(context, key.ToString()))
					return new AttributeComparer(context, key.ToString(), descending);
				else if (decl.hasMethod(context, key.ToString()))
					return new CategoryMethodComparer(context, key.ToString(), descending);
				else if (globalMethodExists(context, key.ToString())) // TODO support 2 args
					return new GlobalMethodComparer(context, key.ToString(), descending, this);
				else
					return new ExpressionComparer(context, key, descending);
			}
			else
				throw new Exception("Unsupported!");
		}


		private bool globalMethodExists(Context context, String name)
		{
			try
			{
				IExpression exp = new ValueExpression(this, newInstance(context));
				ArgumentAssignment arg = new ArgumentAssignment(null, exp);
				ArgumentAssignmentList args = new ArgumentAssignmentList();
				args.Add(arg);
				MethodCall proto = new MethodCall(new MethodSelector(name), args);
				MethodFinder finder = new MethodFinder(context, proto);
				return finder.findMethod(true) != null;
			}
			catch (PromptoError)
			{
				return false;
			}
		}


	}

}

namespace prompto.type.category 
{

	class AttributeComparer : ValueComparer<IInstance>
	{
		String name;

		public AttributeComparer(Context context, String name, bool descending)
			: base(context, descending)
		{
			this.name = name;
		}


		protected override int DoCompare(IInstance o1, IInstance o2)
		{
			Object value1 = o1.GetMember(context, name, false);
			Object value2 = o2.GetMember(context, name, false);
			return ObjectUtils.CompareValues(value1, value2);
		}

	}

	class CategoryMethodComparer : Comparer<IValue>
	{
		/*Context context;
		bool descending;
		string v;*/

		public CategoryMethodComparer(Context context, String methodName, bool descending)
		{
			/*this.context = context;
			this.v = v;
			this.descending = descending;*/
			throw new NotImplementedException();
		}

		public override int Compare(IValue x, IValue y)
		{
			throw new NotImplementedException();
		}
	}

	class GlobalMethodComparer : ValueComparer<IInstance>
	{
		CategoryType type;
		MethodCall methodCall;

		public GlobalMethodComparer(Context context, String methodName, bool descending, CategoryType type)
			: base(context, descending)
		{
			this.type = type;
           	this.methodCall = buildMethodCall(methodName);
 		}

		private MethodCall buildMethodCall(String methodName)
		{
			IExpression exp = new ValueExpression(type, type.newInstance(context));
			ArgumentAssignment arg = new ArgumentAssignment(null, exp);
			ArgumentAssignmentList args = new ArgumentAssignmentList();
			args.Add(arg);
            return new MethodCall(new MethodSelector(methodName), args);
		}

		protected override int DoCompare(IInstance o1, IInstance o2)
		{
			ArgumentAssignment assignment = methodCall.getAssignments()[0];
			assignment.setExpression(new ValueExpression(type, o1));
			Object value1 = methodCall.interpret(context);
			assignment.setExpression(new ValueExpression(type, o2));
			Object value2 = methodCall.interpret(context);
			return ObjectUtils.CompareValues(value1, value2);
		}

	}


	class ExpressionComparer : ValueComparer<ConcreteInstance>
	{
		IExpression key;

		public ExpressionComparer(Context context, IExpression key, bool descending)
			: base(context, descending)
		{
			this.key = key;
		}


		protected override int DoCompare(ConcreteInstance o1, ConcreteInstance o2)
		{
			Context co = context.newInstanceContext(o1, false);
			Object value1 = key.interpret(co);
			co = context.newInstanceContext(o2, false);
			Object value2 = key.interpret(co);
			return ObjectUtils.CompareValues(value1, value2);
		}

	}



}