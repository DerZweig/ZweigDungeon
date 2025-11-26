#ifndef ZE_VIDEO_H
#define ZE_VIDEO_H

#include "common.h"

/**************************************************
 * Screen Class
 **************************************************/
struct Color final
{
        uint8_t r;
        uint8_t g;
        uint8_t b;
        uint8_t a;
};

struct ScreenBuffer : virtual Common
{
        ScreenBuffer()                               = default;
        ScreenBuffer(ScreenBuffer&&)                 = delete;
        ScreenBuffer(const ScreenBuffer&)            = delete;
        ScreenBuffer& operator=(ScreenBuffer&&)      = delete;
        ScreenBuffer& operator=(const ScreenBuffer&) = delete;
        ~ScreenBuffer() noexcept override;

        [[nodiscard]] uint16_t HorizontalCapacity() const noexcept
        {
                return m_logical_width;
        }

        [[nodiscard]] uint16_t VerticalCapacity() const noexcept
        {
                return m_logical_height;
        }

        virtual void MakeDisplay(uint16_t width, uint16_t height);
        virtual void SetupFrame();
        virtual void RenderFrame();

private:
        virtual void ReallocateBuffers(uint16_t width, uint16_t height) = 0;
        virtual void BlitBuffers(const void* src, uint32_t pitch, uint32_t rows) = 0;

        uint16_t m_logical_width    = 0;
        uint16_t m_logical_height   = 0;
        uint32_t m_allocated_width  = 0;
        uint32_t m_allocated_height = 0;
        uint32_t m_capacity         = 0;
        Color*   m_screen           = nullptr;
};

#endif //ZE_VIDEO_H
