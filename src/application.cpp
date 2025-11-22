#include "common.h"
#include "application.h"
#include "platform.h"
#include <fstream>

#include "video.h"

/**************************************************
 * Application Globals
 **************************************************/

static std::optional<std::ofstream> g_log_file;

/**************************************************
 * Application Quit & Shutdown
 **************************************************/
struct AppExitException final : std::exception
{
        [[nodiscard]] char const* what() const override
        {
                return "Application Exit";
        }
};

[[noreturn]] void App_Quit()
{
        throw AppExitException();
}

void App_Shutdown()
{
        Platform_Shutdown();
        g_log_file.reset();
}

/**************************************************
 * Application Init
 **************************************************/
bool App_Init(int argc, char** argv)
{
        try
        {
                g_log_file.emplace();
                g_log_file->open("current.log");
                if (!g_log_file->is_open())
                {
                        return false;
                }

                if (!Platform_Init())
                {
                        return false;
                }

                return true;
        }
        catch (const std::exception& /*ignored*/)
        {
                App_Shutdown();
                return false;
        }
}

/**************************************************
 * Application Run
 **************************************************/
void App_Run()
{
        try
        {
                while (Platform_BeginFrame())
                {
                        Video_DrawScreen();
                        Platform_FinishFrame();
                }
        }
        catch (const std::exception& ex)
        {
                App_Print(ex.what());
        }

        App_Shutdown();
}

/**************************************************
 * Application Print
 **************************************************/
void App_Print(std::string_view text)
{
        if (!g_log_file)
        {
                return;
        }

        g_log_file.value() << text << "\n";
}

/**************************************************
 * Application Error
 **************************************************/
void App_Error(std::string_view reason)
{
        App_Print(reason);
        App_Quit();
}
