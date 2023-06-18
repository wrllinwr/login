using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;


class Win32API
{
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VMOperation = 0x00000008,
        VMRead = 0x00000010,
        VMWrite = 0x00000020,
        DupHandle = 0x00000040,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        Synchronize = 0x00100000
    }
    
    public const int HSHELL_WINDOWCREATED = 1;
    public const int HSHELL_WINDOWDESTROYED = 2;
    public const int HSHELL_ACTIVATESHELLWINDOW = 3;
    public const int HSHELL_WINDOWACTIVATED = 4;
    public const int HSHELL_GETMINRECT = 5;
    public const int HSHELL_REDRAW = 6;
    public const int HSHELL_TASKMAN = 7;
    public const int HSHELL_LANGUAGE = 8;

    public const int PAGE_READWRITE = 0x4;
    public const int PAGE_EXECUTE_READWRITE = 0x40;
    public const int MEM_COMMIT = 4096;
    public const int MEM_RELEASE = 0x8000;
    public const int MEM_DECOMMIT = 0x4000;

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, int bInheritHandle, int dwProcessId);
    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead);
    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, short[] lpBuffer, int dwSize, int lpNumberOfBytesRead);
    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, int[] lpBuffer, int dwSize, int lpNumberOfBytesRead);
    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);
    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, ref byte lpBuffer, int nSize, int lpNumberOfBytesWritten);
    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, ref short lpBuffer, int nSize, int lpNumberOfBytesWritten);
    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, ref int lpBuffer, int nSize, int lpNumberOfBytesWritten);

    [DllImport("user32.dll")]
    public static extern bool IsZoomed(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern bool IsIconic(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern bool IsWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern int GetClassName(System.IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("kernel32.dll")]
    public static extern System.Int32 VirtualAllocEx(IntPtr hProcess, int lpAddress, int dwSize, int flAllocationType, int flProtect);
    [DllImport("kernel32.dll")]
    public static extern System.Int32 VirtualFreeEx(IntPtr hProcess, int lpAddress, int dwSize, int flAllocationType);
    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateRemoteThread(IntPtr hProcess, int lpThreadAttributes, int dwStackSize, int lpStartAddress, int lpParameter, int dwCreationFlags, int lpThreadId);
    [DllImport("kernel32.dll")]
    public static extern uint WaitForSingleObject(IntPtr handle, uint milliseconds);

    [DllImport("ntdll.dll")]
    public static extern int RtlAdjustPrivilege(int Privilege, bool Enable, bool CurrentThread, ref int Enabled);
    [DllImport("user32.dll")]
    public static extern int RegisterWindowMessage(string lpString);
    [DllImport("user32.dll")]
    public static extern int RegisterShellHookWindow(IntPtr Hwnd);
    [DllImport("user32.dll")]
    public static extern int DeregisterShellHookWindow(IntPtr Hwnd);
    [DllImport("shell32.dll", EntryPoint = "#181")]
    public static extern int RegisterShellHook(IntPtr Hwnd, int nAction);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("gdi32.dll")]
    public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
    [DllImport("kernel32.dll")]
    public static extern uint GetTickCount();
    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);
    public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern int GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, int nSize, string lpFileName);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern int WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);


    public static int GetINI_Int(string lpAppName, string lpKeyName, int lpDefault, string lpFileName)
    {
        return GetPrivateProfileInt(lpAppName, lpKeyName, lpDefault, lpFileName);
    }

    public static string GetINI(string lpAppName, string lpKeyName, string lpDefault, string lpFileName)
    {
        StringBuilder sb = new StringBuilder(1024);
        GetPrivateProfileString(lpAppName, lpKeyName, lpDefault, sb, sb.Capacity, lpFileName);
        return sb.ToString();
    }

    public static NameValueCollection GetINI_Section(string Section, string lpFileName)
    {
        NameValueCollection values = new NameValueCollection();
        if (File.Exists(lpFileName))
        {
            int FileSize = Convert.ToInt32(new FileInfo(lpFileName).Length);
            byte[] buff = new byte[FileSize + 1];
            int bytesRead = GetPrivateProfileSection(Section, buff, FileSize, lpFileName);
            int Pos = 0;
            string name = null;
            string value = null;
            int iPos = 0;
            for (int iCnt = 0; iCnt <= bytesRead - 1; iCnt++)
            {
                if (buff[iCnt] == 0)
                {
                    string tmp = ASCIIEncoding.Default.GetString(buff, iPos, iCnt - iPos);
                    iPos = iCnt + 1;
                    Pos = tmp.IndexOf("=");
                    name = tmp.Substring(0, Pos);
                    value = tmp.Substring(Pos + 1);
                    values.Add(name, value);
                }
            }
        }
        
        return values;
    }

    public static int WriteINI(string lpAppName, string lpKeyName, string lpString, string lpFileName)
    {
        return WritePrivateProfileString(lpAppName, lpKeyName, lpString, lpFileName);
    }

    public static bool WriteINI_Section(string Section, string lpString, string lpFileName)
    {
        return WritePrivateProfileSection(Section, lpString, lpFileName);
    }

    public static string GetClassName(IntPtr hwnd)
    {
        StringBuilder sb = new StringBuilder(1024);
        GetClassName(hwnd, sb, sb.Capacity);
        return sb.ToString();
    }

    public static string GetWindowText(IntPtr hwnd)
    {
        StringBuilder sb = new StringBuilder(1024);
        GetWindowText(hwnd, sb, sb.Capacity);
        return sb.ToString();
    }

    [DllImport("kernel32.dll")]
    public static extern bool SetProcessWorkingSetSize(IntPtr hProcess, Int32 dwMinimumWorkingSetSize, Int32 dwMaximumWorkingSetSize);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

    public enum GetWindow_Cmd : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }

}
