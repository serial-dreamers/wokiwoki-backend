using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
	public static void AddApplicationServices(this IHostApplicationBuilder builder)
	{
		builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

	}

}

