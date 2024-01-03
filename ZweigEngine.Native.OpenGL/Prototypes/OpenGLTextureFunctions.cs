using ZweigEngine.Native.OpenGL.Constants;

namespace ZweigEngine.Native.OpenGL.Prototypes;

internal delegate void PfnActiveTextureDelegate(OpenGLTextureUnit unit);

internal delegate void PfnBindTextureDelegate(OpenGLTextureTarget target, uint texture);

internal delegate void PfnDeleteTexturesDelegate(int count, uint[] textures);

internal delegate void PfnGenTexturesDelegate(int count, uint[] textures);

internal delegate void PfnTexImage2DDelegate(OpenGLTextureUploadTarget target, int level, OpenGLTextureInternalFormat internalformat, int width, int height, int border, OpenGLTextureComponentFormat format, OpenGLTextureDataType type, IntPtr pixels);

internal delegate void PfnTexParameteriDelegate(OpenGLTextureTarget target, OpenGLTextureParameterName pname, OpenGLTextureParameter param);

internal delegate void PfnTexSubImage2DDelegate(OpenGLTextureUploadTarget target, int level, int xoffset, int yoffset, int width, int height, OpenGLTextureComponentFormat format, OpenGLTextureDataType type, IntPtr pixels);

