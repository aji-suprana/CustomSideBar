using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using CustomSideBar.Utilities;
using System.Net;
using System.Text.RegularExpressions;

namespace CustomSideBar
{
  enum FileFormats
  {
    googlelink,
    folder,
    executable,

  }

  public class CSB_FileDropDetection
  {
    public static ImageSource img = null;
    public static CSB_FileDropDetection instance = null;

    private class DraggedData
    {
      public string Path { get; set; }
      public string Name { get; set; }
      public string FullName { get; set; }
      public ImageSource Icon { get; set; }
    }

    public CSB_FileDropDetection()
    {
      instance = this;
      MainWindow.instance.Drop += Drop;
      Testing();
    }

    void Testing()
    {
    }

    static public ImageSource getExtensionIcon(string path,DraggedFileType fileType)
    {
      CSB_IconManager.ItemState fileItemState = CSB_IconManager.ItemState.Undefined;
      Icon icon = null;
      if (fileType == DraggedFileType.Folder)
      {
        fileItemState = CSB_IconManager.ItemState.Open;
        icon = CSB_IconManager.GetIcon(path, fileType, CSB_IconManager.IconSize.Small, fileItemState);
      }
      else
      {
        icon = Icon.ExtractAssociatedIcon(path);
      }

      return ConvertIcoToImageSource(icon);
    }

    static public ImageSource ConvertIcoToImageSource(Icon ico)
    {
      ImageSource imageSource;

      using (Bitmap bmp = ico.ToBitmap())
      {
        var stream = new MemoryStream();
        bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        imageSource = BitmapFrame.Create(stream);
        return imageSource;
      }
    }

    void Drop(object obj, DragEventArgs e)
    {
      Console.WriteLine("[CSB_FileDropDetection] Drop()");
      List<DraggedData> newItems = ProcessDraggedData(e);

      foreach(var it in newItems)
      {
        bool docExisted = false;
        foreach(var docPanel in UC_DocPanel.docPanelObjects)
        {
          if (docPanel.FullName == it.FullName)
            docExisted |= true;
        }
        if (docExisted)
          return;

        UC_DocPanel newDocPanel = new UC_DocPanel();
        newDocPanel.DocName = it.Name;
        newDocPanel.FullName = it.FullName;
        newDocPanel.DocPath = it.Path;
        newDocPanel.Icon = it.Icon;

        MainWindow.instance.DocPanelList.Children.Add(newDocPanel);
      }
    }

    List<DraggedData> ProcessDraggedData(DragEventArgs e)
    {
      List<DraggedData> draggedData = new List<DraggedData>();
      Console.WriteLine("[CSB_FileDropDetection]:ProcessDraggedData");
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        // Note that you can have more than one file.
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

        foreach (var it in files)
        {
          DraggedData data = ParseFileData(it);
          if (data != null)
            draggedData.Add(data);
        }
        return draggedData;
      }
      else if (e.Data.GetDataPresent(DataFormats.Text))
      {
        string files = (string)e.Data.GetData(DataFormats.Text);
        Console.WriteLine(files);
        DraggedData data = ParseTextData(files);
        if(data != null)
          draggedData.Add(data);
        return draggedData;
      }
      return null;
    }


    /////////////////////////////////////////////////////////////////////////////////
    //Parsing
    /////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Parsing path into DraggedData, a docPanel Readable data. Which contains
    /// Name, Path, Icon
    /// </summary>
    /// <param name="_path"> path to a file/folder data</param>
    /// <returns>DraggedData return will be processed in ProcessDraggedData</returns>
    DraggedData ParseFileData(string _path)
    {
      Console.WriteLine("[CSB_FileDropDetection][ParseFileData]");

      DraggedData returnData = new DraggedData();
      // get the file attributes for file or directory
      FileAttributes attr = File.GetAttributes(_path);
      //detect whether its a directory or file

      if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
      {
        Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:{0} is a directory", _path);
        returnData.Name = Path.GetFileName(_path);
        returnData.FullName = returnData.Name + ".winDirectory";
        returnData.Path = _path;
        returnData.Icon = getExtensionIcon(_path,DraggedFileType.Folder);
        Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:Name is {0}", returnData.Name);
      }
      else
      {
        Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:{0} a file", _path);
        returnData.Name = Path.GetFileNameWithoutExtension(_path);
        returnData.FullName = Path.GetFileName(_path);
        returnData.Path = _path;
        returnData.Icon = getExtensionIcon(_path, DraggedFileType.File);
        Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:Name is {0}", returnData.Name);

      }
      return returnData;
    }

    /// <summary>
    /// Parsing path into DraggedData, a docPanel Readable data. Which contains
    /// Name, Path, Icon
    /// </summary>
    /// <param name="text"> dragged text into CSB window</param>
    /// <returns>DraggedData return will be processed in ProcessDraggedData</returns>
    DraggedData ParseTextData(string text)
    {
      Console.WriteLine("[CSB_FileDropDetection][ParseTextData]");


      bool validLink = Uri.IsWellFormedUriString(text, UriKind.Absolute);
      Console.WriteLine("[CSB_FileDropDetection][ParseTextData]:Text is a valid link?{0}", validLink);
      if(validLink)
      {
        return ParseWebLink(text);
      }
      return null;
    }

    /// <summary>
    /// Parsing weblink into DraggedData, a docPanel Readable data. Which contains
    /// Name, Path, Icon
    /// </summary>
    /// <param name="path"> path to a web url</param>
    /// <returns>DraggedData return will be processed in ProcessDraggedData</returns>
    DraggedData ParseWebLink(string path)
    {
      DraggedData returnedData = new DraggedData();

      using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
      {
        // Or you can get the file content without saving it
        string htmlCode = client.DownloadString(path);
        string title = Regex.Match(htmlCode, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

        returnedData.Name = title;
        returnedData.FullName =title + ".webLinkOnline";
        returnedData.Path = path;
        Icon ico = CSB_IconManager.GetIcon(".html", DraggedFileType.File, CSB_IconManager.IconSize.Small, CSB_IconManager.ItemState.Undefined);
        returnedData.Icon = ConvertIcoToImageSource(ico);
      }

      Console.WriteLine("[CSB_FileDropDetection][ParseTextData]:Name is {0}", returnedData.Name);

      return returnedData;
    }

  }
}
