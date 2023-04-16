using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SceenDimmer
{
  using static System.Environment;
  public static class Utils
  {
    private static UInt16[] GammaArray = new UInt16[3 * 256];
    public static readonly string GammaPath;
    public static readonly string SavePath;
    public static readonly string lnk_Path;

    static Utils()
    {
      lnk_Path = Path.Combine(GetFolderPath(SpecialFolder.Startup), "Dimmer.lnk");
      string path = GetFolderPath(SpecialFolder.ApplicationData);
      path = Path.Combine(path, "ScreenDimmer");
      Directory.CreateDirectory(path);
      GammaPath = Path.Combine(path, "GammaArray");
      SavePath = Path.Combine(path, "save.ini");

      int byteCount = sizeof(UInt16) * GammaArray.Length;
      byte[] bytes = new byte[byteCount];
      // Read gamma data
      if (File.Exists(GammaPath))
      {
        using (FileStream fs = new FileStream(GammaPath, FileMode.Open, FileAccess.Read))
        {
          fs.Read(bytes, 0, byteCount);
          Buffer.BlockCopy(bytes, 0, GammaArray, 0, byteCount);
        }
      }
      // Store gamma data of first monitor.
      else
      {
        GCHandle GCHandle = GCHandle.Alloc(GammaArray, GCHandleType.Pinned);
        GetGammaRamp(GCHandle.AddrOfPinnedObject());
        GCHandle.Free();
        using (FileStream fs = new FileStream(GammaPath, FileMode.Create))
        {
          Buffer.BlockCopy(GammaArray, 0, bytes, 0, byteCount);
          fs.Write(bytes, 0, byteCount);
        }
      }
    }

    public static void SetGammaRamp(float Brightness)
    {
      if(Brightness < 0.1f || Brightness > 1.0f)
      {
        throw new ArgumentOutOfRangeException("Brightness", "Value should between 0.1 and 1.0.");
      }

      GCHandle GCHandle = GCHandle.Alloc(GammaArray, GCHandleType.Pinned);
      SetGammaRamp(GCHandle.AddrOfPinnedObject(), Brightness);
      GCHandle.Free();
    }

    [DllImport("GammaRamp.dll")]
    private static extern int SetGammaRamp(IntPtr OutGammaArray, float Brightness);

    [DllImport("GammaRamp.dll")]
    private static extern void GetGammaRamp(IntPtr InGammaArray);
  }
}
