#include "common.h"
#include "application.h"
#include "platform.h"
#include "video.h"
#include <SDL3/SDL.h>
#include <SDL3/SDL_render.h>
#include <algorithm>
#include <utility>


/**************************************************
 * SDL Globals & Constants
 **************************************************/
constexpr auto APP_TITLE            = "ZweigDungeon";
constexpr auto APP_VERSION          = "0.1.0";
constexpr auto APP_IDENTIFIER       = "com.zweig.dungeon";
constexpr auto APP_DEFAULT_WIDTH    = 800;
constexpr auto APP_DEFAULT_HEIGHT   = 600;
constexpr auto APP_VIDEO_MIN_WIDTH  = 640;
constexpr auto APP_VIDEO_MIN_HEIGHT = 480;

static SDL_Window*   g_sdl_window;
static SDL_Renderer* g_sdl_renderer;
static SDL_Texture*  g_sdl_screen;
static SDL_FRect     g_src_rect;
static SDL_FRect     g_dst_rect;
static SDL_Rect      g_window_position;
static SDL_Rect      g_window_viewport;
static int           g_screen_width;
static int           g_screen_height;
static int           g_mouse_position_left;
static int           g_mouse_position_top;

/**************************************************
 * SDL Properties
 **************************************************/
int Platform_GetScreenWidth()
{
        return g_screen_width;
}

int Platform_GetScreenHeight()
{
        return g_screen_height;
}

int Platform_GetMousePositionLeft()
{
        return g_mouse_position_left;
}

int Platform_GetMousePositionTop()
{
        return g_mouse_position_top;
}

/**************************************************
 * SDL Initialization
 **************************************************/
bool Platform_Init()
{
        Log_Info("SDL", "Initializing Video...");
        if (!SDL_SetAppMetadata(APP_TITLE, APP_VERSION, APP_IDENTIFIER))
        {
                Log_Error("SDL", "Failed to set application metadata");
                return false;
        }

        if (!SDL_Init(SDL_INIT_VIDEO))
        {
                Log_Error("SDL", "Failed to init video sub system");
                return false;
        }

        if (!SDL_CreateWindowAndRenderer(APP_TITLE,
                                         APP_DEFAULT_WIDTH,
                                         APP_DEFAULT_HEIGHT,
                                         SDL_WINDOW_HIDDEN |
                                         SDL_WINDOW_RESIZABLE,
                                         &g_sdl_window,
                                         &g_sdl_renderer))
        {
                Log_Error("SDL", "Failed to create window & renderer");
                return false;
        }


        if (!SDL_SetRenderVSync(g_sdl_renderer, 1))
        {
                Log_Error("SDL", "Failed to configure renderer");
                return false;
        }

        g_sdl_screen = SDL_CreateTexture(g_sdl_renderer,
                                         SDL_PIXELFORMAT_RGBA32,
                                         SDL_TEXTUREACCESS_STREAMING,
                                         Video_GetMaxHorizontalResolution(),
                                         Video_GetMaxVerticalResolution());

        if (g_sdl_screen == nullptr)
        {
                Log_Error("SDL", "Failed to create screen texture");
                return false;
        }

        if (!SDL_SetTextureBlendMode(g_sdl_screen, SDL_BLENDMODE_NONE) ||
            !SDL_SetTextureScaleMode(g_sdl_screen, SDL_SCALEMODE_NEAREST))
        {
                Log_Error("SDL", "Failed to configure screen texture");
                return false;
        }

        if (!SDL_ShowWindow(g_sdl_window))
        {
                Log_Error("SDL", SDL_GetError());
                return false;
        }

        return true;
}

/**************************************************
 * SDL Shutdown
 **************************************************/
void Platform_Shutdown()
{
        Log_Info("SDL", "Shutdown Video");
        if (g_sdl_renderer)
        {
                SDL_DestroyRenderer(g_sdl_renderer);
                g_sdl_renderer = nullptr;
        }

        if (g_sdl_window)
        {
                SDL_DestroyWindow(g_sdl_window);
                g_sdl_window = nullptr;
        }

        SDL_Quit();
}

/**************************************************
 * SDL Begin Frame
 **************************************************/
bool Platform_BeginFrame()
{
        SDL_Event ev;
        while (g_sdl_window && SDL_PollEvent(&ev))
        {
                switch (ev.type)
                {
                case SDL_EVENT_QUIT:
                        return false;
                default:
                        break;
                }
        }

        if (!g_sdl_window || !g_sdl_renderer)
        {
                return false;
        }

        if (!SDL_GetWindowPosition(g_sdl_window, &g_window_position.x, &g_window_position.y) ||
            !SDL_GetWindowSize(g_sdl_window, &g_window_position.w, &g_window_position.h) ||
            !SDL_GetRenderViewport(g_sdl_renderer, &g_window_viewport))
        {
                Log_Info("SDL", "Failed to read window properties");
                return false;
        }

        g_window_viewport.w = std::max(g_window_viewport.w, APP_VIDEO_MIN_WIDTH);
        g_window_viewport.h = std::max(g_window_viewport.h, APP_VIDEO_MIN_HEIGHT);

        float scaleX = 1.0f;
        float scaleY = 1.0f;
        if (g_window_viewport.w >= g_window_viewport.h)
        {
                scaleY = static_cast<float>(g_window_viewport.h) /
                         static_cast<float>(g_window_position.w);
        }
        else
        {
                scaleX = static_cast<float>(g_window_viewport.w) /
                         static_cast<float>(g_window_position.h);
        }

        g_screen_width  = static_cast<int>(scaleX * static_cast<float>(Video_GetMaxHorizontalResolution()));
        g_screen_height = static_cast<int>(scaleY * static_cast<float>(Video_GetMaxVerticalResolution()));

        if (SDL_HasMouse())
        {
                float left;
                float top;
                (void)SDL_GetGlobalMouseState(&left, &top); //ignore buttons

                //scale position to screen coords
                left -= static_cast<float>(g_window_position.x);
                left /= static_cast<float>(g_window_viewport.w);
                left *= static_cast<float>(g_screen_width);

                top -= static_cast<float>(g_window_position.y);
                top /= static_cast<float>(g_window_viewport.h);
                top *= static_cast<float>(g_screen_height);

                g_mouse_position_left = std::clamp(static_cast<int>(left), 0, g_screen_width - 1);
                g_mouse_position_top  = std::clamp(static_cast<int>(top), 0, g_screen_height - 1);
        }


        return true;
}

/**************************************************
 * SDL Blit Buffers
 **************************************************/
bool SDL_BlitBuffers()
{
        void* screen_pix;
        int   screen_pitch;
        if (!SDL_LockTexture(g_sdl_screen, nullptr, &screen_pix, &screen_pitch))
        {
                Log_Error("SDL", SDL_GetError());
                App_Error("Failed to blit video buffer.");
                return false;
        }

        auto const  pitch  = Video_GetBufferPitch();
        auto* const buffer = Video_GetBufferAddress();
        if (std::cmp_less(screen_pitch, pitch))
        {
                SDL_UnlockTexture(g_sdl_screen);
                return false;
        }


        if (std::cmp_equal(screen_pitch, pitch))
        {
                std::memcpy(screen_pix, buffer, static_cast<size_t>(pitch) * g_screen_height);
        }
        else if (std::cmp_greater_equal(screen_pitch, pitch))
        {
                auto*       dst = static_cast<uint8_t*>(screen_pix);
                const auto* src = static_cast<uint8_t*>(buffer);

                for (auto iter = 0; iter < g_screen_height; ++iter)
                {
                        std::memcpy(dst, src, pitch);
                        dst += screen_pitch;
                        src += pitch;
                }
        }

        SDL_UnlockTexture(g_sdl_screen);
        return true;
}

/**************************************************
 * SDL Finish Frame
 **************************************************/
void Platform_FinishFrame()
{
        if (!g_sdl_window || !g_sdl_renderer)
        {
                return;
        }


        g_src_rect.x = 0.0f;
        g_src_rect.y = 0.0f;
        g_src_rect.w = static_cast<float>(g_screen_width);
        g_src_rect.h = static_cast<float>(g_screen_height);

        g_dst_rect.x = 0.0f;
        g_dst_rect.y = 0.0f;
        g_dst_rect.w = static_cast<float>(g_window_viewport.w);
        g_dst_rect.h = static_cast<float>(g_window_viewport.h);

        if (!SDL_SetRenderDrawColor(g_sdl_renderer, 0, 0, 0, 0) ||
            !SDL_RenderClear(g_sdl_renderer) ||
            !SDL_BlitBuffers() ||
            !SDL_RenderTexture(g_sdl_renderer, g_sdl_screen, &g_src_rect, &g_dst_rect) ||
            !SDL_RenderPresent(g_sdl_renderer))
        {
                Log_Error("SDL", SDL_GetError());
                App_Error("Failed to present video.");
        }
}
