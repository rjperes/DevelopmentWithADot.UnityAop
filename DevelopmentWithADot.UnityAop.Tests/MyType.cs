using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentWithADot.UnityAop.Tests
{
	public class MyObserver : IObserver<IMyType>
	{
		public void OnCompleted()
		{
		}

		public void OnError(Exception error)
		{
		}

		public void OnNext(IMyType value)
		{
		}
	}

	public class MyType : IMyType
	{
		public Object DoSomething()
		{
			Console.WriteLine("MyType.DoSomething()");

			throw (new Exception("AQUI"));

			return (null);
		}

		public Object Property
		{
			get;
			set;
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
