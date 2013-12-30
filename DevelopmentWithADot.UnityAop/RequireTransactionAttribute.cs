using System;
using System.Transactions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class RequireTransactionAttribute : HandlerAttribute, ICallHandler
	{
		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			IMethodReturn result = null;

			if (Transaction.Current == null)
			{
				result = input.CreateExceptionMethodReturn(new Exception("Ambient transaction required"));
			}
			else
			{
				result = getNext()(input, getNext);
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
