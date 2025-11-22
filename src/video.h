#ifndef ZE_VIDEO_H
#define ZE_VIDEO_H


/**************************************************
 * Video Functions
 **************************************************/
int Video_GetMaxHorizontalResolution();
int Video_GetMaxVerticalResolution();
int Video_GetBufferPitch();

void* Video_GetBufferAddress();
void  Video_DrawScreen();

#endif //ZE_VIDEO_H
