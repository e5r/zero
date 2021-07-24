// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System.Threading.Tasks;

using E5R.Architecture.Business;
using E5R.Architecture.Core;
using E5R.Zero.Domain.Entities;
using E5R.Zero.Rules;

namespace E5R.Zero.Features.AccessKeys
{
    /// <summary>
    /// Registra uma nova chave de acesso
    /// </summary>
    public class RegisterNewAccessKeyFeature : BusinessFeature<AccessKeyEntity, AccessKeyEntity>
    {
        private RuleSet<AccessKeyEntity> Rules { get; }

        public RegisterNewAccessKeyFeature(RuleSet<AccessKeyEntity> rules)
        {
            Checker.NotNullArgument(rules, nameof(rules));

            Rules = rules;
        }

        protected override async Task<AccessKeyEntity> ExecActionAsync(AccessKeyEntity input)
        {
            await Rules.ByCategory(AccessKeyRuleGroup.RnAkWriteCategory).EnsureAsync(input);

            // TODO: Criar RN fingerprint é unico no sistema

            throw new System.NotImplementedException();
        }
    }
}
