using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ffxivList
{
    public class Auth0Settings
    {
        public string Domain
        {
            get { return "cottonglow.eu.auth0.com"; }
        }

        public string CallbackUrl
        {
            get { return "http://localhost:50258/signin-auth0"; }
        }

        public string ClientId
        {
            get { return "UyAKTB5gteDVhvEp4oq1t0zg1PrhsRlF"; }
        }

        public string ClientSecret
        {
            get { return "C_iMbfn9hVp7Ttc_agrPDatYFn_PJzm-Kqa2Fqabr8FaTbG0G7D3uHQc2jXzl2U8"; }
        }
    }
}