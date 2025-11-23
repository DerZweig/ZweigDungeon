#include "common.h"
#include "application.h"
#include "platform.h"
#include <optional>


/**************************************************
 * Application Class
 **************************************************/
struct Application final : Platform
{
        Application()                              = default;
        ~Application()                             = default;
        Application(Application&&)                 = delete;
        Application(const Application&)            = delete;
        Application& operator=(Application&&)      = delete;
        Application& operator=(const Application&) = delete;

        void Start(int argc, char** argv);
        void SetupFrame() override;
        void UpdateFrame() override;
};

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
                g_current->UpdateFrame();
        }
}

/**************************************************
 * App Start
 **************************************************/
void Application::Start(int argc, char** argv)
{
        CreateWindow();
}

/**************************************************
 * App Frame
 **************************************************/
void Application::SetupFrame()
{
        Platform::SetupFrame();
}

void Application::UpdateFrame()
{
        Platform::UpdateFrame();
}
