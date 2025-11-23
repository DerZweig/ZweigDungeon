#include "common.h"
#include <string>
#include <chrono>
#include <fstream>

/**************************************************
 * Common Globals & Constants
 **************************************************/
static constexpr auto LOG_BUFFER_CAPACITY = 256;

struct Common::Variables
{
        std::string   m_log_text{};
        std::ofstream m_log_file{};
};

Common::Common() noexcept
{
        m_vars = new Variables();
        m_vars->m_log_text.reserve(LOG_BUFFER_CAPACITY);
        m_vars->m_log_file.open("current.log", std::ios::out);
}

Common::~Common() noexcept
{
        delete m_vars;
}

/**************************************************
 * Common Frame
 **************************************************/
void Common::SetupFrame()
{

}

void Common::UpdateFrame()
{
}

/**************************************************
 * Logger Functions
 **************************************************/
void Common::Log_Info(std::string_view where, std::string_view text) const
{
        Print("info", where, text);
}

void Common::Log_Warning(std::string_view where, std::string_view text) const
{
        Print("warning", where, text);
}

void Common::Log_Error(std::string_view where, std::string_view text) const
{
        Print("error", where, text);
}

void Common::Print(std::string_view level, std::string_view where, std::string_view text) const
{
        auto const now  = std::chrono::system_clock::now();
        auto const secs = std::chrono::time_point_cast<std::chrono::seconds>(now);

        m_vars->m_log_text.clear();
        std::format_to_n(std::back_inserter(m_vars->m_log_text),
                         LOG_BUFFER_CAPACITY,
                         "{0:%Y %m %d} [{1}] {2} : {3}",
                         secs, level, where, text);

        m_vars->m_log_file << m_vars->m_log_text << "\n";
}
