using System;
using System.Collections.Generic;

namespace SafeVault.Helpers
{
	public static class ValidationHelper
	{
		public static bool IsValidInput(string input, string allowedSpecialCharacters)
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}

			var allowedChars = new HashSet<char>(allowedSpecialCharacters ?? string.Empty);

			foreach (char c in input)
			{
				if (!char.IsLetterOrDigit(c) && !allowedChars.Contains(c))
				{
					return false;
				}
			}

			return true;
		}
	}
}
