using FirstFloor.ModernUI.Windows.Media;
using SceenDimmer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IWshRuntimeLibrary;
using System.Windows.Threading;

namespace WinScreenDimmer
{
  using System.IO;
  /// <summary>
  /// Interaction logic for HomePage.xaml
  /// </summary>
  public partial class HomePage : UserControl, INotifyPropertyChanged
  {
    private float brightness;
    public float Brightness
    {
      get { return brightness; }
      set { 
        brightness = value;
        try
        {
          Utils.SetGammaRamp(value);
        }
        catch (Exception e)
        {
          MessageBox.Show(e.Message, e.GetType().ToString());
          Environment.Exit(0);
        }
        NotifyPropertyChanged();
      }
    }

    public bool RunAtStartup
    {
      get 
      {
        return File.Exists(Utils.lnk_Path);
      }
      set 
      {
        bool exists = File.Exists(Utils.lnk_Path);
        if(value == false && RunAtStartup)
            File.Delete(Utils.lnk_Path);
        else if(value == true && !RunAtStartup)
        {
          WshShell shell = new WshShell();
          IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Utils.lnk_Path);
          shortcut.TargetPath = Path.Combine(Environment.CurrentDirectory, "WinScreenDimmer.exe");
          shortcut.Save();
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public HomePage()
    {
      InitializeComponent();
      if(File.Exists(Utils.SavePath))
        using (StreamReader stream = new StreamReader(Utils.SavePath))
        {
          string line = stream.ReadLine();
          Brightness = (float)Convert.ToDouble(line);
        }
      else
        Brightness = 1.0f;

      DispatcherTimer timer = new DispatcherTimer();
      timer.Interval = TimeSpan.FromMilliseconds(500);
      timer.Tick += RegularSetGamma;
      timer.Start();

      // Refresh shortcut for startup. Because user maybe move this app to other folder.
      if(RunAtStartup)
      {
        WshShell shell = new WshShell();
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Utils.lnk_Path);
        shortcut.TargetPath = Path.Combine(Environment.CurrentDirectory, "WinScreenDimmer.exe");
        shortcut.Save();
      }

    }
    private void RegularSetGamma(object sender, EventArgs e)
    {
      Utils.SetGammaRamp(Brightness);
    }

    ~HomePage()
    {
      Brightness = 1.0f;
      using (StreamWriter stream = new StreamWriter(Utils.SavePath, false))
      {
        stream.WriteLine(this.Brightness);
      }
    }

    private void ModernButton_Click(object sender, RoutedEventArgs e)
    {
      Brightness = 1.0f;

      if (File.Exists(Utils.GammaPath))
        File.Delete(Utils.GammaPath);

      if (File.Exists(Utils.SavePath))
        File.Delete(Utils.SavePath);

      if (File.Exists(Utils.lnk_Path))
        File.Delete(Utils.lnk_Path);

      var result = MessageBox.Show("APP has deleted all caches. Restart computer will restore screen gammma. Restart now?", @"\(￣︶￣*\))", MessageBoxButton.YesNo);
      if(result == MessageBoxResult.Yes)
        System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
      Environment.Exit(0);
    }
  }
}
