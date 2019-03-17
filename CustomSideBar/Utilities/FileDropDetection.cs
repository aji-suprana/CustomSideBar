using System;
using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using CustomSideBar.Utilities;
using System.Net;
using CustomSideBar.UserControls;

namespace CustomSideBar
{
  public enum FileFormats
  {
    webLink,
    folder,
    file
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
      public FileFormats Formats { get; set; }
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

    /// <summary>
    /// Getting Icon from a folder path/extension.
    /// </summary>
    /// <returns></returns>
    static public ImageSource getExtensionIcon(string path,FileFormats fileType)
    {
      CSB_IconManager.ItemState fileItemState = CSB_IconManager.ItemState.Undefined;
      Icon icon = null;
      switch (fileType)
      {
        case FileFormats.file:
          icon = Icon.ExtractAssociatedIcon(path);
          break;
        case FileFormats.folder:
          fileItemState = CSB_IconManager.ItemState.Open;
          icon = CSB_IconManager.GetIcon(path, DraggedFileType.Folder, CSB_IconManager.IconSize.Small, fileItemState);
          break;
        case FileFormats.webLink:
          icon = CSB_IconManager.GetIcon(".html", DraggedFileType.File, CSB_IconManager.IconSize.Small, fileItemState);

          break;
      }
      return ConvertIcoToImageSource(icon);
    }
    
    /// <summary>
    /// Converting Icon datatype to ImageSource Datatype, WPF does not display Icon Data type.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Drop Event Handler, is Binded to MainWindow.Drop Event.
    /// </summary>
    void Drop(object obj, DragEventArgs e)
    {
      Console.WriteLine("[CSB_FileDropDetection] Drop()");
      List<DraggedData> newItems = ProcessDraggedData(e);

      foreach(var it in newItems)
      {
        bool docExisted = false;
        foreach(var docPanel in UC_Collections.DocItems)
        {
          if (docPanel.FullName == it.FullName)
            docExisted |= true;
        }
        if (docExisted)
          return;

        UC_DocItem newDocPanel = UC_DocItem.Add(it.Name,it.Path,it.FullName,it.Formats,it.Icon);

        if (newDocPanel.DocName == "")
          newDocPanel.DocName = newDocPanel.FullName;
      }

      Serializer.CSB_SaveLoad.SaveDocItems();
    }
    
    /// <summary>
    /// Getting Data from Drop Event Retrieve File/Other Data Dragged onto CSB window. Returning all data as DraggedData, docPanel Readable data.
    /// </summary>
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

      //Check if this fil is directoryy or file
      if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
      {
        Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:{0} is a directory", _path);
        //Display name on doc panel
        returnData.Name = Path.GetFileName(_path);
        //Fullname to check wether same docpanel existed
        returnData.FullName = returnData.Name + ".winDirectory";
        //path to execute when doc panel is double clicked
        returnData.Path = _path;
        //fileformat store for deserializing data
        returnData.Formats = FileFormats.folder;
        //Display icon
        returnData.Icon = getExtensionIcon(_path,FileFormats.folder);


      }
      else
      {
        Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:{0} a file", _path);
        //Display name on doc panel
        returnData.Name = Path.GetFileNameWithoutExtension(_path);
        //Fullname to check wether same docpanel existed
        returnData.FullName = Path.GetFileName(_path);
        //path to execute when doc panel is double clicked
        returnData.Path = _path;
        //fileformat store for deserializing data
        returnData.Formats = FileFormats.file;
        //Display icon
        returnData.Icon = getExtensionIcon(_path, FileFormats.file);

      }
      Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:Name is {0}", returnData.Name);
      Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:Path is {0}", returnData.Path);
      Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:FullName is {0}", returnData.FullName);
      Console.WriteLine("[CSB_FileDropDetection][ParseFileData]:Format is {0}", returnData.Formats.ToString());
      Console.WriteLine("");

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

      //Check text is a valid web link
      bool validLink = Uri.IsWellFormedUriString(text, UriKind.Absolute);
      Console.WriteLine("[CSB_FileDropDetection][ParseTextData]:Text is a valid link? {0}", validLink);
      if(validLink)
      {
        return ParseWebLink(text);
      }
      //Do text next checking bellow ...

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

      returnedData.Path = path;
      returnedData.FullName = path;
      returnedData.Formats = FileFormats.webLink;
      ImageSource ico = getExtensionIcon(".html",FileFormats.webLink);
      returnedData.Icon = ico;

      Console.WriteLine("[CSB_FileDropDetection][ParseTextData]:Name is {0}", returnedData.Name);

      return returnedData;
    }

  }
}
