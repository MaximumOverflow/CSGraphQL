using System.Linq;
using CaseExtensions;

namespace CSGraphQL.Extensions
{
	public static class StringExtensions
	{
		public static bool IsAllUpperCase(this string str) =>
			str.Where(char.IsLetter).All(char.IsUpper);
		
		public static bool IsAllLowerCase(this string str) =>
			str.Where(char.IsLetter).All(char.IsLower);

		public static string ToUpperSnakeCase(this string str) => str.ToSnakeCase().ToUpper();
	}
}