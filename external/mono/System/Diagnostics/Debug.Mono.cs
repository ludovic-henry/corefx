// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
	// Intentionally excluding visibility so it defaults to internal except for
	// the one public version in System.Diagnostics.Debug which defines
	// another version of this partial class with the public visibility 
	static partial class Debug
	{
		private static string NewLine => PlatformHelper.IsWindows ? Windows.NewLine : Unix.NewLine;

		// internal and not readonly so that the tests can swap this out.
		internal static IDebugLogger s_logger = PlatformHelper.IsWindows ? Windows.s_logger : Unix.s_logger;
	}
}
