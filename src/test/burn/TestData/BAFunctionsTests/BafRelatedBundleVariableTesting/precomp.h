#pragma once
// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.


#include <windows.h>

#pragma warning(push)
#pragma warning(disable:4458) // declaration of 'xxx' hides class member
#include <gdiplus.h>
#pragma warning(pop)

#include <msiquery.h>
#include <objbase.h>
#include <shlobj.h>
#include <shlwapi.h>
#include <stdlib.h>
#include <strsafe.h>
#include <CommCtrl.h>
#include <sddl.h>

#include "dutil.h"
#include "dictutil.h"
#include "fileutil.h"
#include "locutil.h"
#include "memutil.h"
#include "pathutil.h"
#include "procutil.h"
#include "strutil.h"
#include "thmutil.h"
#include "regutil.h"
#include "xmlutil.h"

#include "BalBaseBootstrapperApplication.h"
#include "balutil.h"

#include "BAFunctions.h"
#include "IBAFunctions.h"

HRESULT WINAPI CreateBAFunctions(
    __in HMODULE hModule,
    __in const BA_FUNCTIONS_CREATE_ARGS* pArgs,
    __inout BA_FUNCTIONS_CREATE_RESULTS* pResults
    );
