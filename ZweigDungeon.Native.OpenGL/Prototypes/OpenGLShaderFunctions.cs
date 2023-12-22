using System.Numerics;
using System.Runtime.InteropServices;
using ZweigDungeon.Native.OpenGL.Constants;

namespace ZweigDungeon.Native.OpenGL.Prototypes;

internal delegate void PfnAttachShaderDelegate(uint program, uint shader);

internal delegate void PfnCompileShaderDelegate(uint shader);

internal delegate uint PfnCreateProgramDelegate();

internal delegate uint PfnCreateShaderDelegate(OpenGLShaderType openGLShaderType);

internal delegate void PfnDeleteProgramDelegate(uint program);

internal delegate void PfnDeleteShaderDelegate(uint shader);

internal delegate void PfnDetachShaderDelegate(uint program, uint shader);

internal delegate void PfnGetProgramDelegate(uint program, OpenGLProgramProperty property, ref int result);

internal delegate void PfnGetProgramInfoLogDelegate(uint program, uint maxLength, ref uint length, IntPtr infoLog);

internal delegate void PfnGetShaderDelegate(uint shader, OpenGLShaderProperty property, ref int result);

internal delegate void PfnGetShaderInfoLogDelegate(uint shader, uint maxLength, ref uint length, IntPtr infoLog);

internal delegate void PfnLinkProgramDelegate(uint program);

internal delegate void PfnShaderSourceDelegate(uint shader, uint count,
                                               [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
                                               string[] sources,
                                               [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)]
                                               int[] lengths);

internal delegate void PfnUseProgramDelegate(uint program);

internal delegate int PfnGetAttributeLocationDelegate(uint program, [MarshalAs(UnmanagedType.LPStr)] string name);

internal delegate int PfnGetUniformLocationDelegate(uint program, [MarshalAs(UnmanagedType.LPStr)] string name);

internal delegate void PfnUniform1IDelegate(int location, int v0);

internal delegate void PfnUniform1FDelegate(int location, float v0);

internal delegate void PfnUniform2FDelegate(int location, float v0, float v1);

internal delegate void PfnUniform3FDelegate(int location, float v0, float v1, float v2);

internal delegate void PfnUniform4FDelegate(int location, float v0, float v1, float v2, float v3);

internal delegate void PfnUniformMatrix4FvDelegate(int location, int count, bool transpose, Matrix4x4[] matrix);