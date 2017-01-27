// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Net
{
    partial class ContextAwareResult
    {
        private void SafeCaptureIdentity()
        {
            // FIXME add support for UAP
            if (Environment.IsRunningOnWindows)
                Windows_SafeCaptureIdentity();
            else
                Unix_SafeCaptureIdentity();
        }

#if false
        internal WindowsIdentity Identity
        {
            get {
                if (Environment.IsRunningOnWindows)
                    Windows_Identity;
                else
                    throw new PlatformNotSupportedException();
            }
        }
#endif

        private void CleanupInternal()
        {
            // FIXME add support for UAP
            if (Environment.IsRunningOnWindows)
                Windows_CleanupInternal();
            else
                Unix_CleanupInternal();
        }
    }
}
