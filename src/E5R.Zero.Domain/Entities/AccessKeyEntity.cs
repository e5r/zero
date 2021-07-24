// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

using System.IO;

using E5R.Architecture.Core;

namespace E5R.Zero.Domain.Entities
{
    public class AccessKeyEntity : IIdentifiable
    {
        public object[] Identifiers => new object[] { Fingerprint };

        public string Fingerprint { get; set; }
        public Stream KeyData { get; set; }
    }
}
