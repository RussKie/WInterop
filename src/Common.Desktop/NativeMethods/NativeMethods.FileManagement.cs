﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WInterop
{
    using Authorization;
    using ErrorHandling;
    using FileManagement;
    using Handles;
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    public static partial class NativeMethods
    {
        public static partial class FileManagement
        {
            /// <summary>
            /// Direct P/Invokes aren't recommended. Use the wrappers that do the heavy lifting for you.
            /// </summary>
            /// <remarks>
            /// By keeping the names exactly as they are defined we can reduce string count and make the initial P/Invoke call slightly faster.
            /// </remarks>
#if DESKTOP
            [SuppressUnmanagedCodeSecurity] // We don't want a stack walk with every P/Invoke.
#endif
            public static partial class Direct
            {
                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858.aspx
                [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
                public static extern SafeFileHandle CreateFileW(
                    string lpFileName,
                    uint dwDesiredAccess,
                    [MarshalAs(UnmanagedType.U4)] System.IO.FileShare dwShareMode,
                    IntPtr lpSecurityAttributes,
                    [MarshalAs(UnmanagedType.U4)] System.IO.FileMode dwCreationDisposition,
                    uint dwFlagsAndAttributes,
                    IntPtr hTemplateFile);

                // Ex version is supported by WinRT apps
                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364944.aspx
                [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
                public static extern FileAttributes GetFileAttributesW(
                    string lpFileName);

                // Ex version is supported by WinRT apps
                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364952.aspx
                [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool GetFileInformationByHandle(
                    SafeFileHandle hFile,
                    out BY_HANDLE_FILE_INFORMATION lpFileInformation);

                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364980.aspx (kernel32)
                [DllImport(ApiSets.api_ms_win_core_file_l1_1_0, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
                public static extern uint GetLongPathNameW(
                    string lpszShortPath,
                    SafeHandle lpszLongPath,
                    uint cchBuffer);

                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364989.aspx (kernel32)
                [DllImport(ApiSets.api_ms_win_core_file_l1_1_0, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
                public static extern uint GetShortPathNameW(
                    string lpszLongPath,
                    SafeHandle lpszShortPath,
                    uint cchBuffer);

                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364962.aspx (kernel32)
                [DllImport(ApiSets.api_ms_win_core_file_l1_1_0, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
                public static extern uint GetFinalPathNameByHandleW(
                    SafeFileHandle hFile,
                    SafeHandle lpszFilePath,
                    uint cchFilePath,
                    GetFinalPathNameByHandleFlags dwFlags);

                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363852.aspx
                // CopyFile calls CopyFileEx with COPY_FILE_FAIL_IF_EXISTS if fail if exists is set
                // (Neither are available in WinRT- use CopyFile2)
                [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool CopyFileExW(
                    string lpExistingFileName,
                    string lpNewFileName,
                    CopyProgressRoutine lpProgressRoutine,
                    IntPtr lpData,
                    [MarshalAs(UnmanagedType.Bool)] ref bool pbCancel,
                    CopyFileExFlags dwCopyFlags);

                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363866.aspx
                // Note that CreateSymbolicLinkW returns a BOOLEAN (byte), not a BOOL (int)
                [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
                public static extern byte CreateSymbolicLinkW(
                    string lpSymlinkFileName,
                    string lpTargetFileName,
                    uint dwFlags);

                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa364021.aspx
                [DllImport(Libraries.Advapi32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool EncryptFileW(
                    string lpFileName);

                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363903.aspx
                [DllImport(Libraries.Advapi32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool DecryptFileW(
                    string lpFileName,
                    uint dwReserved);

                // Adding Users to an Encrypted File
                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363765.aspx
                //
                // 1. LookupAccountName() to get SID
                // 2. CertOpenSystemStore((HCRYPTPROV)NULL,L"TrustedPeople") to get cert store
                // 3. CertFindCertificateInStore() to find the desired cert (PCCERT_CONTEXT)
                //
                //   EFS_CERTIFICATE.cbTotalLength = Marshal.Sizeof(EFS_CERTIFICATE)
                //   EFS_CERTIFICATE.pUserSid = &SID
                //   EFS_CERTIFICATE.pCertBlob.dwCertEncodingType = CCERT_CONTEXT.dwCertEncodingType
                //   EFS_CERTIFICATE.pCertBlob.cbData = CCERT_CONTEXT.cbCertEncoded
                //   EFS_CERTIFICATE.pCertBlob.pbData = CCERT_CONTEXT.pbCertEncoded

                // https://msdn.microsoft.com/en-us/library/windows/desktop/aa363770.aspx
                //
                //  DWORD WINAPI AddUsersToEncryptedFile(
                //      _In_ LPCWSTR lpFileName,
                //      _In_ PENCRYPTION_CERTIFICATE_LIST pUsers
                //  );
                [DllImport(Libraries.Advapi32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
                internal static extern uint AddUsersToEncryptedFile(
                    string lpFileName,

                    /// <summary>
                    /// Pointer to ENCRYPTION_CERTIFICATE_LIST array
                    /// </summary>
                    IntPtr pUsers);
            }

            public static string GetLongPathName(string path)
            {
                return BufferInvoke((buffer) => Direct.GetLongPathNameW(path, buffer, buffer.CharCapacity));
            }

            public static string GetShortPathName(string path)
            {
                return BufferInvoke((buffer) => Direct.GetShortPathNameW(path, buffer, buffer.CharCapacity));
            }

#if !PORTABLE
            // We want to prefer the CreateFile2 version of this for portable as it will work with WinRT
            public static SafeFileHandle CreateFile(
                string path,
                System.IO.FileAccess fileAccess,
                System.IO.FileShare fileShare,
                System.IO.FileMode creationDisposition,
                FileAttributes fileAttributes = FileAttributes.NONE,
                FileFlags fileFlags = FileFlags.NONE,
                SecurityQosFlags securityQosFlags = SecurityQosFlags.NONE)
            {
                if (creationDisposition == System.IO.FileMode.Append) creationDisposition = System.IO.FileMode.OpenOrCreate;

                uint dwDesiredAccess =
                    ((fileAccess & System.IO.FileAccess.Read) != 0 ? (uint)GenericAccessRights.GENERIC_READ : 0) |
                    ((fileAccess & System.IO.FileAccess.Write) != 0 ? (uint)GenericAccessRights.GENERIC_WRITE : 0);

                uint flags = (uint)fileAttributes | (uint)fileFlags | (uint)securityQosFlags;

                SafeFileHandle handle = Direct.CreateFileW(path, dwDesiredAccess, fileShare, IntPtr.Zero, creationDisposition, flags, IntPtr.Zero);
                if (handle.IsInvalid)
                    throw ErrorHelper.GetIoExceptionForLastError(path);

                return handle;
            }
#endif
        }
    }
}