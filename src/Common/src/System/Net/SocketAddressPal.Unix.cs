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
        internal static class Unix
        {
            public const int DataOffset = 0;

            public static readonly int IPv6AddressSize = GetIPv6AddressSize();
            public static readonly int IPv4AddressSize = GetIPv4AddressSize();

            private static unsafe int GetIPv6AddressSize()
            {
                int ipv6AddressSize, unused;
                Interop.Unix.Error err = Interop.Unix.Sys.GetIPSocketAddressSizes(&unused, &ipv6AddressSize);
                Debug.Assert(err == Interop.Unix.Error.SUCCESS, $"Unexpected err: {err}");
                return ipv6AddressSize;
            }

            private static unsafe int GetIPv4AddressSize()
            {
                int ipv4AddressSize, unused;
                Interop.Unix.Error err = Interop.Unix.Sys.GetIPSocketAddressSizes(&ipv4AddressSize, &unused);
                Debug.Assert(err == Interop.Unix.Error.SUCCESS, $"Unexpected err: {err}");
                return ipv4AddressSize;
            }

            private static void ThrowOnFailure(Interop.Unix.Error err)
            {
                switch (err)
                {
                    case Interop.Unix.Error.SUCCESS:
                        return;

                    case Interop.Unix.Error.EFAULT:
                        // The buffer was either null or too small.
                        throw new IndexOutOfRangeException();

                    case Interop.Unix.Error.EAFNOSUPPORT:
                        // There was no appropriate mapping from the platform address family.
                        throw new PlatformNotSupportedException();

                    default:
                        Debug.Fail("Unexpected failure in GetAddressFamily");
                        throw new PlatformNotSupportedException();
                }
            }

            public static unsafe AddressFamily GetAddressFamily(byte[] buffer)
            {
                AddressFamily family;
                Interop.Unix.Error err;
                fixed (byte* rawAddress = buffer)
                {
                    err = Interop.Unix.Sys.GetAddressFamily(rawAddress, buffer.Length, (int*)&family);
                }

                ThrowOnFailure(err);
                return family;
            }

            public static unsafe void SetAddressFamily(byte[] buffer, AddressFamily family)
            {
                Interop.Unix.Error err;
                fixed (byte* rawAddress = buffer)
                {
                    err = Interop.Unix.Sys.SetAddressFamily(rawAddress, buffer.Length, (int)family);
                }

                ThrowOnFailure(err);
            }

            public static unsafe ushort GetPort(byte[] buffer)
            {
                ushort port;
                Interop.Unix.Error err;
                fixed (byte* rawAddress = buffer)
                {
                    err = Interop.Unix.Sys.GetPort(rawAddress, buffer.Length, &port);
                }

                ThrowOnFailure(err);
                return port;
            }

            public static unsafe void SetPort(byte[] buffer, ushort port)
            {
                Interop.Unix.Error err;
                fixed (byte* rawAddress = buffer)
                {
                    err = Interop.Unix.Sys.SetPort(rawAddress, buffer.Length, port);
                }

                ThrowOnFailure(err);
            }

            public static unsafe uint GetIPv4Address(byte[] buffer)
            {
                uint ipAddress;
                Interop.Unix.Error err;
                fixed (byte* rawAddress = buffer)
                {
                    err = Interop.Unix.Sys.GetIPv4Address(rawAddress, buffer.Length, &ipAddress);
                }

                ThrowOnFailure(err);
                return ipAddress;
            }

            public static unsafe void GetIPv6Address(byte[] buffer, byte[] address, out uint scope)
            {
                uint localScope;
                Interop.Unix.Error err;
                fixed (byte* rawAddress = buffer)
                fixed (byte* ipAddress = address)
                {
                    err = Interop.Unix.Sys.GetIPv6Address(rawAddress, buffer.Length, ipAddress, address.Length, &localScope);
                }

                ThrowOnFailure(err);
                scope = localScope;
            }

            public static unsafe void SetIPv4Address(byte[] buffer, uint address)
            {
                Interop.Unix.Error err;
                fixed (byte* rawAddress = buffer)
                {
                    err = Interop.Unix.Sys.SetIPv4Address(rawAddress, buffer.Length, address);
                }

                ThrowOnFailure(err);
            }

            public static unsafe void SetIPv4Address(byte[] buffer, byte* address)
            {
                uint addr = (uint)System.Runtime.InteropServices.Marshal.ReadInt32((IntPtr)address);
                SetIPv4Address(buffer, addr);
            }

            public static unsafe void SetIPv6Address(byte[] buffer, byte[] address, uint scope)
            {
                fixed (byte* rawInput = address)
                {
                    SetIPv6Address(buffer, rawInput, address.Length, scope);
                }
            }

            public static unsafe void SetIPv6Address(byte[] buffer, byte* address, int addressLength, uint scope)
            {
                Interop.Unix.Error err;
                fixed (byte* rawAddress = buffer)
                {
                    err = Interop.Unix.Sys.SetIPv6Address(rawAddress, buffer.Length, address, addressLength, scope);
                }

                ThrowOnFailure(err);
            }
        }

#if !MONO
        public const int DataOffset = Unix.DataOffset;

        public static readonly int IPv6AddressSize = Unix.IPv6AddressSize;
        public static readonly int IPv4AddressSize = Unix.IPv4AddressSize;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe AddressFamily GetAddressFamily(byte[] buffer)
        {
            return Unix.GetAddressFamily (buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetAddressFamily(byte[] buffer, AddressFamily family)
        {
            Unix.SetAddressFamily (buffer, family);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ushort GetPort(byte[] buffer)
        {
            return Unix.GetPort (buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetPort(byte[] buffer, ushort port)
        {
            Unix.SetPort(buffer, port);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint GetIPv4Address(byte[] buffer)
        {
            return Unix.GetIPv4Address(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void GetIPv6Address(byte[] buffer, byte[] address, out uint scope)
        {
            Unix.GetIPv6Address(buffer, address, out scope);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetIPv4Address(byte[] buffer, uint address)
        {
            Unix.SetIPv4Address(buffer, address);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SetIPv6Address(byte[] buffer, byte[] address, uint scope)
        {
            Unix.SetIPv6Address(buffer, address, scope);
        }
#endif
    }
}
