// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System.Collections;
using System.Collections.Generic;
using System.IO;

using E5R.Zero.Domain.Entities;
using E5R.Zero.Rules.AccessKeys;

using Moq;

namespace E5R.Zero.UnitTest.Features
{
    public class InvalidWriteRulesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // Fingerprint
            yield return new object[] {new AccessKeyEntity {Fingerprint = null}, new FingerprintIsRequiredToWrite()};
            yield return new object[] {new AccessKeyEntity {Fingerprint = ""}, new FingerprintIsRequiredToWrite()};
            yield return new object[] {new AccessKeyEntity {Fingerprint = "   "}, new FingerprintIsRequiredToWrite()};

            // KeyData
            yield return new object[]
                {new AccessKeyEntity {KeyData = null}, new KeyDataIsRequiredAndMustHaveContentToWrite()};

            var mockLength = new Mock<Stream>();
            mockLength.Setup(m => m.Length).Returns(0);
            yield return new object[]
            {
                new AccessKeyEntity {KeyData = mockLength.Object}, new KeyDataIsRequiredAndMustHaveContentToWrite()
            };

            var mockNoCanRead = new Mock<Stream>();
            mockNoCanRead.Setup(m => m.CanRead).Returns(false);
            yield return new object[]
            {
                new AccessKeyEntity {KeyData = mockNoCanRead.Object}, new KeyDataIsRequiredAndMustHaveContentToWrite()
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
