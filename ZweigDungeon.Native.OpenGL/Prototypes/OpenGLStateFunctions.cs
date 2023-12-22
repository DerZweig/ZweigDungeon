using ZweigDungeon.Native.OpenGL.Constants;

namespace ZweigDungeon.Native.OpenGL.Prototypes;

internal delegate void PfnBlendColorDelegate(float red, float green, float blue, float alpha);

internal delegate void PfnBlendFunctionDelegate(OpenGLBlendSourceFactor sourceFactor, OpenGLBlendDestinationFactor destinationFactor);

internal delegate void PfnClearColorDelegate(float r, float g, float b, float a);

internal delegate void PfnClearDelegate(OpenGLClearMask mask);

internal delegate void PfnClearDepthDelegate(double d);

internal delegate void PfnCullFaceDelegate(OpenGLCullFaceMode mode);

internal delegate void PfnDepthFunctionDelegate(OpenGLDepthFunction func);

internal delegate void PfnDepthMaskDelegate(bool flag);

internal delegate void PfnColorMaskDelegate(bool red, bool green, bool blue, bool alpha);

internal delegate void PfnDisableDelegate(OpenGLEnableCap cap);

internal delegate void PfnEnableDelegate(OpenGLEnableCap cap);

internal delegate void PfnFrontFaceDelegate(OpenGLFrontFaceMode mode);

internal delegate OpenGLErrror PfnGetErrorDelegate();

internal delegate void PfnScissorDelegate(int x, int y, int width, int height);

internal delegate void PfnViewportDelegate(int x, int y, int width, int height);

