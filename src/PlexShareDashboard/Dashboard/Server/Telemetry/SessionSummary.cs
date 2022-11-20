﻿/// <author>Rupesh Kumar</author>
/// <summary>
/// this is a class model to store the session summary about the total chat count of the session, total usercount and finally the score of the session 
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PlexShareDashboard.Dashboard.Server.Telemetry
{
    public class SessionSummary
    {
        public int chatCount;

        public int score;

        public int userCount;
    }
}
