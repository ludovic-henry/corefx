// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace System.Net.Sockets
{
    internal sealed partial class ConnectOverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        public void CompletionCallback(SocketError errorCode)
        {
            if (Environment.IsRunningOnWindows)
                throw new PlatformNotSupportedException ();
            else
                Unix_CompletionCallback (errorCode);
        }

        internal override object PostCompletion(int numBytes)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_PostCompletion (numBytes);
            else
                return Unix_PostCompletion (numBytes);
        }
    }
}
