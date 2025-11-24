#include "common.h"
#include "application.h"
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

        void Start(int argc, char** argv);
};

/**************************************************
 * App Start
 **************************************************/
void Application::Start(int argc, char** argv)
{
        Platform::InitializeDisplay();
}


/**************************************************
 * App Run
 **************************************************/
static std::optional<Application> g_current;

[[noreturn]] void App_Quit()
{
        g_current.reset();
        std::exit(EXIT_SUCCESS); // NOLINT(concurrency-mt-unsafe)
}


[[noreturn]] void App_Run(int argc, char** argv)
{
        g_current.emplace();
        g_current->Start(argc, argv);
        while (true)
        {
                g_current->SetupFrame();
                g_current->RenderFrame();
        }
}
