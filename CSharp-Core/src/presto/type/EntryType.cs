using presto.runtime;
using System;
using presto.literal;

namespace presto.type
{

public class EntryType : BaseType {

	IType itemType;
	
	public EntryType(IType itemType) 
			: base(itemType.GetName()+"{}[]")
   {
		this.itemType = itemType;
	}

    override
    public IType CheckMember(Context context, String name)
    {
        if ("key" == name)
            return TextType.Instance;
        else if ("value" == name)
            return itemType;
        else
            return base.CheckMember(context, name);
    }
    
    public IType getItemType()
    {
		return itemType;
	}


	override
	public Type ToCSharpType() {
		return typeof(DictEntry);
	}

	override
	public void checkUnique(Context context) {
		throw new Exception("Should never get there!");
	}

	override
	public void checkExists(Context context) {
		throw new Exception("Should never get there!");
	}

	override
	public bool isAssignableTo(Context context, IType other) {
		throw new Exception("Should never get there!");
	}

	override
	public bool isMoreSpecificThan(Context context, IType other) {
		throw new Exception("Should never get there!");
	}

}


}