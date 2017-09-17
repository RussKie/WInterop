﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// #define WIN32FIND

using System;
using System.Collections.Generic;
using System.Text;
using WInterop.FileManagement.Types;
using WInterop.Support;

namespace WInterop.FileManagement
{
    public interface IDirectFindTransform<T>
    {
#if WIN32FIND
        unsafe T TransformResult(ref WIN32_FIND_DATA record, string directory);
#else
        unsafe T TransformResult(FILE_FULL_DIR_INFORMATION* record, string directory);
#endif
    }

    public static class DirectFindTransforms
    {
        public class ToFullPath : IDirectFindTransform<string>
        {
            private ToFullPath() { }
            public static ToFullPath Instance = new ToFullPath();

#if WIN32FIND
            public unsafe string TransformResult(ref WIN32_FIND_DATA record, string directory)
            {
                fixed (void* v = &record.cFileName)
                {
                    char* c = (char*)v;
                    int count = 0;
                    while (*c != '\0') { c++; count++; }

                    int totalLength = count + directory.Length + 1;
                    string fullPath = new string('\0', totalLength);
                    fixed (char* f = fullPath)
                    fixed (char* d = directory)
                    {
                        Buffer.MemoryCopy(d, f, fullPath.Length * sizeof(char), directory.Length * sizeof(char));
                        f[directory.Length] = Paths.DirectorySeparator;
                        Buffer.MemoryCopy(v, f + directory.Length + 1, fullPath.Length * sizeof(char), count * sizeof(char));
                    }
                    return fullPath;
                }
            }
#else
            public unsafe string TransformResult(FILE_FULL_DIR_INFORMATION* record, string directory)
            {
                int totalLength = (int)(record->FileNameLength / sizeof(char)) + directory.Length + 1;
                string fullPath = new string('\0', totalLength);
                fixed (char* f = fullPath)
                fixed (char* d = directory)
                {
                    Buffer.MemoryCopy(d, f, fullPath.Length * sizeof(char), directory.Length * sizeof(char));
                    f[directory.Length] = Paths.DirectorySeparator;
                    Buffer.MemoryCopy((void*)&record->FileName, f + directory.Length + 1, fullPath.Length * sizeof(char), record->FileNameLength);
                }
                return fullPath;
            }
#endif
        }

        public class ToFindResult : IDirectFindTransform<FindResult>
        {
            private ToFindResult() { }
            public static ToFindResult Instance = new ToFindResult();

#if WIN32FIND
            public unsafe FindResult TransformResult(ref WIN32_FIND_DATA record, string directory) => new FindResult(ref record, directory);
#else
            public unsafe FindResult TransformResult(FILE_FULL_DIR_INFORMATION* record, string directory) => new FindResult(record, directory);
#endif
        }
    }
}