using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.GeocachingCom
{
	public static class ROT13Coder
	{
		public static string Decode(string encoded)
		{
			var decoded = new StringBuilder(encoded.Length);
			const int shift = ('z' - 'a' + 1) / 2;

			foreach (var c in encoded)
			{
				if (c >= 'a' && c <= 'z')
				{
					decoded.Append((char)(c + (c <= 'm' ? shift : -shift)));
				}
				else if (c >= 'A' && c <= 'Z')
				{
					decoded.Append((char)(c + (c <= 'M' ? shift : -shift)));
				}
				else
				{
					decoded.Append(c);
				}
			}

			return decoded.ToString();
		}
	}
}
