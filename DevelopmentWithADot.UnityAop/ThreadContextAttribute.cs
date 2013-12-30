using System;
using System.Collections;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ThreadContextAttribute : HandlerAttribute, ICallHandler
	{
		#region Private fields
		private Boolean rolesSet;
		private Boolean identityNameSet;
		private Boolean cultureSet;
		private Boolean uiCultureSet;
		private Boolean environmentSet;
		private Boolean threadPrioritySet;
		private Boolean apartmentStateSet;
		private Boolean principalPolicySet;

		private String [] roles;
		private String identityName;
		private String culture;
		private String uiCulture;
		private String environment;
		private ThreadPriority threadPriority;
		private ApartmentState apartmentState;
		private PrincipalPolicy principalPolicy;
		#endregion
		
		#region Public properties
		public String Culture
		{
			get
			{
				return (this.culture);
			}
			set
			{
				this.cultureSet = true;
				this.culture = value;
			}
		}

		public String UICulture
		{
			get
			{
				return (this.uiCulture);
			}
			set
			{
				this.uiCultureSet = true;
				this.uiCulture = value;
			}
		}

		public String IdentityName
		{
			get
			{
				return (this.identityName);
			}
			set
			{
				this.identityNameSet = true;
				this.identityName = value;
			}
		}

		public String [] Roles
		{
			get
			{
				return (this.roles);
			}
			set
			{
				this.rolesSet = true;
				this.roles = value;
			}
		}

		public PrincipalPolicy PrincipalPolicy
		{
			get
			{
				return (this.principalPolicy);
			}
			set
			{
				this.principalPolicySet = true;
				this.principalPolicy = value;
			}
		}

		public ThreadPriority ThreadPriority
		{
			get
			{
				return (this.threadPriority);
			}
			set
			{
				this.threadPrioritySet = true;
				this.threadPriority = value;
			}
		}

		public ApartmentState ApartmentState
		{
			get
			{
				return (this.apartmentState);
			}
			set
			{
				this.apartmentStateSet = true;
				this.apartmentState = value;
			}
		}

		public String Environment
		{
			get
			{
				return (this.environment);
			}
			set
			{
				this.environmentSet = true;
				this.environment = value;
			}
		}
		#endregion

		#region Public override methods
		public override ICallHandler CreateHandler(IUnityContainer container)
		{
			return (this);
		}
		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			IPrincipal currentPrincipal = Thread.CurrentPrincipal;
			ApartmentState currentApartmentState = Thread.CurrentThread.GetApartmentState();
			ThreadPriority currentThreadPriority = Thread.CurrentThread.Priority;
			PrincipalPolicy currentPrincipalPolicy = (Thread.CurrentPrincipal == null) ? PrincipalPolicy.NoPrincipal : (Thread.CurrentPrincipal is WindowsPrincipal) ? PrincipalPolicy.WindowsPrincipal : PrincipalPolicy.UnauthenticatedPrincipal;
			IDictionary currentEnvironment = System.Environment.GetEnvironmentVariables();

			if ((this.environmentSet == true) && (String.IsNullOrWhiteSpace(this.Environment) == false))
			{
				String[] parts = this.Environment.Split(',');

				foreach (String part in parts)
				{
					String[] p = part.Split('=');

					if (p.Length == 2)
					{
						System.Environment.SetEnvironmentVariable(p[0], p[1]);
					}
				}
			}

			if ((this.identityNameSet == true) && (String.IsNullOrWhiteSpace(this.identityName) == false))
			{
				String[] roles = this.Roles ?? new String[0];
				GenericPrincipal principal = new GenericPrincipal(new GenericIdentity(this.identityName), roles);
			}

			if ((this.cultureSet == true) && (String.IsNullOrWhiteSpace(this.culture) == false))
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(this.culture);
			}

			if ((this.uiCultureSet == true) && (String.IsNullOrWhiteSpace(this.uiCulture) == false))
			{
				Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(this.uiCulture);
			}

			if (this.apartmentStateSet == true)
			{
				Thread.CurrentThread.SetApartmentState(this.ApartmentState);
			}

			if (this.threadPrioritySet == true)
			{
				Thread.CurrentThread.Priority = this.ThreadPriority;
			}

			if (this.principalPolicySet == true)
			{
				AppDomain.CurrentDomain.SetPrincipalPolicy(this.PrincipalPolicy);
			}

			IMethodReturn result = getNext()(input, getNext);

			if ((this.environmentSet == true) && (String.IsNullOrWhiteSpace(this.Environment) == false))
			{
				foreach (String key in currentEnvironment.Keys)
				{
					System.Environment.SetEnvironmentVariable(key, currentEnvironment[key] as String);
				}
			}

			Thread.CurrentThread.CurrentCulture = currentCulture;
			Thread.CurrentThread.CurrentUICulture = currentUICulture;
			Thread.CurrentPrincipal = currentPrincipal;
			Thread.CurrentThread.Priority = currentThreadPriority;
			Thread.CurrentThread.SetApartmentState(currentApartmentState);
			
			AppDomain.CurrentDomain.SetPrincipalPolicy(currentPrincipalPolicy);

			return (result);
		}

		#endregion
	}
}
