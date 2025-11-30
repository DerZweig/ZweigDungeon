#include "com_local.h"
#include <memory>

static std::unique_ptr<CommonInstance> g_current = {};

/**************************************************
 * Common Startup / Shutdown
 **************************************************/
void Common_Init()
{
        if (g_current)
        {
                return;
        }

        g_current = std::make_unique<CommonInstance>();
        Common_LogInfo("Common", "Initialize");

        g_current->frame_timer.StartOrReset();
}

void Common_Shutdown() noexcept
{
        if (!g_current)
        {
                return;
        }

        Common_LogInfo("Common", "Shutdown");
        g_current.reset();
}

/**************************************************
 * Common Frame
 **************************************************/
void Common_SetupFrame(frame_t& frame)
{
        auto& com = *g_current;

        com.frame_timer.Update();
        com.frame_timer.ExtractStart(frame.timer_started_at);
        com.frame_timer.ExtractNow(frame.frame_started_at);
        frame.timer_elapsed_milliseconds = com.frame_timer.GetElapsedMilliseconds();
}

/**************************************************
 * Common Logger Interface
 **************************************************/
void Common_LogInfo(std::string_view where, std::string_view text)
{
        if (g_current)
        {
                g_current->logger.Print("info", where, text);
        }
}

void Common_LogWarning(std::string_view where, std::string_view text)
{
        if (g_current)
        {
                g_current->logger.Print("warning", where, text);
        }
}

void Common_LogError(std::string_view where, std::string_view text)
{
        if (g_current)
        {
                g_current->logger.Print("error", where, text);
        }
}
