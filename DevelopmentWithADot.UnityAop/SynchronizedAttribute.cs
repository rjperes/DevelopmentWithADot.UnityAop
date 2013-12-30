using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SynchronizedAttribute : HandlerAttribute, ICallHandler
	{
		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			Object syncRoot = input.Target ?? input.MethodBase.DeclaringType;

			PropertyInfo syncRootProperty = syncRoot.GetType().GetProperty("SyncRoot");

			if (syncRootProperty != null)
			{
				syncRoot = syncRootProperty.GetValue(syncRoot, null);
			}

			lock (syncRoot)
			{
				return(getNext()(input, getNext));
			}
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
