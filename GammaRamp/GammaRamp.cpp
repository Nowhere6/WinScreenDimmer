#include "pch.h" // use stdafx.h in Visual Studio 2017 and earlier
#include "GammaRamp.h"
#include <iostream>
#include <windows.h>
#include <vector>

using namespace std;

void GetGammaRamp(WORD* OutGammaArray)
{
  DISPLAY_DEVICE tempDevice;
  tempDevice.cb = sizeof(DISPLAY_DEVICE);
  EnumDisplayDevicesW(NULL, 0, &tempDevice, 0);
  HDC dc = CreateDCW(L"DISPLAY", tempDevice.DeviceName, NULL, NULL);
  GetDeviceGammaRamp(dc, OutGammaArray);
  DeleteDC(dc);
}

void SetGammaRamp(const WORD* InGammaArray, float Brightness)
{
  std::vector<HDC> hdcs;
  std::vector<DISPLAY_DEVICE> devices;
  WCHAR FirstAdaptor[256] = {};
  WORD GammaArray[3 * 256] = {};
  DISPLAY_DEVICE tempDevice{ sizeof(DISPLAY_DEVICE) };

  // Store display_device for first adaptor
  for (int i = 0; EnumDisplayDevicesW(NULL, i, &tempDevice, 0); ++i)
  {
    if (i == 0)
      wcscpy_s(FirstAdaptor, tempDevice.DeviceString);
    else if (wcscmp(FirstAdaptor, tempDevice.DeviceString) != 0)
      continue;
    devices.push_back(tempDevice);
  }

  // Create DC for every monitor.
  // DeviceName like this: L"\\\\.\\DISPLAY1"
  for (const DISPLAY_DEVICE& device : devices)
  {
    HDC tempDc = CreateDCW(L"DISPLAY", &device.DeviceName[0], NULL, NULL);
    // Ignore invalid hdc
    if (tempDc == 0)
      continue;
    hdcs.push_back(tempDc);
  };

  // Compute new gamma array
  for (INT32 i = 0; i < 256; i++)
  {
    GammaArray[0 + i] = InGammaArray[0 + i] * Brightness;
    GammaArray[256 + i] = InGammaArray[256 + i] * Brightness;
    GammaArray[512 + i] = InGammaArray[512 + i] * Brightness;
  }


  // Set gamma array
  for (const HDC& hdc : hdcs)
  {
    SetDeviceGammaRamp(hdc, GammaArray);
    DeleteDC(hdc);
  }
}