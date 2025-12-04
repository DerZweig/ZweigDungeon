#include "sdl_local.h"
#include <algorithm>

/**************************************************
 * SDLDisplay Shutdown
 **************************************************/
SDLDisplay::~SDLDisplay() noexcept
{
        Common_LogInfo("SDL::Display", "Shutdown");

        if (m_target)
        {
                SDL_DestroyTexture(m_target);
                m_target = nullptr;
        }

        if (m_renderer)
        {
                SDL_DestroyRenderer(m_renderer);
                m_renderer = nullptr;
        }

        if (m_window)
        {
                SDL_DestroyWindow(m_window);
                m_window = nullptr;
        }
}

/**************************************************
 * SDLDisplay Initialization
 **************************************************/
void SDLDisplay::Initialize()
{
        Common_LogInfo("SDL::Display", "Initialize");
        if (!SDL_WasInit(SDL_INIT_VIDEO) && !SDL_Init(SDL_INIT_VIDEO))
        {
                Common_LogInfo("SDL::Display", "Failed to initialize subsystem");
        }

        if (!SDL_CreateWindowAndRenderer(APP_TITLE,
                                         APP_DEFAULT_WIDTH,
                                         APP_DEFAULT_HEIGHT,
                                         SDL_WINDOW_HIDDEN |
                                         SDL_WINDOW_RESIZABLE,
                                         &m_window,
                                         &m_renderer))
        {
                App_Error("SDL::Display", "Failed to create window & renderer");
        }

        if (!SDL_SetWindowMinimumSize(m_window, APP_VIDEO_WIDTH, APP_VIDEO_HEIGHT))
        {
                App_Error("SDL::Display", "Failed to configure window");
        }

        if (!SDL_SetRenderVSync(m_renderer, 1))
        {
                App_Error("SDL::Display", "Failed to configure renderer");
        }

        ResizeBuffer(APP_VIDEO_WIDTH, APP_VIDEO_HEIGHT);

        if (!SDL_ShowWindow(m_window))
        {
                App_Error("SDL::Display", "Failed to show window");
        }
}

/**************************************************
 * SDLDisplay Frame
 **************************************************/
void SDLDisplay::UpdateProperties()
{
        if (!m_window || !m_renderer || !m_target)
        {
                App_Error("SDL::Display", "Attempting to update uninitialized display");
        }

        if (!SDL_GetWindowPosition(m_window, &m_window_position.x, &m_window_position.y) ||
            !SDL_GetWindowSize(m_window, &m_window_position.w, &m_window_position.h) ||
            !SDL_GetRenderViewport(m_renderer, &m_window_viewport))
        {
                App_Error("SDL::Display", "Failed to read display properties");
        }

        //scale screen coords for window viewport
        auto const viewW   = static_cast<float>(m_window_viewport.w);
        auto const viewH   = static_cast<float>(m_window_viewport.h);
        float      screenW = std::max(m_target_width, static_cast<uint16_t>(1u));
        float      screenH = std::max(m_target_height, static_cast<uint16_t>(1u));

        if (m_window_viewport.w >= m_window_viewport.h)
        {
                screenH *= screenW / screenH;
                screenH *= viewH / viewW;
        }
        else
        {
                screenW *= screenH / screenW;
                screenW *= viewW / viewH;
        }

        auto const swI  = std::clamp<int>(static_cast<int>(screenW), 0, std::numeric_limits<uint16_t>::max());
        auto const shI  = std::clamp<int>(static_cast<int>(screenH), 0, std::numeric_limits<uint16_t>::max());
        m_scaled_width  = static_cast<uint16_t>(swI);
        m_scaled_height = static_cast<uint16_t>(shI);
}

void SDLDisplay::PresentBuffer() const
{
        if (!m_window || !m_renderer || !m_target)
        {
                App_Error("SDL::Display", "Attempting to present uninitialized display");
        }

        static SDL_FRect src_rect{};
        static SDL_FRect dst_rect{};

        src_rect.w = m_scaled_width;
        src_rect.h = m_scaled_height;
        dst_rect.w = static_cast<float>(m_window_viewport.w);
        dst_rect.h = static_cast<float>(m_window_viewport.h);


        if (!SDL_SetRenderDrawColor(m_renderer, 0, 0, 0, 0) ||
            !SDL_RenderClear(m_renderer) ||
            !SDL_RenderTexture(m_renderer, m_target, &src_rect, &dst_rect) ||
            !SDL_RenderPresent(m_renderer))
        {
                App_Error("SDL::Display", "Failed to present render target");
        }
}

/**************************************************
 * SDLDisplay Screen Resize
 **************************************************/
void SDLDisplay::ResizeBuffer(uint16_t width, uint16_t height)
{
        if (m_target_width == width && m_target_height == height)
        {
                return;
        }

        if (m_target)
        {
                SDL_DestroyTexture(m_target);
                m_target = nullptr;
        }

        m_target = SDL_CreateTexture(m_renderer,
                                     SDL_PIXELFORMAT_RGBA32,
                                     SDL_TEXTUREACCESS_STREAMING,
                                     width,
                                     height);
        if (!m_target)
        {
                App_Error("SDL::Display", "Failed to allocate render target");
        }

        if (!SDL_SetTextureBlendMode(m_target, SDL_BLENDMODE_NONE) ||
            !SDL_SetTextureScaleMode(m_target, SDL_SCALEMODE_NEAREST))
        {
                App_Error("SDL::Display", "Failed to configure screen render target");
        }

        m_target_width  = width;
        m_target_height = height;
}

/**************************************************
 * SDLDisplay Screen Blit
 **************************************************/
void SDLDisplay::CopyToBuffer(const void* ptr, uint32_t pitch, uint32_t rows) const
{
        if (!m_window || !m_renderer || !m_target)
        {
                App_Error("SDL::Display", "Attempting to update uninitialized render target");
        }

        auto guard = guard_func([this]
        {
                SDL_UnlockTexture(m_target);
        });

        void* dstPtr;
        int   dstPitch;
        if (!SDL_LockTexture(m_target, nullptr, &dstPtr, &dstPitch))
        {
                guard.discard();
                App_Error("SDL::Display", "Failed to lock render target");
        }

        if (std::cmp_equal(dstPitch, pitch))
        {
                std::memcpy(dstPtr, ptr, static_cast<size_t>(pitch) * rows);
        }
        else
        {
                const auto len = std::min(static_cast<long>(pitch), static_cast<long>(dstPitch));

                auto* dst = static_cast<uint8_t*>(dstPtr);
                auto* src = static_cast<const uint8_t*>(ptr);

                for (auto iter = 0u; iter < rows; ++iter)
                {
                        std::memcpy(dst, src, len);
                        dst += dstPitch;
                        src += pitch;
                }
        }
}
