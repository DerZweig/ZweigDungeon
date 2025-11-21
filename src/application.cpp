#include "common.h"
#include "application.h"

struct Application final : IApplication
{
        bool init(int argc, char** argv) override;
        void run() override;
};


static Application instance;
IApplication&      app = instance;


bool Application::init(int argc, char** argv)
{
        return true;
}

void Application::run()
{
}