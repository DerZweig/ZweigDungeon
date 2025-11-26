#include "common.h"
#include <array>
#include <chrono>
#include <fstream>

/**************************************************
 * Common Globals & Constants
 **************************************************/
static constexpr auto LOG_BUFFER_CAPACITY = 128;

using log_buffer_t = std::array<char, LOG_BUFFER_CAPACITY>;

struct Common::Variables
{
        std::ofstream log_file{};
        log_buffer_t  log_text{};
};

Common::Common() noexcept
{
        m_vars = new Variables();
        m_vars->log_file.open("current.log", std::ios::out);
}

Common::~Common() noexcept
{
        delete m_vars;
}

/**************************************************
 * Logger Functions
 **************************************************/
void Common::Log_Info(std::string_view where, std::string_view text) const
{
        Log_Print("info", where, text);
}

void Common::Log_Warning(std::string_view where, std::string_view text) const
{
        Log_Print("warning", where, text);
}

void Common::Log_Error(std::string_view where, std::string_view text) const
{
        Log_Print("error", where, text);
}

void Common::Log_Print(std::string_view level, std::string_view where, std::string_view text) const
{
        using std::chrono::system_clock;
        using std::chrono::seconds;

        auto const now = system_clock::now();
        auto const sec = std::chrono::time_point_cast<seconds>(now);

        auto const beg = m_vars->log_text.begin();
        auto const end = std::format_to_n(beg, LOG_BUFFER_CAPACITY,
                                          "{0:%Y %m %d} [{1}] {2} : {3}\0",
                                          sec, level, where, text).out;

        m_vars->log_file << std::string_view{beg, end} << '\n';
}
