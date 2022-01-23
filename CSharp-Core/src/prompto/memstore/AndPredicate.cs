﻿using System;
using System.Collections.Generic;

namespace prompto.memstore
{
	public class AndPredicate : IPredicate
	{

		IPredicate left;
		IPredicate right;

		public AndPredicate(IPredicate left, IPredicate right)
		{
			this.left = left;
			this.right = right;
		}

		public bool matches(IDictionary<String, Object> document)
		{
			return left.matches(document) && right.matches(document);
		}

	}

}
