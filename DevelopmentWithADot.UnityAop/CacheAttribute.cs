using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class CacheAttribute : HandlerAttribute, ICallHandler
	{
		#region Private fields
		private static readonly Guid KeyGuid = new Guid("ECFD1B0F-0CBA-4AA1-89A0-179B636381CA");
		#endregion

		#region Public properties
		public TimeSpan SlidingExpirationTime
		{
			get;
			private set;
		}
		#endregion

		#region Public Constructors
		public CacheAttribute(Int32 hours, Int32 minutes, Int32 seconds) : this(new TimeSpan(hours, minutes, seconds).ToString())
		{
		}

		public CacheAttribute(String slidingExpirationTime)
		{
			this.SlidingExpirationTime = (TimeSpan)TypeDescriptor.GetConverter(typeof(TimeSpan)).ConvertFrom(slidingExpirationTime);
		}

		#endregion

		#region Public override methods
		public override ICallHandler CreateHandler(IUnityContainer ignored)
		{
			return (this);
		}
		#endregion

		#region ICallHandler Members

		public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
		{
			if (this.targetMethodReturnsVoid(input) == true)
			{
				return (getNext()(input, getNext));
			}

			Object [] inputs = new Object [ input.Inputs.Count ];
			
			for (Int32 i = 0; i < inputs.Length; ++i)
			{
				inputs [ i ] = input.Inputs [ i ];
			}

			String cacheKey = this.createCacheKey(input.MethodBase, inputs);
			ObjectCache cache = MemoryCache.Default;
			Object [] cachedResult = (Object []) cache.Get(cacheKey);

			if (cachedResult == null)
			{
				IMethodReturn realReturn = getNext()(input, getNext);
				
				if (realReturn.Exception == null)
				{
					this.addToCache(cacheKey, realReturn.ReturnValue);
				}

				return (realReturn);
			}

			IMethodReturn cachedReturn = input.CreateMethodReturn(cachedResult [ 0 ], input.Arguments);
			
			return (cachedReturn);
		}

		#endregion

		#region Private methods
		private Boolean targetMethodReturnsVoid(IMethodInvocation input)
		{
			MethodInfo targetMethod = input.MethodBase as MethodInfo;
			return ((targetMethod != null) && (targetMethod.ReturnType == typeof(void)));
		}

		private void addToCache(String key, Object value)
		{
			ObjectCache cache = MemoryCache.Default;
			Object [] cacheValue = new Object [] { value };
			cache.Add(key, cacheValue, DateTime.Now + this.SlidingExpirationTime);
		}

		private String createCacheKey(MethodBase method, params Object [] inputs)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}:", Process.GetCurrentProcess().Id);
			sb.AppendFormat("{0}:", KeyGuid);
				
			if (method.DeclaringType != null)
			{
				sb.Append(method.DeclaringType.FullName);
			}
				
			sb.Append(':');
			sb.Append(method);

			if (inputs != null)
			{
				foreach (Object input in inputs)
				{
					sb.Append(':');
						
					if (input != null)
					{
						sb.Append(input.GetHashCode().ToString());
					}
				}
			}

			return (sb.ToString());
		}

		#endregion
	}
}
