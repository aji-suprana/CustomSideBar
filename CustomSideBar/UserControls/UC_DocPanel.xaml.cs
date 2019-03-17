using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using CustomSideBar.Serializer;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CustomSideBar.UserControls
{

  public partial class UC_DocItem : UserControl, ISerializableObject, INotifyPropertyChanged
  {
    //delegates
    public delegate void DocPanelClicked(int DocPanelID);

    //events
    public static DocPanelClicked docPanelClicked = null;
    public event PropertyChangedEventHandler PropertyChanged;

    //selection states
    // public static List< UC_DocItem> docPanelObjects = new List< UC_DocItem>();
    private static int selected = -1;
    public static int hoveredID = -1;

    //Property
    public string DocName { get { return this.docName; } set { this.docName = value; NotifyPropertyChanged(); } }
    public string DocPath { get; set; }
    public FileFormats DocFormat { get; set; }
    public ImageSource Icon { get; set; }
    public int Id { get; set; }
    public string FullName { get; set; }

    private string docName = null;
    //Privates
    private bool isHovered = false;

    ///////////////////////////////////////////////////////////////////////
    /// Selected Colors
    ///////////////////////////////////////////////////////////////////////
    public SolidColorBrush SelectedColor
    {
      get { return (SolidColorBrush)GetValue(SelectedColorProperty); }
      set { SetValue(SelectedColorProperty, value); }
    }

    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(
          "SelectedColor",
          typeof(SolidColorBrush),
          typeof(UC_DocItem),
          new PropertyMetadata((SolidColorBrush)(new BrushConverter().ConvertFrom("#333333"))));


    ///////////////////////////////////////////////////////////////////////
    /// Normal Colors
    ///////////////////////////////////////////////////////////////////////
    public SolidColorBrush NormalColor
    {
      get { return (SolidColorBrush)GetValue(NormalColorProperty); }
      set { SetValue(NormalColorProperty, value); }
    }

    public static readonly DependencyProperty NormalColorProperty =
        DependencyProperty.Register(
          "NormalColor", 
          typeof(SolidColorBrush),
          typeof(UC_DocItem), 
          new PropertyMetadata(new BrushConverter().ConvertFrom("#1e1e1e")));




    ///////////////////////////////////////////////////////////////////////
    /// Constructor
    ///////////////////////////////////////////////////////////////////////
    public UC_DocItem()
    {
      InitializeComponent();
      DataContext = this;
    }

    ~UC_DocItem()
    {
    }

    ///////////////////////////////////////////////////////////////////////
    /// ISerializable interface implementation
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Serialize from a parsed JsonObject
    /// </summary>
    public JObject Serialize()
    {
      JObject DocData = new JObject();
      DocData.Add(new JProperty("DocName", DocName));
      DocData.Add(new JProperty("DocPath", DocPath));
      DocData.Add(new JProperty("FullName", FullName));
      DocData.Add(new JProperty("FileFormat", DocFormat));

      return DocData;
    }

    /// <summary>
    /// Deserialize from a parsed JsonObject
    /// </summary>
    public void Deserialize(JToken JsonString)
    {
      DocName = JsonString["DocName"].ToObject<string>();
      DocPath = JsonString["DocPath"].ToObject<string>();
      FullName = JsonString["FullName"].ToObject<string>();
      DocFormat = JsonString["FileFormat"].ToObject<FileFormats>();

      Icon = CSB_FileDropDetection.getExtensionIcon(DocPath, DocFormat);
    }

    ///////////////////////////////////////////////////////////////////////
    /// Utilities
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Get Hovered DocItems ID, return -1 if none is hovered
    /// </summary>
    int GetHoveredID()
    {
      foreach(var it in UC_Collections.DocItems)
      {
        if (it.isHovered)
          return it.Id;
      }
      return -1;
    }

    private string title = null;
    /// <summary>
    /// Start Thread to fetch web title
    /// </summary>
    void CheckIfWebLinkTitleUnavailable()
    {
      Console.WriteLine("[UC_DocItem][CheckIfWebLinkTitleUnavailable]");

      if (DocFormat != FileFormats.webLink)
        return;

      Console.WriteLine("[UC_DocItem][CheckIfWebLinkTitleUnavailable] : isWebLink");

      if (DocName == "isLoading..." || DocName == null)
      {
        Console.WriteLine("[UC_DocItem][CheckIfWebLinkTitleUnavailable] : need to Reload");

        Thread FetchWebLinkThread = new Thread(FetchWebLink);
        FetchWebLinkThread.Start();

        Thread WaitforResponseThread = new Thread(WaitforTitleFetched);
        WaitforResponseThread.Start();
      }
    }
    void WaitforTitleFetched()
    {
      bool isLoading = false;
      while(title == null)
      {
        if(!isLoading)
        {
          DocName = "isLoading...";
          isLoading = true;
        }
      }
      DocName = title;
      CSB_SaveLoad.SaveDocItems();
    }
    void FetchWebLink()
    {
      Console.WriteLine("[UC_DocItem][StartThreadFetchWebTitle]");
      string URL = DocPath;

      using (WebClient client = new WebClient {Encoding = Encoding.UTF8}) // WebClient class inherits IDisposable
      {
        // Or you can get the file content without saving it
        string htmlCode = client.DownloadString(DocPath);
        title = Regex.Match(htmlCode, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

        if (title == "")
          title = DocPath;
      }
    }
    ///////////////////////////////////////////////////////////////////////
    /// Handler
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Triggered when keyDown event triggered
    /// </summary>
    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
      if (selected != Id)
        return;

      if (e.Key == Key.Delete)
      {
        Console.WriteLine("[UC_DocItem]:{0} going to be deleted", Id);
        Remove(this);
        Serializer.CSB_SaveLoad.SaveDocItems();

        e.Handled = true;
      }
    }

    /// <summary>
    /// Triggered on DocItemLoaded
    /// </summary>
    private void DocItemLoaded(object sender, RoutedEventArgs e)
    {
      var window = (MainWindow)Window.GetWindow(this);
      window.Root.MouseLeftButtonUp += Select;
      window.Root.MouseMove += CheckIsHovered;
      window.Root.KeyDown += KeyDownHandler;
      MouseDoubleClick += MouseButtonEventHandler;

    }

    /// <summary>
    /// Triggered on Mouse button up
    /// </summary>
    void Select(object obj, EventArgs e)
    {
      isHovered = IsMouseOver;
      hoveredID = GetHoveredID();

      if (hoveredID == -1)
      {
        foreach (var it in UC_Collections.DocItems)
        {
          it.Root.Background = NormalColor;
        }
        return;
      }

      if (hoveredID == Id)
      {
        Console.WriteLine("[UC_DocItem]:Id {1}: Name {0}: Clicked", DocName, Id);

        UC_Collections.DocItems[Id].Root.Background = SelectedColor;
        selected = Id;
      }
      else
      {
        UC_Collections.DocItems[Id].Root.Background = NormalColor;
      }

      if (docPanelClicked != null)
        docPanelClicked(Id);
    }

    /// <summary>
    /// Triggered on Mouse Move
    /// </summary>
    void CheckIsHovered(object obj, MouseEventArgs e)
    {
      isHovered = IsMouseOver;
      hoveredID = GetHoveredID();
    }

    /// <summary>
    /// Notify property changed
    /// </summary>
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Doubleclicked handler
    /// </summary>
    void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
    {
      Console.WriteLine("[UC_DocItem]:Id {1}: Name {0}: DoubleClicked", DocName, Id);
      Utilities.ProcessManager.AddProcess(DocName, DocPath, DocFormat);

    }
    ///////////////////////////////////////////////////////////////////////
    /// Creation & Deletion
    ///////////////////////////////////////////////////////////////////////
    public static UC_DocItem Add()
    {
      Console.WriteLine("[UC)DocItem]:AddNew Empty");

      UC_DocItem newDocItem = new UC_DocItem();

      UC_Collections.DocItems.Add(newDocItem);
      MainWindow.instance.DocItemsPanel.Children.Add(newDocItem);

      return newDocItem;
    }
    public static UC_DocItem Add(JToken Token)
    {
      Console.WriteLine("[UC)DocItem]:AddNew  Token");

      UC_DocItem newDocItem = new UC_DocItem();
      newDocItem.Deserialize(Token);

      UC_Collections.DocItems.Add(newDocItem);
      MainWindow.instance.DocItemsPanel.Children.Add(newDocItem);

      newDocItem.CheckIfWebLinkTitleUnavailable();

      return newDocItem;
    }
    public static UC_DocItem Add(string Name, string Path, string FullName, FileFormats Format, ImageSource Icon = null)
    {
      Console.WriteLine("[UC)DocItem]:AddNew 6 params");
      UC_DocItem newDocItem = new UC_DocItem();
      newDocItem.Id = UC_Collections.DocItems.Count();
      newDocItem.DocName = Name;
      newDocItem.DocPath = Path;
      newDocItem.DocFormat = Format;
      newDocItem.FullName = FullName;
      newDocItem.Icon = Icon;

      if(Icon == null)
        Icon = CSB_FileDropDetection.getExtensionIcon(Path, Format);

      newDocItem.CheckIfWebLinkTitleUnavailable();

      UC_Collections.DocItems.Add(newDocItem);
      MainWindow.instance.DocItemsPanel.Children.Add(newDocItem);

      return newDocItem;
    }
    public static void Remove(UC_DocItem docItem)
    {
      var window = (MainWindow)Window.GetWindow(docItem);

      window.Root.MouseLeftButtonUp -= docItem.Select;
      window.Root.MouseMove -= docItem.CheckIsHovered;
      window.Root.KeyDown -= docItem.KeyDownHandler;

      UC_Collections.DeleteDocItem(docItem);

    }
  }
}
