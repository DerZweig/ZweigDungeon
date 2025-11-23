#include "common.h"
#include "application.h"
#include <optional>

#include "platform.h"

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
        void Quit() override;
        void SetupFrame() override;
        void UpdateFrame() override;
};

static std::optional<Application> g_current;

/**************************************************
 * App Start
 **************************************************/
void Application::Start(int argc, char** argv)
{
        CreateWindow();
}

/**************************************************
 * App Quit
 **************************************************/
void Application::Quit()
{
        g_current.reset();
        std::exit(EXIT_SUCCESS); // NOLINT(concurrency-mt-unsafe)
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

/**************************************************
 * App Run
 **************************************************/
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
