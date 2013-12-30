using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ParallelizeAttribute : HandlerAttribute, ICallHandler
	{
		#region Public constructors
		public ParallelizeAttribute(UInt32 times)
		{
			this.Times = times;
		}
		#endregion

		#region Public properties
		public UInt32 Times
		{
			get;
			set;
		}
		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			if (((input.MethodBase as MethodInfo).ReturnType != typeof(void)) || (this.Times == 1))
			{
				return (getNext()(input, getNext));
			}
			else
			{
				Parallel.For(0, this.Times, i => getNext()(input, getNext));

				return (input.CreateMethodReturn(null));
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
