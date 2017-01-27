// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    internal static partial class SocketPal
    {
        // The API that uses this information is not supported on *nix, and will throw
        // PlatformNotSupportedException instead.
        public static int ProtocolInformationSize
        {
            get {
                if (Environment.IsRunningOnWindows)
                    return Windows.ProtocolInformationSize;
                else
                    return Unix.ProtocolInformationSize;
            }
        }

        public static bool SupportsMultipleConnectAttempts
        {
            get {
                if (Environment.IsRunningOnWindows)
                    return Windows.SupportsMultipleConnectAttempts;
                else
                    return Unix.SupportsMultipleConnectAttempts;
            }
        }

        public static void Initialize()
        {
            if (Environment.IsRunningOnWindows)
                Windows.Initialize ();
            else
                Unix.Initialize ();
        }

        public static void CheckDualModeReceiveSupport(Socket socket)
        {
            if (Environment.IsRunningOnWindows)
                Windows.CheckDualModeReceiveSupport (socket);
            else
                Unix.CheckDualModeReceiveSupport (socket);
        }

        public static SocketError CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, out SafeCloseSocket socket)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.CreateSocket (addressFamily, socketType, protocolType, out socket);
            else
                return Unix.CreateSocket (addressFamily, socketType, protocolType, out socket);
        }

        public static SocketError SetBlocking(SafeCloseSocket handle, bool shouldBlock, out bool willBlock)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SetBlocking (handle, shouldBlock, out willBlock);
            else
                return Unix.SetBlocking (handle, shouldBlock, out willBlock);
        }

        public static unsafe SocketError GetSockName(SafeCloseSocket handle, byte[] buffer, ref int nameLen)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetSockName (handle, buffer, ref nameLen);
            else
                return Unix.GetSockName (handle, buffer, ref nameLen);
        }

        public static unsafe SocketError GetAvailable(SafeCloseSocket handle, out int available)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetAvailable(handle, out available);
            else
                return Unix.GetAvailable(handle, out available);
        }

        public static unsafe SocketError GetPeerName(SafeCloseSocket handle, byte[] buffer, ref int nameLen)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetPeerName(handle, buffer, ref nameLen);
            else
                return Unix.GetPeerName(handle, buffer, ref nameLen);
        }

        public static unsafe SocketError Bind(SafeCloseSocket handle, byte[] buffer, int nameLen)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Bind(handle, buffer, nameLen);
            else
                return Unix.Bind(handle, buffer, nameLen);
        }

        public static SocketError Listen(SafeCloseSocket handle, int backlog)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Listen(handle, backlog);
            else
                return Unix.Listen(handle, backlog);
        }

        public static SocketError Accept(SafeCloseSocket handle, byte[] buffer, ref int nameLen, out SafeCloseSocket socket)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Accept(handle, buffer, ref nameLen, out socket);
            else
                return Unix.Accept(handle, buffer, ref nameLen, out socket);
        }

        public static SocketError Connect(SafeCloseSocket handle, byte[] socketAddress, int socketAddressLen)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Connect(handle, socketAddress, socketAddressLen);
            else
                return Unix.Connect(handle, socketAddress, socketAddressLen);
        }

        public static SocketError Send(SafeCloseSocket handle, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out int bytesTransferred)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Send(handle, buffers, socketFlags, out bytesTransferred);
            else
                return Unix.Send(handle, buffers, socketFlags, out bytesTransferred);
        }

        public static SocketError Send(SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, out int bytesTransferred)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Send(handle, buffer, offset, count, socketFlags, out bytesTransferred);
            else
                return Unix.Send(handle, buffer, offset, count, socketFlags, out bytesTransferred);
        }

        public static SocketError SendFile(SafeCloseSocket handle, FileStream fileStream)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SendFile(handle, fileStream);
            else
                return Unix.SendFile(handle, fileStream);
        }

        public static SocketError SendTo(SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, byte[] socketAddress, int socketAddressLen, out int bytesTransferred)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SendTo(handle, buffer, offset, count, socketFlags, socketAddress, socketAddressLen, out bytesTransferred);
            else
                return Unix.SendTo(handle, buffer, offset, count, socketFlags, socketAddress, socketAddressLen, out bytesTransferred);
        }

        public static SocketError Receive(SafeCloseSocket handle, IList<ArraySegment<byte>> buffers, ref SocketFlags socketFlags, out int bytesTransferred)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Receive(handle, buffers, ref socketFlags, out bytesTransferred);
            else
                return Unix.Receive(handle, buffers, ref socketFlags, out bytesTransferred);
        }

        public static SocketError Receive(SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, out int bytesTransferred)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Receive(handle, buffer, offset, count, socketFlags, out bytesTransferred);
            else
                return Unix.Receive(handle, buffer, offset, count, socketFlags, out bytesTransferred);
        }

        public static SocketError ReceiveMessageFrom(Socket socket, SafeCloseSocket handle, byte[] buffer, int offset, int count, ref SocketFlags socketFlags, Internals.SocketAddress socketAddress, out Internals.SocketAddress receiveAddress, out IPPacketInformation ipPacketInformation, out int bytesTransferred)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.ReceiveMessageFrom(socket, handle, buffer, offset, count, ref socketFlags, socketAddress, out receiveAddress, out ipPacketInformation, out bytesTransferred);
            else
                return Unix.ReceiveMessageFrom(socket, handle, buffer, offset, count, ref socketFlags, socketAddress, out receiveAddress, out ipPacketInformation, out bytesTransferred);
        }

        public static SocketError ReceiveFrom(SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, byte[] socketAddress, ref int socketAddressLen, out int bytesTransferred)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.ReceiveFrom(handle, buffer, offset, count, socketFlags, socketAddress, ref socketAddressLen, out bytesTransferred);
            else
                return Unix.ReceiveFrom(handle, buffer, offset, count, socketFlags, socketAddress, ref socketAddressLen, out bytesTransferred);
        }

        public static SocketError WindowsIoctl(SafeCloseSocket handle, int ioControlCode, byte[] optionInValue, byte[] optionOutValue, out int optionLength)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.WindowsIoctl(handle, ioControlCode, optionInValue, optionOutValue, out optionLength);
            else
                return Unix.WindowsIoctl(handle, ioControlCode, optionInValue, optionOutValue, out optionLength);
        }

        public static unsafe SocketError SetSockOpt(SafeCloseSocket handle, SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SetSockOpt(handle, optionLevel, optionName, optionValue);
            else
                return Unix.SetSockOpt(handle, optionLevel, optionName, optionValue);
        }

        public static unsafe SocketError SetSockOpt(SafeCloseSocket handle, SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SetSockOpt(handle, optionLevel, optionName, optionValue);
            else
                return Unix.SetSockOpt(handle, optionLevel, optionName, optionValue);
        }

        public static unsafe SocketError SetMulticastOption(SafeCloseSocket handle, SocketOptionName optionName, MulticastOption optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SetMulticastOption(handle, optionName, optionValue);
            else
                return Unix.SetMulticastOption(handle, optionName, optionValue);
        }

        public static unsafe SocketError SetIPv6MulticastOption(SafeCloseSocket handle, SocketOptionName optionName, IPv6MulticastOption optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SetIPv6MulticastOption(handle, optionName, optionValue);
            else
                return Unix.SetIPv6MulticastOption(handle, optionName, optionValue);
        }

        public static unsafe SocketError SetLingerOption(SafeCloseSocket handle, LingerOption optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SetLingerOption(handle, optionValue);
            else
                return Unix.SetLingerOption(handle, optionValue);
        }

        public static void SetReceivingDualModeIPv4PacketInformation(Socket socket)
        {
            if (Environment.IsRunningOnWindows)
                Windows.SetReceivingDualModeIPv4PacketInformation(socket);
            else
                Unix.SetReceivingDualModeIPv4PacketInformation(socket);
        }

        public static void SetIPProtectionLevel(Socket socket, SocketOptionLevel optionLevel, int protectionLevel)
        {
            if (Environment.IsRunningOnWindows)
                Windows.SetIPProtectionLevel(socket, optionLevel, protectionLevel);
            else
                Unix.SetIPProtectionLevel(socket, optionLevel, protectionLevel);
        }

        public static unsafe SocketError GetSockOpt(SafeCloseSocket handle, SocketOptionLevel optionLevel, SocketOptionName optionName, out int optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetSockOpt(handle, optionLevel, optionName, out optionValue);
            else
                return Unix.GetSockOpt(handle, optionLevel, optionName, out optionValue);
        }

        public static unsafe SocketError GetSockOpt(SafeCloseSocket handle, SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue, ref int optionLength)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetSockOpt(handle, optionLevel, optionName, optionValue, ref optionLength);
            else
                return Unix.GetSockOpt(handle, optionLevel, optionName, optionValue, ref optionLength);
        }

        public static unsafe SocketError GetMulticastOption(SafeCloseSocket handle, SocketOptionName optionName, out MulticastOption optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetMulticastOption(handle, optionName, out optionValue);
            else
                return Unix.GetMulticastOption(handle, optionName, out optionValue);
        }

        public static unsafe SocketError GetIPv6MulticastOption(SafeCloseSocket handle, SocketOptionName optionName, out IPv6MulticastOption optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetIPv6MulticastOption(handle, optionName, out optionValue);
            else
                return Unix.GetIPv6MulticastOption(handle, optionName, out optionValue);
        }

        public static unsafe SocketError GetLingerOption(SafeCloseSocket handle, out LingerOption optionValue)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.GetLingerOption(handle, out optionValue);
            else
                return Unix.GetLingerOption(handle, out optionValue);
        }

        public static unsafe SocketError Poll(SafeCloseSocket handle, int microseconds, SelectMode mode, out bool status)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Poll(handle, microseconds, mode, out status);
            else
                return Unix.Poll(handle, microseconds, mode, out status);
        }

        public static unsafe SocketError Select(IList checkRead, IList checkWrite, IList checkError, int microseconds)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Select(checkRead, checkWrite, checkError, microseconds);
            else
                return Unix.Select(checkRead, checkWrite, checkError, microseconds);
        }

        public static SocketError Shutdown(SafeCloseSocket handle, bool isConnected, bool isDisconnected, SocketShutdown how)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Shutdown(handle, isConnected, isDisconnected, how);
            else
                return Unix.Shutdown(handle, isConnected, isDisconnected, how);
        }

        public static SocketError ConnectAsync(Socket socket, SafeCloseSocket handle, byte[] socketAddress, int socketAddressLen, ConnectOverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.ConnectAsync(socket, handle, socketAddress, socketAddressLen, asyncResult);
            else
                return Unix.ConnectAsync(socket, handle, socketAddress, socketAddressLen, asyncResult);
        }

        public static SocketError SendAsync(SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SendAsync(handle, buffer, offset, count, socketFlags, asyncResult);
            else
                return Unix.SendAsync(handle, buffer, offset, count, socketFlags, asyncResult);
        }

        public static SocketError SendAsync(SafeCloseSocket handle, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SendAsync(handle, buffers, socketFlags, asyncResult);
            else
                return Unix.SendAsync(handle, buffers, socketFlags, asyncResult);
        }

        public static SocketError SendFileAsync(SafeCloseSocket handle, FileStream fileStream, Action<long, SocketError> callback)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SendFileAsync(handle, fileStream, callback);
            else
                return Unix.SendFileAsync(handle, fileStream, callback);
        }

        public static SocketError SendToAsync(SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, Internals.SocketAddress socketAddress, OverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.SendToAsync(handle, buffer, offset, count, socketFlags, socketAddress, asyncResult);
            else
                return Unix.SendToAsync(handle, buffer, offset, count, socketFlags, socketAddress, asyncResult);
        }

        public static SocketError ReceiveAsync(SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.ReceiveAsync(handle, buffer, offset, count, socketFlags, asyncResult);
            else
                return Unix.ReceiveAsync(handle, buffer, offset, count, socketFlags, asyncResult);
        }

        public static SocketError ReceiveAsync(SafeCloseSocket handle, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, OverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.ReceiveAsync(handle, buffers, socketFlags, asyncResult);
            else
                return Unix.ReceiveAsync(handle, buffers, socketFlags, asyncResult);
        }

        public static SocketError ReceiveFromAsync(SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, Internals.SocketAddress socketAddress, OverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.ReceiveFromAsync(handle, buffer, offset, count, socketFlags, socketAddress, asyncResult);
            else
                return Unix.ReceiveFromAsync(handle, buffer, offset, count, socketFlags, socketAddress, asyncResult);
        }

        public static SocketError ReceiveMessageFromAsync(Socket socket, SafeCloseSocket handle, byte[] buffer, int offset, int count, SocketFlags socketFlags, Internals.SocketAddress socketAddress, ReceiveMessageOverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.ReceiveMessageFromAsync(socket, handle, buffer, offset, count, socketFlags, socketAddress, asyncResult);
            else
                return Unix.ReceiveMessageFromAsync(socket, handle, buffer, offset, count, socketFlags, socketAddress, asyncResult);
        }

        public static SocketError AcceptAsync(Socket socket, SafeCloseSocket handle, SafeCloseSocket acceptHandle, int receiveSize, int socketAddressSize, AcceptOverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.AcceptAsync(socket, handle, acceptHandle, receiveSize, socketAddressSize, asyncResult);
            else
                return Unix.AcceptAsync(socket, handle, acceptHandle, receiveSize, socketAddressSize, asyncResult);
        }

        internal static SocketError DisconnectAsync(Socket socket, SafeCloseSocket handle, bool reuseSocket, DisconnectOverlappedAsyncResult asyncResult)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.DisconnectAsync(socket, handle, reuseSocket, asyncResult);
            else
                return Unix.DisconnectAsync(socket, handle, reuseSocket, asyncResult);
        }

        internal static SocketError Disconnect(Socket socket, SafeCloseSocket handle, bool reuseSocket)
        {
            if (Environment.IsRunningOnWindows)
                return Windows.Disconnect(socket, handle, reuseSocket);
            else
                return Unix.Disconnect(socket, handle, reuseSocket);
        }
    }
}
