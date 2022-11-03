using System.Text;

namespace Ripple.Exceptions
{
	internal class CodeValidationException : Exception
	{
		public CodeValidationException(List<string> errors)
			: base(CreateErrorMessage(errors)) { }

		private static string CreateErrorMessage(List<string> errors)
		{
			StringBuilder sb = new();
			sb.AppendLine();
			sb.AppendLine("".PadLeft(80, '-'));
			sb.AppendLine("- Ripple code validation errors");
			sb.AppendLine("".PadLeft(80, '-'));
			foreach (string error in errors)
			{
				sb.AppendLine($"  {error}");
			}
			sb.AppendLine("".PadLeft(80, '-'));

			return sb.ToString();
		}
	}
}
