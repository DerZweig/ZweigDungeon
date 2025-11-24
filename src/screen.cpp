#include "common.h"
#include "screen.h"


/**************************************************
 * VideoScreen Shutdown
 **************************************************/
ScreenBuffer::~ScreenBuffer() noexcept
{
        delete[] m_screen;
}

/**************************************************
 * VideoScreen Initialize Components
 **************************************************/
void ScreenBuffer::InitializeDisplay()
{
        ResizeScreen(256, 256);
}

/**************************************************
 * VideoScreen Frame
 **************************************************/
void ScreenBuffer::SetupFrame()
{
        std::memset(m_screen, 0, sizeof(VideoPixel) * m_capacity);
}

void ScreenBuffer::RenderFrame()
{
        m_screen[0] = { .r = 255, .g = 255, .b = 255, .a = 255 };
        BlitBuffers(m_screen, sizeof(VideoPixel) * m_width, m_height);
}

/**************************************************
 * VideoScreen Set Screen Resolution
 **************************************************/
void ScreenBuffer::ResizeScreen(uint32_t width, uint32_t height)
{
        if (m_width == width || m_height == height)
        {
                return;
        }

        m_width = width;
        m_height = height;

        const auto capacity = width * height;
        if (capacity > m_capacity)
        {
                auto* ptr = new VideoPixel[capacity];
                std::swap(m_screen, ptr);
                delete[] ptr;

                m_capacity = capacity;
        }

        AllocateBuffers(width, height);
}