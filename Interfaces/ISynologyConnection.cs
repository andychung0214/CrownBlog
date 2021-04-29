using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CrownBlog.Interfaces
{
    public interface ISynologyConnection
    {
		/// <summary>
		/// 
		/// </summary>
		ILogger Logger { get; }

		/// <summary>
		/// 
		/// </summary>
		ISynologyConnectionSettings Settings { get; }

		/// <summary>
		/// 
		/// </summary>
		HttpClient Client { get; }

		/// <summary>
		/// 
		/// </summary>
		IServiceProvider ServiceProvider { get; }
	}
}
