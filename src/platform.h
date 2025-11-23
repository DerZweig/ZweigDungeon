#ifndef ZE_PLATFORM_H
#define ZE_PLATFORM_H

#include "video.h"

/**************************************************
 * Platform Class
 **************************************************/
struct Platform : VideoScreen
{
protected:
        Platform();
        ~Platform() noexcept;
public:
        Platform(Platform&&) = delete;
        Platform(const Platform&) = delete;
        Platform& operator=(Platform&&) = delete;
        Platform& operator=(const Platform&) = delete;

        void InitializeComponents() override;

        void SetupFrame() override;
        void UpdateFrame() override;
private:
        void AllocateBuffers(uint32_t width, uint32_t height) override;
        void BlitBuffers(const void* ptr, uint32_t pitch, uint32_t rows) override;
        struct Variables;

        Variables* m_vars;
};

#endif //ZE_PLATFORM_H