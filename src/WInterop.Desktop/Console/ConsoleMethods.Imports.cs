﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using WInterop.Console.Types;

namespace WInterop.Desktop.Console
{
    public static partial class ConsoleMethods
    {
        /// <summary>
        /// Direct usage of Imports isn't recommended. Use the wrappers that do the heavy lifting for you.
        /// </summary>
        public static partial class Imports
        {
            // https://docs.microsoft.com/en-us/windows/console/attachconsole
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool AttachConsole(
                uint dwProcessId);

            // https://docs.microsoft.com/en-us/windows/console/freeconsole
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool FreeConsole();

            // https://docs.microsoft.com/en-us/windows/console/allocconsole
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool AllocConsole();

            // https://docs.microsoft.com/en-us/windows/console/getstdhandle
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr GetStdHandle(
                StandardHandleType nStdHandle);

            // https://docs.microsoft.com/en-us/windows/console/setstdhandle
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool SetStdHandle(
                StandardHandleType nStdHandle,
                SafeHandle hHandle);

            // https://docs.microsoft.com/en-us/windows/console/getconsolemode
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool GetConsoleMode(
                SafeHandle hConsoleHandle,
                out uint lpMode);

            // https://docs.microsoft.com/en-us/windows/console/setconsolemode
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool SetConsoleMode(
                SafeHandle hConsoleHandle,
                uint lpMode);

            // https://docs.microsoft.com/en-us/windows/console/getconsolecp
            [DllImport(Libraries.Kernel32, ExactSpelling = true)]
            public static extern uint GetConsoleCP();

            // https://docs.microsoft.com/en-us/windows/console/getconsoleoutputcp
            [DllImport(Libraries.Kernel32, ExactSpelling = true)]
            public static extern uint GetConsoleOutputCP();

            // https://docs.microsoft.com/en-us/windows/console/peekconsoleinput
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool PeekConsoleInputW(
                SafeHandle hConsoleInput,
                ref INPUT_RECORD lpBuffer,
                uint nLength,
                out uint lpNumberOfEventsRead);

            // https://docs.microsoft.com/en-us/windows/console/readconsoleinput
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool ReadConsoleInputW(
                SafeHandle hConsoleInput,
                ref INPUT_RECORD lpBuffer,
                uint nLength,
                out uint lpNumberOfEventsRead);

            // https://docs.microsoft.com/en-us/windows/console/getnumberofconsoleinputevents
            [DllImport(Libraries.Kernel32, SetLastError = true, ExactSpelling = true)]
            public static extern bool GetNumberOfConsoleInputEvents(
                SafeHandle hConsoleInput,
                out uint lpcNumberOfEvents);
        }
    }
}
