﻿using prompto.error;
using prompto.expression;
using prompto.parser;
using prompto.runtime;
using prompto.store;
using prompto.type;
using prompto.utils;
using prompto.value;
using System;
using System.Collections.Generic;

namespace prompto.statement
{


	public class StoreStatement : BaseStatement
	{

		ExpressionList deletables;
		ExpressionList storables;
		StatementList andThen;

		public StoreStatement (ExpressionList deletables, ExpressionList storables, StatementList andThen)
		{
			this.deletables = deletables;
			this.storables = storables;
			this.andThen = andThen;
		}

		public override bool IsSimple
		{
			get
			{
				return andThen==null;
			}
		}

		public override void ToDialect (CodeWriter writer)
		{
			if (deletables != null) {
				writer.append ("delete ");
				if (writer.getDialect () == Dialect.E)
					deletables.toDialect (writer);
				else {
					writer.append ('(');
					deletables.toDialect (writer);
					writer.append (')');
				}
				if (storables != null)
					writer.append (" and ");
			}
			if (storables != null) {
				writer.append ("store ");
				if (writer.getDialect () == Dialect.E)
					storables.toDialect (writer);
				else {
					writer.append ('(');
					storables.toDialect (writer);
					writer.append (')');
				}
			}
			if(andThen!=null) {
				if(writer.getDialect()==Dialect.O) {
					writer.append(" then {").newLine().indent();
					andThen.ToDialect(writer);
					writer.dedent().append("}");
				} else {
					writer.append(" then:").newLine().indent();
					andThen.ToDialect(writer);
					writer.dedent();
				}
			}
		}

		public override String ToString ()
		{
			return "store " + storables.ToString ();
		}

		public override bool Equals (Object obj)
		{
			if (obj == this)
				return true;
			if (obj == null)
				return false;
			if (!(obj is StoreStatement))
				return false;
			StoreStatement other = (StoreStatement)obj;
			return this.storables.Equals (other.storables);
		}

		public override IType check (Context context)
		{
			// TODO check expression
			return VoidType.Instance;
		}

		public override IValue interpret (Context context)
		{
			List<object> toDelete = null;
			if (deletables != null)
			{
				toDelete = new List<object>();
				deletables.ForEach((exp) => CollectDeletables(context, exp, toDelete));
			}
			List<IStorable> toStore = null;
			if (storables != null)
			{
				toStore = new List<IStorable>();
				storables.ForEach((exp) => CollectStorables(context, exp, toStore));
			}
			if (deletables != null || storables != null)
				DataStore.Instance.Store(toDelete, toStore);
			if(andThen!=null)
				andThen.interpret(context);
			return null;
		}

		void CollectDeletables(Context context, IExpression exp, List<object> deletables)
		{
			IValue value = exp.interpret(context);
			if (value == NullValue.Instance)
				return;
			else if (value is IInstance)
			{
				IValue dbId = ((IInstance)value).GetMemberValue(context, "dbId", false);
				if (dbId != null && dbId != NullValue.Instance)
					deletables.add(dbId.GetStorableData());
			}
			else if (value is IEnumerable<IValue>)
			{
				IEnumerator<IValue> iter = ((IEnumerable<IValue>)value).GetEnumerator();
				while (iter.MoveNext())
				{
					IValue item = iter.Current;
					if (item is IInstance)
					{
						IValue dbId = ((IInstance)value).GetMemberValue(context, "dbId", false);
						if (dbId != null && dbId!=NullValue.Instance)
							deletables.add(dbId.GetStorableData());
					}
				}
			}
			else
				throw new NotSupportedException("Can't delete " + value.GetType());
		}

		void CollectStorables(Context context, IExpression exp, List<IStorable> storables)
		{
			IValue value = exp.interpret(context);
			value.CollectStorables(storables);
		}
	}
}
