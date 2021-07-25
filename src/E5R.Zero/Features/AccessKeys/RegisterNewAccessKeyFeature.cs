// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System.Threading.Tasks;

using E5R.Architecture.Business;
using E5R.Architecture.Core;
using E5R.Architecture.Data.Abstractions;
using E5R.Zero.Domain.Entities;

using static E5R.Zero.Rules.AccessKeyRuleGroup.WriteCategory;

namespace E5R.Zero.Features.AccessKeys
{
    /// <summary>
    /// Registra uma nova chave de acesso
    /// </summary>
    public class RegisterNewAccessKeyFeature : BusinessFeature<AccessKeyEntity, AccessKeyEntity>
    {
        private IRuleSet<AccessKeyEntity> Rules { get; }
        private ILazy<IStorage<AccessKeyEntity>> Storage { get; }

        public RegisterNewAccessKeyFeature(IRuleSet<AccessKeyEntity> rules, ILazy<IStorage<AccessKeyEntity>> storage)
        {
            Checker.NotNullArgument(rules, nameof(rules));
            Checker.NotNullArgument(storage, nameof(storage));

            Rules = rules;
            Storage = storage;
        }

        protected override async Task<AccessKeyEntity> ExecActionAsync(AccessKeyEntity input)
        {
            await Rules.ByCategory(WriteCategoryKey).EnsureAsync(input);

            return Storage.Value.Create(input);
        }
    }
}
