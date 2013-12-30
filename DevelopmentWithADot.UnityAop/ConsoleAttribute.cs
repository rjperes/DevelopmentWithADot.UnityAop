using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DevelopmentWithADot.UnityAop
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ConsoleAttribute : HandlerAttribute, ICallHandler
	{
		#region Private fields
		private Boolean cursorSet;
		private Boolean backgroundColorSet;
		private Boolean foregroundColorSet;
		private Boolean windowWidthSet;
		private Boolean windowHeightSet;
		private Boolean windowTopSet;
		private Boolean windowLeftSet;

		private Cursor cursor;
		private ConsoleColor backgroundColor;
		private ConsoleColor foregroundColor;
		private Int32 windowWidth;
		private Int32 windowHeight;
		private Int32 windowTop;
		private Int32 windowLeft;
		#endregion

		#region Public constructor
		public ConsoleAttribute()
		{
			this.BackgroundColor = ConsoleColor.Black;
			this.ForegroundColor = ConsoleColor.White;
		}
		#endregion

		#region Public properties
		public Cursor Cursor
		{
			get
			{
				return (this.cursor);
			}

			set
			{
				this.cursor = value;
				this.cursorSet = true;
			}
		}

		public String OutFile
		{
			get;
			set;
		}

		public String ErrorFile
		{
			get;
			set;
		}

		public ConsoleColor BackgroundColor
		{
			get
			{
				return (this.backgroundColor);
			}
			set
			{
				this.backgroundColorSet = true;
				this.backgroundColor = value;
			}
		}

		public ConsoleColor ForegroundColor
		{
			get
			{
				return (this.foregroundColor);
			}
			set
			{
				this.foregroundColorSet = true;
				this.foregroundColor = value;
			}
		}

		public String Title
		{
			get;
			set;
		}

		public Int32 WindowWidth
		{
			get
			{
				return (this.windowWidth);
			}
			set
			{
				this.windowWidthSet = true;
				this.windowWidth = value;
			}
		}

		public Int32 WindowHeight
		{
			get
			{
				return (this.windowHeight);
			}
			set
			{
				this.windowHeightSet = true;
				this.windowHeight = value;
			}
		}

		public Int32 WindowTop
		{
			get
			{
				return (this.windowTop);
			}
			set
			{
				this.windowTopSet = true;
				this.windowTop = value;
			}
		}

		public Int32 WindowLeft
		{
			get
			{
				return (this.windowLeft);
			}
			set
			{
				this.windowLeftSet = true;
				this.windowLeft = value;
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
			ConsoleColor currentBackgroundColor = Console.BackgroundColor;
			ConsoleColor currentForegroundColor = Console.ForegroundColor;
			String currentTitle = Console.Title;
			Int32 currentWindowWidth = Console.WindowWidth;
			Int32 currentWindowHeight = Console.WindowHeight;
			Int32 currentWindowTop = Console.WindowTop;
			Int32 currentWindowLeft = Console.WindowLeft;
			TextWriter currentOut = Console.Out;
			TextWriter currentError = Console.Error;
			System.Windows.Forms.Cursor currentCursor = System.Windows.Forms.Cursor.Current;

			if (this.cursorSet == true)
			{
				System.Windows.Forms.Cursor.Current = TypeDescriptor.GetProperties(typeof(System.Windows.Forms.Cursors)) [ this.Cursor.ToString() ].GetValue(null) as System.Windows.Forms.Cursor;
			}

			if (String.IsNullOrWhiteSpace(this.Title) == false)
			{
				Console.Title = this.Title;
			}

			if (this.backgroundColorSet == true)
			{
				Console.BackgroundColor = this.BackgroundColor;
			}

			if (this.foregroundColorSet == true)
			{
				Console.ForegroundColor = this.ForegroundColor;
			}

			if (this.windowHeightSet == true)
			{
				Console.WindowHeight = this.WindowHeight;
			}

			if (this.windowWidthSet == true)
			{
				Console.WindowWidth = this.WindowWidth;
			}

			if (this.windowTopSet == true)
			{
				Console.WindowTop = this.WindowTop;
			}

			if (this.windowLeftSet == true)
			{
				Console.WindowLeft = this.WindowLeft;
			}

			if (String.IsNullOrWhiteSpace(this.OutFile) == false)
			{
				TextWriter outFile = null;
				
				try
				{
					outFile = new StreamWriter(this.OutFile);
					Console.SetOut(outFile);
				}
				catch
				{
				}
			}

			if (String.IsNullOrWhiteSpace(this.ErrorFile) == false)
			{
				TextWriter errorFile = null;

				try
				{
					errorFile = new StreamWriter(this.ErrorFile);
					Console.SetError(errorFile);
				}
				catch
				{
				}
			}

			IMethodReturn result = getNext()(input, getNext);

			System.Windows.Forms.Cursor.Current = currentCursor;
			Console.BackgroundColor = currentBackgroundColor;
			Console.ForegroundColor = currentForegroundColor;
			Console.Title = currentTitle;
			Console.WindowHeight = currentWindowHeight;
			Console.WindowWidth = currentWindowWidth;
			Console.WindowTop = currentWindowTop;
			Console.WindowLeft = currentWindowLeft;

			if (Console.Out != currentOut)
			{
				Console.Out.Dispose();
				Console.SetOut(currentOut);
			}

			if (Console.Error != currentError)
			{
				Console.Error.Dispose();
				Console.SetError(currentError);
			}

			return (result);
		}

		#endregion
	}
}
