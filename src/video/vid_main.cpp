#include "vid_local.h"
#include <optional>

static std::optional<VideoInstance> g_current = {};

/**************************************************
 * Video Init & Shutdown
 **************************************************/
void Video_Init()
{
        if (g_current)
        {
                return;
        }

        Common_LogInfo("Video", "Initialize");
        g_current.emplace();
        g_current->screen.Resize(APP_VIDEO_WIDTH, APP_VIDEO_HEIGHT);
}

void Video_Shutdown() noexcept
{
        if (!g_current)
        {
                return;
        }

        Common_LogInfo("Video", "Shutdown");
        g_current.reset();
}

/**************************************************
 * Video Frame
 **************************************************/
void Video_DrawFrame(const frame_t& frame)
{
        auto& vid = *g_current;
        vid.screen.Clear();

        vid.screen.BlitToScreen();
}
