#version 330 core

layout (location = 0) in vec3  aPosition;
layout (location = 1) in vec4  aParamDestination;
layout (location = 2) in vec4  aParamSource;
layout (location = 3) in vec4  aParamTint;

uniform                  vec2  uViewport;
out vec2                       sampleCoords;
out vec4                       sampleBounds;
out vec4                       tintColor;

void main() {
    vec2 dstPosition  = aParamDestination.xy;
    vec2 dstExtends   = aParamDestination.zw;
    vec2 srcPosition  = aParamSource.xy;
    vec2 srcExtends   = aParamSource.zw;
    vec2 halfView     = uViewport.xy * 0.5f;

    vec2 oPos   = aPosition.xy * dstExtends + dstPosition;
    vec2 nPos   = (oPos - halfView) / halfView;
    gl_Position = vec4(nPos.x, -nPos.y, aPosition.z, 1.0);
    sampleCoords =  aPosition.xy * srcExtends + srcPosition;
    sampleBounds = aParamSource;
    tintColor  = aParamTint / 255.0;
}
