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
using System;
using System.Collections.Generic;

namespace QueryAzureADSignInLogs.Models
{
    public class AuthenticationProcessingDetail
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class DeviceDetail
    {
        public string deviceId { get; set; }
        public string displayName { get; set; }
        public string operatingSystem { get; set; }
        public string browser { get; set; }
        public bool isCompliant { get; set; }
        public bool isManaged { get; set; }
        public string trustType { get; set; }
    }

    public class GeoCoordinates
    {
        public object altitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class Location
    {
        public string city { get; set; }
        public string state { get; set; }
        public string countryOrRegion { get; set; }
        public GeoCoordinates geoCoordinates { get; set; }
    }

    public class MfaDetail
    {
        public object authMethod { get; set; }
        public object authDetail { get; set; }
    }

    public class PrivateLinkDetails
    {
        public string policyId { get; set; }
        public string policyName { get; set; }
        public string resourceId { get; set; }
        public string policyTenantId { get; set; }
    }

    public class Status
    {
        public int errorCode { get; set; }
        public string failureReason { get; set; }
        public string additionalDetails { get; set; }
    }

    public class SignInLog
    {
        public string id { get; set; }
        public DateTime createdDateTime { get; set; }
        public string userDisplayName { get; set; }
        public string userPrincipalName { get; set; }
        public string userId { get; set; }
        public string appId { get; set; }
        public string appDisplayName { get; set; }
        public string ipAddress { get; set; }
        public object ipAddressFromResourceProvider { get; set; }
        public string clientAppUsed { get; set; }
        public string userAgent { get; set; }
        public string correlationId { get; set; }
        public string conditionalAccessStatus { get; set; }
        public string originalRequestId { get; set; }
        public bool isInteractive { get; set; }
        public string tokenIssuerName { get; set; }
        public string tokenIssuerType { get; set; }
        public string clientCredentialType { get; set; }
        public int processingTimeInMilliseconds { get; set; }
        public string riskDetail { get; set; }
        public string riskLevelAggregated { get; set; }
        public string riskLevelDuringSignIn { get; set; }
        public string riskState { get; set; }
        public List<object> riskEventTypes_v2 { get; set; }
        public string resourceDisplayName { get; set; }
        public string resourceId { get; set; }
        public string resourceTenantId { get; set; }
        public string homeTenantId { get; set; }
        public string homeTenantName { get; set; }
        public List<object> authenticationMethodsUsed { get; set; }
        public string authenticationRequirement { get; set; }
        public string signInIdentifier { get; set; }
        public object signInIdentifierType { get; set; }
        public object servicePrincipalName { get; set; }
        public List<string> signInEventTypes { get; set; }
        public string servicePrincipalId { get; set; }
        public object federatedCredentialId { get; set; }
        public string userType { get; set; }
        public bool flaggedForReview { get; set; }
        public bool isTenantRestricted { get; set; }
        public int autonomousSystemNumber { get; set; }
        public string crossTenantAccessType { get; set; }
        public object servicePrincipalCredentialKeyId { get; set; }
        public string servicePrincipalCredentialThumbprint { get; set; }
        public string uniqueTokenIdentifier { get; set; }
        public string incomingTokenType { get; set; }
        public string authenticationProtocol { get; set; }
        public string resourceServicePrincipalId { get; set; }
        public object authenticationAppDeviceDetails { get; set; }
        public Status status { get; set; }
        public DeviceDetail deviceDetail { get; set; }
        public Location location { get; set; }
        public MfaDetail mfaDetail { get; set; }
        public List<object> appliedConditionalAccessPolicies { get; set; }
        public List<object> authenticationContextClassReferences { get; set; }
        public List<AuthenticationProcessingDetail> authenticationProcessingDetails { get; set; }
        public List<object> networkLocationDetails { get; set; }
        public List<object> authenticationDetails { get; set; }
        public List<object> authenticationRequirementPolicies { get; set; }
        public List<object> sessionLifetimePolicies { get; set; }
        public PrivateLinkDetails privateLinkDetails { get; set; }
        public List<object> appliedEventListeners { get; set; }
        public List<object> authenticationAppPolicyEvaluationDetails { get; set; }
    }
}
