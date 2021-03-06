﻿using BookKeeping.Domain;
using BookKeeping.Domain.Contracts;
using System;
using System.Runtime.InteropServices;

namespace BookKeeping
{
    /// <summary>
    /// Equivalent to System.Void which is not allowed to be used in the code for some reason.
    /// </summary>
    [ComVisible(true)]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct unit : ICriterion
    {
        public static readonly unit it = default(unit);
    }
}