using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentWithADot.UnityAop.Tests
{
	public interface IMyType : INotifyPropertyChanged
	{
		[Cache("00:00:10")]
		Object DoSomething();

		Object Property
		{
			get;
			set;
		}
	}
}
