using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class NotifyPropertyChangeAttribute : HandlerAttribute, ICallHandler
	{
		#region Public override methods
		public override ICallHandler CreateHandler(IUnityContainer container)
		{
			return (this);
		}
		#endregion

		#region Private methods
		private void fireEvent(Object target, String propertyName, String eventName)
		{
			Type type = target.GetType();
			FieldInfo eventInfo = type.GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic);

			if (eventInfo != null)
			{
				Object eventMember = eventInfo.GetValue(target);

				if (eventMember != null)
				{
					if (eventName == "PropertyChanging")
					{
						eventMember.GetType().GetMethod("Invoke").Invoke(eventMember, new Object [] { this, new PropertyChangingEventArgs(propertyName) });
					}
					else if (eventName == "PropertyChanged")
					{
						eventMember.GetType().GetMethod("Invoke").Invoke(eventMember, new Object [] { this, new PropertyChangedEventArgs(propertyName) });
					}
				}
			}
		}
		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			Boolean isNotityPropertyChanging = typeof(INotifyPropertyChanging).IsAssignableFrom(input.Target.GetType());
			Boolean isNotityPropertyChanged = typeof(INotifyPropertyChanged).IsAssignableFrom(input.Target.GetType());
			Boolean isSetter = input.MethodBase.Name.StartsWith("set_") == true;
			String propertyName = (isSetter == true) ? input.MethodBase.Name.Substring(4) : null;

			if ((isSetter == true) && (isNotityPropertyChanging == true))
			{
				this.fireEvent(input.Target, propertyName, "PropertyChanging");
			}

			IMethodReturn result = getNext()(input, getNext);

			if ((isSetter == true) && (isNotityPropertyChanged == true))
			{
				this.fireEvent(input.Target, propertyName, "PropertyChanged");
			}

			return (result);
		}

		#endregion
	}
}
