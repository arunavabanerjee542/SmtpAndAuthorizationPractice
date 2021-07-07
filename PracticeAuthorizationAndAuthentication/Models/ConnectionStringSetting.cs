using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeAuthorizationAndAuthentication.Models
{
    public class ConnectionStringSetting
    {
        public const string SectionName = "ConnectionString";

        public string DefaultConnection { get; set; }
    }
}