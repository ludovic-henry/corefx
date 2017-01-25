// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace System.Net.Sockets
{
    internal partial class SafeCloseSocket :
#if DEBUG
        DebugSafeHandleMinusOneIsInvalid
#else
        SafeHandleMinusOneIsInvalid
#endif
    {
        private int _receiveTimeout = -1;
        private int _sendTimeout = -1;
        private bool _nonBlocking;
        private SocketAsyncContext _asyncContext;

        public SocketAsyncContext AsyncContext
        {
            get
            {
                if (Volatile.Read(ref _asyncContext) == null)
                {
                    Interlocked.CompareExchange(ref _asyncContext, new SocketAsyncContext(this), null);
                }

                return _asyncContext;
            }
        }

        public bool IsNonBlocking
        {
            get
            {
                return _nonBlocking;
            }
            set
            {
                _nonBlocking = value;

                //
                // If transitioning to non-blocking, we need to set the native socket to non-blocking mode.
                // If we ever transition back to blocking, we keep the native socket in non-blocking mode, and emulate
                // blocking.  This avoids problems with switching to native blocking while there are pending async
                // operations.
                //
                if (value)
                {
                    AsyncContext.SetNonBlocking();
                }
            }
        }

#if !MONO
        public int ReceiveTimeout
        {
            get
            {
                return _receiveTimeout;
            }
            set
            {
                Debug.Assert(value == -1 || value > 0, $"Unexpected value: {value}");
                _receiveTimeout = value;;
            }
        }
#endif

#if !MONO
        public int SendTimeout
        {
            get
            {
                return _sendTimeout;
            }
            set
            {
                Debug.Assert(value == -1 || value > 0, $"Unexpected value: {value}");
                _sendTimeout = value;
            }
        }
#endif

        internal static partial class Unix
        {
            public static unsafe SafeCloseSocket CreateSocket(IntPtr fileDescriptor)
            {
                return SafeCloseSocket.CreateSocket(InnerSafeCloseSocket.Unix.CreateSocket(fileDescriptor));
            }
        }

#if !MONO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe SafeCloseSocket CreateSocket(IntPtr fileDescriptor)
        {
            return Unix.CreateSocket(fileDescriptor);
        }
#endif

        internal static partial class Unix
        {
            public static unsafe SocketError CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, out SafeCloseSocket socket)
            {
                SocketError errorCode;
                socket = SafeCloseSocket.CreateSocket(InnerSafeCloseSocket.Unix.CreateSocket(addressFamily, socketType, protocolType, out errorCode));
                return errorCode;
            }
        }

#if !MONO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe SocketError CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, out SafeCloseSocket socket)
        {
            return Unix.CreateSocket(addressFamily, socketType, protocolType, out socket);
        }
#endif

        internal static partial class Unix
        {
            public static unsafe SocketError Accept(SafeCloseSocket socketHandle, byte[] socketAddress, ref int socketAddressSize, out SafeCloseSocket socket)
            {
                SocketError errorCode;
                socket = SafeCloseSocket.CreateSocket(InnerSafeCloseSocket.Unix.Accept(socketHandle, socketAddress, ref socketAddressSize, out errorCode));
                return errorCode;
            }
        }

#if !MONO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe SocketError Accept(SafeCloseSocket socketHandle, byte[] socketAddress, ref int socketAddressSize, out SafeCloseSocket socket)
        {
            return Unix.Accept(socketHandle, socketAddress, ref socketAddressSize, out socket);
        }
#endif

#if !MONO
        private void InnerReleaseHandle()
        {
            if (_asyncContext != null)
            {
                _asyncContext.Close();
            }
        }
#endif

        internal sealed partial class InnerSafeCloseSocket : SafeHandleMinusOneIsInvalid
        {
#if !MONO
            private unsafe SocketError InnerReleaseHandle()
            {
                int errorCode;

                // If _blockable was set in BlockingRelease, it's safe to block here, which means
                // we can honor the linger options set on the socket.  It also means closesocket() might return WSAEWOULDBLOCK, in which
                // case we need to do some recovery.
                if (_blockable)
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle} Following 'blockable' branch.");

                    errorCode = Interop.Unix.Sys.Close(handle);
                    if (errorCode == -1)
                    {
                        errorCode = (int)Interop.Unix.Sys.GetLastError();
                    }

                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, close()#1:{errorCode}");
#if DEBUG
                    _closeSocketHandle = handle;
                    _closeSocketResult = SocketPal.Unix.GetSocketErrorForErrorCode((Interop.Unix.Error)errorCode);
#endif

                    // If it's not EWOULDBLOCK, there's no more recourse - we either succeeded or failed.
                    if (errorCode != (int)Interop.Unix.Error.EWOULDBLOCK)
                    {
                        return SocketPal.Unix.GetSocketErrorForErrorCode((Interop.Unix.Error)errorCode);
                    }

                    // The socket must be non-blocking with a linger timeout set.
                    // We have to set the socket to blocking.
                    errorCode = Interop.Unix.Sys.Fcntl.DangerousSetIsNonBlocking(handle, 0);
                    if (errorCode == 0)
                    {
                        // The socket successfully made blocking; retry the close().
                        errorCode = Interop.Unix.Sys.Close(handle);

                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, close()#2:{errorCode}");
#if DEBUG
                        _closeSocketHandle = handle;
                        _closeSocketResult = SocketPal.Unix.GetSocketErrorForErrorCode((Interop.Unix.Error)errorCode);
#endif
                        return SocketPal.Unix.GetSocketErrorForErrorCode((Interop.Unix.Error)errorCode);
                    }

                    // The socket could not be made blocking; fall through to the regular abortive close.
                }

                // By default or if CloseAsIs() path failed, set linger timeout to zero to get an abortive close (RST).
                var linger = new Interop.Unix.Sys.LingerOption {
                    OnOff = 1,
                    Seconds = 0
                };

                errorCode = (int)Interop.Unix.Sys.SetLingerOption(handle, &linger);
#if DEBUG
                _closeSocketLinger = SocketPal.Unix.GetSocketErrorForErrorCode((Interop.Unix.Error)errorCode);
#endif
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, setsockopt():{errorCode}");

                if (errorCode != 0 && errorCode != (int)Interop.Unix.Error.EINVAL && errorCode != (int)Interop.Unix.Error.ENOPROTOOPT)
                {
                    // Too dangerous to try closesocket() - it might block!
                    return SocketPal.Unix.GetSocketErrorForErrorCode((Interop.Unix.Error)errorCode);
                }

                errorCode = Interop.Unix.Sys.Close(handle);
#if DEBUG
                _closeSocketHandle = handle;
                _closeSocketResult = SocketPal.Unix.GetSocketErrorForErrorCode((Interop.Unix.Error)errorCode);
#endif
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, close#3():{(errorCode == -1 ? (int)Interop.Unix.Sys.GetLastError() : errorCode)}");

                return SocketPal.Unix.GetSocketErrorForErrorCode((Interop.Unix.Error)errorCode);
            }
#endif

            internal static partial class Unix
            {
                public static InnerSafeCloseSocket CreateSocket(IntPtr fileDescriptor)
                {
                    var res = new InnerSafeCloseSocket();
                    res.SetHandle(fileDescriptor);
                    return res;
                }
            }

#if !MONO
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static InnerSafeCloseSocket CreateSocket(IntPtr fileDescriptor)
            {
                return Unix.CreateSocket (fileDescriptor);
            }
#endif

            internal static partial class Unix
            {
                public static unsafe InnerSafeCloseSocket CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, out SocketError errorCode)
                {
                    IntPtr fd;
                    Interop.Unix.Error error = Interop.Unix.Sys.Socket(addressFamily, socketType, protocolType, &fd);
                    if (error == Interop.Unix.Error.SUCCESS)
                    {
                        Debug.Assert(fd != (IntPtr)(-1), "fd should not be -1");

                        errorCode = SocketError.Success;

                        // The socket was created successfully; enable IPV6_V6ONLY by default for AF_INET6 sockets.
                        if (addressFamily == AddressFamily.InterNetworkV6)
                        {
                            int on = 1;
                            error = Interop.Unix.Sys.SetSockOpt(fd, SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, (byte*)&on, sizeof(int));
                            if (error != Interop.Unix.Error.SUCCESS)
                            {
                                Interop.Unix.Sys.Close(fd);
                                fd = (IntPtr)(-1);
                                errorCode = SocketPal.Unix.GetSocketErrorForErrorCode(error);
                            }
                        }
                    }
                    else
                    {
                        Debug.Assert(fd == (IntPtr)(-1), $"Unexpected fd: {fd}");

                        errorCode = SocketPal.Unix.GetSocketErrorForErrorCode(error);
                    }

                    var res = new InnerSafeCloseSocket();
                    res.SetHandle(fd);
                    return res;
                }
            }

#if !MONO
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static InnerSafeCloseSocket CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, out SocketError errorCode)
            {
                return Unix.CreateSocket (addressFamily, socketType, protocolType, out socket);
            }
#endif

            internal static partial class Unix
            {
                public static unsafe InnerSafeCloseSocket Accept(SafeCloseSocket socketHandle, byte[] socketAddress, ref int socketAddressLen, out SocketError errorCode)
                {
                    IntPtr acceptedFd;
                    if (!socketHandle.IsNonBlocking)
                    {
                        errorCode = socketHandle.AsyncContext.Accept(socketAddress, ref socketAddressLen, -1, out acceptedFd);
                    }
                    else
                    {
                        SocketPal.Unix.TryCompleteAccept(socketHandle, socketAddress, ref socketAddressLen, out acceptedFd, out errorCode);
                    }

                    var res = new InnerSafeCloseSocket();
                    res.SetHandle(acceptedFd);
                    return res;
                }
            }

#if !MONO
            public static unsafe InnerSafeCloseSocket Accept(SafeCloseSocket socketHandle, byte[] socketAddress, ref int socketAddressLen, out SocketError errorCode)
            {
                return Unix.Accept(socketHandle, socketAddress, ref socketAddressLen, out errorCode);
            }
#endif
        }
    }
}
