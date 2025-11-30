#ifndef ZE_VID_SHARED_H
#define ZE_VID_SHARED_H

/**************************************************
 * Video Interface
 **************************************************/
void Video_Init();
void Video_Shutdown() noexcept;
void Video_DrawFrame(const frame_t& frame);

#endif //ZE_VID_SHARED_H