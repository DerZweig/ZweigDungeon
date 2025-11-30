#include "sdl_local.h"
#include <memory>

static std::unique_ptr<SDLInstance> g_current = {};

/**************************************************
 * Platform Init & Shutdown
 **************************************************/
void System_Init()
{
        if (g_current)
        {
                return;
        }

        Common_LogInfo("SDL", "Initialize");
        if (!SDL_SetAppMetadata(APP_TITLE, APP_VERSION, APP_IDENTIFIER))
        {
                App_Error("SDL", "Failed to register metadata");
        }

        g_current = std::make_unique<SDLInstance>();
        g_current->input.Initialize();
        g_current->display.Initialize();
        g_current->sound.Initialize();
}

void System_Shutdown() noexcept
{
        if (!g_current)
        {
                return;
        }

        g_current.reset();
        SDL_Quit();
}

/**************************************************
 * Platform Frame
 **************************************************/
void System_SetupFrame(frame_t& frame)
{
        auto& sdl = *g_current;

        sdl.input.Poll();
        sdl.display.UpdateProperties();

        frame.viewport_width  = sdl.display.GetBufferViewportWidth();
        frame.viewport_height = sdl.display.GetBufferViewportHeight();
}

void System_FinishFrame(const frame_t& frame)
{
        auto& sdl = *g_current;
        sdl.display.PresentBuffer();
}

void System_BlitToScreen(const void* ptr, uint32_t pitch, uint32_t rows)
{
        auto& sdl = *g_current;
        sdl.display.CopyToBuffer(ptr, pitch, rows);
}
