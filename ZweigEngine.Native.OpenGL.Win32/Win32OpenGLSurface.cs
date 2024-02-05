using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Native.OpenGL.Interfaces;
using ZweigEngine.Native.OpenGL.Win32.Constants;
using ZweigEngine.Native.OpenGL.Win32.Prototypes;
using ZweigEngine.Native.OpenGL.Win32.Structures;

namespace ZweigEngine.Native.OpenGL.Win32;

public class Win32OpenGLSurface : IDisposable, IOpenGLSurface
{
	private const byte                                PIXEL_FORMAT_COLOR_BITS             = 32;
	private const byte                                PIXEL_FORMAT_DEPTH_BITS             = 24;
	private const byte                                PIXEL_FORMAT_STENCIL_BITS           = 8;
	private const Win32PixelFormatDescriptorFlags     PIXEL_FORMAT_FLAGS                  = Win32PixelFormatDescriptorFlags.DrawToWindow | Win32PixelFormatDescriptorFlags.SupportOpenGL | Win32PixelFormatDescriptorFlags.Doublebuffer;
	private const Win32PixelFormatDescriptorPixelType PIXEL_FORMAT_TYPE                   = Win32PixelFormatDescriptorPixelType.TypeRgba;
	private const int                                 WGL_CONTEXT_ATTRIBUTE_FLAGS         = 0x2094;
	private const int                                 WGL_CONTEXT_ATTRIBUTE_MAJOR_VERSION = 0x2091;
	private const int                                 WGL_CONTEXT_ATTRIBUTE_MINOR_VERSION = 0x2092;
	private const int                                 WGL_CONTEXT_ATTRIBUTE_PROFILE_MASK  = 0x9126;
	private const int                                 WGL_CONTEXT_CORE_PROFILE_BIT        = 0x00000001;
	private const int                                 WGL_CONTEXT_FORWARD_COMPATIBLE_BIT  = 0x00000002;

	// ReSharper disable InconsistentNaming
	private readonly PfnGetDeviceContextDelegate     GetDeviceContext;
	private readonly PfnReleaseDeviceContextDelegate ReleaseDeviceContext;
	private readonly PfnSetPixelFormatDelegate       SetPixelFormat;
	private readonly PfnChoosePixelFormatDelegate    ChoosePixelFormat;
	private readonly PfnSwapBuffersDelegate          SwapBuffers;
	private readonly PfnCreateContextDelegate        WglCreateContext;
	private readonly PfnDeleteContextDelegate        WglDeleteContext;
	private readonly PfnGetProcAddressDelegate       WglGetProcAddress;
	private readonly PfnMakeCurrentDelegate          WglMakeCurrent;
	// ReSharper restore InconsistentNaming

	private readonly NativeLibraryLoader         m_loader;
	private readonly IPlatformWindow             m_window;
	private readonly Dictionary<string, object?> m_functions;
	private          IntPtr                      m_deviceContext;
	private          IntPtr                      m_graphicsContext;
	private          IntPtr                      m_dummyContext;

	public Win32OpenGLSurface(NativeLibraryLoader loader, IPlatformWindow window, int majorVersion, int minorVersion)
	{
		var pixelFormatDescriptor = new Win32PixelFormatDescriptor
		{
			nSize        = (ushort)Marshal.SizeOf(typeof(Win32PixelFormatDescriptor)),
			nVersion     = 1,
			dwFlags      = PIXEL_FORMAT_FLAGS,
			iPixelType   = PIXEL_FORMAT_TYPE,
			cColorBits   = PIXEL_FORMAT_COLOR_BITS,
			cDepthBits   = PIXEL_FORMAT_DEPTH_BITS,
			cStencilBits = PIXEL_FORMAT_STENCIL_BITS,
			iLayerType   = Win32PixelFormatDescriptorLayerTypes.MainPlane
		};

		var contextAttributes = new[]
		{
			WGL_CONTEXT_ATTRIBUTE_MAJOR_VERSION, majorVersion,
			WGL_CONTEXT_ATTRIBUTE_MINOR_VERSION, minorVersion,
			WGL_CONTEXT_ATTRIBUTE_FLAGS, WGL_CONTEXT_FORWARD_COMPATIBLE_BIT,
			WGL_CONTEXT_ATTRIBUTE_PROFILE_MASK, WGL_CONTEXT_CORE_PROFILE_BIT,
			0
		};

		loader.LoadFunction("user32", "GetDC", out GetDeviceContext);
		loader.LoadFunction("user32", "ReleaseDC", out ReleaseDeviceContext);
		loader.LoadFunction("gdi32", "ChoosePixelFormat", out ChoosePixelFormat);
		loader.LoadFunction("gdi32", "SetPixelFormat", out SetPixelFormat);
		loader.LoadFunction("gdi32", "SwapBuffers", out SwapBuffers);
		loader.LoadFunction("opengl32", "wglCreateContext", out WglCreateContext);
		loader.LoadFunction("opengl32", "wglDeleteContext", out WglDeleteContext);
		loader.LoadFunction("opengl32", "wglMakeCurrent", out WglMakeCurrent);
		loader.LoadFunction("opengl32", "wglGetProcAddress", out WglGetProcAddress);

		m_loader    = loader;
		m_window    = window;
		m_functions = new Dictionary<string, object?>();

		m_deviceContext = GetDeviceContext(m_window.GetNativePointer());
		if (m_deviceContext == IntPtr.Zero)
		{
			throw new Exception("Couldn't retrieve device context from window.");
		}

		var pixelFormatIdentifier = ChoosePixelFormat(m_deviceContext, ref pixelFormatDescriptor);
		if (pixelFormatIdentifier == 0 || !SetPixelFormat(m_deviceContext, pixelFormatIdentifier, ref pixelFormatDescriptor))
		{
			throw new Exception("Couldn't configure suitable device pixel format.");
		}

		m_dummyContext = WglCreateContext(m_deviceContext);
		if (m_dummyContext == IntPtr.Zero || !WglMakeCurrent(m_deviceContext, m_dummyContext))
		{
			throw new Exception("Couldn't setup generic opengl context.");
		}

		LoadFunction("wglCreateContextAttribsARB", out PfnWglCreateContextAttribsArb wglCreateContextAttribsArb);

		m_graphicsContext = wglCreateContextAttribsArb(m_deviceContext, IntPtr.Zero, contextAttributes);
		if (m_graphicsContext == IntPtr.Zero || !WglMakeCurrent(m_deviceContext, m_graphicsContext))
		{
			throw new Exception("Couldn't setup opengl core context.");
		}

		WglDeleteContext(m_dummyContext);
		m_dummyContext = IntPtr.Zero;
		Width          = m_window.GetViewportWidth();
		Height         = m_window.GetViewportHeight();
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_dummyContext != IntPtr.Zero || m_graphicsContext != IntPtr.Zero)
		{
			WglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
			if (m_dummyContext != IntPtr.Zero)
			{
				WglDeleteContext(m_dummyContext);
				m_dummyContext = IntPtr.Zero;
			}

			if (m_graphicsContext != IntPtr.Zero)
			{
				WglDeleteContext(m_graphicsContext);
				m_graphicsContext = IntPtr.Zero;
			}
		}

		if (m_deviceContext != IntPtr.Zero)
		{
			ReleaseDeviceContext(m_window.GetNativePointer(), m_deviceContext);
			m_deviceContext = IntPtr.Zero;
		}
	}

	private void Dispose(bool disposing)
	{
		ReleaseUnmanagedResources();
		if (disposing)
		{
			m_loader.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~Win32OpenGLSurface()
	{
		Dispose(false);
	}

	public int Width  { get; private set; }
	public int Height { get; private set; }

	public void Resize(int width, int height)
	{
		Width  = width;
		Height = height;
	}

	public void Present()
	{
		SwapBuffers(m_deviceContext);
	}

	public void LoadFunction<TDelegate>(string exportName, out TDelegate func) where TDelegate : Delegate
	{
		if (!TryLoadFunction<TDelegate>(exportName, out var temp))
		{
			throw new Exception($"Couldn't load required function {exportName}.");
		}
		else
		{
			func = temp;
		}
	}

	public bool TryLoadFunction<TDelegate>(string exportName, out TDelegate func) where TDelegate : Delegate
	{
		if (m_functions.TryGetValue(exportName, out var cached))
		{
			func = (TDelegate?)cached!;
			return cached != null;
		}

		var address = WglGetProcAddress(exportName);
		if (address != IntPtr.Zero)
		{
			func = Marshal.GetDelegateForFunctionPointer<TDelegate>(address);

			m_functions[exportName] = func;
			return true;
		}
		else if (m_loader.TryLoadFunction("opengl32", exportName, out func))
		{
			m_functions[exportName] = func;
			return true;
		}

		return false;
	}
}