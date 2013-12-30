using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class DelayAttribute : HandlerAttribute, ICallHandler
	{
		#region Public constructors
		public DelayAttribute(String delay)
		{
			
			this.Delay = (TimeSpan) TypeDescriptor.GetConverter(typeof(TimeSpan)).ConvertFrom(delay);
		}

		public DelayAttribute(UInt32 delayMilliseconds) : this(TimeSpan.FromMilliseconds(delayMilliseconds).ToString())
		{
		}
		#endregion

		#region Public properties
		public TimeSpan Delay
		{
			get;
			private set;
		}
		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			IMethodReturn result = getNext()(input, getNext);

			Thread.Sleep(this.Delay);

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
