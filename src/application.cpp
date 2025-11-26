#include "common.h"
#include "platform.h"
#include <optional>


/**************************************************
 * Application Class
 **************************************************/
struct Application final : virtual Platform
{
        Application()                              = default;
        ~Application() override                    = default;
        Application(Application&&)                 = delete;
        Application(const Application&)            = delete;
        Application& operator=(Application&&)      = delete;
        Application& operator=(const Application&) = delete;

        void Sys_Start(int argc, char** argv);
        void Sys_Quit() const override;
        void Sys_Error(std::string_view where, std::string_view text) const override;
};


/**************************************************
 * App Run
 **************************************************/
static std::optional<Application> g_current;

[[noreturn]] void App_Run(int argc, char** argv)
{
        g_current.emplace();
        g_current->Sys_Start(argc, argv);
        while (true)
        {
                g_current->SetupFrame();
                g_current->RenderFrame();
        }
}

/**************************************************
 * App Start
 **************************************************/
void Application::Sys_Start(int argc, char** argv)
{
        MakeDisplay(320, 240);
}

/**************************************************
 * App Termination
 **************************************************/
void Application::Sys_Quit() const
{
        g_current.reset();
        std::exit(EXIT_SUCCESS); // NOLINT(concurrency-mt-unsafe)
}

void Application::Sys_Error(std::string_view where, std::string_view text) const
{
        Log_Error(where, text);
        Sys_Quit();
}
