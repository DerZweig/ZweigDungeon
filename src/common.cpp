#include "common.h"
#include "application.h"
#include <string>
#include <chrono>

/**************************************************
 * Logger Functions
 **************************************************/
constexpr auto LOG_BUFFER_CAPACITY = 256;

void Log_Print(std::string_view level, std::string_view where, std::string_view text)
{
        thread_local std::string log_buffer;

        auto const now  = std::chrono::system_clock::now();
        auto const secs = std::chrono::time_point_cast<std::chrono::seconds>(now);

        log_buffer.reserve(LOG_BUFFER_CAPACITY);
        log_buffer.clear();

        std::format_to_n(std::back_inserter(log_buffer),
                         LOG_BUFFER_CAPACITY,
                         "{0:%Y %m %d} [{1}] {2} : {3}",
                         secs, level, where, text);

        App_Print(log_buffer);
}

void Log_Info(std::string_view where, std::string_view text)
{
        Log_Print("info", where, text);
}

void Log_Warning(std::string_view where, std::string_view text)
{
        Log_Print("warning", where, text);
}

void Log_Error(std::string_view where, std::string_view text)
{
        Log_Print("error", where, text);
}
