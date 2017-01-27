// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Net.Sockets
{
    internal static class SocketErrorPal
    {
        internal static SocketError GetSocketErrorForNativeError(Interop.Unix.Error errno)
        {
            if (Environment.IsRunningOnWindows)
                throw new PlatformNotSupportedException ();
            else
                return Unix.GetSocketErrorForNativeError(errno);
        }

        internal static Interop.Unix.Error GetNativeErrorForSocketError(SocketError error)
        {
            if (Environment.IsRunningOnWindows)
                throw new PlatformNotSupportedException ();
            else
                return Unix.GetNativeErrorForSocketError(error);
        }
    }
}
