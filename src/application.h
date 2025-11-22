#ifndef ZE_APPLICATION_H
#define ZE_APPLICATION_H

/**************************************************
 * Application System Interface
 **************************************************/
[[noreturn]] void App_Quit();

bool App_Init(int argc, char** argv);
void App_Run();
void App_Print(std::string_view text);
void App_Error(std::string_view reason);


#endif //ZE_APPLICATION_H
