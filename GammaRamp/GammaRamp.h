#pragma once


#ifdef GAMMARAMP_EXPORTS
#define GAMMARAMP_API extern "C" __declspec(dllexport)
#else
#define GAMMARAMP_API extern "C" __declspec(dllimport)
#endif

GAMMARAMP_API void GetGammaRamp(WORD* OutGammaArray);
GAMMARAMP_API void SetGammaRamp(const WORD* InGammaArray, float Brightness);