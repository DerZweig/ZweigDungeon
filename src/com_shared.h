#ifndef ZE_COMMON_H
#define ZE_COMMON_H

#include <cstdint>
#include <string_view>

/**************************************************
 * System Constants
 **************************************************/
constexpr auto APP_TITLE          = "ZweigDungeon";
constexpr auto APP_VERSION        = "0.1.0";
constexpr auto APP_IDENTIFIER     = "com.zweig.dungeon";
constexpr auto APP_DEFAULT_WIDTH  = 800;
constexpr auto APP_DEFAULT_HEIGHT = 600;
constexpr auto APP_VIDEO_WIDTH    = 320;
constexpr auto APP_VIDEO_HEIGHT   = 240;

/**************************************************
 * Common Types
 **************************************************/
struct frame_t
{
        uint16_t viewport_width;
        uint16_t viewport_height;
        int32_t  current_date_year;
        uint32_t current_date_month;
        uint32_t current_date_day;
        uint32_t current_time_hour;
        uint32_t current_time_minute;
        uint32_t current_time_second;
        uint64_t milliseconds_since_init;
};

/**************************************************
 * App Termination
 **************************************************/
[[noreturn]] void App_Quit() noexcept;
[[noreturn]] void App_Error(std::string_view where, std::string_view text) noexcept;

/**************************************************
 * Common Interface
 **************************************************/
void Common_Init();
void Common_Shutdown() noexcept;

void Common_LogInfo(std::string_view where, std::string_view text);
void Common_LogWarning(std::string_view where, std::string_view text);
void Common_LogError(std::string_view where, std::string_view text);

#endif //ZE_COMMON_H
