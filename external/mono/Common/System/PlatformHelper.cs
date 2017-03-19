// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
	internal static partial class PlatformHelper
	{
		public static readonly bool IsUnix = Environment.OSVersion.Platform == PlatformID.Unix;
		public static readonly bool IsWindows = Environment.OSVersion.Platform != PlatformID.Unix;
	}
}