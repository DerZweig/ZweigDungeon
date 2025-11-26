#include "platform.h"
#include <SDL3/SDL.h>

/**************************************************
 * SDL Globals & Constants
 **************************************************/
constexpr auto APP_TITLE               = "ZweigDungeon";
constexpr auto APP_VERSION             = "0.1.0";
constexpr auto APP_IDENTIFIER          = "com.zweig.dungeon";
constexpr auto APP_DEFAULT_WIDTH       = 800;
constexpr auto APP_DEFAULT_HEIGHT      = 600;
constexpr auto APP_VIEWPORT_MIN_WIDTH  = 640;
constexpr auto APP_VIEWPORT_MIN_HEIGHT = 480;

struct Platform::Variables
{
        bool          init;
        SDL_Window*   window;
        SDL_Renderer* renderer;
        SDL_Texture*  screen;
        SDL_Rect      window_position;
        SDL_Rect      window_viewport;
        uint32_t      texture_width;
        uint32_t      texture_height;
};


/**************************************************
 * SDL Shutdown
 **************************************************/
Platform::Platform()
{
        m_vars          = new Variables();
        m_scaled_width  = 0;
        m_scaled_height = 0;
}

Platform::~Platform() noexcept
{
        Log_Info("SDL", "Shutdown...");
        if (m_vars->renderer)
        {
                SDL_DestroyRenderer(m_vars->renderer);
        }

        if (m_vars->window)
        {
                SDL_DestroyWindow(m_vars->window);
        }

        if (SDL_WasInit(SDL_INIT_EVENTS))
        {
                SDL_Quit();
        }

        delete m_vars;
}

/**************************************************
 * SDL Init
 **************************************************/
void Platform::MakeDisplay(uint16_t width, uint16_t height)
{
        if (SDL_WasInit(SDL_INIT_VIDEO))
        {
                ScreenBuffer::MakeDisplay(width, height);
                return;
        }

        Log_Info("SDL", "Initializing Platform...");
        if (!SDL_SetAppMetadata(APP_TITLE, APP_VERSION, APP_IDENTIFIER))
        {
                Sys_Error("SDL", "Failed to register app metadata");
        }

        if (!SDL_Init(SDL_INIT_EVENTS))
        {
                Sys_Error("SDL", "Failed to init event sub system");
        }

        if (!SDL_Init(SDL_INIT_VIDEO))
        {
                Sys_Error("SDL", "Failed to init video sub system");
        }


        if (!SDL_CreateWindowAndRenderer(APP_TITLE,
                                         APP_DEFAULT_WIDTH,
                                         APP_DEFAULT_HEIGHT,
                                         SDL_WINDOW_HIDDEN |
                                         SDL_WINDOW_RESIZABLE,
                                         &m_vars->window,
                                         &m_vars->renderer))
        {
                Sys_Error("SDL", "Failed to create window & renderer");
        }

        if (!SDL_SetRenderVSync(m_vars->renderer, 1))
        {
                Sys_Error("SDL", "Failed to configure renderer");
        }

        ScreenBuffer::MakeDisplay(width, height);
        if (!SDL_ShowWindow(m_vars->window))
        {
                Sys_Error("SDL", "Failed to show window");
        }
}


/**************************************************
 * SDL Setup Frame
 **************************************************/
void Platform::SetupFrame()
{
        static SDL_Event event;
        while (SDL_PollEvent(&event))
        {
                switch (event.type)
                {
                case SDL_EVENT_QUIT:
                        Sys_Quit();
                default:
                        break;
                }
        }

        if (!m_vars->window || !m_vars->renderer)
        {
                return;
        }

        if (!SDL_GetWindowPosition(m_vars->window, &m_vars->window_position.x, &m_vars->window_position.y) ||
            !SDL_GetWindowSize(m_vars->window, &m_vars->window_position.w, &m_vars->window_position.h) ||
            !SDL_GetRenderViewport(m_vars->renderer, &m_vars->window_viewport))
        {
                Sys_Error("SDL", "Failed to read window properties");
        }

        m_vars->window_viewport.w = std::max(m_vars->window_viewport.w, APP_VIEWPORT_MIN_WIDTH);
        m_vars->window_viewport.h = std::max(m_vars->window_viewport.h, APP_VIEWPORT_MIN_HEIGHT);


        //scale screen coords for window viewport
        const auto  viewW   = static_cast<float>(m_vars->window_viewport.w);
        const auto  viewH   = static_cast<float>(m_vars->window_viewport.h);
        const float screenW = std::max(HorizontalCapacity(), static_cast<uint16_t>(1u));
        const float screenH = std::max(VerticalCapacity(), static_cast<uint16_t>(1u));
        float       scaleX  = 1.0f;
        float       scaleY  = 1.0f;

        if (m_vars->window_viewport.w >= m_vars->window_viewport.h)
        {
                scaleY = viewH / viewW;
        }
        else
        {
                scaleX = viewW / viewH;
        }

        const float aspectRatio = screenW / screenH;
        if (aspectRatio >= 1.0f)
        {
                scaleX /= aspectRatio;
        }
        else
        {
                scaleY *= aspectRatio;
        }


        m_scaled_width  = static_cast<uint16_t>(screenW * scaleX);
        m_scaled_height = static_cast<uint16_t>(screenH * scaleY);
        ScreenBuffer::SetupFrame();
}

/**************************************************
 * SDL Update Frame
 **************************************************/
void Platform::RenderFrame()
{
        static SDL_FRect src_rect{};
        static SDL_FRect dst_rect{};

        if (!m_vars->screen)
        {
                return;
        }

        ScreenBuffer::RenderFrame();
        src_rect.w = m_scaled_width;
        src_rect.h = m_scaled_height;
        dst_rect.w = static_cast<float>(m_vars->window_viewport.w);
        dst_rect.h = static_cast<float>(m_vars->window_viewport.h);

        if (!SDL_SetRenderDrawColor(m_vars->renderer, 0, 0, 0, 0) ||
            !SDL_RenderClear(m_vars->renderer) ||
            !SDL_RenderTexture(m_vars->renderer, m_vars->screen, &src_rect, &dst_rect) ||
            !SDL_RenderPresent(m_vars->renderer))
        {
                Sys_Error("SDL", "Failed to present screen.");
        }
}

/**************************************************
 * SDL Set Screen Resolution
 **************************************************/
void Platform::ReallocateBuffers(uint16_t width, uint16_t height)
{
        if (m_vars->texture_width == width && m_vars->texture_height == height)
        {
                return;
        }

        if (m_vars->screen != nullptr)
        {
                SDL_DestroyTexture(m_vars->screen);
                m_vars->screen = nullptr;
        }

        m_vars->screen = SDL_CreateTexture(m_vars->renderer,
                                           SDL_PIXELFORMAT_RGBA32,
                                           SDL_TEXTUREACCESS_STREAMING,
                                           width,
                                           height);

        if (m_vars->screen == nullptr)
        {
                Sys_Error("SDL", "Failed to create screen texture");
        }

        if (!SDL_SetTextureBlendMode(m_vars->screen, SDL_BLENDMODE_NONE) ||
            !SDL_SetTextureScaleMode(m_vars->screen, SDL_SCALEMODE_NEAREST))
        {
                Sys_Error("SDL", "Failed to configure screen texture");
        }

        m_vars->texture_width  = width;
        m_vars->texture_height = height;
}

/**************************************************
 * SDL Blit Buffers
 **************************************************/
void Platform::BlitBuffers(const void* ptr, uint32_t pitch, uint32_t rows)
{
        if (m_vars->screen == nullptr)
        {
                return;
        }

        void* dstPtr;
        int   dstPitch;
        if (!SDL_LockTexture(m_vars->screen, nullptr, &dstPtr, &dstPitch))
        {
                Sys_Error("SDL", "Failed to lock screen texture");
        }

        if (std::cmp_equal(dstPitch, pitch))
        {
                std::memcpy(dstPtr, ptr, static_cast<size_t>(pitch) * rows);
        }
        else if (std::cmp_greater_equal(dstPitch, pitch))
        {
                auto* dst = static_cast<uint8_t*>(dstPtr);
                auto* src = static_cast<const uint8_t*>(ptr);

                for (auto iter = 0u; iter < rows; ++iter)
                {
                        std::memcpy(dst, src, pitch);
                        dst += dstPitch;
                        src += pitch;
                }
        }
        SDL_UnlockTexture(m_vars->screen);
}
