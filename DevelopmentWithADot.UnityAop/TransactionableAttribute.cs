using System;
using System.Reflection;
using System.Transactions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class TransactionableAttribute : HandlerAttribute, ICallHandler
	{
		#region Private fields
		private volatile Boolean isSet = false;
		private Object originalValue = null;
		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			String propertyName = input.MethodBase.Name.Substring(4);
			Boolean isSetter = input.MethodBase.Name.StartsWith("set_") == true;

			if (isSetter == true)
			{
				if (Transaction.Current != null)
				{
					if (this.isSet == false)
					{
						TransactionCompletedEventHandler handler = delegate(Object sender, TransactionEventArgs e)
						 {
							 if (e.Transaction.TransactionInformation.Status == TransactionStatus.Aborted)
							 {
								 if (this.isSet == true)
								 {
									 PropertyInfo propertySetter = input.Target.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
									 propertySetter.SetValue(input.Target, this.originalValue, null);
									 this.originalValue = null;
									 this.isSet = false;
								 }
							 }
							 else if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
							 {
								 this.originalValue = null;
								 this.isSet = false;
							 }
						 };

						Transaction.Current.TransactionCompleted += handler;

						PropertyInfo propertyGetter = input.Target.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
						this.originalValue = propertyGetter.GetValue(input.Target, null);
					}

					this.isSet = true;
				}
			}

			IMethodReturn result = getNext()(input, getNext);

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
