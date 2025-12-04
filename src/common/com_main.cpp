#include "com_local.h"
#include <optional>

static std::optional<CommonInstance> g_current = {};

/**************************************************
 * Common Startup / Shutdown
 **************************************************/
void Common_Init()
{
        if (g_current)
        {
                return;
        }

        g_current.emplace();
        Common_LogInfo("Common", "Initialize");
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
