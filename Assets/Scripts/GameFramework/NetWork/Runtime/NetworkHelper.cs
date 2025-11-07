using System.Net;

namespace Framework.Net
{
	internal static class NetworkHelper
	{
        //-----------------------------------------------------
        public static IPEndPoint ToIPEndPoint(string host, int port)
		{
			return new IPEndPoint(IPAddress.Parse(host), port);
		}
        //-----------------------------------------------------
        public static IPEndPoint ToIPEndPoint(string address)
		{
			int index = address.LastIndexOf(':');
			string host = address.Substring(0, index);
			string p = address.Substring(index + 1);
			int port = int.Parse(p);
			return ToIPEndPoint(host, port);
		}
        //-----------------------------------------------------
        // 优先获取IPV4的地址
        public static IPAddress GetHostAddress(string hostName, System.Net.Sockets.AddressFamily addressFamily = System.Net.Sockets.AddressFamily.InterNetwork)
        {
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
            IPAddress returnIpAddress = null;
            foreach (IPAddress ipAddress in ipAddresses)
            {
                returnIpAddress = ipAddress;
                if (ipAddress.AddressFamily == addressFamily)
                {
                    return ipAddress;
                }
            }
            return returnIpAddress;
        }
    }
}
