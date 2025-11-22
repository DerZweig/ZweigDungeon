#ifndef ZE_COMMON_H
#define ZE_COMMON_H

#include <cstdint>
#include <string_view>
#include <optional>

/**************************************************
 * Logger Functions
 **************************************************/
void Log_Info(std::string_view where, std::string_view text);
void Log_Warning(std::string_view where, std::string_view text);
void Log_Error(std::string_view where, std::string_view text);

#endif //ZE_COMMON_H
