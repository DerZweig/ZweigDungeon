#ifndef ZE_PLATFORM_H
#define ZE_PLATFORM_H

/**************************************************
 * Platform Interface
 **************************************************/
bool Platform_Init();
void Platform_Shutdown();
bool Platform_BeginFrame();
void Platform_FinishFrame();

int Platform_GetScreenWidth();
int Platform_GetScreenHeight();
int Platform_GetMousePositionLeft();
int Platform_GetMousePositionTop();

#endif //ZE_PLATFORM_H
