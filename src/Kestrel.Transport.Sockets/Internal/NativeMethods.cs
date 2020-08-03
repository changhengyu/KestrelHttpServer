// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets.Internal
{
    internal static class NativeMethods
    {
        /// <summary>
        /// 控制哪些子进程能继承内核对象句柄，可调用SetHandleInformation函数改变内核对象句柄的继承标志。
        /// </summary>
        /// <param name="hObject"></param>
        /// <param name="dwMask"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetHandleInformation(IntPtr hObject, HANDLE_FLAGS dwMask, HANDLE_FLAGS dwFlags);

        [Flags]
        private enum HANDLE_FLAGS : uint
        {
            None = 0,
            //INHERIT 用CreateProcess(bInheritHandle设为TRUE)创建出来的子进程可以继承对象句柄
            INHERIT = 1,
            //PROTECT_FROM_CLOSE 无法调用CloseHandle关闭对象句柄
            PROTECT_FROM_CLOSE = 2
        }

        internal static void DisableHandleInheritance(Socket socket)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //要打开一个内核对象句柄的继承标志，可以像下面这样写：
                //SetHandleInformation(hObj, HANDLE_FLAG_INHERIT, HANDLE_FLAG_INHERIT);
                //要关闭这个标志，可以像下面这样写：
                //SetHandleInformation(hObj, HANDLE_FLAG_INHERIT, 0)
                SetHandleInformation(socket.Handle, HANDLE_FLAGS.INHERIT, 0);
            }
        }
    }
}
