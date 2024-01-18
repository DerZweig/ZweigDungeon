#version 330 core

layout (location = 0) out vec4 color;
in vec2 sampleCoords;
in vec4 tintColor;

uniform sampler2D uTextureSprite;

vec4 sample_sprite(vec2 coords)
{
    ivec2 uvs = ivec2(coords.x, coords.y);
    return texelFetch(uTextureSprite, uvs, 0);
}

void main()
{
    color = sample_sprite(sampleCoords) * tintColor;
}