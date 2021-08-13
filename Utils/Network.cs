using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Konata.Core.Utils
{
    public class Network
    {
        /// <summary>
        /// Pinging IP
        /// </summary>
        /// <param name="hostIp"><b>[In] </b>Host IP adress</param>
        /// <param name="timeout"><b>[Opt] </b>Pinging timeout default 1000 ms</param>
        /// <returns></returns>
        public static long PingTest(string hostIp, int timeout = 1000)
        {
            using (var ping = new Ping())
            {
                var reply = ping.Send(hostIp, timeout);
                {
                    if (reply.Status == IPStatus.Success)
                    {
                        return reply.RoundtripTime;
                    }
                }
            }

            return long.MaxValue;
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<byte[]> Download(string url, int timeout = 8000)
        {
            // Create request
            var request = WebRequest.CreateHttp(url);
            {
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
            }

            var response = await request.GetResponseAsync();
            {
                using (var stream = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(stream);
                    return stream.ToArray();
                }
            }
        }

        public static string UintToIPBE(uint ip)
            => $"{(ip >> 0) & 0xFF}." + $"{(ip >> 8) & 0xFF}." +
                $"{(ip >> 16) & 0xFF}." + $"{(ip >> 24) & 0xFF}";
    }
}
