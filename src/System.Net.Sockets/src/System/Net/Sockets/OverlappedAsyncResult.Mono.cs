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
    internal partial class OverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        // Windows

        internal WSABuffer _singleBuffer {
            get {
                if (Environment.IsRunningOnWindows)
                    return Windows_singleBuffer;
                else
                    throw new PlatformNotSupportedException ();
            }
            set {
                if (Environment.IsRunningOnWindows)
                    Windows_singleBuffer = value;
                else
                    throw new PlatformNotSupportedException ();
            }
        }

        internal WSABuffer[] _wsaBuffers {
            get {
                if (Environment.IsRunningOnWindows)
                    return Windows_wsaBuffers;
                else
                    throw new PlatformNotSupportedException ();
            }
            set {
                if (Environment.IsRunningOnWindows)
                    Windows_wsaBuffers = value;
                else
                    throw new PlatformNotSupportedException ();
            }
        }

        internal IntPtr GetSocketAddressPtr()
        {
            if (Environment.IsRunningOnWindows)
                return Windows_GetSocketAddressPtr();
            else
                throw new PlatformNotSupportedException ();
        }

        internal IntPtr GetSocketAddressSizePtr()
        {
            if (Environment.IsRunningOnWindows)
                return Windows_GetSocketAddressSizePtr();
            else
                throw new PlatformNotSupportedException ();
        }

        internal unsafe int GetSocketAddressSize()
        {
            if (Environment.IsRunningOnWindows)
                return Windows_GetSocketAddressSize();
            else
                return Unix_GetSocketAddressSize();
        }

        internal void SetUnmanagedStructures(byte[] buffer, int offset, int size, Internals.SocketAddress socketAddress, bool pinSocketAddress)
        {
            if (Environment.IsRunningOnWindows)
                Windows_SetUnmanagedStructures(buffer, offset, size, socketAddress, pinSocketAddress);
            else
                throw new PlatformNotSupportedException ();
        }

        internal void SetUnmanagedStructures(IList<ArraySegment<byte>> buffers)
        {
            if (Environment.IsRunningOnWindows)
                Windows_SetUnmanagedStructures(buffers);
            else
                throw new PlatformNotSupportedException ();
        }

        internal override object PostCompletion(int numBytes)
        {
            if (Environment.IsRunningOnWindows)
                Windows_PostCompletion(numBytes);
            else
                throw new PlatformNotSupportedException ();
        }

        // Unix

        public void CompletionCallback(int numBytes, byte[] socketAddress, int socketAddressSize, SocketFlags receivedFlags, SocketError errorCode)
        {
            if (Environment.IsRunningOnWindows)
                throw new PlatformNotSupportedException ();
            else
                Unix_CompletionCallback(numBytes, socketAddress, socketAddressSize, receivedFlags, errorCode);
        }
    }
}
