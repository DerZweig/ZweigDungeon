#include "com_local.h"

/**************************************************
 * Logger Init
 **************************************************/
CommonLogger::CommonLogger()
{
        m_log_file.open("current.log");
}


CommonLogger::~CommonLogger() noexcept
{
        m_log_file.close();
}

/**************************************************
 * Logger Print
 **************************************************/
void CommonLogger::Print(std::string_view level, std::string_view where, std::string_view text)
{
        using std::chrono::system_clock;
        using std::chrono::seconds;

        auto const now = system_clock::now();
        auto const sec = std::chrono::time_point_cast<seconds>(now);

        auto const beg = m_log_buffer.begin();
        auto const end = std::format_to_n(beg, LOG_BUFFER_SIZE,
                                          "{0:%Y/%m/%d %H:%M:%S} [{1}] {2}: {3}\0",
                                          sec, level, where, text).out;

        m_log_file << std::string_view{beg, end} << '\n';
}
