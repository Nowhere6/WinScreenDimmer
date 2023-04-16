using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Drawing;
using Forms = System.Windows.Forms;
using FirstFloor.ModernUI.Windows.Controls;
using SceenDimmer;
using System.Windows.Controls.Primitives;

namespace WinScreenDimmer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : ModernWindow
  {
    private Forms.NotifyIcon notifyIcon;

    private bool _FirstShow = true;
    private bool FirstShow
    {
      get
      {
        if (_FirstShow == true)
        {
          _FirstShow = false;
          return true;
        }
        else
          return false;
      }
    }
    public MainWindow()
    {
      InitializeComponent();
      InitializeNotifyIcon();
      DataContext = this;
    }
    private void InitializeNotifyIcon()
    {
      notifyIcon = new System.Windows.Forms.NotifyIcon();
      notifyIcon.BalloonTipText = "Click the tray icon to show.";
      notifyIcon.Text = this.Title;
      string iconPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Resources\MainIcon.ico");
      notifyIcon.Icon = new System.Drawing.Icon(iconPath);
      notifyIcon.Click += (a, b) => { this.WindowState = WindowState.Normal; };
    }
    private void ModernWindow_Closed(object sender, EventArgs e)
    {
      if (notifyIcon != null)
        notifyIcon.Dispose();
    }

    private void ModernWindow_StateChanged(object sender, EventArgs e)
    {
      switch (WindowState)
      {
        case WindowState.Minimized:
          this.ShowInTaskbar = false;
          notifyIcon.Visible = true;
          if (FirstShow)
            notifyIcon.ShowBalloonTip(0);
          break;

        case WindowState.Normal:
          this.ShowInTaskbar = true;
          notifyIcon.Visible = false;
          break;
      }
    }
  }
}
