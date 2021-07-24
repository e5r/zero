// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using E5R.Architecture.Core;
using E5R.Zero.Domain.Entities;
using E5R.Zero.Features.AccessKeys;
using E5R.Zero.Rules;

using Moq;

using Xunit;

using static E5R.Zero.Rules.AccessKeyRuleGroup.WriteCategory;
using static E5R.Zero.UnitTest.TestUtils.TraitName;

namespace E5R.Zero.UnitTest.Features
{
    [Trait(nameof(Entity), nameof(AccessKeyEntity))]
    [Trait(nameof(Feature), nameof(RegisterNewAccessKeyFeature))]
    [Trait(nameof(Target), nameof(RegisterNewAccessKeyFeature))]
    public class RegisterNewAccessKeyFeatureTests
    {
        [Fact(DisplayName = nameof(RegisterNewAccessKeyFeature) + " requer um RuleSet<AccessKeyEntity>")]
        public void Requer_RuleFor()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new RegisterNewAccessKeyFeature(null));

            Assert.Equal("rules", ex.ParamName);
        }

        [Fact(DisplayName = nameof(RegisterNewAccessKeyFeature) + " requer um input não nulo")]
        public async Task Requer_InputNaoNulo()
        {
            var feature =
                new RegisterNewAccessKeyFeature(new Mock<RuleSet<AccessKeyEntity>>(new Mock<IServiceProvider>().Object)
                    .Object);

            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => feature.ExecAsync(null));

            Assert.Equal("input", ex.ParamName);
        }

        [Theory(DisplayName = nameof(RegisterNewAccessKeyFeature) + " valida todas as regras da categoria \"" +
                              WriteCategoryKey + "\"")]
        [ClassData(typeof(InvalidWriteRulesTestData))]
        public async Task ValidatesAllRules_OfRnAkWriteCategory(AccessKeyEntity inputError,
            IRuleFor<AccessKeyEntity> ruleFor)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(m => m.GetService(typeof(IEnumerable<IRuleFor<AccessKeyEntity>>)))
                .Returns(new List<IRuleFor<AccessKeyEntity>> {ruleFor});

            var feature = new RegisterNewAccessKeyFeature(new RuleSet<AccessKeyEntity>(serviceProviderMock.Object));
            var ex = await Assert.ThrowsAsync<AggregateException>(() => feature.ExecAsync(inputError));

            Assert.Contains($"{ruleFor.Code}: {ruleFor.Description}", ex.Message);
        }
    }
}
