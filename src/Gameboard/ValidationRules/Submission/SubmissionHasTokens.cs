// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that submission has tokens
    /// </summary>
    public class SubmissionHasTokens : IValidationRule<SubmissionCreate>
    {
        public SubmissionHasTokens()
            : base() { }

        public async Task Validate(SubmissionCreate model)
        {
            if (model.Tokens == null || model.Tokens.Length == 0)
                throw new InvalidOperationException("Submitted flag is empty.");
        }
    }
}

