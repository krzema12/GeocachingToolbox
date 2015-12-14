using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeocachingToolbox.Opencaching
{
	public class AccessTokenStore : IAccessTokenStore
	{
		public bool Populated { get; set; }
		public string Token { get; set; }
		public string TokenSecret { get; set; }
		private string TokensFilePath;

		public AccessTokenStore()
		{
			TokensFilePath = Path.Combine(Path.Combine(Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData),
				"Geocaching Toolbox"),
				"Tokens.txt");

			if (File.Exists(TokensFilePath))
			{
				var lines = File.ReadAllLines(TokensFilePath);
				Token = lines[0];
				TokenSecret = lines[1];
				Populated = true;
			}
			else
			{
				Populated = false;
			}
		}

		public void SetValues(string token, string tokenSecret)
		{
			Token = token;
			TokenSecret = tokenSecret;

			var dirToBeCreated = Path.GetDirectoryName(TokensFilePath);
			Directory.CreateDirectory(dirToBeCreated);
			File.WriteAllLines(TokensFilePath, new string[] { Token, TokenSecret });
			Populated = true;
		}
	}
}
