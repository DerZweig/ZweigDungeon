#include "sdl_local.h"
#include <optional>
#include <chrono>

static std::optional<SDLInstance> g_current = {};

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

        g_current.emplace();
        g_current->started_at = system_clock::now();
        g_current->input.Initialize();
        g_current->display.Initialize();
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
void System_SetupFrame(frame_t & frame)
{
        using std::chrono::days;
        using std::chrono::milliseconds;

        //update timer before polling events
        auto const now  = system_clock::now();
        auto const base = std::chrono::floor<days>(now);
        auto const ymd  = std::chrono::year_month_day{base};
        auto const hms  = std::chrono::hh_mm_ss{now - base};

        auto & sdl = *g_current;

        sdl.updated_at = now;
        sdl.input.Poll();
        sdl.display.UpdateProperties();

        frame.viewport_width  = sdl.display.GetBufferViewportWidth();
        frame.viewport_height = sdl.display.GetBufferViewportHeight();

        frame.current_date_year   = static_cast<int32_t>(ymd.year());
        frame.current_date_month  = static_cast<uint32_t>(ymd.month());
        frame.current_date_day    = static_cast<uint32_t>(ymd.day());
        frame.current_time_hour   = static_cast<uint32_t>(hms.hours().count());
        frame.current_time_minute = static_cast<uint32_t>(hms.minutes().count());
        frame.current_time_second = static_cast<uint32_t>(hms.seconds().count());

        auto const delta = now - g_current->started_at;
        auto const ms    = std::chrono::duration_cast<milliseconds>(delta);

        frame.milliseconds_since_init = ms.count();
}

void System_FinishFrame(const frame_t & frame)
{
        auto & sdl = *g_current;
        sdl.display.PresentBuffer();
}

void System_BlitToScreen(const void * ptr, uint32_t pitch, uint32_t rows)
{
        auto & sdl = *g_current;
        sdl.display.CopyToBuffer(ptr, pitch, rows);
}
