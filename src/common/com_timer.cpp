#include "com_local.h"

/**************************************************
 * Common Timer Elapsed
 **************************************************/
uint64_t CommonTimer::GetElapsedMilliseconds() const noexcept
{
        auto const delta    = m_started_at - m_current_now;
        auto const delta_ms = std::chrono::duration_cast<milliseconds>(delta);

        return delta_ms.count();
}

/**************************************************
 * Common Timer Controls
 **************************************************/
void CommonTimer::StartOrReset() noexcept
{
        m_was_started = true;
        m_started_at  = system_clock::now();
        m_current_now = m_started_at;
}

void CommonTimer::Update() noexcept
{
        if (m_was_started)
        {
                m_current_now = system_clock::now();
        }
}

/**************************************************
 * Common Timer Extract
 **************************************************/
void CommonTimer::ExtractStart(date_time_t& dst) const noexcept
{
        auto const base = std::chrono::floor<days>(m_started_at);
        auto const ymd  = std::chrono::year_month_day{base};
        auto const hms  = std::chrono::hh_mm_ss{m_started_at - base};

        dst.date_year   = static_cast<int32_t>(ymd.year());
        dst.date_month  = static_cast<uint32_t>(ymd.month());
        dst.date_day    = static_cast<uint32_t>(ymd.day());
        dst.time_hour   = static_cast<uint32_t>(hms.hours().count());
        dst.time_minute = static_cast<uint32_t>(hms.minutes().count());
        dst.time_second = static_cast<uint32_t>(hms.seconds().count());
}


void CommonTimer::ExtractNow(date_time_t& dst) const noexcept
{
        auto const base = std::chrono::floor<days>(m_current_now);
        auto const ymd  = std::chrono::year_month_day{base};
        auto const hms  = std::chrono::hh_mm_ss{m_current_now - base};

        dst.date_year   = static_cast<int32_t>(ymd.year());
        dst.date_month  = static_cast<uint32_t>(ymd.month());
        dst.date_day    = static_cast<uint32_t>(ymd.day());
        dst.time_hour   = static_cast<uint32_t>(hms.hours().count());
        dst.time_minute = static_cast<uint32_t>(hms.minutes().count());
        dst.time_second = static_cast<uint32_t>(hms.seconds().count());
}
