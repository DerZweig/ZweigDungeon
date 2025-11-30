#ifndef ZE_COM_LOCAL_H
#define ZE_COM_LOCAL_H

#include "../com_shared.h"
#include <chrono>
#include <array>
#include <fstream>

/**************************************************
 * Common Timer
 **************************************************/
struct CommonTimer final
{
        CommonTimer() noexcept                     = default;
        CommonTimer(CommonTimer&&)                 = delete;
        CommonTimer(const CommonTimer&)            = delete;
        CommonTimer& operator=(CommonTimer&&)      = delete;
        CommonTimer& operator=(const CommonTimer&) = delete;
        ~CommonTimer() noexcept                    = default;

        [[nodiscard]] bool IsStarted() const noexcept
        {
                return m_was_started;
        }

        [[nodiscard]] uint64_t GetElapsedMilliseconds() const noexcept;

        void StartOrReset() noexcept;
        void Update() noexcept;
        void ExtractStart(date_time_t& dst) const noexcept;
        void ExtractNow(date_time_t& dst) const noexcept;

private:
        using system_clock = std::chrono::system_clock;
        using time_point   = std::chrono::system_clock::time_point;
        using days         = std::chrono::days;
        using milliseconds = std::chrono::milliseconds;

        time_point m_started_at  = {};
        time_point m_current_now = {};
        bool       m_was_started = false;
};

/**************************************************
 * Common Logger
 **************************************************/
struct CommonLogger final
{
        CommonLogger();
        CommonLogger(CommonLogger&&)                 = delete;
        CommonLogger(const CommonLogger&)            = delete;
        CommonLogger& operator=(CommonLogger&&)      = delete;
        CommonLogger& operator=(const CommonLogger&) = delete;
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
        CommonTimer  frame_timer = {};
        CommonLogger logger      = {};
};


#endif //ZE_COM_LOCAL_H
