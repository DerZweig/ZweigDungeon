/* only use minimal win32 imports from windows header */
#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

/* remove conflicting helper macros from windows header */
#ifndef NOMINMAX
#define NOMINMAX
#endif

#include <cstdlib>
#include <Windows.h>

[[noreturn]] void App_Run(int argc, char** argv);

/*****************************************************
 * WinMain
 *
 * Entry Point for Windows Application
 *****************************************************/
int WINAPI WinMain(_In_ HINSTANCE /*hInstance*/,
                   _In_opt_ HINSTANCE /*hPrevInstance*/,
                   _In_ LPSTR /*lpCmdLine*/,
                   _In_ int /*nCmdShow*/)
{
        App_Run(__argc, __argv);
        return 0;
}
