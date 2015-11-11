using System.Collections.Generic;

namespace Framework
{
	public abstract class IDatabase
	{
		public abstract string[] columns { get; }
	}
}