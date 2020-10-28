// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Identity;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that team organization name matches Identity claim
    /// </summary>
    public class TeamOrganizationClaimIsValid :
        IValidationRule<TeamUpdate>
    {
        public TeamOrganizationClaimIsValid(IStackIdentityResolver identityResolver, OrganizationOptions organizationOptions)
        {
            IdentityResolver = identityResolver;
            OrganizationOptions = organizationOptions;
        }
        OrganizationOptions OrganizationOptions { get; }

        IStackIdentityResolver IdentityResolver { get; }

        public async Task Validate(TeamUpdate model)
        {
            var organizationNameClaimKey = OrganizationOptions.ClaimKey;
            if (!string.IsNullOrWhiteSpace(organizationNameClaimKey))
            {
                var identity = await IdentityResolver.GetIdentityAsync() as UserIdentity;
                var organizationNameClaim = identity.ClaimsPrincipal.FindFirst(organizationNameClaimKey)?.Value;

                if (!string.IsNullOrWhiteSpace(organizationNameClaim) && model.OrganizationName != organizationNameClaim)
                    throw new InvalidOperationException("Identity organization mismatch.");
            }
        }
    }
}

