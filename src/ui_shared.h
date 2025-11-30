#ifndef ZE_UI_SHARED_H
#define ZE_UI_SHARED_H

/**************************************************
 * UI Interface
 **************************************************/
void UI_Init();
void UI_Shutdown() noexcept;
void UI_UpdateFrame(const frame_t& frame);

#endif //ZE_UI_SHARED_H