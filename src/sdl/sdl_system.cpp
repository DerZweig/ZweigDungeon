#include "sdl_local.h"

/**************************************************
 * SDL System
 **************************************************/
struct SDLSystem final : ISystem
{
        SDLSystem() noexcept                   = default;
        SDLSystem(SDLSystem&&)                 = delete;
        SDLSystem(const SDLSystem&)            = delete;
        SDLSystem& operator=(SDLSystem&&)      = delete;
        SDLSystem& operator=(const SDLSystem&) = delete;
        ~SDLSystem() noexcept override         = default;

        void GetCurrenTime(sys_date_time& dt) override;
        void GetMillisecondsSinceInit() override;
};

/**************************************************
 * System Init & Shutdown
 **************************************************/
std::unique_ptr<ISystem> Sys_Create()
{
        return std::make_unique<SDLSystem>();
}


void SDLSystem::GetCurrenTime(sys_date_time& dt)
{
}

void SDLSystem::GetMillisecondsSinceInit()
{
}