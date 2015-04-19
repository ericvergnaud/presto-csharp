using System;
using presto.runtime;
using presto.value;
using Decimal = presto.value.Decimal;
using presto.grammar;

namespace presto.type
{

    public class IntegerType : NativeType
    {

        static IntegerType instance_ = new IntegerType();

        public static IntegerType Instance
        {
            get
            {
                return instance_;
            }
        }

        private IntegerType()
            : base("Integer")
        {
        }

        override
        public Type ToCSharpType()
        {
            return typeof(Integer);
        }

        override
        public bool isAssignableTo(Context context, IType other)
        {
            return (other is IntegerType) || (other is DecimalType) || (other is AnyType);
        }

        
		public override IType checkAdd(Context context, IType other, bool tryReverse)
        {
            if (other is IntegerType)
                return this;
            if (other is DecimalType)
                return other;
			return base.checkAdd(context, other, tryReverse);
        }

        override
        public IType checkSubstract(Context context, IType other)
        {
            if (other is IntegerType)
                return this;
            if (other is DecimalType)
                return other;
            return base.checkSubstract(context, other);
        }

        
		public override IType checkMultiply(Context context, IType other, bool tryReverse)
        {
            if (other is IntegerType)
                return this;
            if (other is DecimalType)
                return other;
            if (other is CharacterType)
                return TextType.Instance;
            if (other is TextType)
                return other;
            if (other is PeriodType)
                return other;
            if (other is ListType)
                return other;
			return base.checkMultiply(context, other, tryReverse);
        }

        override
        public IType checkDivide(Context context, IType other)
        {
            if (other is IntegerType)
                return DecimalType.Instance;
            if (other is DecimalType)
                return other;
            return base.checkDivide(context, other);
        }

        override
       public IType checkIntDivide(Context context, IType other)
        {
            if (other is IntegerType)
                return IntegerType.Instance;
            return base.checkDivide(context, other);
        }

        override
        public IType CheckModulo(Context context, IType other)
        {
            if (other is IntegerType)
                return IntegerType.Instance;
            return base.checkDivide(context, other);
        }

        override
        public IType checkCompare(Context context, IType other)
        {
            if (other is IntegerType)
                return BooleanType.Instance;
            if (other is DecimalType)
                return BooleanType.Instance;
            return base.checkCompare(context, other);
        }

        override
        public IType checkRange(Context context, IType other)
        {
            if (other is IntegerType)
                return new RangeType(this);
            return base.checkCompare(context, other);
        }

        override
        public IRange newRange(Object left, Object right)
        {
            if (left is Integer && right is Integer)
                return new IntegerRange((Integer)left, (Integer)right);
            return base.newRange(left, right);
        }

        override
		public ListValue sort(Context context, IContainer list)
        {
			return this.doSort(context, list, new IntegerComparer(context));
        }

        override
         public IValue ConvertCSharpValueToPrestoValue(Object value)
        {
            if (value is Int16)
                return new Integer((Int16)value);
            else if (value is Int16?)
                return new Integer(((Int16?)value).Value);
            else if (value is Int32)
                return new Integer((Int32)value);
            else if (value is Int32?)
                return new Integer(((Int32?)value).Value);
            else if (value is Int64)
                return new Integer((Int64)value);
            else if (value is Int64?)
                return new Integer(((Int64?)value).Value);
            else
                return (IValue)value; // TODO for now
        }

        override
    public IType CheckMember(Context context, String name)
        {
		if(name == "min")
			return this;
		else if(name=="max")
			return this;
		else
            return base.CheckMember(context, name);
	}

	override
	public IValue getMember(Context context, String name) {
		if(name=="min")
			return new Integer(Int64.MinValue);
		else if(name=="max")
            return new Integer(Int64.MaxValue);
		else
            return base.getMember(context, name);
	}

    }

    class IntegerComparer : ExpressionComparer<INumber>
    {
  
        public IntegerComparer(Context context)
            : base(context)
        {
         }

        override
        protected int DoCompare(INumber o1, INumber o2)
        {
            return o1.IntegerValue.CompareTo(o2.IntegerValue);
        }

    }

}
