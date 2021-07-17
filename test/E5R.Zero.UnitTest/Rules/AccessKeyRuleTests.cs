// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System.IO;

using E5R.Zero.Domain.Entities;
using E5R.Zero.Rules;
using E5R.Zero.Rules.AccessKeys;

using Moq;

using Xunit;

using static E5R.Zero.UnitTest.TestUtils.TraitName;

namespace E5R.Zero.UnitTest.Rules
{
    [Trait(nameof(Entity), nameof(AccessKeyEntity))]
    public class AccessKeyRuleTests
    {
        [Fact(DisplayName = nameof(FingerprintIsRequiredToWrite)+ " corresponde a RNAK-001")]
        [Trait(nameof(Target), nameof(FingerprintIsRequiredToWrite))]
        public void FingerprintIsRequiredToSave_CorrespondeA_RNAK001()
        {
            var rule = new FingerprintIsRequiredToWrite();

            Assert.Equal(AccessKeyRuleGroup.RnAkWriteCategory, rule.Category);
            Assert.Equal(AccessKeyRuleGroup.RnAk01.Code, rule.Code);
            Assert.Equal(AccessKeyRuleGroup.RnAk01.Description, rule.Description);
        }

        [Theory(DisplayName = nameof(FingerprintIsRequiredToWrite) + " garante que campo 'Fingerprint' seja obrigatório")]
        [Trait(nameof(Target), nameof(FingerprintIsRequiredToWrite))]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void FingerprintIsRequiredToSave_GaranteQue_CampoFingerprint_SejaObrigatorio(string value)
        {
            var rule = new FingerprintIsRequiredToWrite();
            var result = rule.Check(new AccessKeyEntity {Fingerprint = value});

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }

        [Fact(DisplayName = nameof(KeyDataIsRequiredAndMustHaveContentToWrite) + " corresponde a RNAK-002")]
        [Trait(nameof(Target), nameof(KeyDataIsRequiredAndMustHaveContentToWrite))]
        public void KeyDataIsRequiredToSave_CorrespondeA_RNAK002()
        {
            var rule = new KeyDataIsRequiredAndMustHaveContentToWrite();

            Assert.Equal(AccessKeyRuleGroup.RnAkWriteCategory, rule.Category);
            Assert.Equal(AccessKeyRuleGroup.RnAk02.Code, rule.Code);
            Assert.Equal(AccessKeyRuleGroup.RnAk02.Description, rule.Description);
        }

        [Fact(DisplayName = nameof(KeyDataIsRequiredAndMustHaveContentToWrite) + " garante que campo 'KeyData' seja obrigatório")]
        [Trait(nameof(Target), nameof(KeyDataIsRequiredAndMustHaveContentToWrite))]
        public void KeyDataIsRequiredToSave_GaranteQue_CampoKeyData_SejaObrigatorio()
        {
            var rule = new KeyDataIsRequiredAndMustHaveContentToWrite();
            var result = rule.Check(new AccessKeyEntity {KeyData = null});

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }

        [Fact(DisplayName = nameof(KeyDataIsRequiredAndMustHaveContentToWrite) + " garante que campo 'KeyData' tenha conteúdo")]
        [Trait(nameof(Target), nameof(KeyDataIsRequiredAndMustHaveContentToWrite))]
        public void KeyDataIsRequiredToSave_GaranteQue_CampoKeyData_TenhaConteudo()
        {
            var streamMock = new Mock<Stream>();

            streamMock.Setup(m => m.Length).Returns(0);

            var rule = new KeyDataIsRequiredAndMustHaveContentToWrite();
            var result = rule.Check(new AccessKeyEntity {KeyData = streamMock.Object});

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }

        [Fact(DisplayName = nameof(KeyDataIsRequiredAndMustHaveContentToWrite) + " garante que campo 'KeyData' permita leitura")]
        [Trait(nameof(Target), nameof(KeyDataIsRequiredAndMustHaveContentToWrite))]
        public void KeyDataIsRequiredToSave_GaranteQue_CampoKeyData_PermitaLeitura()
        {
            var streamMock = new Mock<Stream>();

            streamMock.Setup(m => m.Length).Returns(1);
            streamMock.Setup(m => m.CanRead).Returns(false);

            var rule = new KeyDataIsRequiredAndMustHaveContentToWrite();
            var result = rule.Check(new AccessKeyEntity {KeyData = streamMock.Object});

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }
    }
}
