#ifndef ZE_VIDEO_H
#define ZE_VIDEO_H

#include "common.h"

/**************************************************
 * VideoScreen Class
 **************************************************/
struct VideoPixel final
{
        uint8_t r;
        uint8_t g;
        uint8_t b;
        uint8_t a;
};

struct VideoScreen : Common
{
protected:
        VideoScreen() = default;
        ~VideoScreen() noexcept;

public:
        VideoScreen(VideoScreen&&)                 = delete;
        VideoScreen(const VideoScreen&)            = delete;
        VideoScreen& operator=(VideoScreen&&)      = delete;
        VideoScreen& operator=(const VideoScreen&) = delete;

        void SetupFrame() override;
        void UpdateFrame() override;

        virtual void SetScreenResolution(uint32_t width, uint32_t height);
private:
        virtual void BlitBuffers(const void* ptr, uint32_t pitch, uint32_t rows) = 0;

        uint32_t    m_width    = 0;
        uint32_t    m_height   = 0;
        uint32_t    m_capacity = 0;
        VideoPixel* m_screen   = nullptr;
};

#endif //ZE_VIDEO_H
