using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class RetryAttribute : HandlerAttribute, ICallHandler
	{
		#region Public constructors
		public RetryAttribute(UInt32 retryCount) : this(retryCount, TimeSpan.FromSeconds(5).ToString())
		{
		}
		public RetryAttribute(UInt32 retryCount, String retryDelay)
		{
			this.RetryCount = retryCount;
			this.RetryDelay = (TimeSpan) TypeDescriptor.GetConverter(typeof(TimeSpan)).ConvertFrom(retryDelay);
		}
		#endregion

		#region Public properties
		public UInt32 RetryCount
		{
			get;
			private set;
		}

		public TimeSpan RetryDelay
		{
			get;
			private set;
		}
		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			IMethodReturn result = null;

			for (Int32 i = 0; i < this.RetryCount; ++i)
			{
				result = getNext()(input, getNext);

				if (result.Exception == null)
				{
					break;
				}

				Thread.Sleep(this.RetryDelay);
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
