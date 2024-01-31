using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Interfaces.Libraries;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.Interfaces;
using ZweigEngine.Native.Win32.Prototypes;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.Video;

public sealed class Win32OpenGLOutput : IDisposable, IPlatformVideoOutput, IWin32WindowComponent, ICustomFunctionLoader
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
	private const int                                 OPENGL_VERSION_MAJOR                = 3;
	private const int                                 OPENGL_VERSION_MINOR                = 3;

	private readonly Dictionary<string, object?> m_cachedResults;
	private readonly NativeLibraryLoader         m_libraryLoader;
	private readonly Win32Window                 m_window;

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

	private IntPtr m_device;
	private IntPtr m_owner;
	private IntPtr m_dummy;
	private IntPtr m_graphics;
	private int    m_width;
	private int    m_height;

	public Win32OpenGLOutput(NativeLibraryLoader libraryLoader, Win32Window window)
	{
		m_cachedResults = new Dictionary<string, object?>();
		m_libraryLoader = libraryLoader;
		m_window        = window;

		libraryLoader.LoadFunction("user32", "GetDC", out GetDeviceContext);
		libraryLoader.LoadFunction("user32", "ReleaseDC", out ReleaseDeviceContext);
		libraryLoader.LoadFunction("gdi32", "ChoosePixelFormat", out ChoosePixelFormat);
		libraryLoader.LoadFunction("gdi32", "SetPixelFormat", out SetPixelFormat);
		libraryLoader.LoadFunction("gdi32", "SwapBuffers", out SwapBuffers);
		libraryLoader.LoadFunction("opengl32", "wglCreateContext", out WglCreateContext);
		libraryLoader.LoadFunction("opengl32", "wglDeleteContext", out WglDeleteContext);
		libraryLoader.LoadFunction("opengl32", "wglMakeCurrent", out WglMakeCurrent);
		libraryLoader.LoadFunction("opengl32", "wglGetProcAddress", out WglGetProcAddress);

		m_window.AddComponent(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_window.RemoveComponent(this);
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~Win32OpenGLOutput()
	{
		ReleaseUnmanagedResources();
	}

	public event PlatformVideoDeviceDelegate? OnActivated;
	public event PlatformVideoDeviceDelegate? OnDeactivating;
	public event PlatformVideoDeviceDelegate? OnBeginFrame;
	public event PlatformVideoDeviceDelegate? OnFinishFrame;

	public int GetViewportWidth()  => m_width;
	public int GetViewportHeight() => m_height;

	void IWin32WindowComponent.OnAttach()
	{
		if (m_device != IntPtr.Zero || !m_window.IsAvailable())
		{
			return;
		}

		m_owner  = m_window.GetHandle();
		m_device = GetDeviceContext(m_owner);
		if (m_device == IntPtr.Zero)
		{
			throw new Exception("Couldn't acquire win32 device context.");
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

		var pixelFormatIdentifier = ChoosePixelFormat(m_device, ref pixelFormatDescriptor);
		if (pixelFormatIdentifier == 0 || !SetPixelFormat(m_device, pixelFormatIdentifier, ref pixelFormatDescriptor))
		{
			throw new Exception("Couldn't configure suitable device pixel format.");
		}

		//create dummy context in order to load extension for core context creation
		m_dummy = WglCreateContext(m_device);
		if (m_dummy == IntPtr.Zero)
		{
			throw new Exception("Couldn't setup generic opengl context.");
		}

		if (!WglMakeCurrent(m_device, m_dummy))
		{
			throw new Exception("Couldn't activate generic opengl context.");
		}

		LoadFunction("wglCreateContextAttribsARB", out PfnWglCreateContextAttribsArb wglCreateContextAttribsArb);
		var contextAttributes = new[]
		{
			WGL_CONTEXT_ATTRIBUTE_MAJOR_VERSION, OPENGL_VERSION_MAJOR,
			WGL_CONTEXT_ATTRIBUTE_MINOR_VERSION, OPENGL_VERSION_MINOR,
			WGL_CONTEXT_ATTRIBUTE_FLAGS, WGL_CONTEXT_FORWARD_COMPATIBLE_BIT,
			WGL_CONTEXT_ATTRIBUTE_PROFILE_MASK, WGL_CONTEXT_CORE_PROFILE_BIT,
			0
		};

		m_graphics = wglCreateContextAttribsArb(m_device, IntPtr.Zero, contextAttributes);
		if (m_graphics == IntPtr.Zero)
		{
			throw new Exception("Couldn't setup opengl core context.");
		}

		WglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		WglDeleteContext(m_dummy);
		if (!WglMakeCurrent(m_device, m_graphics))
		{
			throw new Exception("Couldn't activate opengl core context.");
		}

		m_cachedResults.Clear();
		m_width  = m_window.GetViewportWidth();
		m_height = m_window.GetViewportHeight();
		OnActivated?.Invoke(this);
	}

	void IWin32WindowComponent.OnDetach()
	{
		if (m_device == IntPtr.Zero)
		{
			return;
		}

		try
		{
			OnDeactivating?.Invoke(this);
		}
		finally
		{
			m_cachedResults.Clear();
			if (m_dummy != IntPtr.Zero || m_graphics != IntPtr.Zero)
			{
				WglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
			}

			if (m_dummy != IntPtr.Zero)
			{
				WglDeleteContext(m_dummy);
			}

			if (m_graphics != IntPtr.Zero)
			{
				WglDeleteContext(m_graphics);
			}

			ReleaseDeviceContext(m_owner, m_device);
			m_device = IntPtr.Zero;
			m_owner  = IntPtr.Zero;
		}
	}

	void IWin32WindowComponent.OnBeginUpdate()
	{
		m_width  = m_window.GetViewportWidth();
		m_height = m_window.GetViewportHeight();
		OnBeginFrame?.Invoke(this);
	}

	void IWin32WindowComponent.OnFinishUpdate()
	{
		if (m_device == IntPtr.Zero)
		{
			return;
		}

		try
		{
			OnFinishFrame?.Invoke(this);
		}
		finally
		{
			SwapBuffers(m_device);
		}
	}

	void IWin32WindowComponent.OnMessage(long lTime, nint hWindow, Win32MessageType uMessage, nint wParam, nint lParam)
	{
	}

	public void LoadFunction<TDelegate>(string exportName, out TDelegate func) where TDelegate : Delegate
	{
		if (!TryLoadFunction<TDelegate>(exportName, out var temp) || temp == null)
		{
			throw new Exception($"Couldn't load required function {exportName}.");
		}
		else
		{
			func = temp;
		}
	}

	public bool TryLoadFunction<TDelegate>(string exportName, out TDelegate? func) where TDelegate : Delegate
	{
		if (m_cachedResults.TryGetValue(exportName, out var cached))
		{
			func = (TDelegate?)cached;
			return func != null;
		}

		var address = WglGetProcAddress(exportName);
		if (address != IntPtr.Zero)
		{
			func = Marshal.GetDelegateForFunctionPointer<TDelegate>(address);

			m_cachedResults[exportName] = func;
			return true;
		}
		else if (m_libraryLoader.TryLoadFunction("opengl32", exportName, out func))
		{
			m_cachedResults[exportName] = func;
			return true;
		}

		return false;
	}

	private delegate IntPtr PfnWglCreateContextAttribsArb(IntPtr deviceContext, IntPtr openglContext, int[] attributes);
}