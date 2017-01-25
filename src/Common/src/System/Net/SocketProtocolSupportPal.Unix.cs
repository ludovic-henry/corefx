// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Internals;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net
{
    internal partial class SocketProtocolSupportPal
    {
        internal static class Unix
        {
            private static bool s_ipv4 = true;
            private static bool s_ipv6 = true;

            private static bool s_initialized;
            private static readonly object s_initializedLock = new object();

            public static bool OSSupportsIPv6
            {
                get
                {
                    EnsureInitialized();
                    return s_ipv6;
                }
            }

            public static bool OSSupportsIPv4
            {
                get
                {
                    EnsureInitialized();
                    return s_ipv4;
                }
            }

            private static void EnsureInitialized()
            {
                if (!Volatile.Read(ref s_initialized))
                {
                    lock (s_initializedLock)
                    {
                        if (!s_initialized)
                        {
                            s_ipv4 = IsProtocolSupported(AddressFamily.InterNetwork);
                            s_ipv6 = IsProtocolSupported(AddressFamily.InterNetworkV6);

                            Volatile.Write(ref s_initialized, true);
                        }
                    }
                }
            }

            private static unsafe bool IsProtocolSupported(AddressFamily af)
            {
                IntPtr socket = (IntPtr)(-1);
                try
                {
                    Interop.Unix.Error err = Interop.Unix.Sys.Socket(af, SocketType.Dgram, (ProtocolType)0, &socket);
                    return err != Interop.Unix.Error.EAFNOSUPPORT;
                }
                finally
                {
                    if (socket != (IntPtr)(-1))
                    {
                        Interop.Unix.Sys.Close(socket);
                    }
                }
            }
        }

#if !MONO
        public static bool OSSupportsIPv6
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Unix.OSSupportsIPv6; }
        }

        public static bool OSSupportsIPv4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Unix.OSSupportsIPv4; }
        }
#endif
    }
}
