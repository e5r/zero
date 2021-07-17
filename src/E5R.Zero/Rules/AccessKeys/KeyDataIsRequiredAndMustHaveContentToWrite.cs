// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System.Threading.Tasks;

using E5R.Architecture.Core;
using E5R.Zero.Domain.Entities;

using static E5R.Architecture.Core.RuleCheckResult;
using static E5R.Zero.Rules.AccessKeyRuleGroup;

namespace E5R.Zero.Rules.AccessKeys
{
    /// <summary>
    /// RNAK-02: Os dados da chave requer conteúdo para que a chave seja gravada
    /// </summary>
    public class KeyDataIsRequiredAndMustHaveContentToWrite : RuleFor<AccessKeyEntity>
    {
        public KeyDataIsRequiredAndMustHaveContentToWrite() : base(RnAk02.Code, RnAkWriteCategory, RnAk02.Description)
        {
        }

        public override Task<RuleCheckResult> CheckAsync(AccessKeyEntity target)
        {
            return target.KeyData == null || target.KeyData.Length < 1 || !target.KeyData.CanRead
                ? Task.FromResult(Fail)
                : Task.FromResult(Success);
        }
    }
}
