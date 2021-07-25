// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using E5R.Architecture.Core;
using E5R.Architecture.Data.Abstractions;
using E5R.Zero.Domain.Entities;
using E5R.Zero.Features.AccessKeys;

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
        public void Requer_RuleSet()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new RegisterNewAccessKeyFeature(null, new Mock<ILazy<IStorage<AccessKeyEntity>>>().Object));

            Assert.Equal("rules", ex.ParamName);
        }

        [Fact(DisplayName = nameof(RegisterNewAccessKeyFeature) + " requer um IStorage<AccessKeyEntity>")]
        public void Requer_Storage()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new RegisterNewAccessKeyFeature(new Mock<IRuleSet<AccessKeyEntity>>().Object, null));

            Assert.Equal("storage", ex.ParamName);
        }

        [Fact(DisplayName = nameof(RegisterNewAccessKeyFeature) + " requer um input não nulo")]
        public async Task Requer_InputNaoNulo()
        {
            var feature =
                new RegisterNewAccessKeyFeature(new Mock<IRuleSet<AccessKeyEntity>>().Object,
                    new Mock<ILazy<IStorage<AccessKeyEntity>>>().Object);

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

            var feature = new RegisterNewAccessKeyFeature(new RuleSet<AccessKeyEntity>(serviceProviderMock.Object),
                new Mock<ILazy<IStorage<AccessKeyEntity>>>().Object);
            var ex = await Assert.ThrowsAsync<AggregateException>(() => feature.ExecAsync(inputError));

            Assert.Contains($"{ruleFor.Code}: {ruleFor.Description}", ex.Message);
        }

        [Fact(DisplayName = nameof(RegisterNewAccessKeyFeature) + " cria um objeto no armazenamento quando nenhuma regra é violada")]
        public async Task CriaUmObjetoNoArmazenamento_QuantoNenhumaRegraEViolada()
        {
            var toCreate = new AccessKeyEntity
            {
                Fingerprint = "added-fingerprint",
                KeyData = new MemoryStream(new byte[] {1, 2, 3, 4, 5})
            };

            var storageMock = new Mock<IStorage<AccessKeyEntity>>();
            var lazyMock = new Mock<ILazy<IStorage<AccessKeyEntity>>>();

            lazyMock.Setup(m => m.Value).Returns(storageMock.Object);

            var ruleSetMock = new Mock<IRuleSet<AccessKeyEntity>>();

            ruleSetMock.Setup(m => m.ByCategory(It.IsAny<string>())).Returns(ruleSetMock.Object);

            var feature = new RegisterNewAccessKeyFeature(ruleSetMock.Object, lazyMock.Object);

            await feature.ExecAsync(toCreate);

            storageMock.Verify(v => v.Create(toCreate), Times.Exactly(1));
        }

        [Fact(DisplayName = nameof(RegisterNewAccessKeyFeature) + " retorna o objeto criado pelo armazenamento")]
        public async Task RetornaObjetoCriadoNoArmazenamento()
        {
            var toCreate = new AccessKeyEntity
            {
                Fingerprint = "added-fingerprint",
                KeyData = new MemoryStream(new byte[] {1, 2, 3, 4, 5})
            };

            var storageMock = new Mock<IStorage<AccessKeyEntity>>();
            storageMock.Setup(m => m.Create(It.IsAny<AccessKeyEntity>())).Returns(new AccessKeyEntity
            {
                Fingerprint = "added-fingerprint",
                KeyData = new MemoryStream(new byte[] {1, 2, 3, 4, 5})
            });

            var lazyMock = new Mock<ILazy<IStorage<AccessKeyEntity>>>();
            lazyMock.Setup(m => m.Value).Returns(storageMock.Object);

            var ruleSetMock = new Mock<IRuleSet<AccessKeyEntity>>();

            ruleSetMock.Setup(m => m.ByCategory(It.IsAny<string>())).Returns(ruleSetMock.Object);

            var feature = new RegisterNewAccessKeyFeature(ruleSetMock.Object, lazyMock.Object);

            var result = await feature.ExecAsync(toCreate);

            Assert.NotNull(result);
            Assert.Equal(toCreate.Fingerprint, result.Fingerprint);

            var resultBuffer = new byte[5];
            await result.KeyData.ReadAsync(resultBuffer, 0, 5);

            Assert.Equal(1, resultBuffer[0]);
            Assert.Equal(2, resultBuffer[1]);
            Assert.Equal(3, resultBuffer[2]);
            Assert.Equal(4, resultBuffer[3]);
            Assert.Equal(5, resultBuffer[4]);

            ruleSetMock.Verify(v => v.ByCategory(It.IsAny<string>()), Times.Exactly(1));
            ruleSetMock.Verify(v => v.EnsureAsync(It.IsAny<AccessKeyEntity>(), null), Times.Exactly(1));
        }
    }
}
