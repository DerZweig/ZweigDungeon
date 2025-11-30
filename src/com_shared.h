#ifndef ZE_COMMON_H
#define ZE_COMMON_H

#include <cstdint>
#include <string_view>

/**************************************************
 * Common Types
 **************************************************/
struct date_time_t
{
        int32_t  date_year;
        uint32_t date_month;
        uint32_t date_day;
        uint32_t time_hour;
        uint32_t time_minute;
        uint32_t time_second;
};

struct frame_t
{
        uint16_t    viewport_width;
        uint16_t    viewport_height;
        date_time_t frame_started_at;
        date_time_t timer_started_at;
        uint64_t    timer_elapsed_milliseconds;
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
void Common_SetupFrame(frame_t& frame);

void Common_LogInfo(std::string_view where, std::string_view text);
void Common_LogWarning(std::string_view where, std::string_view text);
void Common_LogError(std::string_view where, std::string_view text);

#endif //ZE_COMMON_H
