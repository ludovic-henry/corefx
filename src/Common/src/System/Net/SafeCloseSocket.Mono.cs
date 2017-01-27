
using System;
using System.Threading;

namespace System.Net.Sockets
{
    internal partial class SafeCloseSocket
    {
        public ThreadPoolBoundHandle IOCPBoundHandle
        {
            get
            {
                if (Environment.IsRunningOnWindows)
                    return Windows_IOCPBoundHandle;
                else
                    throw new PlatformNotSupportedException();
            }
        }

        public ThreadPoolBoundHandle GetOrAllocateThreadPoolBoundHandle()
        {
            if (Environment.IsRunningOnWindows)
                return Windows_GetOrAllocateThreadPoolBoundHandle();
            else
                throw new PlatformNotSupportedException();
        }

        private void InnerReleaseHandle()
        {
            if (Environment.IsRunningOnWindows)
                Windows_InnerReleaseHandle();
            else
                Unix_InnerReleaseHandle();
        }

        internal sealed partial class InnerSafeCloseSocket
        {
            private unsafe SocketError InnerReleaseHandle()
            {
                if (Environment.IsRunningOnWindows)
                    return Windows_InnerReleaseHandle();
                else
                    return Unix_InnerReleaseHandle();
            }
        }
    }
}