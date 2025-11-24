#ifndef ZE_PLATFORM_H
#define ZE_PLATFORM_H

#include "screen.h"

/**************************************************
 * Platform Class
 **************************************************/
struct Platform : virtual ScreenBuffer
{
        Platform();
        Platform(Platform&&)                 = delete;
        Platform(const Platform&)            = delete;
        Platform& operator=(Platform&&)      = delete;
        Platform& operator=(const Platform&) = delete;
        ~Platform() noexcept override;

        [[nodiscard]] uint16_t HorizontalSize() const noexcept
        {
                return m_scaled_width;
        }

        [[nodiscard]] uint16_t VerticalSize() const noexcept
        {
                return m_scaled_height;
        }

        void MakeDisplay(uint16_t width, uint16_t height) override;
        void SetupFrame() override;
        void RenderFrame() override;

private:
        void ReallocateBuffers(uint16_t width, uint16_t height) override;
        void BlitBuffers(const void* ptr, uint32_t pitch, uint32_t rows) override;
        struct Variables;

        Variables* m_vars;

        uint16_t m_scaled_width;
        uint16_t m_scaled_height;
};

#endif //ZE_PLATFORM_H
