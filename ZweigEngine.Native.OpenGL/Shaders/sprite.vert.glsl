#version 330 core
layout (location = 0) in vec3  aPosition;
layout (location = 1) in vec4  aParamDestination;
layout (location = 2) in vec4  aParamSource;
layout (location = 3) in vec4  aParamTint;
layout (location = 4) in vec4  aParamFlags;

uniform                  vec2  uViewport;
out vec2                       sampleCoords;
out vec4                       tintColor;

void main()
{
    vec2 dstPosition  = aParamDestination.xy;
    vec2 dstExtends   = aParamDestination.zw;
    vec2 srcPosition  = aParamSource.xy;
    vec2 srcExtends   = aParamSource.zw;

    vec2 halfView     = uViewport.xy * 0.5f;
    vec2 outputPos    = aPosition.xy * dstExtends + dstPosition;
    vec2 normalizePos = (outputPos - halfView) / halfView;

    vec2 mirrorFlags    = clamp(aParamFlags.xy, 0.0f, 1.0f);
    vec2 mirrorFlagsInv = vec2(1.0f, 1.0f) - mirrorFlags;
    
    vec2 texCoordsNormal = mirrorFlagsInv * aPosition.xy;
    vec2 texCoordsInv    = mirrorFlags * (vec2(1.0f, 1.0f) - aPosition.xy);

    gl_Position = vec4(normalizePos.x, -normalizePos.y, aPosition.z, 1.0);
    sampleCoords = (texCoordsNormal + texCoordsInv) * srcExtends + srcPosition;
    tintColor  = aParamTint / 255.0;
}