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

struct ScreenBuffer : virtual Common
{
        ScreenBuffer()                              = default;
        ScreenBuffer(ScreenBuffer&&)                 = delete;
        ScreenBuffer(const ScreenBuffer&)            = delete;
        ScreenBuffer& operator=(ScreenBuffer&&)      = delete;
        ScreenBuffer& operator=(const ScreenBuffer&) = delete;
        ~ScreenBuffer() noexcept override;

        virtual void InitializeDisplay();
        virtual void SetupFrame();
        virtual void RenderFrame();

private:
        void         ResizeScreen(uint32_t width, uint32_t height);
        virtual void AllocateBuffers(uint32_t width, uint32_t height) = 0;
        virtual void BlitBuffers(const void* ptr, uint32_t pitch, uint32_t rows) = 0;

        uint32_t    m_width    = 0;
        uint32_t    m_height   = 0;
        uint32_t    m_capacity = 0;
        VideoPixel* m_screen   = nullptr;
};

#endif //ZE_VIDEO_H
