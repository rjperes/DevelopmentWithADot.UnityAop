using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class NotNullAttribute : HandlerAttribute, ICallHandler
	{
		#region Public override methods
		public override ICallHandler CreateHandler(IUnityContainer container)
		{
			return (this);
		}

		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			Boolean isSetter = input.MethodBase.Name.StartsWith("set_");
			IMethodReturn result = null;

			if (isSetter == true)
			{
				if (input.Arguments[0] == null)
				{
					result = input.CreateExceptionMethodReturn(new NullReferenceException("Value is null"));
				}
			}

			if (result == null)
			{
				result = getNext()(input, getNext);
			}

			return (result);
		}

		#endregion
	}
}
