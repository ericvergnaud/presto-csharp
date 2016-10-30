﻿using System;
using prompto.runtime;
using prompto.store;

namespace prompto.type
{

	public abstract class BinaryType : NativeType
	{

		protected BinaryType (TypeFamily family)
			: base (family)
		{
		}

	
		public override IType checkMember (Context context, String name)
		{
			if ("name" == name)
				return TextType.Instance;
			else if ("format" == name)
				return TextType.Instance;
			else
				return base.checkMember (context, name);
		}

		public override Type ToCSharpType ()
		{
			throw new NotImplementedException ();
		}
	
	}
}