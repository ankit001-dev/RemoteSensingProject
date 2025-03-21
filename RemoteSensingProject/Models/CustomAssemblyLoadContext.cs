using System;
using System.Runtime.InteropServices;
using System.IO;

public class CustomAssemblyLoader
{
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr LoadLibrary(string path);

    public static void LoadUnmanagedLibrary(string absolutePath)
    {
        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException($"DLL not found: {absolutePath}");
        }

        IntPtr handle = LoadLibrary(absolutePath);
        if (handle == IntPtr.Zero)
        {
            throw new Exception($"Failed to load unmanaged library: {absolutePath}");
        }
    }
}
