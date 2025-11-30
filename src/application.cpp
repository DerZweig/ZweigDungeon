#include "com_shared.h"
#include "sys_shared.h"
#include "vid_shared.h"
#include "snd_shared.h"
#include "ui_shared.h"
#include "ent_shared.h"

/**************************************************
 * App Init & Shutdown
 **************************************************/
void App_Init(int argc, char** argv)
{
        Common_Init();
        Common_LogInfo("App", "Starting...");

        System_Init();
        Video_Init();
        UI_Init();
        Entity_Init();
}

void App_Shutdown() noexcept
{
        Common_LogInfo("App", "Shutdown...");

        Entity_Shutdown();
        UI_Shutdown();
        Video_Shutdown();
        System_Shutdown();
        Common_Shutdown();
}

/**************************************************
 * App Update
 **************************************************/
void App_Update()
{
        static frame_t frame;
        frame = {};

        Common_SetupFrame(frame); //update timers, config, files, etc...
        System_SetupFrame(frame); //poll input events

        Entity_UpdateFrame(frame); //update world objects
        UI_UpdateFrame(frame); //update GUI widgets
        Sound_UpdateFrame(frame); //mix sounds
        Video_DrawFrame(frame); //draw everything

        System_FinishFrame(frame); //present screen, play sounds
}

/**************************************************
 * App Termination
 **************************************************/
[[noreturn]] void App_Quit() noexcept
{
        App_Shutdown();
        std::exit(EXIT_SUCCESS); // NOLINT(concurrency-mt-unsafe)
}

[[noreturn]] void App_Error(std::string_view where, std::string_view text) noexcept
{
        Common_LogError(where, text);
        App_Quit();
}

/**************************************************
 * App Main
 **************************************************/
void App_Main(int argc, char** argv)
{ // NOLINT(clang-diagnostic-missing-noreturn)
        struct Guard // NOLINT(cppcoreguidelines-special-member-functions)
        {
                ~Guard() noexcept { App_Shutdown(); }
        } g{};

        App_Init(argc, argv);
        //while (true)
        {
                App_Update();
        }
}

/**************************************************
 * Win32 Entry Point
 **************************************************/
#if(WIN32)
/* only use minimal win32 imports from windows header */
#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

/* remove conflicting helper macros from windows header */
#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <Windows.h>

int WINAPI WinMain(_In_ HINSTANCE /*hInstance*/,
                   _In_opt_ HINSTANCE /*hPrevInstance*/,
                   _In_ LPSTR /*lpCmdLine*/,
                   _In_ int /*nCmdShow*/)
{
        App_Main(__argc, __argv);
        return 0;
}
#endif
