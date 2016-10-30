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

		public bool Mutable { get; set; } 

		public override void ToDialect(CodeWriter writer)
		{
			if (Mutable)
				writer.append ("mutable ");
			writer.append (typeName);
		}

		public override string GetTypeName()
		{
			return typeName;
		}
	
		public override IType checkMember(Context context, String name)
        {
			CategoryDeclaration cd = context.getRegisteredDeclaration<CategoryDeclaration>(GetTypeName());
            if (cd == null)
				throw new SyntaxError("Unknown category:" + GetTypeName());
            if (!cd.hasAttribute(context, name))
				throw new SyntaxError("No attribute:" + name + " in category:" + GetTypeName());
            AttributeDeclaration ad = context.getRegisteredDeclaration<AttributeDeclaration>(name);
            if (ad == null)
                throw new SyntaxError("Unknown atttribute:" + name);
            return ad.GetIType(context);
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
				actual = context.getRegisteredDeclaration<EnumeratedNativeDeclaration> (typeName);
            if (actual == null)
                throw new SyntaxError("Unknown category: \"" + typeName + "\"");
            return actual;
        }

		public override IType checkMultiply(Context context, IType other, bool tryReverse) {
			IType type = checkOperator(context, other, tryReverse, Operator.MULTIPLY);
			if(type!=null)
				return type;
			else
				return base.checkMultiply(context, other, tryReverse);
		}

		public override IType checkDivide(Context context, IType other) {
			IType type = checkOperator(context, other, false, Operator.DIVIDE);
			if(type!=null)
				return type;
			else
				return base.checkDivide(context, other);
		}

		public override IType checkIntDivide(Context context, IType other) {
			IType type = checkOperator(context, other, false, Operator.IDIVIDE);
			if(type!=null)
				return type;
			else
				return base.checkIntDivide(context, other);
		}

		public override IType checkModulo(Context context, IType other) {
			IType type = checkOperator(context, other, false, Operator.MODULO);
			if(type!=null)
				return type;
			else
				return base.checkModulo(context, other);
		}

		public override IType checkAdd(Context context, IType other, bool tryReverse) {
			IType type = checkOperator(context, other, tryReverse, Operator.PLUS);
			if(type!=null)
				return type;
			else
				return base.checkAdd(context, other, tryReverse);
		}

		public override IType checkSubstract(Context context, IType other) {
			IType type = checkOperator(context, other, false, Operator.MINUS);
			if(type!=null)
				return type;
			else
				return base.checkSubstract(context, other);
		}

		private IType checkOperator(Context context, IType other, bool tryReverse, Operator oper) {
			IDeclaration actual = getDeclaration(context);
			if(actual is ConcreteCategoryDeclaration) try {
				IMethodDeclaration method = ((ConcreteCategoryDeclaration)actual).findOperator(context, oper, other);
				if(method==null)
					return null;
				context = context.newInstanceContext(this);
				Context local = context.newLocalContext();
				method.registerArguments(local);
				return method.check(local);
			} catch(SyntaxError ) {
				// ok to pass, will try reverse
			}
			if(tryReverse)
				return null;
			else
				throw new SyntaxError("Unsupported operation: " + this.typeName + " " + Enums.OperatorToString(oper) + " " + other.GetTypeName());
		}

		override
        public void checkExists(Context context)
        {
            getDeclaration(context);
        }

        
		public override bool isAssignableTo(Context context, IType other)
        {
			if (typeName.Equals(other.GetTypeName()))
                return true;
			if (other is NullType || other is AnyType || other is MissingType)
                return true;
            if (!(other is CategoryType))
                return false;
            return isAssignableTo(context, (CategoryType)other);
        }

        bool isAssignableTo(Context context, CategoryType other)
        {
			if (typeName.Equals(other.GetTypeName()))
                return true;
            try
            {
                IDeclaration d = getDeclaration(context);
				if(d is CategoryDeclaration)
				{
					CategoryDeclaration cd = (CategoryDeclaration)d;
					return isDerivedFromCompatibleCategory(context, cd, other)
                    	|| isAssignableToAnonymousCategory(context, cd, other);
				}
				else
					return false;
            }
            catch (SyntaxError)
            {
                return false;
            }
        }

        bool isDerivedFromCompatibleCategory(Context context, CategoryDeclaration decl, CategoryType other)
        {
            if (decl.getDerivedFrom() == null)
                return false;
            foreach (String derived in decl.getDerivedFrom())
            {
                CategoryType ct = new CategoryType(derived);
                if (ct.isAssignableTo(context, other))
                    return true;
            }
            return false;
        }

        bool isAssignableToAnonymousCategory(Context context, CategoryDeclaration decl, CategoryType other)
        {
            if (!other.isAnonymous())
                return false;
            try
            {
				IDeclaration d = other.getDeclaration(context);
				if(d is CategoryDeclaration) 
				{
					CategoryDeclaration cd = (CategoryDeclaration)d;
	                return isAssignableToAnonymousCategory(context, decl, cd);
				}
				else
					return false;
            }
            catch (SyntaxError)
            {
                return false;
            }
        }

        public bool isAnonymous()
        {
            return Char.IsLower(typeName[0]); // since it's the name of the argument
        }

        bool isAssignableToAnonymousCategory(Context context, CategoryDeclaration decl, CategoryDeclaration other)
        {
            // an anonymous category : 1 and only 1 category
            String baseName = other.getDerivedFrom()[0];
            // check we derive from root category (if not extending 'Any')
            if (!"any".Equals(baseName) && !decl.isDerivedFrom(context, new CategoryType(baseName)))
                return false;
            foreach (String attribute in other.getAttributes())
            {
                if (!decl.hasAttribute(context, attribute))
                    return false;
            }
            return true;
        }

        override
        public bool isMoreSpecificThan(Context context, IType other)
        {
			if(other is NullType || other is AnyType || other is MissingType)
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

		public IInstance newInstance(Context context, IStored stored) {
			CategoryDeclaration decl = context.getRegisteredDeclaration<CategoryDeclaration>(this.GetTypeName());
			return decl.newInstance(context, stored);
		}


        public ListValue sort(Context context, IContainer list, IExpression key, bool descending)
        {
            if (key == null)
                key = new UnresolvedIdentifier("key");
            IDeclaration d = getDeclaration(context);
			if (d is CategoryDeclaration) {
				CategoryDeclaration decl = (CategoryDeclaration)d;
				if (decl.hasAttribute (context, key.ToString ()))
					return sortByAttribute (context, list, key.ToString (), descending);
				else if (decl.hasMethod (context, key.ToString (), null))
					return sortByClassMethod (context, list, key.ToString (),descending);
				else if (globalMethodExists (context, list, key.ToString ()))
					return sortByGlobalMethod (context, list, key.ToString (), descending);
				else
					return sortByExpression (context, list, key, descending);
			} else
				throw new Exception ("Unsupported!");
        }


		public override IValue ConvertCSharpValueToIValue(Context context, object value)
		{
			IDeclaration decl = getDeclaration(context);
			if (decl is CategoryDeclaration)
				return ConvertCSharpValueToIValue(context, (CategoryDeclaration)decl, value);
			else
				return base.ConvertCSharpValueToIValue(context, value);
		}

		private IValue ConvertCSharpValueToIValue(Context context, CategoryDeclaration decl, Object value) 
		{
			if(DataStore.Instance.GetDbIdType().IsInstanceOfType(value))
				value = DataStore.Instance.FetchUnique(value);
			if(value is IStored)
				return decl.newInstance(context, (IStored)value);
			else
				return base.ConvertCSharpValueToIValue(context, value);
		}


		class ConcreteInstanceComparer : ExpressionComparer<ConcreteInstance>
        {
            CategoryType type;
            Context context;
            IExpression key;

            public ConcreteInstanceComparer(CategoryType type, Context context, IExpression key, bool descending)
                : base(context, descending)
            {
                this.type = type;
                this.context = context;
                this.key = key;
            }

            override
            protected int DoCompare(ConcreteInstance o1, ConcreteInstance o2)
            {
                Context co = context.newInstanceContext(o1);
                Object key1 = key.interpret(co);
                co = context.newInstanceContext(o2);
                Object key2 = key.interpret(co);
                return type.compareKeys(key1, key2);
            }

        }
		private ListValue sortByExpression(Context context, IContainer list, IExpression key, bool descending)
        {
			return this.doSort(context, list, new ConcreteInstanceComparer(this, context, key, descending));
        }

        public class InstanceAttributeComparer : ExpressionComparer<IInstance>
        {
            CategoryType type;
            Context context;
            String name;

            public InstanceAttributeComparer(CategoryType type, Context context, String name, bool descending)
				: base(context, descending)
            {
                this.type = type;
                this.context = context;
                this.name = name;
            }

            override
            protected int DoCompare(IInstance o1, IInstance o2)
            {
                Object key1 = o1.GetMember(context, name, false);
                Object key2 = o2.GetMember(context, name, false);
                return type.compareKeys(key1, key2);
            }

        }

		private ListValue sortByAttribute(Context context, IContainer list, String name, bool descending)
        {
			return this.doSort( context, list, new InstanceAttributeComparer(this, context, name, descending));
        }

		private ListValue sortByClassMethod(Context context, IContainer list, String name, bool descending)
        {
            return null;
        }

		private bool globalMethodExists(Context context, IContainer list, String name)
        {
            try
            {
				IExpression exp = new ExpressionValue(this, newInstance(context));
                ArgumentAssignment arg = new ArgumentAssignment(null, exp);
                ArgumentAssignmentList args = new ArgumentAssignmentList(arg);
                MethodCall proto = new MethodCall(new MethodSelector(name), args);
                MethodFinder finder = new MethodFinder(context, proto);
                return finder.findMethod(true) != null;
            }
            catch (PromptoError)
            {
                return false;
            }
        }

		private ListValue sortByGlobalMethod(Context context, IContainer list, String name, bool descending)
        {
			IExpression exp = new ExpressionValue(this, newInstance(context));
            ArgumentAssignment arg = new ArgumentAssignment(null, exp);
            ArgumentAssignmentList args = new ArgumentAssignmentList(arg);
            MethodCall proto = new MethodCall(new MethodSelector(name), args);
            MethodFinder finder = new MethodFinder(context, proto);
            IMethodDeclaration method = finder.findMethod(true);
			return sortByGlobalMethod(context, list, proto, method, descending);
        }

        class InstanceGlobalMethodComparer : ExpressionComparer<IInstance>
        {
            CategoryType type;
            Context context;
            MethodCall method;

            public InstanceGlobalMethodComparer(CategoryType type, Context context, MethodCall method, bool descending)
                : base(context, descending)
            {
                this.type = type;
                this.context = context;
                this.method = method;
            }

            override
            protected int DoCompare(IInstance o1, IInstance o2)
            {
                ArgumentAssignment assignment = method.getAssignments()[0];
				assignment.setExpression(new ExpressionValue(type, o1));
                Object key1 = method.interpret(context);
				assignment.setExpression(new ExpressionValue(type, o2));
                Object key2 = method.interpret(context);
                return type.compareKeys(key1, key2);
            }

        }
		private ListValue sortByGlobalMethod(Context context, IContainer list, MethodCall method, IMethodDeclaration declaration, bool descending)
        {
			return this.doSort(context, list, new InstanceGlobalMethodComparer(this, context, method, descending));
        }

        private int compareKeys(Object key1, Object key2)
        {
            if (key1 == null && key2 == null)
                return 0;
            else if (key1 == null)
                return -1;
            else if (key2 == null)
                return 1;
            else if (key1 is IComparable && key2 is IComparable)
                return ((IComparable)key1).CompareTo((IComparable)key2); 
            else
                return key1.ToString().CompareTo(key2.ToString());
        }

    }

}