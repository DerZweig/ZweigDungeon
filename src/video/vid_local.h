#ifndef ZE_VID_LOCAL_H
#define ZE_VID_LOCAL_H

#include "../com_shared.h"
#include "../vid_shared.h"

/**************************************************
 * Video Buffer
 **************************************************/

struct VideoBuffer final
{
        VideoBuffer() noexcept                     = default;
        VideoBuffer(VideoBuffer&&)                 = delete;
        VideoBuffer(const VideoBuffer&)            = delete;
        VideoBuffer& operator=(VideoBuffer&&)      = delete;
        VideoBuffer& operator=(const VideoBuffer&) = delete;
        ~VideoBuffer() noexcept;

        void Resize(uint16_t width, uint16_t height);
        
        void Clear() noexcept;
        void BlitToScreen() const;
private:
        uint16_t       m_width;
        uint16_t       m_height;
        uint32_t       m_capacity;
        video_color_t* m_data;
};

/**************************************************
 * Video Instance
 **************************************************/
struct VideoInstance final
{
        VideoBuffer screen;
};

#endif //ZE_VID_LOCAL_H
