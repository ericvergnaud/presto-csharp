using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using prompto.runtime;
using prompto.type;

namespace prompto.value
{
	public interface IContainer : IIterable
    {
        bool HasItem(Context context, IValue iValue);
        IValue GetItem(Context context, IValue item);
    }
}
