using System;
using prompto.value;
using prompto.error;
using prompto.runtime;
using prompto.grammar;
using System.Collections.Generic;
using prompto.expression;
using prompto.type;
using Newtonsoft.Json;
using prompto.store;
using System.Text;

namespace prompto.value
{


	public class ListValue : List<IValue>, ISliceable, IFilterable, IMultiplyable
	{
		List<Object> storables;
		ListType type;
		bool mutable = false;

		public ListValue(IType itemType)
		{
			type = new ListType(itemType);
		}

		public ListValue(IType itemType, IValue value)
		{
			type = new ListType(itemType);
			Add(value);
		}

		public ListValue(IType itemType, List<IValue> values)
		{
			type = new ListType(itemType);
			AddRange(values);
		}

		public ListValue(IType itemType, List<IValue> values, bool mutable)
		{
			type = new ListType(itemType);
			AddRange(values);
			this.mutable = mutable;
		}

		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			foreach (Object o in this)
			{
				sb.Append(o.ToString());
				sb.Append(", ");
			}
			if (sb.Length > 2)
				sb.Length = sb.Length - 2;
			sb.Append("]");
			return sb.ToString();
		}

		public void ToJson(Context context, JsonWriter generator, Object instanceId, String fieldName, bool withType, Dictionary<String, byte[]> binaries)
		{
			throw new NotSupportedException("No ToJson support for " + this.GetType().Name);
		}

		public bool IsMutable()
		{
			return this.mutable;
		}

		public void SetIType(IType type)
		{
			this.type = (ListType)type;
		}

		public IType GetIType()
		{
			return type;
		}

		public IType ItemType
		{
			get { return type.GetItemType(); }
		}

		public Object ConvertTo(Type type)
		{
			return this;
		}


		public IEnumerable<IValue> GetEnumerable(Context context)
		{
			return this;
		}


		public void CollectStorables(List<IStorable> storables)
		{
			this.ForEach((item) => item.CollectStorables(storables));
		}

		public Object GetStorableData()
		{
			if (storables == null)
			{
				storables = new List<Object>();
				this.ForEach((item) => storables.Add(item.GetStorableData()));
			}
			return storables;
		}



		public bool Empty()
		{
			return Count == 0;
		}

		public long Length()
		{
			return this.Count;
		}


		public bool Equals(Context context, IValue rval)
		{
			if (!(rval is ListValue))
				return false;
			ListValue list = (ListValue)rval;
			if (this.Count != list.Count)
				return false;
			IEnumerator<Object> li = this.GetEnumerator();
			IEnumerator<Object> ri = list.GetEnumerator();
			while (li.MoveNext() && ri.MoveNext())
			{
				Object lival = li.Current;
				if (lival is IExpression)
					lival = ((IExpression)lival).interpret(context);
				Object rival = ri.Current;
				if (rival is IExpression)
					rival = ((IExpression)rival).interpret(context);
				if (lival is IValue && rival is IValue)
				{
					if (!((IValue)lival).Equals(context, (IValue)rival))
						return false;
				}
				else if (!lival.Equals(rival))
					return false;
			}
			return true;
		}

		public bool Roughly(Context context, IValue lval)
		{
			return this.Equals(context, lval); // TODO
		}

		public Int32 CompareTo(Context context, IValue value)
		{
			throw new NotSupportedException("Compare not supported by " + this.GetType().Name);
		}



		public IValue Add(Context context, IValue value)
		{
			if (value is ListValue)
			{
				ListValue result = new ListValue(type.GetItemType());
				result.AddRange(this);
				result.AddRange((ListValue)value);
				return result;
			}
			else if (value is SetValue)
			{
				ListValue result = new ListValue(type.GetItemType());
				result.AddRange(this);
				result.AddRange(((SetValue)value).getItems());
				return result;
			}
			else
				throw new SyntaxError("Illegal : List + " + value.GetType().Name);
		}


		public IValue Multiply(Context context, IValue value)
		{
			if (value is Integer)
			{
				int count = (int)((Integer)value).IntegerValue;
				if (count < 0)
					throw new SyntaxError("Negative repeat count:" + count);
				if (count == 0)
					return new ListValue(this.ItemType);
				if (count == 1)
					return this;
				ListValue result = new ListValue(this.ItemType);
				for (long i = 0; i < count; i++)
					result.AddRange(this); // TODO: interpret items ?
				return result;
			}
			else
				throw new SyntaxError("Illegal: List * " + value.GetType().Name);
		}

		public IValue Subtract(Context context, IValue value)
		{
			throw new NotSupportedException("Subtract not supported by " + this.GetType().Name);
		}

		public IValue Divide(Context context, IValue value)
		{
			throw new NotSupportedException("Divide not supported by " + this.GetType().Name);
		}

		public IValue IntDivide(Context context, IValue value)
		{
			throw new NotSupportedException("IntDivide not supported by " + this.GetType().Name);
		}

		public IValue Modulo(Context context, IValue value)
		{
			throw new NotSupportedException("Modulo not supported by " + this.GetType().Name);
		}

		public IValue GetMember(Context context, String name, bool autoCreate)
		{
			if ("count" == name)
				return new Integer(this.Count);
			else if ("text" == name)
				return new Text(this.ToString());
			else
				throw new NotSupportedException("No such member " + name);
		}

		public void SetMember(Context context, String name, IValue value)
		{
			throw new NotSupportedException("No such member:" + name);
		}

		public bool HasItem(Context context, IValue value)
		{
			return this.Contains(value);
		}

		public virtual IValue GetItem(Context context, IValue index)
		{
			if (index is Integer)
			{
				try
				{
					return this[(int)((Integer)index).IntegerValue - 1];
				}
				catch (ArgumentOutOfRangeException)
				{
					throw new IndexOutOfRangeError();
				}


			}
			else
				throw new InvalidDataError("No such item:" + index.ToString());
		}

		public virtual void SetItem(Context context, IValue item, IValue value)
		{
			if (!(item is Integer))
				throw new InvalidDataError("No such item:" + item.ToString());
			int index = (int)((Integer)item).IntegerValue;
			try
			{
				this[index - 1] = value;
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new IndexOutOfRangeError();
			}
		}

		public ISliceable Slice(Context context, Integer fi, Integer li)
		{
			long fi_ = (fi == null) ? 1L : fi.IntegerValue;
			if (fi_ < 0)
				throw new IndexOutOfRangeError();
			long li_ = (li == null) ? (long)Count : li.IntegerValue;
			if (li_ < 0)
				li_ = Count + 1 + li_;
			else if (li_ > Count)
				throw new IndexOutOfRangeError();
			ListValue result = new ListValue(type.GetItemType());
			long idx = 0;
			foreach (IValue e in this)
			{
				if (++idx < fi_)
					continue;
				if (idx > li_)
					break;
				result.Add(e);
			}
			return result;
		}

		public IFilterable Filter(Context context, String itemName, IExpression filter)
		{
			ListValue result = new ListValue(type.GetItemType());
			foreach (IValue o in this)
			{
				context.setValue(itemName, o);
				Object test = filter.interpret(context);
				if (!(test is Boolean))
					throw new InternalError("Illegal test result: " + test);
				if (((Boolean)test).Value)
					result.Add(o);
			}
			return result;
		}



	}
}
