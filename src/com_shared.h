#ifndef ZE_COMMON_H
#define ZE_COMMON_H

#include <cstdint>
#include <string_view>

/**************************************************
 * Per Frame Data
 **************************************************/
struct frame_t
{
        uint16_t viewport_width;
        uint16_t viewport_height;

        struct
        {
                uint16_t year;
                uint8_t  month;
                uint8_t  day;
        } current_date;

        struct
        {
                uint8_t hour;
                uint8_t minute;
                uint8_t second;
                uint8_t padding;
        } current_time;

        struct
        {
                uint64_t milliseconds;
        } time_since_start;
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
