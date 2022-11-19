using Dashboard;
using PlexShare.Dashboard;
using PlexShareDashboard.Dashboard.Client.SessionManagement;
using PlexShareDashboard.Dashboard.Server.SessionManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareApp
{
    public class HomePageViewModel
    {
        IUXServerSessionManager serverSessionManager;
        IUXClientSessionManager clientSessionManager;

        public HomePageViewModel()
        {
            serverSessionManager = SessionManagerFactory.GetServerSessionManager();
            clientSessionManager = SessionManagerFactory.GetClientSessionManager();
        }

        public HomePageViewModel(IUXClientSessionManager clientSessionManager,IUXServerSessionManager serverSessionManager)
        {
            this.serverSessionManager = serverSessionManager;
            this.clientSessionManager = clientSessionManager;
        }   

        bool ValidateUserName(string name)
        {
            if(string.IsNullOrEmpty(name))
                return false;
            return true;
        }


        /// <summary>
        /// Checks if the IP address is valid or not
        /// </summary>
        /// <param name="ip">IP address in a string format</param>
        /// <returns>true if valid else false</returns>
        private bool ValidateIP(string ip)
        {
            if (ip.Length == 0)
                return false;
            string[] ipTokens = ip.Split('.');
            if (ipTokens.Length != 4)
                return false;
            foreach (string token in ipTokens)
            {
                if (token.Length == 0)
                    return false;
                int tokenValue = Int32.Parse(token);
                //System.Diagnostics.Debug.WriteLine(token_value.ToString
                if (tokenValue < 0 || tokenValue > 255)
                    return false;
            }
            var byteValues = ip.Split('.');
            // IPV4 contains 4 bytes separated by .
            if (byteValues.Length != 4) return false;
            // We have 4 bytes in a address
            //byte tempForParsing;

            // for each part(elements of byteValues list), we check whether the string 
            // can be successfully converted into a byte or not.
            return byteValues.All(r => byte.TryParse(r, out var tempForParsing));
        }


        bool ValidateIpAddress(string ip)
        {
            // Server Session IP Address
            if (ip == "-1")
                return true;
            if (string.IsNullOrEmpty(ip) || !ValidateIP(ip))
            {
                return false;
            }
            return true;
        }

        bool ValidatePort(string port)
        {
            if (string.IsNullOrEmpty(port) || port.Length>18  || Int64.Parse(port)<=1024 || Int64.Parse(port)> 65535)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Uses the dashboard's function to assess if the credentials entered is okay
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>List<string> { ip, port, isValidUserName, isValidIpAddress, isValidPort, isServer, verified}</string></returns>
        public List<string> VerifyCredentials(string name, string ip, string port, string email, string url)
        {
            Trace.WriteLine("[UX] Enetering HomeScreen Now");
            bool isValidUserName = ValidateUserName(name), isServer = ip=="-1"?true:false;
            bool isValidIpAddress = true, isValidPort = true;
            if (isServer==false)
            {
                isValidIpAddress = ValidateIpAddress(ip);
                isValidPort = ValidatePort(port);
            }
            bool isVerified = false;
            if(isValidUserName && isValidIpAddress && isValidPort)
            {
                if (isServer)
                {
                    Trace.WriteLine("[UX] Instaniating a server");
                    MeetingCredentials meetingCredentials = serverSessionManager.GetPortsAndIPAddress();
                    isVerified = clientSessionManager.AddClient(meetingCredentials.ipAddress, meetingCredentials.port, name, email, url);
                    ip = meetingCredentials.ipAddress;
                    port = meetingCredentials.port.ToString();
                }
                else
                {
                    Trace.WriteLine("[UX] Instaniating a client");
                    isVerified  = clientSessionManager.AddClient(ip, int.Parse(port), name, email, url);
                }
            }
            List<string> result = new List<string>();
            result.Add(ip);
            result.Add(port);
            result.Add(isValidUserName.ToString());
            result.Add(isValidIpAddress.ToString());  
            result.Add(isValidPort.ToString());
            result.Add(isServer.ToString());
            result.Add(isVerified.ToString());
            Trace.WriteLine("[UX] The client verification returned : " + isVerified);
            return result;
        }
    }
}
