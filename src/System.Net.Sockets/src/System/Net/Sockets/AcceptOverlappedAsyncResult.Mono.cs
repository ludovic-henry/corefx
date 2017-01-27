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
    internal sealed partial class AcceptOverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        internal Socket AcceptSocket
        {
            set {
                if (Environment.IsRunningOnWindows)
                    Windows_AcceptSocket = value;
                else
                    Unix_AcceptSocket = value;
            }
        }

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

        internal void SetUnmanagedStructures(byte[] buffer, int addressBufferLength)
        {
            if (Environment.IsRunningOnWindows)
                Windows_SetUnmanagedStructures (buffer, addressBufferLength);
            else
                throw new PlatformNotSupportedException ();
        }
    }
}
