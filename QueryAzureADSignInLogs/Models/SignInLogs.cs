//===============================================================================
// Microsoft FastTrack for Azure
// Query Azure AD Sign In Logs Samples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Newtonsoft.Json;
using System.Collections.Generic;

namespace QueryAzureADSignInLogs.Models
{
    public class SignInLogs
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        public List<SignInLog> value { get; set; }
    }
}
