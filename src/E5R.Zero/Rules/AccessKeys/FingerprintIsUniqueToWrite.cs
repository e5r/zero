// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System.Threading.Tasks;

using E5R.Architecture.Core;
using E5R.Architecture.Data.Abstractions;
using E5R.Zero.Domain.Entities;

using static E5R.Zero.Rules.AccessKeyRuleGroup.WriteCategory;

namespace E5R.Zero.Rules.AccessKeys
{
    /// <summary>
    /// RNAK-03: A impressão digital da chave é única
    /// </summary>
    public class FingerprintIsUniqueToWrite : RuleFor<AccessKeyEntity>
    {
        private ILazy<IStorage<AccessKeyEntity>> Storage { get; }

        public FingerprintIsUniqueToWrite(ILazy<IStorage<AccessKeyEntity>> storage) : base(RnAk03.Code,
            WriteCategoryKey, RnAk03.Description)
        {
            Checker.NotNullArgument(storage, nameof(storage));

            Storage = storage;
        }

        public override async Task<RuleCheckResult> CheckAsync(AccessKeyEntity target)
        {
            return Storage.Value.AsFluentQuery().Find(target?.Identifiers) != null ? await Fail() : await Success();
        }
    }
}
