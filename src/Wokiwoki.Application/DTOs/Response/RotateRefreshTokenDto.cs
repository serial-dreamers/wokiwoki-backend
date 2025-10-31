using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wokiwoki.Application.DTOs.Response
{
	public class RotateRefreshTokenDto
	{
		public string AccessToken { get; set; } = default!;
		public string RefreshToken { get; set; } = default!;
	}
}
