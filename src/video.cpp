#include "common.h"
#include "video.h"

#include "platform.h"

/**************************************************
 * Video Interface
 **************************************************/
struct VideoPixel final
{
        uint8_t r;
        uint8_t g;
        uint8_t b;
        uint8_t a;
};

constexpr auto VIDEO_MAX_RESOLUTION_X = 256;
constexpr auto VIDEO_MAX_RESOLUTION_Y = 256;

static VideoPixel Video_Buffer[VIDEO_MAX_RESOLUTION_X * VIDEO_MAX_RESOLUTION_Y] = {};

/**************************************************
 * Video Properties
 **************************************************/
int Video_GetBufferPitch()
{
        return VIDEO_MAX_RESOLUTION_X * sizeof(VideoPixel);
}

int Video_GetMaxHorizontalResolution()
{
        return VIDEO_MAX_RESOLUTION_X;
}

int Video_GetMaxVerticalResolution()
{
        return VIDEO_MAX_RESOLUTION_Y;
}

void* Video_GetBufferAddress()
{
        return Video_Buffer;
}

/**************************************************
 * Video Draw Screen
 **************************************************/
void Video_DrawScreen()
{
        std::memset(Video_Buffer, 0, sizeof(Video_Buffer));


        auto const mx = Platform_GetMousePositionLeft() & 0xFF;
        auto const my = Platform_GetMousePositionTop() & 0xFF;
        auto const ma = (my << 8) + mx;

        Video_Buffer[ma] = {.r = 255, .g = 255, .b = 255, .a = 255};
}
