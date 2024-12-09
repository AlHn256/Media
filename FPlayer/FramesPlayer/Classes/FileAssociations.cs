using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

public class FileAssociation
{

    // Associate file extension with progID, description, icon and application
    public static void Associate(string extension, string progID, string description,
                                 string icon, string application)
    {
        Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progID);
        if (progID != null && progID.Length > 0)
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID))
            {
                if (description != null) key.SetValue("", description);
                if (icon != null) key.CreateSubKey("DefaultIcon").SetValue("", icon);
                if (application != null) key.CreateSubKey(@"Shell\Open\Command").SetValue("", application + " \"%1\"");
            }
    }

    // Return true if extension already associated in registry
    public static bool IsAssociated(string extension)
    {
        return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
    }

    [DllImport("Kernel32.dll")]
    private static extern uint GetShortPathName(string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

}