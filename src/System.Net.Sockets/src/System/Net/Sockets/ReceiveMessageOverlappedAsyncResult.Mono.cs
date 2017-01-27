// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using System.Collections.Generic;

namespace System.Net.Sockets
{
    internal unsafe sealed partial class ReceiveMessageOverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        internal unsafe int GetSocketAddressSize()
        {
            if (Environment.IsRunningOnWindows)
                return Windows_GetSocketAddressSize();
            else
                return Unix_GetSocketAddressSize ();
        }

        internal void SetUnmanagedStructures(byte[] buffer, int offset, int size, Internals.SocketAddress socketAddress, SocketFlags socketFlags)
        {
            if (Environment.IsRunningOnWindows)
                Windows_SetUnmanagedStructures(buffer, offset, size, socketAddress, socketFlags);
            else
                throw new PlatformNotSupportedException ();
        }

        internal void SyncReleaseUnmanagedStructures()
        {
            if (Environment.IsRunningOnWindows)
                Windows_SyncReleaseUnmanagedStructures();
            else
                throw new PlatformNotSupportedException ();
        }

        protected override void ForceReleaseUnmanagedStructures()
        {
            if (Environment.IsRunningOnWindows)
                Windows_ForceReleaseUnmanagedStructures();
            else
                base.ForceReleaseUnmanagedStructures ();
        }

        internal override object PostCompletion(int numBytes)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_PostCompletion(numBytes);
            else
                return base.PostCompletion (numBytes);
        }

        public void CompletionCallback(int numBytes, byte[] socketAddress, int socketAddressSize, SocketFlags receivedFlags, IPPacketInformation ipPacketInformation, SocketError errorCode)
        {
            if (Environment.IsRunningOnWindows)
                throw new PlatformNotSupportedException ();
            else
                Unix_CompletionCallback(numBytes, socketAddress, socketAddressSize, receivedFlags, ipPacketInformation, errorCode);
        }
    }
}
