// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Runtime.CompilerServices;

namespace System.Net
{
    internal static partial class SocketAddressPal
    {
        public static int DataOffset
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (Environment.IsRunningOnWindows)
                    return Windows.DataOffset;
                else
                    return Unix.DataOffset;
            }
        }

        public static int IPv6AddressSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (Environment.IsRunningOnWindows)
                    return Windows.IPv6AddressSize;
                else
                    return Unix.IPv6AddressSize;
            }
        }

        public static int IPv4AddressSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (Environment.IsRunningOnWindows)
                    return Windows.IPv4AddressSize;
                else
                    return Unix.IPv4AddressSize;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe AddressFamily GetAddressFamily(byte[] buffer)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetAddressFamily (buffer);
            else
                return Unix.GetAddressFamily (buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetAddressFamily(byte[] buffer, AddressFamily family)
        {
            if (Environment.IsRunningOnWindows)
                Windows.SetAddressFamily (buffer, family);
            else
                Unix.SetAddressFamily (buffer, family);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ushort GetPort(byte[] buffer)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetPort (buffer);
            else
                return Unix.GetPort (buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetPort(byte[] buffer, ushort port)
        {
            if (Environment.IsRunningOnWindows)
                Windows.SetPort (buffer, port);
            else
                Unix.SetPort (buffer, port);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint GetIPv4Address(byte[] buffer)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetIPv4Address(buffer);
            else
                return Unix.GetIPv4Address(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void GetIPv6Address(byte[] buffer, byte[] address, out uint scope)
        {
            if (Environment.IsRunningOnWindows)
                Windows.GetIPv6Address(buffer, address, out scope);
            else
                Unix.GetIPv6Address(buffer, address, out scope);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetIPv4Address(byte[] buffer, uint address)
        {
            if (Environment.IsRunningOnWindows)
                Windows.SetIPv4Address(buffer, address);
            else
                Unix.SetIPv4Address(buffer, address);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetIPv4Address(byte[] buffer, byte* address)
        {
            if (Environment.IsRunningOnWindows)
                throw new PlatformNotSupportedException ();
            else
                Unix.SetIPv4Address(buffer, address);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetIPv6Address(byte[] buffer, byte[] address, uint scope)
        {
            if (Environment.IsRunningOnWindows)
                Windows.SetIPv6Address(buffer, address, scope);
            else
                Unix.SetIPv6Address(buffer, address, scope);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetIPv6Address(byte[] buffer, byte* address, int addressLength, uint scope)
        {
            if (Environment.IsRunningOnWindows)
                throw new PlatformNotSupportedException ();
            else
                Unix.SetIPv6Address(buffer, address, addressLength, scope);
        }
    }
}
