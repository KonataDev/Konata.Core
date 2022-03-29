using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace Konata.Core.Utils.Network;

internal static class Http
{
    /// <summary>
    /// Http get
    /// </summary>
    /// <param name="url"></param>
    /// <param name="header"></param>
    /// <param name="timeout"></param>
    /// <param name="limitLen"></param>
    /// <returns></returns>
    public static async Task<byte[]> Get(string url,
        Dictionary<string, string> header = null, int timeout = 8000, int limitLen = 1048576)
    {
        // Create request
        var request = new HttpClient();
        {
            request.Timeout = new TimeSpan(0, 0, timeout);
            request.MaxResponseContentBufferSize = limitLen;

            // Append request header
            if (header != null)
            {
                foreach (var (k, v) in header)
                    request.DefaultRequestHeaders.Add(k, v);
            }
        }

        // Receive the response data
        var response = await request.GetAsync(url);
        return await response.Content.ReadAsByteArrayAsync();
    }

    /// <summary>
    /// Http post
    /// </summary>
    /// <param name="url"></param>
    /// <param name="data"></param>
    /// <param name="header"></param>
    /// <param name="param"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static async Task<byte[]> Post(string url, byte[] data,
        Dictionary<string, string> header = null, Dictionary<string, string> param = null,
        int timeout = 8000)
    {
        // Append params to url
        if (param != null)
        {
            var args = param.Aggregate("", (current, i)
                => current + $"&{i.Key}={i.Value}");

            url += $"?{args[1..]}";
        }

        // Create request
        var request = new HttpClient();
        {
            request.Timeout = new TimeSpan(0, 0, timeout);

            // Append request header
            if (header != null)
            {
                foreach (var (k, v) in header)
                    request.DefaultRequestHeaders.Add(k, v);
            }
        }

        // Receive the response data
        var reqstream = await request.PostAsync(url, new ByteArrayContent(data));
        return await reqstream.Content.ReadAsByteArrayAsync();
    }
}
