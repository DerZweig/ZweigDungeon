#include "common.h"
#include "screen.h"


/**************************************************
 * Screen Shutdown
 **************************************************/
ScreenBuffer::~ScreenBuffer() noexcept
{
        delete[] m_screen;
}

/**************************************************
 * Screen Display Creation
 **************************************************/
void ScreenBuffer::MakeDisplay(uint16_t width, uint16_t height)
{
        m_logical_width  = width;
        m_logical_height = height;

        //align sizes
        width  = (width + 0xE) & ~0xF;
        height = (height + 0xE) & ~0xF;

        const auto capacity = static_cast<uint32_t>(width) * height;
        if (capacity != m_capacity)
        {
                auto* ptr = new VideoPixel[capacity];
                std::swap(m_screen, ptr);
                delete[] ptr;

                m_capacity = capacity;
        }

        m_allocated_width  = width;
        m_allocated_height = height;
        ReallocateBuffers(width, height);
}

/**************************************************
 * Screen Frame
 **************************************************/
void ScreenBuffer::SetupFrame()
{
        std::memset(m_screen, 0, sizeof(VideoPixel) * m_capacity);
        m_screen[0] = {.r = 255, .g = 255, .b = 255, .a = 255};
}

void ScreenBuffer::RenderFrame()
{
        BlitBuffers(m_screen, sizeof(VideoPixel) * m_allocated_width, m_allocated_height);
}
