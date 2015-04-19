using presto.runtime;
using System;
namespace presto.type
{

    public class RangeType : CollectionType
    {

        public RangeType(IType itemType)
			: base(itemType.GetName() + "[..]", itemType)
        {
        }

        override
        public bool isAssignableTo(Context context, IType other)
        {
            return this.Equals(other);
        }

        override
        public System.Type ToCSharpType()
        {
            return null; // no equivalent
        }


        override
        public bool Equals(Object obj)
        {
            if (obj == this)
                return true;
            if (obj == null)
                return false;
            if (!(obj is RangeType))
                return false;
            RangeType other = (RangeType)obj;
            return this.GetItemType().Equals(other.GetItemType());
        }

        override
        public IType checkItem(Context context, IType other)
        {
            if (other == IntegerType.Instance)
                return itemType;
            else
                return base.checkItem(context, other);
        }

        override
        public IType checkSlice(Context context)
        {
            return this;
        }

        override
        public IType checkIterator(Context context)
        {
            return itemType;
        }

    }

}
