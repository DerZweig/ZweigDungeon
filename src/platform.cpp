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
        uint32_t      screen_width;
        uint32_t      screen_height;
        uint32_t      screen_scaled_width;
        uint32_t      screen_scaled_height;
};


/**************************************************
 * SDL Shutdown
 **************************************************/
Platform::Platform()
{
        m_vars = new Variables();
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

        if (SDL_WasInit(SDL_INIT_VIDEO) || SDL_WasInit(SDL_INIT_AUDIO))
        {
                SDL_Quit();
        }

        delete m_vars;
}

/**************************************************
 * SDL Init
 **************************************************/
void Platform::CreateWindow()
{
        if (SDL_WasInit(SDL_INIT_VIDEO))
        {
                return;
        }

        Log_Info("SDL", "Initializing Platform...");
        if (!SDL_SetAppMetadata(APP_TITLE, APP_VERSION, APP_IDENTIFIER))
        {
                Log_Error("SDL", "Failed to register app metadata");
                Quit();
        }


        if (!SDL_Init(SDL_INIT_VIDEO))
        {
                Log_Error("SDL", "Failed to init video sub system");
                Quit();
        }

        if (!SDL_Init(SDL_INIT_AUDIO))
        {
                Log_Error("SDL", "Failed to init audio sub system");
                Quit();
        }


        if (!SDL_CreateWindowAndRenderer(APP_TITLE,
                                         APP_DEFAULT_WIDTH,
                                         APP_DEFAULT_HEIGHT,
                                         SDL_WINDOW_HIDDEN |
                                         SDL_WINDOW_RESIZABLE,
                                         &m_vars->window,
                                         &m_vars->renderer))
        {
                Log_Error("SDL", "Failed to create window & renderer");
                Quit();
        }

        if (!SDL_SetRenderVSync(m_vars->renderer, 1))
        {
                Log_Error("SDL", "Failed to create window & renderer");
                Quit();
        }

        if (!SDL_ShowWindow(m_vars->window))
        {
                Log_Error("SDL", "Failed to show window");
                Quit();
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
                        Quit();
                        return;
                default:
                        break;
                }
        }

        if (!m_vars->window || !m_vars->renderer)
        {
                VideoScreen::SetupFrame();
        }

        if (!SDL_GetWindowPosition(m_vars->window, &m_vars->window_position.x, &m_vars->window_position.y) ||
            !SDL_GetWindowSize(m_vars->window, &m_vars->window_position.w, &m_vars->window_position.h) ||
            !SDL_GetRenderViewport(m_vars->renderer, &m_vars->window_viewport))
        {
                Log_Error("SDL", "Failed to read window properties");
                Quit();
        }

        m_vars->window_viewport.w = std::max(m_vars->window_viewport.w, APP_VIEWPORT_MIN_WIDTH);
        m_vars->window_viewport.h = std::max(m_vars->window_viewport.h, APP_VIEWPORT_MIN_HEIGHT);

        float scaleX = 1.0f;
        float scaleY = 1.0f;
        if (m_vars->window_viewport.w >= m_vars->window_viewport.h)
        {
                scaleY = static_cast<float>(m_vars->window_viewport.h) / static_cast<float>(m_vars->window_viewport.w);
        }
        else
        {
                scaleX = static_cast<float>(m_vars->window_viewport.w) / static_cast<float>(m_vars->window_viewport.h);
        }

        const float screenW     = std::max(static_cast<float>(m_vars->screen_width), 1.0f);
        const float screenH     = std::max(static_cast<float>(m_vars->screen_height), 1.0f);
        const float aspectRatio = screenW / screenH;

        if (aspectRatio >= 1.0f)
        {
                m_vars->screen_scaled_width  = static_cast<int>(scaleX * screenW / aspectRatio);
                m_vars->screen_scaled_height = static_cast<int>(scaleY * screenH);
        }
        else
        {
                m_vars->screen_scaled_width  = static_cast<int>(scaleX * screenW);
                m_vars->screen_scaled_height = static_cast<int>(scaleY * screenH * aspectRatio);
        }

        VideoScreen::SetupFrame();
}

/**************************************************
 * SDL Update Frame
 **************************************************/
void Platform::UpdateFrame()
{
        static SDL_FRect src_rect;
        static SDL_FRect dst_rect;

        if (m_vars->window == nullptr || m_vars->renderer == nullptr)
        {
                VideoScreen::UpdateFrame();
                return;
        }

        src_rect.x = 0.0f;
        src_rect.y = 0.0f;
        src_rect.w = static_cast<float>(m_vars->screen_scaled_width);
        src_rect.h = static_cast<float>(m_vars->screen_scaled_height);

        dst_rect.x = 0.0f;
        dst_rect.y = 0.0f;
        dst_rect.w = static_cast<float>(m_vars->window_viewport.w);
        dst_rect.h = static_cast<float>(m_vars->window_viewport.h);

        VideoScreen::UpdateFrame();
        if (m_vars->screen)
        {
                if (!SDL_SetRenderDrawColor(m_vars->renderer, 0, 0, 0, 0) ||
                    !SDL_RenderClear(m_vars->renderer) ||
                    !SDL_RenderTexture(m_vars->renderer, m_vars->screen, &src_rect, &dst_rect) ||
                    !SDL_RenderPresent(m_vars->renderer))
                {
                        Log_Error("SDL", "Failed to present screen.");
                        Quit();
                }
        }
        else
        {
                if (!SDL_SetRenderDrawColor(m_vars->renderer, 0, 0, 0, 0) ||
                    !SDL_RenderClear(m_vars->renderer) ||
                    !SDL_RenderPresent(m_vars->renderer))
                {
                        Log_Error("SDL", "Failed to present screen.");
                        Quit();
                }
        }
}

/**************************************************
 * SDL Set Screen Resolution
 **************************************************/
void Platform::SetScreenResolution(uint32_t width, uint32_t height)
{
        VideoScreen::SetScreenResolution(width, height);
        if (m_vars->screen_width == width && m_vars->screen_width == height)
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
                                           static_cast<int>(width),
                                           static_cast<int>(height));

        if (m_vars->screen == nullptr)
        {
                Log_Error("SDL", "Failed to create screen texture");
                Quit();
        }

        if (!SDL_SetTextureBlendMode(m_vars->screen, SDL_BLENDMODE_NONE) ||
            !SDL_SetTextureScaleMode(m_vars->screen, SDL_SCALEMODE_NEAREST))
        {
                Log_Error("SDL", "Failed to configure screen texture");
                Quit();
        }

        m_vars->screen_width  = width;
        m_vars->screen_height = height;
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

        void* screen_pix;
        int   screen_pitch;
        if (!SDL_LockTexture(m_vars->screen, nullptr, &screen_pix, &screen_pitch))
        {
                Log_Error("SDL", "Failed to lock screen texture");
                Quit();
        }

        if (std::cmp_equal(screen_pitch, pitch))
        {
                std::memcpy(screen_pix, ptr, static_cast<size_t>(pitch) * rows);
        }
        else if (std::cmp_greater_equal(screen_pitch, pitch))
        {
                auto* dst = static_cast<uint8_t*>(screen_pix);
                auto* src = static_cast<const uint8_t*>(ptr);

                for (auto iter = 0u; iter < rows; ++iter)
                {
                        std::memcpy(dst, src, pitch);
                        dst += screen_pitch;
                        src += pitch;
                }
        }
        else
        {
                //todo maybe just crash?
                auto* dst = static_cast<uint8_t*>(screen_pix);
                auto* src = static_cast<const uint8_t*>(ptr);

                for (auto iter = 0u; iter < rows; ++iter)
                {
                        std::memcpy(dst, src, screen_pitch);
                        dst += screen_pitch;
                        src += pitch;
                }
        }

        SDL_UnlockTexture(m_vars->screen);
}
