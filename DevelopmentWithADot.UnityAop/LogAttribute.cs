using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class LogAttribute : HandlerAttribute, ICallHandler
	{
		#region Public properties
		public String Message
		{
			get;
			set;
		}

		public Boolean Before
		{
			get;
			set;
		}

		public Boolean After
		{
			get;
			set;
		}

		public Boolean Exception
		{
			get;
			set;
		}

		public String [] Categories
		{
			get;
			set;
		}

		public Int32 Priority
		{
			get;
			set;
		}

		public TraceEventType Severity
		{
			get;
			set;
		}
		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			if (this.Before == true)
			{
				String arguments = String.Join(",", input.Arguments.Cast<Object>().Select(a => a != null ? a.ToString() : "null").ToArray());
				String beforeMessage = String.Format(this.Message, input.MethodBase, arguments);

				Logger.Write(beforeMessage, this.Categories, this.Priority, 0, this.Severity);
			}

			IMethodReturn result = getNext()(input, getNext);

			if (result.Exception != null)
			{
				if (this.Exception == true)
				{
					String afterMessage = String.Format(this.Message, input.MethodBase, result.Exception);

					Logger.Write(afterMessage, this.Categories, this.Priority, 0, this.Severity);
				}
			}
			else
			{
				if (this.After == true)
				{
					String afterMessage = String.Format(this.Message, input.MethodBase, result.ReturnValue);

					Logger.Write(afterMessage, this.Categories, this.Priority, 0, this.Severity);
				}
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
