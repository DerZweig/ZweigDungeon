#version 330 core

layout (location = 0) out vec4 color;

in vec2                sampleCoords;
in vec4                sampleBounds;
in vec4                tintColor;
uniform sampler2D      uTextureSprite;

vec4 sample_sprite(vec2 coords)
{
    vec2 clip = clamp(coords, sampleBounds.xy, sampleBounds.zw);
    ivec2 uvs = ivec2(clip.x, clip.y);
    return texelFetch(uTextureSprite, uvs, 0);
}

void main()
{
    color = sample_sprite(sampleCoords) * tintColor;
}
