// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System.Threading.Tasks;

using E5R.Architecture.Core;
using E5R.Zero.Domain.Entities;

using static E5R.Zero.Rules.AccessKeyRuleGroup;

namespace E5R.Zero.Rules.AccessKeys
{
    /// <summary>
    /// RNAK-01: A impressão digital da chave requer um conteúdo para que a chave seja gravada
    /// </summary>
    public class FingerprintIsRequiredToWrite : RuleFor<AccessKeyEntity>
    {
        public FingerprintIsRequiredToWrite() : base(WriteCategory.RnAk01.Code, WriteCategory.WriteCategoryKey,
            WriteCategory.RnAk01.Description)
        {
        }

        public override async Task<RuleCheckResult> CheckAsync(AccessKeyEntity target)
        {
            return string.IsNullOrWhiteSpace(target.Fingerprint) ? await Fail() : await Success();
        }
    }
}
