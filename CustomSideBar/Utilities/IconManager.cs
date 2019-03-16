using System;
using System.Drawing;
using System.Runtime.InteropServices;
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct SHFileInfo
{
  public IntPtr hIcon;

  public int iIcon;

  public uint dwAttributes;

  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
  public string szDisplayName;

  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
  public string szTypeName;
};


namespace CustomSideBar.Utilities
{
  public enum DraggedFileType : short
  {
    Folder,
    File
  }

  public class Interop
  {
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFileInfo psfi, uint cbFileInfo, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(IntPtr hIcon);

    public const uint SHGFI_ICON = 0x000000100;
    public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
    public const uint SHGFI_OPENICON = 0x000000002;
    public const uint SHGFI_SMALLICON = 0x000000001;
    public const uint SHGFI_LARGEICON = 0x000000000;
    public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
    public const uint FILE_ATTRIBUTE_FILE = 0x00000100;


  }

  public class CSB_IconManager
  {
    public enum IconSize : short
    {
      Small,
      Large
    }

    public enum ItemState : short
    {
      Undefined,
      Open,
      Close
    }


    public static Icon GetIcon(string path, DraggedFileType type, IconSize size, ItemState state)
    {
      var flags = (uint)(Interop.SHGFI_ICON | Interop.SHGFI_USEFILEATTRIBUTES);
      var attribute = (uint)(object.Equals(type, DraggedFileType.Folder) ? Interop.FILE_ATTRIBUTE_DIRECTORY : Interop.FILE_ATTRIBUTE_FILE);
      if (object.Equals(type, DraggedFileType.Folder) && object.Equals(state, ItemState.Open))
      {
        flags += Interop.SHGFI_OPENICON;
      }
      if (object.Equals(size, IconSize.Small))
      {
        flags += Interop.SHGFI_SMALLICON;
      }
      else
      {
        flags += Interop.SHGFI_LARGEICON;
      }
      var shfi = new SHFileInfo();
      var res = Interop.SHGetFileInfo(path, attribute, out shfi, (uint)Marshal.SizeOf(shfi), flags);
      if (object.Equals(res, IntPtr.Zero)) throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
      try
      {
        Icon.FromHandle(shfi.hIcon);
        return (Icon)Icon.FromHandle(shfi.hIcon).Clone();
      }
      catch
      {
        throw;
      }
      finally
      {
        Interop.DestroyIcon(shfi.hIcon);
      }
    }
  }
}
