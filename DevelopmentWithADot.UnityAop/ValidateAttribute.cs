using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ValidateAttribute : HandlerAttribute, ICallHandler
	{
		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			Dictionary<Type, Validator> validators = new Dictionary<Type, Validator>();
			Int32 i = 0;
			ValidationResults results = new ValidationResults();
			IMethodReturn result = null;

			foreach (Type type in input.MethodBase.GetParameters().Select(p => p.ParameterType))
			{
				if (validators.ContainsKey(type) == false)
				{
					validators[type] = ValidationFactory.CreateValidator(type, this.Ruleset);
				}

				Validator validator = validators[type];
				validator.Validate(input.Arguments[i], results);

				++i;
			}

			if (results.IsValid == false)
			{
				result = input.CreateExceptionMethodReturn(new Exception(String.Join(Environment.NewLine, results.Select(r => r.Message).ToArray())));
			}
			else
			{
				result = getNext()(input, getNext);
			}

			return (result);
		}

		#endregion

		#region Public properties
		public String Ruleset
		{
			get;
			set;
		}
		#endregion

		#region Public override methods
		public override ICallHandler CreateHandler(IUnityContainer container)
		{
			return (this);
		}
		#endregion
	}
}
