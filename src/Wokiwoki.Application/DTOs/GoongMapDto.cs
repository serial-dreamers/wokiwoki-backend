namespace Wokiwoki.Application.DTOs
{
	public class GoongMapDto
	{
		public List<Result>? Results { get; set; }
		public class Result { public Geometry? Geometry { get; set; } }
		public class Geometry { public Location? Location { get; set; } }
		public class Location { public double Lat { get; set; } public double Lng { get; set; } }
	}
}
