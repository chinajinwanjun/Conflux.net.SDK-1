﻿namespace Conflux.ENS
{
    public static class TextDataKeyExtensions
    {
        public static string GetDataKeyAsString(this TextDataKey textDataKey)
        {
            return textDataKey.ToString().Replace("_", ".");
        }
    }
}