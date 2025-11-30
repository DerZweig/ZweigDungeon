#ifndef ZE_SYS_SHARED_H
#define ZE_SYS_SHARED_H

/**************************************************
 * System Interface
 **************************************************/
void System_Init();
void System_Shutdown() noexcept;

void System_SetupFrame(frame_t& frame);
void System_FinishFrame(const frame_t& frame);

#endif //ZE_SYS_SHARED_H