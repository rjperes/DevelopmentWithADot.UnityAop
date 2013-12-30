using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	public class DefaultInterception : Interception
	{
		#region Private static readonly fields
		private static readonly IInterceptor [] interceptors = typeof(IInterceptor)
			.Assembly
			.GetExportedTypes()
			.Where(type => (typeof(IInterceptor).IsAssignableFrom(type) == true) &&
				(type.IsAbstract == false) &&
				(type.IsInterface == false))
			.Select(type => Activator.CreateInstance(type) as IInterceptor)
			.ToArray();
		#endregion

		#region Protected override methods
		protected override void Initialize()
		{
			base.Initialize();

			this.Context.Registering += delegate(Object sender, RegisterEventArgs e)
			{				
				this.setTypeInterceptorFor(e.TypeFrom, e.TypeTo, e.Name, e.LifetimeManager);
			};

			this.Context.RegisteringInstance += delegate(Object sender, RegisterInstanceEventArgs e)
			{
				this.setInstanceInterceptorFor(e.RegisteredType, e.Name, e.Instance, e.LifetimeManager);
			};
		}
		#endregion

		#region Private methods
		private void setTypeInterceptorFor(Type typeFrom, Type typeTo, String name, LifetimeManager lifetimeManager)
		{
			foreach (ITypeInterceptor interceptor in interceptors.OfType<ITypeInterceptor>())
			{
				if ((interceptor.CanIntercept(typeFrom) == true) && (interceptor.GetInterceptableMethods(typeFrom, typeTo).Count() != 0))
				{
					this.Container.Configure<Interception>().SetInterceptorFor(typeFrom, name, interceptor);
					break;
				}
			}
		}

		private void setInstanceInterceptorFor(Type registeredType, String name, Object instance, LifetimeManager manager)
		{
			foreach (IInstanceInterceptor interceptor in interceptors.OfType<IInstanceInterceptor>())
			{
				if ((interceptor.CanIntercept(registeredType) == true) && (interceptor.GetInterceptableMethods(registeredType, instance.GetType()).Count() != 0))
				{
					this.Container.Configure<Interception>().SetInterceptorFor(registeredType, name, interceptor);
					break;
				}
			}
		}
		#endregion
	}
}