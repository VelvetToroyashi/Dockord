using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using NSec.Cryptography;

namespace Docker.Discord.Services
{
	public static class HeaderHelpers
	{
		private const string
			TimestampHeaderName = "X-Signature-Timestamp",
			SignatureHeaderName = "X-Signature-Ed25519";
		
		public static bool HasRequisiteHeaders(IHeaderDictionary headers, out string timestamp, out string signature)
		{
			bool hasTimestamp = headers.TryGetValue(TimestampHeaderName, out var timestamps);
			timestamp = hasTimestamp ? timestamps.First() : null;

			bool hasSignature = headers.TryGetValue(SignatureHeaderName, out var signatures);
			signature = hasSignature ? signatures.First() : null;

			return hasSignature && hasSignature;
		}
		
		public static bool ValidateHeaderSignature(string timestamp, string body, string signature, string publicKey)
		{
			byte[] sigBtyes = Encoding.UTF8.GetBytes(signature);
			byte[] keyBytes = Encoding.UTF8.GetBytes(publicKey);
			byte[] bodyBytes = Encoding.UTF8.GetBytes(timestamp + body);

			return Chaos.NaCl.Ed25519.Verify(sigBtyes, bodyBytes, keyBytes);
		}
		
	}
}