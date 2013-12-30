using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class NoOpAttribute : HandlerAttribute, ICallHandler
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
			IMethodReturn result = null;

			if ((input.MethodBase as MethodInfo).ReturnType == typeof(void))
			{
				result = input.CreateMethodReturn(null);
			}
			else
			{
				result = getNext()(input, getNext);
			}

			return (result);
		}

		#endregion
	}
}
