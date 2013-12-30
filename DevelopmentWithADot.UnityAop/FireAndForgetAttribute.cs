using System;
using System.Reflection;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class FireAndForgetAttribute : HandlerAttribute, ICallHandler
	{
		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			IMethodReturn result = null;

			if ((input.MethodBase as MethodInfo).ReturnType != typeof(void))
			{
				result = getNext()(input, getNext);
			}
			else
			{
				ThreadPool.QueueUserWorkItem(x =>
				{
					getNext()(input, getNext);
				});

				result = input.CreateMethodReturn(null);
			}

			return (result);
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
