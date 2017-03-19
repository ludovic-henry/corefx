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
		private static string NewLine => "\n";

		// internal and not readonly so that the tests can swap this out.
		internal static IDebugLogger s_logger = new MonoDebugLogger();

		// --------------
		// PAL ENDS HERE
		// --------------

		internal sealed class MonoDebugLogger : IDebugLogger
		{
			public void ShowAssertDialog(string stackTrace, string message, string detailMessage)
			{
			}

			public void WriteCore(string message)
			{
			}
		}
	}
}
