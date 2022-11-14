namespace ISpan.EStore.infra.Extensions
{
	public static class StringExtensions
	{
		public static int ToInt(this string source, int defaultValue)
		{
			bool isInt = int.TryParse(source, out int number);

			return isInt ? number : defaultValue;
		}

	}
}
