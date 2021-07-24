// Copyright (c) E5R Development Team. All rights reserved.
// This file is a part of E5R.Zero.
// Licensed under the Apache version 2.0: https://github.com/e5r/manifest/blob/master/license/APACHE-2.0.txt

namespace E5R.Zero.Rules
{
    /// <summary>
    /// Grupo de regras para características de chave de acesso
    /// </summary>
    public static class AccessKeyRuleGroup
    {
        public const string RnAkWriteCategory = "accesskey.write";

        public static class RnAk01
        {
            public const string Code = "RNAK-01";
            public const string Description = "The fingerprint field is required to save an access key";
        }

        public static class RnAk02
        {
            public const string Code = "RNAK-02";
            public const string Description = "The keydata field is required to save an access key";
        }

        public static class RnAk03
        {
            public const string Code = "RNAK-03";
            public const string Description = "The fingerprint field is unique";
        }
    }
}
