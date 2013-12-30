using System;
using System.ComponentModel;
using System.Transactions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class TransactionAttribute : HandlerAttribute, ICallHandler
	{
		#region Public constructors
		public TransactionAttribute(): this(TransactionScopeOption.Required)
		{
		}

		public TransactionAttribute(TransactionScopeOption scope): this(scope, IsolationLevel.Unspecified)
		{
		}

		public TransactionAttribute(TransactionScopeOption scope, IsolationLevel isolationLevel):this(scope, isolationLevel, TimeSpan.FromSeconds(30).ToString())
		{
		}

		public TransactionAttribute(TransactionScopeOption scope, IsolationLevel isolationLevel, [TypeConverter(typeof(TimeSpan))] String timeout)
		{
			this.Scope = scope;
			this.IsolationLevel = isolationLevel;
			this.Timeout = (TimeSpan) TypeDescriptor.GetConverter(typeof(TimeSpan)).ConvertFrom(timeout);
		}

		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			using (TransactionScope tx = new TransactionScope(this.Scope, new TransactionOptions { IsolationLevel = this.IsolationLevel, Timeout = this.Timeout }))
			{
				IMethodReturn result = getNext()(input, getNext);

				if ((result.Exception != null) && (this.AutoComplete == true))
				{
					tx.Complete();
				}

				return (result);
			}
		}

		#endregion

		#region Public properties
		public Boolean AutoComplete
		{
			get;
			set;
		}

		public TransactionScopeOption Scope
		{
			get;
			private set;
		}

		public IsolationLevel IsolationLevel
		{
			get;
			private set;
		}

		public TimeSpan Timeout
		{
			get;
			private set;
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
