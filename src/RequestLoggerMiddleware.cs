using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace SimonPrinz.RequestLogger
{
	public class RequestLoggerMiddleware : IMiddleware
	{
		private delegate string? GetValue(HttpContext pContext);

		private static readonly Dictionary<string, GetValue> ValueDictionary = new()
		{
			{ "RemoteAddress", pContext => pContext.Connection.RemoteIpAddress?.ToString() ?? "-" },
			{ "RemoteUser", pContext => pContext.User.Identity?.Name ?? "-" },
			{ "DateTime", _ => DateTime.Now.ToString("O") },
			{ "Method", pContext => pContext.Request.Method },
			{ "Url", pContext => pContext.Request.Path },
			{ "HttpVersion", pContext => pContext.Request.Protocol },
			{ "Status", pContext => pContext.Response.StatusCode.ToString("000") },
			{ "ContentLength", pContext => pContext.Response.ContentLength.ToString() ?? "-" },
			{ "Referrer", pContext => pContext.Request.GetTypedHeaders().Referer?.ToString() ?? "-" },
			{ "UserAgent", pContext => pContext.Request.Headers.TryGetValue("User-Agent", out StringValues lValues) && lValues.Any() ? string.Join(", ", lValues) : "-" },
			{ "ResponseTime", pContext => pContext.Features.Get<Stopwatch>().ElapsedMilliseconds.ToString("D") },
			{ "QueryString", pContext => pContext.Request.QueryString.HasValue ? pContext.Request.QueryString.ToString() : "" },
		};

		private readonly ILogger<RequestLoggerMiddleware> _Logger;
		private readonly string _LogFormat;

		public RequestLoggerMiddleware(ILogger<RequestLoggerMiddleware> pLogger, IConfiguration pConfiguration, IHostEnvironment pEnvironment)
		{
			_Logger = pLogger;
			_LogFormat = pConfiguration.GetValue<string>("RequestLogger")
			             ?? (pEnvironment.IsProduction()
				             ? "{RemoteAddress} - {RemoteUser} {DateTime} \"{Method} {Url} {HttpVersion}\" {Status} {ContentLength} \"{Referrer}\" \"{UserAgent}\""
				             : "{Method} {Url}{QueryString} {Status} {ResponseTime} ms - {ContentLength}");
		}

		public async Task InvokeAsync(HttpContext pContext, RequestDelegate pNext)
		{
			pContext.Features.Set(Stopwatch.StartNew());
			await pNext(pContext);
			pContext.Features.Get<Stopwatch>()!.Stop();

			Match lMatch = Regex.Match(_LogFormat, @"\{(\w+?)\}");
			string lLogMessage = _LogFormat;
			while (lMatch.Success)
			{
				lLogMessage = ValueDictionary.TryGetValue(lMatch.Groups[1].Value, out GetValue? lGetValue)
					? lLogMessage.Replace(lMatch.Value, lGetValue(pContext) ?? "-")
					: lLogMessage.Replace(lMatch.Value, $"?{lMatch.Groups[1].Value}?");

				lMatch = lMatch.NextMatch();
			}

			_Logger.LogInformation(lLogMessage);
		}
	}
}