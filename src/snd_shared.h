#ifndef ZE_SND_SHARED_H
#define ZE_SND_SHARED_H

/**************************************************
 * Sound Interface
 **************************************************/
void Sound_Init();
void Sound_Shutdown() noexcept;
void Sound_UpdateFrame(const frame_t& frame);

#endif //ZE_SND_SHARED_H