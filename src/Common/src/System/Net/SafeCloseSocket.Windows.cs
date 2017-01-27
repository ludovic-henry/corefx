// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net.Sockets
{
    internal partial class SafeCloseSocket :
#if DEBUG
        DebugSafeHandleMinusOneIsInvalid
#else
        SafeHandleMinusOneIsInvalid
#endif
    {
        private ThreadPoolBoundHandle _iocpBoundHandle;
        private object _iocpBindingLock = new object();

#if MONO
        private ThreadPoolBoundHandle Windows_IOCPBoundHandle
#else
        public ThreadPoolBoundHandle IOCPBoundHandle
#endif
        {
            get
            {
                return _iocpBoundHandle;
            }
        }

        // Binds the Socket Win32 Handle to the ThreadPool's CompletionPort.
#if MONO
        private ThreadPoolBoundHandle Windows_GetOrAllocateThreadPoolBoundHandle()
#else
        public ThreadPoolBoundHandle GetOrAllocateThreadPoolBoundHandle()
#endif
        {
            if (_released)
            {
                // Keep the exception message pointing at the external type.
                throw new ObjectDisposedException(typeof(Socket).FullName);
            }

            // Check to see if the socket native _handle is already
            // bound to the ThreadPool's completion port.
            if (_iocpBoundHandle == null)
            {
                lock (_iocpBindingLock)
                {
                    if (_iocpBoundHandle == null)
                    {
                        // Bind the socket native _handle to the ThreadPool.
                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, "calling ThreadPool.BindHandle()");

                        try
                        {
                            // The handle (this) may have been already released:
                            // E.g.: The socket has been disposed in the main thread. A completion callback may
                            //       attempt starting another operation.
                            _iocpBoundHandle = ThreadPoolBoundHandle.BindHandle(this);
                        }
                        catch (Exception exception)
                        {
                            if (ExceptionCheck.IsFatal(exception)) throw;
                            CloseAsIs();
                            throw;
                        }
                    }
                }
            }

            return _iocpBoundHandle;
        }

        internal static partial class Windows
        {
        internal static unsafe SafeCloseSocket CreateWSASocket(byte* pinnedBuffer)
        {
            return SafeCloseSocket.CreateSocket(InnerSafeCloseSocket.Windows.CreateWSASocket(pinnedBuffer));
        }

        internal static SafeCloseSocket CreateWSASocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            return SafeCloseSocket.CreateSocket(InnerSafeCloseSocket.Windows.CreateWSASocket(addressFamily, socketType, protocolType));
        }

        internal static SafeCloseSocket Accept(
            SafeCloseSocket socketHandle,
            byte[] socketAddress,
            ref int socketAddressSize)
        {
            return SafeCloseSocket.CreateSocket(InnerSafeCloseSocket.Windows.Accept(socketHandle, socketAddress, ref socketAddressSize));
        }
        }

#if MONO
        private void Windows_InnerReleaseHandle()
#else
        private void InnerReleaseHandle()
#endif
        {
            // Keep m_IocpBoundHandle around after disposing it to allow freeing NativeOverlapped.
            // ThreadPoolBoundHandle allows FreeNativeOverlapped even after it has been disposed.
            if (_iocpBoundHandle != null)
            {
                _iocpBoundHandle.Dispose();
            }
        }

        internal sealed partial class InnerSafeCloseSocket : SafeHandleMinusOneIsInvalid
        {
#if MONO
            private SocketError Windows_InnerReleaseHandle()
#else
            private SocketError InnerReleaseHandle()
#endif
            {
                SocketError errorCode;

                // If _blockable was set in BlockingRelease, it's safe to block here, which means
                // we can honor the linger options set on the socket.  It also means closesocket() might return WSAEWOULDBLOCK, in which
                // case we need to do some recovery.
                if (_blockable)
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, Following 'blockable' branch");
                    errorCode = Interop.Windows.Winsock.closesocket(handle);
#if DEBUG
                    _closeSocketHandle = handle;
                    _closeSocketResult = errorCode;
#endif
                    if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();

                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, closesocket()#1:{errorCode}");

                    // If it's not WSAEWOULDBLOCK, there's no more recourse - we either succeeded or failed.
                    if (errorCode != SocketError.WouldBlock)
                    {
                        return errorCode;
                    }

                    // The socket must be non-blocking with a linger timeout set.
                    // We have to set the socket to blocking.
                    int nonBlockCmd = 0;
                    errorCode = Interop.Windows.Winsock.ioctlsocket(
                        handle,
                        Interop.Windows.Winsock.IoctlSocketConstants.FIONBIO,
                        ref nonBlockCmd);
                    if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();

                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, ioctlsocket()#1:{errorCode}");

                    // This can fail if there's a pending WSAEventSelect.  Try canceling it.
                    if (errorCode == SocketError.InvalidArgument)
                    {
                        errorCode = Interop.Windows.Winsock.WSAEventSelect(
                            handle,
                            IntPtr.Zero,
                            Interop.Windows.Winsock.AsyncEventBits.FdNone);

                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, WSAEventSelect()#1:{(errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode)}");

                        // Now retry the ioctl.
                        errorCode = Interop.Windows.Winsock.ioctlsocket(
                            handle,
                            Interop.Windows.Winsock.IoctlSocketConstants.FIONBIO,
                            ref nonBlockCmd);

                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, ioctlsocket()#2:{(errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode)}");
                    }

                    // If that succeeded, try again.
                    if (errorCode == SocketError.Success)
                    {
                        errorCode = Interop.Windows.Winsock.closesocket(handle);
#if DEBUG
                        _closeSocketHandle = handle;
                        _closeSocketResult = errorCode;
#endif
                        if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();
                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, closesocket#2():{errorCode}");

                        // If it's not WSAEWOULDBLOCK, there's no more recourse - we either succeeded or failed.
                        if (errorCode != SocketError.WouldBlock)
                        {
                            return errorCode;
                        }
                    }

                    // It failed.  Fall through to the regular abortive close.
                }

                // By default or if CloseAsIs() path failed, set linger timeout to zero to get an abortive close (RST).
                Interop.Windows.Winsock.Linger lingerStruct;
                lingerStruct.OnOff = 1;
                lingerStruct.Time = 0;

                errorCode = Interop.Windows.Winsock.setsockopt(
                    handle,
                    SocketOptionLevel.Socket,
                    SocketOptionName.Linger,
                    ref lingerStruct,
                    4);
#if DEBUG
                _closeSocketLinger = errorCode;
#endif
                if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, setsockopt():{errorCode}");

                if (errorCode != SocketError.Success && errorCode != SocketError.InvalidArgument && errorCode != SocketError.ProtocolOption)
                {
                    // Too dangerous to try closesocket() - it might block!
                    return errorCode;
                }

                errorCode = Interop.Windows.Winsock.closesocket(handle);
#if DEBUG
                _closeSocketHandle = handle;
                _closeSocketResult = errorCode;
#endif
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"handle:{handle}, closesocket#3():{(errorCode == SocketError.SocketError ? (SocketError)Marshal.GetLastWin32Error() : errorCode)}");

                return errorCode;
            }

            internal static partial class Windows
            {
            internal static unsafe InnerSafeCloseSocket CreateWSASocket(byte* pinnedBuffer)
            {
                // NOTE: -1 is the value for FROM_PROTOCOL_INFO.
                InnerSafeCloseSocket result = Interop.Windows.Winsock.WSASocketW((AddressFamily)(-1), (SocketType)(-1), (ProtocolType)(-1), pinnedBuffer, 0, Interop.Windows.Winsock.SocketConstructorFlags.WSA_FLAG_OVERLAPPED);
                if (result.IsInvalid)
                {
                    result.SetHandleAsInvalid();
                }
                return result;
            }

            internal static InnerSafeCloseSocket CreateWSASocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
            {
                InnerSafeCloseSocket result = Interop.Windows.Winsock.WSASocketW(addressFamily, socketType, protocolType, IntPtr.Zero, 0, Interop.Windows.Winsock.SocketConstructorFlags.WSA_FLAG_OVERLAPPED);
                if (result.IsInvalid)
                {
                    result.SetHandleAsInvalid();
                }
                return result;
            }

            internal static InnerSafeCloseSocket Accept(SafeCloseSocket socketHandle, byte[] socketAddress, ref int socketAddressSize)
            {
                InnerSafeCloseSocket result = Interop.Windows.Winsock.accept(socketHandle.DangerousGetHandle(), socketAddress, ref socketAddressSize);
                if (result.IsInvalid)
                {
                    result.SetHandleAsInvalid();
                }
                return result;
            }
            }
        }
    }
}
