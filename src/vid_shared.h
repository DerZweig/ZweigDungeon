#ifndef ZE_VID_SHARED_H
#define ZE_VID_SHARED_H

/**************************************************
 * Video Types
 **************************************************/
struct video_color_t final
{
        uint8_t red;
        uint8_t green;
        uint8_t blue;
        uint8_t alpha;
};

/**************************************************
 * Video Interface
 **************************************************/
void Video_Init();
void Video_Shutdown() noexcept;
void Video_DrawFrame(const frame_t& frame);

#endif //ZE_VID_SHARED_H
