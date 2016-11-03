using prompto.error;
using prompto.grammar;
using System.Collections.Generic;
using System.Threading;
using System;
using prompto.utils;
using System.Text;
using prompto.runtime;
using prompto.declaration;
using prompto.type;
using prompto.store;
using prompto.argument;

namespace prompto.value
{

	public class ConcreteInstance : BaseValue, IInstance, IMultiplyable
	{

		ConcreteCategoryDeclaration declaration;
		Dictionary<String, IValue> values = new Dictionary<String, IValue> ();
		bool mutable = false;
		IStorable storable = null;

		public ConcreteInstance (Context context, ConcreteCategoryDeclaration declaration)
			: base (new CategoryType (declaration.GetName ()))
		{
			this.declaration = declaration;
			if (declaration.Storable)
			{
				List<String> categories = declaration.CollectCategories(context);
				storable = DataStore.Instance.NewStorable(categories);
			}
		}

		public IStorable getStorable() {
			return storable;
		}

		public override void CollectStorables(List<IStorable> list)
		{
			if (storable == null)
				throw new NotStorableError();
			if (storable.Dirty) {
				GetOrCreateDbId();
				list.Add(storable);
			}
			foreach (IValue value in values.Values)
			{
				value.CollectStorables(list);
			}
		}

		public override Object GetStorableData() 
		{
			// this is called when storing the instance as a field value, so we just return the dbId
			// the instance data itself will be collected as part of collectStorables
			if(this.storable==null)
				throw new NotStorableError();
			else
				return this.GetOrCreateDbId();
		}

		private Object GetDbId()
		{
			try
			{
				IValue dbId;
				if (values.TryGetValue("dbId", out dbId))
					return dbId.GetStorableData();
				else
					return null;
			}
			catch (NotStorableError e)
			{
				throw new Exception("Should never get here!", e);
			}
		}

		private Object GetOrCreateDbId() 
		{
			Object dbId = GetDbId();
			if (dbId == null)
			{
				dbId = this.storable.GetOrCreateDbId();
				IValue value = TypeUtils.FieldToValue(null, "dbId", dbId);
				values["dbId"] = value;
			}
			return dbId;
		}


		public bool setMutable (bool set)
		{
			bool result = mutable;
			mutable = set;
			return result;
		}

		public override bool IsMutable ()
		{
			return mutable;
		}

			
		public ConcreteCategoryDeclaration getDeclaration ()
		{
			return declaration;
		}

		public CategoryType getType ()
		{
			return (CategoryType)type;
		}

		public ICollection<String> GetMemberNames ()
		{
			return values.Keys;
		}

		static Dictionary<String, Context> Factory ()
		{
			return new Dictionary<String, Context> ();
		}

		// don't call getters from getters, so register them
		ThreadLocal<Dictionary<String, Context>> activeGetters = new ThreadLocal<Dictionary<String, Context>> (Factory);

        
		public override IValue GetMember (Context context, String attrName, bool autoCreate)
		{
			Context stacked;
			bool first = !activeGetters.Value.TryGetValue (attrName, out stacked);
			if (first)
				activeGetters.Value [attrName] = context;
			try {
				return GetMemberAllowGetter (context, attrName, stacked == null);
			} finally {
				if (first)
					activeGetters.Value [attrName] = null;
			}
		}

		protected IValue GetMemberAllowGetter (Context context, String attrName, bool allowGetter)
		{
			GetterMethodDeclaration getter = allowGetter ? declaration.findGetter (context, attrName) : null;
			if (getter != null) {
				context = context.newInstanceContext (this).newChildContext();
				return getter.interpret(context);
			} else {
				IValue value = null;
				values.TryGetValue (attrName, out value);
				return value;
			}
		}

		// don't call setters from setters, so register them
		ThreadLocal<Dictionary<String, Context>> activeSetters = new ThreadLocal<Dictionary<String, Context>> (Factory);

		public override void SetMember (Context context, String attrName, IValue value)
		{
			if (!mutable)
				throw new NotMutableError ();
			Context stacked;
			bool first = !activeSetters.Value.TryGetValue (attrName, out stacked);
			if (first)
				activeSetters.Value [attrName] = context;
			try {
				SetMember (context, attrName, value, stacked == null);
			} finally {
				if (first)
					activeSetters.Value [attrName] = null;
			}
		}

		public void SetMember (Context context, String attrName, IValue value, bool allowSetter)
		{
			AttributeDeclaration decl = context.getRegisteredDeclaration<AttributeDeclaration> (attrName);
			SetterMethodDeclaration setter = allowSetter ? declaration.findSetter (context, attrName) : null;
			if (setter != null) {
				// use attribute name as parameter name for incoming value
				context = context.newInstanceContext (this).newChildContext();
				context.registerValue (new Variable (attrName, decl.getIType ()));
				context.setValue (attrName, value);
				value = setter.interpret (context);
			}
			value = autocast (decl, value);
			values [attrName] = value;
			if(storable!=null && decl.Storable) {
				// TODO convert object graph if(value instanceof IInstance)
				storable.SetData(attrName, value.GetStorableData());
			}
		}

		private IValue autocast (AttributeDeclaration decl, IValue value)
		{
			if (value != null && value is prompto.value.Integer && decl.getIType () == DecimalType.Instance)
				value = new Decimal (((prompto.value.Integer)value).DecimalValue);
			return value;
		}


        public override bool Equals (Object obj)
		{
			if (!(obj is ConcreteInstance))
				return false;
			return ObjectUtils.EqualDictionaries (this.values, ((ConcreteInstance)obj).values);
		}


        public override String ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append ("{");
			if (values.Count > 0) {
				foreach (KeyValuePair<String, IValue> kvp in values) {
					if ("dbId".Equals(kvp.Key))
						continue;
					sb.Append (kvp.Key);
					sb.Append (":");
					sb.Append (kvp.Value);
					sb.Append (", ");
				}
				sb.Length = sb.Length - 2;
			}
			sb.Append ("}");
			return sb.ToString ();
		}


		public override IValue Multiply (Context context, IValue value)
		{
			try {
				return interpretOperator (context, value, Operator.MULTIPLY);
			} catch (SyntaxError) {
				return base.Multiply (context, value);
			}
		}

		
		public override IValue Divide (Context context, IValue value)
		{
			try {
				return interpretOperator (context, value, Operator.DIVIDE);
			} catch (SyntaxError) {
				return base.Divide (context, value);
			}
		}

		
		public override IValue IntDivide (Context context, IValue value)
		{
			try {
				return interpretOperator (context, value, Operator.IDIVIDE);
			} catch (SyntaxError) {
				return base.IntDivide (context, value);
			}
		}

		
		public override IValue Modulo (Context context, IValue value)
		{
			try {
				return interpretOperator (context, value, Operator.MODULO);
			} catch (SyntaxError) {
				return base.Modulo (context, value);
			}
		}

		
		public override IValue Add (Context context, IValue value)
		{
			try {
				return interpretOperator (context, value, Operator.PLUS);
			} catch (SyntaxError) {
				return base.Add (context, value);
			}
		}

			
		public override IValue Subtract (Context context, IValue value)
		{
			try {
				return interpretOperator (context, value, Operator.MINUS);
			} catch (SyntaxError) {
				return base.Subtract (context, value);
			}
		}


		private IValue interpretOperator (Context context, IValue value, Operator oper)
		{
			IMethodDeclaration decl = declaration.findOperator (context, oper, value.GetIType ());
			context = context.newInstanceContext (this).newChildContext ();
			decl.registerArguments (context);
			IArgument arg = decl.getArguments () [0];
			context.setValue (arg.GetName (), value);
			return decl.interpret (context);
		}

	}

}
