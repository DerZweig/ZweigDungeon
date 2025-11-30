#include "com_local.h"

/**************************************************
 * Common Startup / Shutdown
 **************************************************/
void Common_Init()
{
}

void Common_Shutdown() noexcept
{
}

/**************************************************
 * Common Frame
 **************************************************/
void Common_SetupFrame(frame_t& frame)
{
}

/**************************************************
 * Common Logger Interface
 **************************************************/
void Common_LogInfo(std::string_view where, std::string_view text)
{
}

void Common_LogWarning(std::string_view where, std::string_view text)
{
}

void Common_LogError(std::string_view where, std::string_view text)
{
}
