// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    public partial class Socket
    {
        private Socket GetOrCreateAcceptSocket(Socket acceptSocket, bool checkDisconnected, string propertyName, out SafeCloseSocket handle)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_GetOrCreateAcceptSocket(acceptSocket, checkDisconnected, propertyName, out handle);
            else
                return Unix_GetOrCreateAcceptSocket(acceptSocket, checkDisconnected, propertyName, out handle);
        }

        private void SendFileInternal(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
        {
            if (Environment.IsRunningOnWindows)
                Windows_SendFileInternal(fileName, preBuffer, postBuffer, flags);
            else
                Unix_SendFileInternal(fileName, preBuffer, postBuffer, flags);
        }

        private IAsyncResult BeginSendFileInternal(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags, AsyncCallback callback, object state)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_BeginSendFileInternal(fileName, preBuffer, postBuffer, flags, callback, state);
            else
                return Unix_BeginSendFileInternal(fileName, preBuffer, postBuffer, flags, callback, state);
        }

        private void EndSendFileInternal(IAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                Windows_EndSendFileInternal(asyncResult);
            else
                Unix_EndSendFileInternal(asyncResult);
        }

        internal bool AcceptEx(SafeCloseSocket listenSocketHandle, SafeCloseSocket acceptSocketHandle, IntPtr buffer, int len, int localAddressLength, int remoteAddressLength, out int bytesReceived, SafeHandle overlapped)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_AcceptEx(listenSocketHandle, acceptSocketHandle, buffer, len, localAddressLength, remoteAddressLength, out bytesReceived, overlapped);
            else
                throw new PlatformNotSupportedException ();
        }

        internal void GetAcceptExSockaddrs(IntPtr buffer, int receiveDataLength, int localAddressLength, int remoteAddressLength, out IntPtr localSocketAddress, out int localSocketAddressLength, out IntPtr remoteSocketAddress, out int remoteSocketAddressLength)
        {
            if (Environment.IsRunningOnWindows)
                Windows_GetAcceptExSockaddrs(buffer, receiveDataLength, localAddressLength, remoteAddressLength, out localSocketAddress, out localSocketAddressLength, out remoteSocketAddress, out remoteSocketAddressLength);
            else
                throw new PlatformNotSupportedException ();
        }

        internal bool DisconnectEx(SafeCloseSocket socketHandle, SafeHandle overlapped, int flags, int reserved)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_DisconnectEx(socketHandle, overlapped, flags, reserved);
            else
                throw new PlatformNotSupportedException ();
        }

        internal bool DisconnectExBlocking(SafeCloseSocket socketHandle, IntPtr overlapped, int flags, int reserved)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_DisconnectExBlocking(socketHandle, overlapped, flags, reserved);
            else
                throw new PlatformNotSupportedException ();
        }

        internal bool ConnectEx(SafeCloseSocket socketHandle, IntPtr socketAddress, int socketAddressSize, IntPtr buffer, int dataLength, out int bytesSent, SafeHandle overlapped)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_ConnectEx(socketHandle, socketAddress, socketAddressSize, buffer, dataLength, out bytesSent, overlapped);
            else
                throw new PlatformNotSupportedException ();
        }

        internal SocketError WSARecvMsg(SafeCloseSocket socketHandle, IntPtr msg, out int bytesTransferred, SafeHandle overlapped, IntPtr completionRoutine)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_WSARecvMsg(socketHandle, msg, out bytesTransferred, overlapped, completionRoutine);
            else
                throw new PlatformNotSupportedException ();
        }

        internal SocketError WSARecvMsgBlocking(IntPtr socketHandle, IntPtr msg, out int bytesTransferred, IntPtr overlapped, IntPtr completionRoutine)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_WSARecvMsgBlocking(socketHandle, msg, out bytesTransferred, overlapped, completionRoutine);
            else
                throw new PlatformNotSupportedException ();
        }

        internal bool TransmitPackets(SafeCloseSocket socketHandle, IntPtr packetArray, int elementCount, int sendSize, SafeNativeOverlapped overlapped, TransmitFileOptions flags)
        {
            if (Environment.IsRunningOnWindows)
                return Windows_TransmitPackets(socketHandle, packetArray, elementCount, sendSize, overlapped, flags);
            else
                throw new PlatformNotSupportedException ();
        }

        internal static IntPtr[] SocketListToFileDescriptorSet(IList socketList)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SocketListToFileDescriptorSet(socketList);
            else
                throw new PlatformNotSupportedException ();
        }

        internal static void SelectFileDescriptor(IList socketList, IntPtr[] fileDescriptorSet)
        {
            if (Environment.IsRunningOnWindows)
                Windows.SelectFileDescriptor(socketList, fileDescriptorSet);
            else
                throw new PlatformNotSupportedException ();
        }
    }
}
