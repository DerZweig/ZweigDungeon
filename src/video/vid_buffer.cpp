#include "vid_local.h"
#include "../sys_shared.h"

/**************************************************
 * Video Buffer Shutdown
 **************************************************/
VideoBuffer::~VideoBuffer() noexcept
{
        if (!m_data)
        {
                delete[] m_data;
                m_data = nullptr;
        }
}

/**************************************************
 * Video Buffer Resize
 **************************************************/
void VideoBuffer::Resize(uint16_t width, uint16_t height)
{
        if (m_width == width && m_height == height)
        {
                return;
        }

        const auto capacity = static_cast<uint32_t>(width) * height;
        if (capacity != m_capacity)
        {
                delete[] m_data;
                m_data     = nullptr;
                m_data     = new video_color_t[capacity];
                m_capacity = capacity;
        }

        m_width  = width;
        m_height = height;
}

/**************************************************
 * Video Buffer Rendering
 **************************************************/
void VideoBuffer::Clear() noexcept
{
        if (!m_data)
        {
                return;
        }

        std::memset(m_data, 0, sizeof(video_color_t) * m_capacity);
        m_data[0] =
        {
                .red = 255,
                .green = 255,
                .blue = 255,
                .alpha = 255
        };
}

void VideoBuffer::BlitToScreen() const
{
        if (!m_data)
        {
                return;
        }

        auto const pitch = sizeof(video_color_t) * m_width;
        System_BlitToScreen(m_data, static_cast<uint32_t>(pitch), m_height);
}
