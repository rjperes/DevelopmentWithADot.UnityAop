using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class ReadOnlyAttribute : HandlerAttribute, ICallHandler
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
			Boolean isSetter = input.MethodBase.Name.StartsWith("set_") == true;
			IMethodReturn result = null;

			if (isSetter != true)
			{
				result = getNext()(input, getNext);
			}
			else
			{
				result = input.CreateMethodReturn(null);
			}

			return (result);
		}

		#endregion
	}
}
