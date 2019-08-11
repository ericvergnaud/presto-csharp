using System;
using prompto.parser;
using System.Text;
using prompto.runtime;
using prompto.error;
using prompto.grammar;
using prompto.type;
using prompto.value;
using prompto.param;


namespace prompto.declaration
{

    public abstract class BaseMethodDeclaration : BaseDeclaration, IMethodDeclaration
    {

        CategoryDeclaration memberOf;
        IMethodDeclaration closureOf;
        protected ParameterList parameters;
        protected IType returnType;

        public BaseMethodDeclaration(String name, ParameterList parameters)
            : base(name)
        {
            this.parameters = parameters != null ? parameters : new ParameterList();
            this.returnType = null;
        }

        public BaseMethodDeclaration(String name, ParameterList parameters, IType returnType)
            : base(name)
        {
            this.parameters = parameters != null ? parameters : new ParameterList();
            this.returnType = returnType;
        }

        public void setMemberOf(CategoryDeclaration declaration)
        {
            this.memberOf = declaration;
        }

        public CategoryDeclaration getMemberOf()
        {
            return memberOf;
        }

        public new IMethodDeclaration ClosureOf
        {
            set
            {
                this.closureOf = value;
            }
            get
            {
                return closureOf;
            }
        }


        public IType getReturnType()
        {
            return this.returnType;
        }

        public String getSignature(Dialect dialect)
        {
            StringBuilder sb = new StringBuilder(GetName());
            sb.Append('(');
            foreach (IParameter arg in parameters)
            {
                sb.Append(arg.getSignature(dialect));
                sb.Append(", ");
            }
            if (parameters.Count > 0)
                sb.Length = sb.Length - 2; // strip ", "
            sb.Append(')');
            return ToString();
        }

        override
        public String ToString()
        {
            return GetName() + ":(" + parameters.ToString() + ')';
        }

        public String getProto()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IParameter arg in parameters)
            {
                if (sb.Length > 0)
                    sb.Append('/');
                sb.Append(arg.getProto());
            }
            return sb.ToString();
        }

        public ParameterList getParameters()
        {
            return parameters;
        }

        override
        public void register(Context context)
        {
            context.registerDeclaration(this);
        }

        public void registerParameters(Context context)
        {
            if (parameters != null)
                parameters.register(context);
        }

        override
        public IType GetIType(Context context)
        {
            try
            {
                return check(context);
            }
            catch (SyntaxError e)
            {
                throw new Exception(null, e);
            }
        }

        public bool isAssignableTo(Context context, ArgumentList arguments, bool useInstance)
        {
            try
            {
                Context local = context.newLocalContext();
                registerParameters(local);
                ArgumentList argumentsList = new ArgumentList(arguments);
                foreach (IParameter parameter in parameters)
                {
                    Argument argument = argumentsList.find(parameter.GetName());
                    if (argument == null)
                    {
                        if (parameter.DefaultValue != null)
                            argument = new Argument(parameter, parameter.DefaultValue);
                    }
                    if (argument == null) // missing argument
                        return false;
                    argumentsList.Remove(argument);
                    if (!isAssignableTo(local, parameter, argument, useInstance))
                        return false;
                }
                return argumentsList.Count == 0;
            }
            catch (SyntaxError)
            {
                return false;
            }
        }

        bool isAssignableTo(Context context, IParameter parameter, Argument argument, bool useInstance)
        {
            return computeSpecificity(context, parameter, argument, useInstance) != Specificity.INCOMPATIBLE;
        }

        public Specificity? computeSpecificity(Context context, IParameter parameter, Argument argument, bool useInstance)
        {
            try
            {
                IType required = parameter.GetIType(context);
                IType actual = argument.getExpression().check(context);
                // retrieve actual runtime type
                if (useInstance && actual is CategoryType)
                {
                    Object value = argument.getExpression().interpret((Context)context.getCallingContext());
                    if (value is IInstance)
                        actual = ((IInstance)value).getType();
                }
                if (actual.Equals(required))
                    return Specificity.EXACT;
                if (required.isAssignableFrom(context, actual))
                    return Specificity.INHERITED;
                actual = argument.resolve(context, this, useInstance).check(context);
                if (required.isAssignableFrom(context, actual))
                    return Specificity.RESOLVED;
            }
            catch (PromptoError)
            {
            }
            return Specificity.INCOMPATIBLE;
        }

        public virtual IType checkChild(Context context)
        {
            if (parameters != null)
                parameters.check(context);
            return returnType;
        }

        public virtual IValue interpret(Context context)
        {
            throw new InternalError("Should never get there!");
        }

        public virtual bool isAbstract()
        {
            return false;
        }

        public virtual bool isTemplate()
        {
            return false;
        }

        public virtual bool isEligibleAsMain()
        {
            return false;
        }
    }
}


