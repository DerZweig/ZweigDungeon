using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Interfaces.Libraries;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.Interfaces;
using ZweigEngine.Native.Win32.Prototypes;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.Video;

internal class Win32OpenGLVideoDriver : IWin32VideoDriver
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

	private readonly NativeLibraryLoader m_loader;
	private readonly Win32Window         m_window;
	private readonly int                 m_majorVersion;
	private readonly int                 m_minorVersion;

	public Win32OpenGLVideoDriver(NativeLibraryLoader loader, Win32Window window, int majorVersion, int minorVersion)
	{
		m_loader       = loader;
		m_window       = window;
		m_majorVersion = majorVersion;
		m_minorVersion = minorVersion;

		loader.LoadFunction("user32", "GetDC", out GetDeviceContext);
		loader.LoadFunction("user32", "ReleaseDC", out ReleaseDeviceContext);
		loader.LoadFunction("gdi32", "ChoosePixelFormat", out ChoosePixelFormat);
		loader.LoadFunction("gdi32", "SetPixelFormat", out SetPixelFormat);
		loader.LoadFunction("gdi32", "SwapBuffers", out SwapBuffers);
		loader.LoadFunction("opengl32", "wglCreateContext", out WglCreateContext);
		loader.LoadFunction("opengl32", "wglDeleteContext", out WglDeleteContext);
		loader.LoadFunction("opengl32", "wglMakeCurrent", out WglMakeCurrent);
		loader.LoadFunction("opengl32", "wglGetProcAddress", out WglGetProcAddress);

		Name = $"OpenGL {majorVersion}.{minorVersion}";
	}

	public string Name { get; }

	public IEnumerable<VideoDeviceDescription> EnumerateDevices()
	{
		yield return new VideoDeviceDescription
		{
			Name     = "Generic OpenGL Device",
			VendorId = string.Empty,
			DeviceId = string.Empty
		};
	}

	public IWin32VideoSurface CreateSurface(in VideoDeviceDescription device)
	{
		if (!m_window.IsAvailable())
		{
			throw new AccessViolationException("Attempting to create surface for unavailable window.");
		}

		var surface = new Surface(this, m_loader);
		surface.DeviceContext = GetDeviceContext(m_window.GetHandle());
		if (surface.DeviceContext == IntPtr.Zero)
		{
			throw new Exception("Couldn't retrieve device context from window.");
		}

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

		var pixelFormatIdentifier = ChoosePixelFormat(surface.DeviceContext, ref pixelFormatDescriptor);
		if (pixelFormatIdentifier == 0 || !SetPixelFormat(surface.DeviceContext, pixelFormatIdentifier, ref pixelFormatDescriptor))
		{
			throw new Exception("Couldn't configure suitable device pixel format.");
		}

		surface.DummyContext = WglCreateContext(surface.DeviceContext);
		if (surface.DummyContext == IntPtr.Zero || !WglMakeCurrent(surface.DeviceContext, surface.DummyContext))
		{
			throw new Exception("Couldn't setup generic opengl context.");
		}

		surface.LoadFunction("wglCreateContextAttribsARB", out PfnWglCreateContextAttribsArb wglCreateContextAttribsArb);
		var contextAttributes = new[]
		{
			WGL_CONTEXT_ATTRIBUTE_MAJOR_VERSION, m_majorVersion,
			WGL_CONTEXT_ATTRIBUTE_MINOR_VERSION, m_minorVersion,
			WGL_CONTEXT_ATTRIBUTE_FLAGS, WGL_CONTEXT_FORWARD_COMPATIBLE_BIT,
			WGL_CONTEXT_ATTRIBUTE_PROFILE_MASK, WGL_CONTEXT_CORE_PROFILE_BIT,
			0
		};

		surface.GraphicsContext = wglCreateContextAttribsArb(surface.DeviceContext, IntPtr.Zero, contextAttributes);
		if (surface.GraphicsContext == IntPtr.Zero || !WglMakeCurrent(surface.DeviceContext, surface.GraphicsContext))
		{
			throw new Exception("Couldn't setup opengl core context.");
		}

		WglDeleteContext(surface.DummyContext);
		surface.DummyContext = IntPtr.Zero;

		surface.Width  = m_window.GetViewportWidth();
		surface.Height = m_window.GetViewportHeight();
		return surface;
	}

	private void DestroySurface(Surface surface)
	{
		if (!m_window.IsAvailable())
		{
			return;
		}

		if (surface.DummyContext != IntPtr.Zero || surface.GraphicsContext != IntPtr.Zero)
		{
			WglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
			if (surface.DummyContext != IntPtr.Zero)
			{
				WglDeleteContext(surface.DummyContext);
				surface.DummyContext = IntPtr.Zero;
			}

			if (surface.GraphicsContext != IntPtr.Zero)
			{
				WglCreateContext(surface.GraphicsContext);
				surface.GraphicsContext = IntPtr.Zero;
			}
		}

		if (surface.DeviceContext != IntPtr.Zero)
		{
			ReleaseDeviceContext(m_window.GetHandle(), surface.DeviceContext);
			surface.DeviceContext = IntPtr.Zero;
		}
	}

	private class Surface : IDisposable, IWin32VideoSurface, ICustomFunctionLoader
	{
		private readonly Win32OpenGLVideoDriver      m_driver;
		private readonly NativeLibraryLoader         m_loader;
		private readonly Dictionary<string, object?> m_functions;

		public Surface(Win32OpenGLVideoDriver driver, NativeLibraryLoader loader)
		{
			m_driver    = driver;
			m_loader    = loader;
			m_functions = new Dictionary<string, object?>();
		}

		private void ReleaseUnmanagedResources()
		{
			m_driver.DestroySurface(this);
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~Surface()
		{
			ReleaseUnmanagedResources();
		}

		public int    Width           { get; set; }
		public int    Height          { get; set; }
		public IntPtr DeviceContext   { get; set; }
		public IntPtr GraphicsContext { get; set; }
		public IntPtr DummyContext    { get; set; }

		public void Resize(int width, int height)
		{
			Width  = width;
			Height = height;
		}

		public void Present()
		{
			m_driver.SwapBuffers(DeviceContext);
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

			var address = m_driver.WglGetProcAddress(exportName);
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

	private delegate IntPtr PfnWglCreateContextAttribsArb(IntPtr deviceContext, IntPtr openglContext, int[] attributes);
}