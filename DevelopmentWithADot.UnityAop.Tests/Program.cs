using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop.Tests
{
	class Program
	{
		static void Main(String[] args)
		{
			IUnityContainer unity = new UnityContainer();
			//1 - DefaultInterception is defined on App.config
			unity.LoadConfiguration();
			//2 - register manually DefaultInterception
			//unity.AddNewExtension<DefaultInterception>();
			//3 - two steps
			//unity.AddNewExtension<Interception>();
			//unity.Configure<Interception>().SetDefaultInterceptorFor<IMyType>(new InterfaceInterceptor());
			unity.RegisterType<IMyType, MyType>(new ContainerControlledLifetimeManager());

			IMyType instance = unity.Resolve<IMyType>();
			Lazy<IMyType> lazyInstance = unity.Resolve<Lazy<IMyType>>();

			instance.DoSomething();
		}
	}
}
