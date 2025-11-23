#include "common.h"
#include "video.h"


/**************************************************
 * VideoScreen Shutdown
 **************************************************/
VideoScreen::~VideoScreen() noexcept
{
        delete[] m_screen;
}

/**************************************************
 * VideoScreen Initialize Components
 **************************************************/
void VideoScreen::InitializeComponents()
{
        Common::InitializeComponents();
        ResizeScreen(256, 256);
}

/**************************************************
 * VideoScreen Frame
 **************************************************/
void VideoScreen::SetupFrame()
{
        Common::SetupFrame();
}

void VideoScreen::UpdateFrame()
{
        if (m_screen == nullptr)
        {
                Common::UpdateFrame();
        }
        else
        {
                std::memset(m_screen, 0, sizeof(VideoPixel) * m_capacity);
                m_screen[0] = {.r = 255, .g = 255, .b = 255, .a = 255};
                Common::UpdateFrame();
                BlitBuffers(m_screen, sizeof(VideoPixel) * m_width, m_height);
        }
}

/**************************************************
 * VideoScreen Set Screen Resolution
 **************************************************/
void VideoScreen::ResizeScreen(uint32_t width, uint32_t height)
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