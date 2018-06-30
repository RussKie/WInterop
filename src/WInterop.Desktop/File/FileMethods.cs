﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WInterop.ErrorHandling;
using WInterop.ErrorHandling.Types;
using WInterop.File.Types;
using WInterop.Handles.Types;
using WInterop.SafeString.Types;
using WInterop.Support;
using WInterop.Support.Buffers;

namespace WInterop.File
{
    public static partial class FileMethods
    {
        private struct ShortPathNameWrapper : IBufferFunc<StringBuffer, uint>
        {
            public string Path;

            uint IBufferFunc<StringBuffer, uint>.Func(StringBuffer buffer)
            {
                return Imports.GetShortPathNameW(Path, buffer, buffer.CharCapacity);
            }
        }

        /// <summary>
        /// Get the short (8.3) path version of the given path.
        /// </summary>
        public static string GetShortPathName(string path)
        {
            var wrapper = new ShortPathNameWrapper { Path = path };
            return BufferHelper.ApiInvoke(ref wrapper, path);
        }

        /// <summary>
        /// Gets the file information for the given handle.
        /// </summary>
        public static BY_HANDLE_FILE_INFORMATION GetFileInformationByHandle(SafeFileHandle fileHandle)
        {
            if (!Imports.GetFileInformationByHandle(fileHandle, out BY_HANDLE_FILE_INFORMATION fileInformation))
                throw Errors.GetIoExceptionForLastError();

            return fileInformation;
        }

        /// <summary>
        /// Creates symbolic links.
        /// </summary>
        public static void CreateSymbolicLink(string symbolicLinkPath, string targetPath, bool targetIsDirectory = false)
        {
            if (!Imports.CreateSymbolicLinkW(symbolicLinkPath, targetPath,
                targetIsDirectory ? SYMBOLIC_LINK_FLAG.SYMBOLIC_LINK_FLAG_DIRECTORY : SYMBOLIC_LINK_FLAG.SYMBOLIC_LINK_FLAG_FILE))
                throw Errors.GetIoExceptionForLastError(symbolicLinkPath);
        }

        /// <summary>
        /// CreateFile wrapper. Desktop only. Prefer File.CreateFile() as it will handle all supported platforms.
        /// </summary>
        /// <remarks>Not available in Windows Store applications.</remarks>
        public unsafe static SafeFileHandle CreateFileW(
            string path,
            DesiredAccess desiredAccess,
            ShareModes shareMode,
            CreationDisposition creationDisposition,
            FileAttributes fileAttributes = FileAttributes.None,
            FileFlags fileFlags = FileFlags.None,
            SecurityQosFlags securityQosFlags = SecurityQosFlags.None)
        {
            uint flags = (uint)fileAttributes | (uint)fileFlags | (uint)securityQosFlags;

            SafeFileHandle handle = Imports.CreateFileW(path, desiredAccess, shareMode, lpSecurityAttributes: null, creationDisposition, flags, hTemplateFile: IntPtr.Zero);
            if (handle.IsInvalid)
                throw Errors.GetIoExceptionForLastError(path);
            return handle;
        }

        /// <summary>
        /// NtCreateFile wrapper.
        /// </summary>
        public unsafe static SafeFileHandle CreateFileDirect(
            string path,
            CreateDisposition createDisposition,
            DesiredAccess desiredAccess = DesiredAccess.GenericReadWrite | DesiredAccess.Synchronize,
            ShareModes shareAccess = ShareModes.ReadWrite,
            FileAttributes fileAttributes = FileAttributes.None,
            CreateOptions createOptions = CreateOptions.SynchronousIoNonalert,
            ObjectAttributes objectAttributes = ObjectAttributes.CaseInsensitive)
        {
            return CreateFileRelative(path, null, createDisposition, desiredAccess, shareAccess,
                fileAttributes, createOptions, objectAttributes);
        }

        /// <summary>
        /// NtCreateFile wrapper.
        /// </summary>
        public unsafe static SafeFileHandle CreateFileDirect(
            ReadOnlySpan<char> path,
            CreateDisposition createDisposition,
            DesiredAccess desiredAccess = DesiredAccess.GenericReadWrite | DesiredAccess.Synchronize,
            ShareModes shareAccess = ShareModes.ReadWrite,
            FileAttributes fileAttributes = FileAttributes.None,
            CreateOptions createOptions = CreateOptions.SynchronousIoNonalert,
            ObjectAttributes objectAttributes = ObjectAttributes.CaseInsensitive)
        {
            return CreateFileRelative(path, null, createDisposition, desiredAccess, shareAccess,
                fileAttributes, createOptions, objectAttributes);
        }

        public unsafe static SafeFileHandle CreateFileRelative(
            ReadOnlySpan<char> path,
            SafeFileHandle rootDirectory,
            CreateDisposition createDisposition,
            DesiredAccess desiredAccess = DesiredAccess.GenericReadWrite | DesiredAccess.Synchronize,
            ShareModes shareAccess = ShareModes.ReadWrite,
            FileAttributes fileAttributes = FileAttributes.None,
            CreateOptions createOptions = CreateOptions.SynchronousIoNonalert,
            ObjectAttributes objectAttributes = ObjectAttributes.CaseInsensitive)
        {
            using (var handle = new UnwrapHandle(rootDirectory))
            {
                return new SafeFileHandle(
                    CreateFileRelative(path, handle, createDisposition, desiredAccess,
                        shareAccess, fileAttributes, createOptions, objectAttributes),
                    true);
            }
        }

        public unsafe static SafeFileHandle CreateFileRelative(
            string path,
            SafeFileHandle rootDirectory,
            CreateDisposition createDisposition,
            DesiredAccess desiredAccess = DesiredAccess.GenericReadWrite | DesiredAccess.Synchronize,
            ShareModes shareAccess = ShareModes.ReadWrite,
            FileAttributes fileAttributes = FileAttributes.None,
            CreateOptions createOptions = CreateOptions.SynchronousIoNonalert,
            ObjectAttributes objectAttributes = ObjectAttributes.CaseInsensitive)
        {
            return CreateFileRelative(path.AsSpan(), rootDirectory, createDisposition, desiredAccess,
                shareAccess, fileAttributes, createOptions, objectAttributes);
        }

        public unsafe static IntPtr CreateFileRelative(
            string path,
            IntPtr rootDirectory,
            CreateDisposition createDisposition,
            DesiredAccess desiredAccess = DesiredAccess.GenericReadWrite | DesiredAccess.Synchronize,
            ShareModes shareAccess = ShareModes.ReadWrite,
            FileAttributes fileAttributes = FileAttributes.None,
            CreateOptions createOptions = CreateOptions.SynchronousIoNonalert,
            ObjectAttributes objectAttributes = ObjectAttributes.CaseInsensitive)
        {
            return CreateFileRelative(path.AsSpan(), rootDirectory, createDisposition, desiredAccess,
                shareAccess, fileAttributes, createOptions, objectAttributes);
        }

        public unsafe static IntPtr CreateFileRelative(
            ReadOnlySpan<char> path,
            IntPtr rootDirectory,
            CreateDisposition createDisposition,
            DesiredAccess desiredAccess = DesiredAccess.GenericReadWrite | DesiredAccess.Synchronize,
            ShareModes shareAccess = ShareModes.ReadWrite,
            FileAttributes fileAttributes = FileAttributes.None,
            CreateOptions createOptions = CreateOptions.SynchronousIoNonalert,
            ObjectAttributes objectAttributes = ObjectAttributes.CaseInsensitive)
        {
            fixed (char* c = &MemoryMarshal.GetReference(path))
            {
                UNICODE_STRING name = new UNICODE_STRING(c, path.Length);
                OBJECT_ATTRIBUTES attributes = new OBJECT_ATTRIBUTES(
                    &name,
                    objectAttributes,
                    rootDirectory,
                    null,
                    null);

                NTSTATUS status = Imports.NtCreateFile(
                    out IntPtr handle,
                    desiredAccess,
                    ref attributes,
                    out IO_STATUS_BLOCK statusBlock,
                    AllocationSize: null,
                    FileAttributes: fileAttributes,
                    ShareAccess: shareAccess,
                    CreateDisposition: createDisposition,
                    CreateOptions: createOptions,
                    EaBuffer: null,
                    EaLength: 0);

                if (status != NTSTATUS.STATUS_SUCCESS)
                    throw ErrorMethods.GetIoExceptionForNTStatus(status, path.ToString());

                return handle;
            }
        }

        /// <summary>
        /// Wrapper to create a directory within another directory
        /// </summary>
        public static SafeFileHandle CreateDirectory(SafeFileHandle rootDirectory, string name)
        {
            return CreateFileRelative(
                name,
                rootDirectory,
                CreateDisposition.Create,
                DesiredAccess.ListDirectory | DesiredAccess.Synchronize,
                ShareModes.ReadWrite | ShareModes.Delete,
                FileAttributes.None,
                CreateOptions.SynchronousIoNonalert | CreateOptions.DirectoryFile | CreateOptions.OpenForBackupIntent | CreateOptions.OpenReparsePoint);
        }

        /// <summary>
        /// Creates a directory handle from an existing directory handle.
        /// </summary>
        public static SafeFileHandle CreateDirectoryHandle(SafeFileHandle rootDirectory, string subdirectoryPath)
        {
            return CreateFileRelative(
                subdirectoryPath,
                rootDirectory,
                CreateDisposition.Open,
                DesiredAccess.ListDirectory | DesiredAccess.Synchronize,
                ShareModes.ReadWrite | ShareModes.Delete,
                FileAttributes.None,
                CreateOptions.SynchronousIoNonalert | CreateOptions.DirectoryFile | CreateOptions.OpenForBackupIntent | CreateOptions.OpenReparsePoint);
        }

        /// <summary>
        /// Creates a raw directory handle from an existing directory handle.
        /// </summary>
        public static IntPtr CreateDirectoryHandle(IntPtr rootDirectory, string subdirectoryPath)
        {
            return CreateFileRelative(
                subdirectoryPath,
                rootDirectory,
                CreateDisposition.Open,
                DesiredAccess.ListDirectory | DesiredAccess.Synchronize,
                ShareModes.ReadWrite | ShareModes.Delete,
                FileAttributes.None,
                CreateOptions.SynchronousIoNonalert | CreateOptions.DirectoryFile | CreateOptions.OpenForBackupIntent | CreateOptions.OpenReparsePoint);
        }

        /// <summary>
        /// CopyFileEx wrapper. Desktop only. Prefer File.CopyFile() as it will handle all supported platforms.
        /// </summary>
        /// <param name="overwrite">Overwrite an existing file if true.</param>
        public static void CopyFileEx(string source, string destination, bool overwrite = false)
        {
            bool cancel = false;

            if (!Imports.CopyFileExW(
                lpExistingFileName: source,
                lpNewFileName: destination,
                lpProgressRoutine: null,
                lpData: IntPtr.Zero,
                pbCancel: ref cancel,
                dwCopyFlags: overwrite ? 0 : CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS))
            {
                throw Errors.GetIoExceptionForLastError(source);
            }
        }

        public static string GetFileName(SafeFileHandle fileHandle)
        {
            // https://msdn.microsoft.com/en-us/library/windows/hardware/ff545817.aspx

            //  typedef struct _FILE_NAME_INFORMATION
            //  {
            //      ULONG FileNameLength;
            //      WCHAR FileName[1];
            //  } FILE_NAME_INFORMATION, *PFILE_NAME_INFORMATION;

            return GetFileInformationString(fileHandle, FileInformationClass.FileNameInformation);
        }

        public static string GetVolumeName(SafeFileHandle fileHandle)
        {
            // Same basic structure as FILE_NAME_INFORMATION
            return GetFileInformationString(fileHandle, FileInformationClass.FileVolumeNameInformation);
        }

        /// <summary>
        /// This is the short name for the file/directory name, not the path. Available from WindowsStore.
        /// </summary>
        public static string GetShortName(SafeFileHandle fileHandle)
        {
            // Same basic structure as FILE_NAME_INFORMATION
            return GetFileInformationString(fileHandle, FileInformationClass.FileAlternateNameInformation);
        }

        private unsafe static string GetFileInformationString(SafeFileHandle fileHandle, FileInformationClass fileInformationClass)
        {
            return BufferHelper.BufferInvoke((HeapBuffer buffer) =>
            {
                NTSTATUS status = NTSTATUS.STATUS_BUFFER_OVERFLOW;

                // Start with MAX_PATH
                uint byteLength = 260 * sizeof(char);

                FILE_NAME_INFORMATION* value = null;

                while (status == NTSTATUS.STATUS_BUFFER_OVERFLOW)
                {
                    // Add space for the FileNameLength
                    buffer.EnsureByteCapacity(byteLength + sizeof(uint));

                    status = Imports.NtQueryInformationFile(
                        FileHandle: fileHandle,
                        IoStatusBlock: out _,
                        FileInformation: buffer.VoidPointer,
                        Length: checked((uint)buffer.ByteCapacity),
                        FileInformationClass: fileInformationClass);

                    if (status == NTSTATUS.STATUS_SUCCESS || status == NTSTATUS.STATUS_BUFFER_OVERFLOW)
                    {
                        value = (FILE_NAME_INFORMATION*)buffer.VoidPointer;
                        byteLength = value->FileNameLength;
                    }
                }

                if (status != NTSTATUS.STATUS_SUCCESS)
                    throw ErrorMethods.GetIoExceptionForNTStatus(status);

                return value->FileName.CreateString();
            });
        }

        unsafe private static void GetFileInformation(SafeFileHandle fileHandle, FileInformationClass fileInformationClass, void* value, uint size)
        {
            NTSTATUS status = Imports.NtQueryInformationFile(
                FileHandle: fileHandle,
                IoStatusBlock: out _,
                FileInformation: value,
                Length: size,
                FileInformationClass: fileInformationClass);

            if (status != NTSTATUS.STATUS_SUCCESS)
                throw ErrorMethods.GetIoExceptionForNTStatus(status);
        }

        /// <summary>
        /// Gets the file mode for the given handle.
        /// </summary>
        public unsafe static FileAccessModes GetFileMode(SafeFileHandle fileHandle)
        {
            FileAccessModes info;
            GetFileInformation(fileHandle, FileInformationClass.FileModeInformation, &info, sizeof(FileAccessModes));
            return info;
        }

        /// <summary>
        /// Return whether or not the given expression matches the given name. Takes standard
        /// Windows wildcards (*, ?, &lt;, &gt; &quot;).
        /// </summary>
        public unsafe static bool IsNameInExpression(string expression, string name, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(expression) || string.IsNullOrEmpty(name))
                return false;

            // If ignore case is set, the API will uppercase the name *if* an UpcaseTable
            // is not provided. It then flips to case-sensitive. In this state the expression
            // has to be uppercase to match as expected.

            fixed (char* e = ignoreCase ? expression.ToUpperInvariant() : expression)
            fixed (char* n = name)
            {
                UNICODE_STRING* eus = null;
                UNICODE_STRING* nus = null;

                if (e != null)
                {
                    var temp = new UNICODE_STRING(e, expression.Length);
                    eus = &temp;
                }
                if (n != null)
                {
                    var temp = new UNICODE_STRING(n, name.Length);
                    nus = &temp;
                }

                return Imports.RtlIsNameInExpression(eus, nus, ignoreCase, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Get the access rights applied to the given file handle.
        /// </summary>
        public unsafe static FileAccessRights GetRights(SafeFileHandle fileHandle)
        {
            FILE_ACCESS_INFORMATION access = new FILE_ACCESS_INFORMATION();
            NTSTATUS result = Imports.NtQueryInformationFile(fileHandle, out _,
                &access, (uint)sizeof(FILE_ACCESS_INFORMATION), FileInformationClass.FileAccessInformation);
            if (result != NTSTATUS.STATUS_SUCCESS)
                throw ErrorMethods.GetIoExceptionForNTStatus(result);
            return access.AccessFlags;
        }

        /// <summary>
        /// Get the ids for all processes that have a handle to this file system object.
        /// Does not include the current process.
        /// </summary>
        public unsafe static IEnumerable<UIntPtr> GetProcessIds(SafeFileHandle fileHandle)
        {
            return BufferHelper.BufferInvoke((HeapBuffer buffer) =>
            {
                NTSTATUS status = NTSTATUS.STATUS_INFO_LENGTH_MISMATCH;

                while (status == NTSTATUS.STATUS_INFO_LENGTH_MISMATCH)
                {
                    status = Imports.NtQueryInformationFile(fileHandle, out IO_STATUS_BLOCK statusBlock,
                        buffer.VoidPointer, (uint)buffer.ByteCapacity, FileInformationClass.FileProcessIdsUsingFileInformation);

                    switch (status)
                    {
                        case NTSTATUS.STATUS_SUCCESS:
                            break;
                        case NTSTATUS.STATUS_INFO_LENGTH_MISMATCH:
                            // Not a big enough buffer
                            buffer.EnsureByteCapacity((ulong)statusBlock.Information);
                            break;
                        default:
                            throw ErrorMethods.GetIoExceptionForNTStatus(status);
                    }
                }

                return ((FILE_PROCESS_IDS_USING_FILE_INFORMATION*)buffer.VoidPointer)->ProcessIdList.ToArray();
            });
        }
    }
}
