// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Net.Sockets
{
    internal static class SocketErrorPal
    {
        internal static partial class Unix
        {
#if DEBUG
            static SocketErrorPal()
            {
                Debug.Assert(s_nativeErrorToSocketError.Count == NativeErrorToSocketErrorCount,
                    $"Expected s_nativeErrorToSocketError to have {NativeErrorToSocketErrorCount} count instead of {s_nativeErrorToSocketError.Count}.");

                Debug.Assert(s_socketErrorToNativeError.Count == SocketErrorToNativeErrorCount,
                    $"Expected s_socketErrorToNativeError to have {SocketErrorToNativeErrorCount} count instead of {s_socketErrorToNativeError.Count}.");
            }
#endif

            private const int NativeErrorToSocketErrorCount = 41;
            private const int SocketErrorToNativeErrorCount = 40;

            // No Interop.Errors are included for the following SocketErrors, as there's no good mapping:
            // - SocketError.NoRecovery
            // - SocketError.NotInitialized
            // - SocketError.ProcessLimit
            // - SocketError.SocketError
            // - SocketError.SystemNotReady
            // - SocketError.TypeNotFound
            // - SocketError.VersionNotSupported

            private static readonly Dictionary<Interop.Unix.Error, SocketError> s_nativeErrorToSocketError = new Dictionary<Interop.Unix.Error, SocketError>(NativeErrorToSocketErrorCount)
            {
                { Interop.Unix.Error.EACCES, SocketError.AccessDenied },
                { Interop.Unix.Error.EADDRINUSE, SocketError.AddressAlreadyInUse },
                { Interop.Unix.Error.EADDRNOTAVAIL, SocketError.AddressNotAvailable },
                { Interop.Unix.Error.EAFNOSUPPORT, SocketError.AddressFamilyNotSupported },
                { Interop.Unix.Error.EAGAIN, SocketError.WouldBlock },
                { Interop.Unix.Error.EALREADY, SocketError.AlreadyInProgress },
                { Interop.Unix.Error.EBADF, SocketError.InvalidArgument },
                { Interop.Unix.Error.ECANCELED, SocketError.OperationAborted },
                { Interop.Unix.Error.ECONNABORTED, SocketError.ConnectionAborted },
                { Interop.Unix.Error.ECONNREFUSED, SocketError.ConnectionRefused },
                { Interop.Unix.Error.ECONNRESET, SocketError.ConnectionReset },
                { Interop.Unix.Error.EDESTADDRREQ, SocketError.DestinationAddressRequired },
                { Interop.Unix.Error.EFAULT, SocketError.Fault },
                { Interop.Unix.Error.EHOSTDOWN, SocketError.HostDown },
                { Interop.Unix.Error.ENXIO, SocketError.HostNotFound }, // not perfect, but closest match available
                { Interop.Unix.Error.EHOSTUNREACH, SocketError.HostUnreachable },
                { Interop.Unix.Error.EINPROGRESS, SocketError.InProgress },
                { Interop.Unix.Error.EINTR, SocketError.Interrupted },
                { Interop.Unix.Error.EINVAL, SocketError.InvalidArgument },
                { Interop.Unix.Error.EISCONN, SocketError.IsConnected },
                { Interop.Unix.Error.EMFILE, SocketError.TooManyOpenSockets },
                { Interop.Unix.Error.EMSGSIZE, SocketError.MessageSize },
                { Interop.Unix.Error.ENETDOWN, SocketError.NetworkDown },
                { Interop.Unix.Error.ENETRESET, SocketError.NetworkReset },
                { Interop.Unix.Error.ENETUNREACH, SocketError.NetworkUnreachable },
                { Interop.Unix.Error.ENFILE, SocketError.TooManyOpenSockets },
                { Interop.Unix.Error.ENOBUFS, SocketError.NoBufferSpaceAvailable },
                { Interop.Unix.Error.ENODATA, SocketError.NoData },
                { Interop.Unix.Error.ENOENT, SocketError.AddressNotAvailable },
                { Interop.Unix.Error.ENOPROTOOPT, SocketError.ProtocolOption },
                { Interop.Unix.Error.ENOTCONN, SocketError.NotConnected },
                { Interop.Unix.Error.ENOTSOCK, SocketError.NotSocket },
                { Interop.Unix.Error.ENOTSUP, SocketError.OperationNotSupported },
                { Interop.Unix.Error.EPIPE, SocketError.Shutdown },
                { Interop.Unix.Error.EPFNOSUPPORT, SocketError.ProtocolFamilyNotSupported },
                { Interop.Unix.Error.EPROTONOSUPPORT, SocketError.ProtocolNotSupported },
                { Interop.Unix.Error.EPROTOTYPE, SocketError.ProtocolType },
                { Interop.Unix.Error.ESOCKTNOSUPPORT, SocketError.SocketNotSupported },
                { Interop.Unix.Error.ESHUTDOWN, SocketError.Disconnecting },
                { Interop.Unix.Error.SUCCESS, SocketError.Success },
                { Interop.Unix.Error.ETIMEDOUT, SocketError.TimedOut },
            };

            private static readonly Dictionary<SocketError, Interop.Unix.Error> s_socketErrorToNativeError = new Dictionary<SocketError, Interop.Unix.Error>(SocketErrorToNativeErrorCount)
            {
                // This is *mostly* an inverse mapping of s_nativeErrorToSocketError.  However, some options have multiple mappings and thus
                // can't be inverted directly.  Other options don't have a mapping from native to SocketError, but when presented with a SocketError,
                // we want to provide the closest relevant Error possible, e.g. EINPROGRESS maps to SocketError.InProgress, and vice versa, but 
                // SocketError.IOPending also maps closest to EINPROGRESS.  As such, roundtripping won't necessarily provide the original value 100% of the time,
                // but it's the best we can do given the mismatch between Interop.Unix.Error and SocketError.

                { SocketError.AccessDenied, Interop.Unix.Error.EACCES},
                { SocketError.AddressAlreadyInUse, Interop.Unix.Error.EADDRINUSE  },
                { SocketError.AddressNotAvailable, Interop.Unix.Error.EADDRNOTAVAIL },
                { SocketError.AddressFamilyNotSupported, Interop.Unix.Error.EAFNOSUPPORT  },
                { SocketError.AlreadyInProgress, Interop.Unix.Error.EALREADY },
                { SocketError.ConnectionAborted, Interop.Unix.Error.ECONNABORTED },
                { SocketError.ConnectionRefused, Interop.Unix.Error.ECONNREFUSED },
                { SocketError.ConnectionReset, Interop.Unix.Error.ECONNRESET },
                { SocketError.DestinationAddressRequired, Interop.Unix.Error.EDESTADDRREQ },
                { SocketError.Disconnecting, Interop.Unix.Error.ESHUTDOWN },
                { SocketError.Fault, Interop.Unix.Error.EFAULT },
                { SocketError.HostDown, Interop.Unix.Error.EHOSTDOWN },
                { SocketError.HostNotFound, Interop.Unix.Error.ENXIO }, // not perfect, but closest match available
                { SocketError.HostUnreachable, Interop.Unix.Error.EHOSTUNREACH },
                { SocketError.InProgress, Interop.Unix.Error.EINPROGRESS },
                { SocketError.Interrupted, Interop.Unix.Error.EINTR },
                { SocketError.InvalidArgument, Interop.Unix.Error.EINVAL }, // could also have been EBADF, though that's logically an invalid argument
                { SocketError.IOPending, Interop.Unix.Error.EINPROGRESS },
                { SocketError.IsConnected, Interop.Unix.Error.EISCONN },
                { SocketError.MessageSize, Interop.Unix.Error.EMSGSIZE },
                { SocketError.NetworkDown, Interop.Unix.Error.ENETDOWN },
                { SocketError.NetworkReset, Interop.Unix.Error.ENETRESET },
                { SocketError.NetworkUnreachable, Interop.Unix.Error.ENETUNREACH },
                { SocketError.NoBufferSpaceAvailable, Interop.Unix.Error.ENOBUFS },
                { SocketError.NoData, Interop.Unix.Error.ENODATA },
                { SocketError.NotConnected, Interop.Unix.Error.ENOTCONN },
                { SocketError.NotSocket, Interop.Unix.Error.ENOTSOCK },
                { SocketError.OperationAborted, Interop.Unix.Error.ECANCELED },
                { SocketError.OperationNotSupported, Interop.Unix.Error.ENOTSUP },
                { SocketError.ProtocolFamilyNotSupported, Interop.Unix.Error.EPFNOSUPPORT },
                { SocketError.ProtocolNotSupported, Interop.Unix.Error.EPROTONOSUPPORT },
                { SocketError.ProtocolOption, Interop.Unix.Error.ENOPROTOOPT },
                { SocketError.ProtocolType, Interop.Unix.Error.EPROTOTYPE },
                { SocketError.Shutdown, Interop.Unix.Error.EPIPE },
                { SocketError.SocketNotSupported, Interop.Unix.Error.ESOCKTNOSUPPORT },
                { SocketError.Success, Interop.Unix.Error.SUCCESS },
                { SocketError.TimedOut, Interop.Unix.Error.ETIMEDOUT },
                { SocketError.TooManyOpenSockets, Interop.Unix.Error.ENFILE }, // could also have been EMFILE
                { SocketError.TryAgain, Interop.Unix.Error.EAGAIN }, // not a perfect mapping, but better than nothing
                { SocketError.WouldBlock, Interop.Unix.Error.EAGAIN  },
            };

            internal static SocketError GetSocketErrorForNativeError(Interop.Unix.Error errno)
            {
                SocketError result;
                return s_nativeErrorToSocketError.TryGetValue(errno, out result) ? 
                    result : 
                    SocketError.SocketError; // unknown native error, just treat it as a generic SocketError
            }

            internal static Interop.Unix.Error GetNativeErrorForSocketError(SocketError error)
            {
                Interop.Unix.Error errno;
                return s_socketErrorToNativeError.TryGetValue(error, out errno) ?
                    errno :
                    (Interop.Unix.Error)(int)error; // pass through the SocketError's value, as it at least retains some useful info
            }
        }
    }
}
