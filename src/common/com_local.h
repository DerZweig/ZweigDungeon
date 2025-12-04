#ifndef ZE_COM_LOCAL_H
#define ZE_COM_LOCAL_H

#include "../com_shared.h"
#include <chrono>
#include <array>
#include <fstream>

/**************************************************
 * Common Logger
 **************************************************/
struct CommonLogger final
{
        CommonLogger();
        CommonLogger(CommonLogger &&)                  = delete;
        CommonLogger(const CommonLogger &)             = delete;
        CommonLogger & operator=(CommonLogger &&)      = delete;
        CommonLogger & operator=(const CommonLogger &) = delete;
        ~CommonLogger() noexcept;

        void Print(std::string_view level, std::string_view where, std::string_view text);

private:
        static constexpr auto LOG_BUFFER_SIZE = 64;

        using log_buffer_t = std::array<char, LOG_BUFFER_SIZE>;

        std::ofstream m_log_file   = {};
        log_buffer_t  m_log_buffer = {};
};

/**************************************************
 * Common Instance
 **************************************************/
struct CommonInstance final
{
        CommonLogger logger = {};
};


#endif //ZE_COM_LOCAL_H
