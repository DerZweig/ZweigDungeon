#ifndef ZE_COMMON_H
#define ZE_COMMON_H

#include <cstdint>
#include <string_view>

/**************************************************
 * Common Functions
 **************************************************/
struct Common
{
protected:
        Common() noexcept;
        ~Common() noexcept;

public:
        Common(Common&&)                 = delete;
        Common(const Common&)            = delete;
        Common& operator=(Common&&)      = delete;
        Common& operator=(const Common&) = delete;

        virtual void SetupFrame();
        virtual void UpdateFrame();

        [[noreturn]] void Fatal_Error(std::string_view where, std::string_view text) const;

        void Log_Info(std::string_view where, std::string_view text) const;
        void Log_Warning(std::string_view where, std::string_view text) const;
        void Log_Error(std::string_view where, std::string_view text) const;
private:
        void Print(std::string_view level, std::string_view where, std::string_view text) const;

        struct Variables;
        Variables* m_vars;
};


#endif //ZE_COMMON_H
