// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

#include "pal_compiler.h"

BEGIN_EXTERN_C

#include "pal_types.h"

/*
Shims CCRandomGenerateBytes, putting the resulting CCRNGStatus value in pkCCStatus.

Returns 1 on success, 0 on system error (see pkCCStatus), -1 on input error.
*/
DLLEXPORT void SystemNative_GetNonCryptographicallySecureRandomBytes(uint8_t* buffer, int32_t bufferLength);

END_EXTERN_C
