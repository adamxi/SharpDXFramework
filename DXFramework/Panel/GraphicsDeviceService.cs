using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

// The IGraphicsDeviceService interface requires a DeviceCreated event, but we always just create the device inside our constructor, so we have no place to
// raise that event. The C# compiler warns us that the event is never used, but we don't care so we just disable this warning.
#pragma warning disable 67

namespace WinformXNA {
    /// <summary>
    /// Helper class responsible for creating and managing the GraphicsDevice. All GraphicsDeviceControl instances share the same GraphicsDeviceService,
    /// so even though there can be many controls, there will only ever be a singleunderlying GraphicsDevice. This implements the standard IGraphicsDeviceService
    /// interface, which provides notification events for when the device is reset or disposed.
    /// </summary>
    public class GraphicsDeviceService: IGraphicsDeviceService {
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
        public static int referenceCount;
        private GraphicsDevice graphicsDevice;
        private PresentationParameters parameters;

        /// <summary>
        /// Constructor is private, because this is a singleton class: client controls should use the public AddRef method instead.
        /// </summary>
        public GraphicsDeviceService(IntPtr windowHandle, int width, int height) {
            parameters = new PresentationParameters();
            MaxBufferWidth = Math.Max(width, 1);
            MaxBufferHeight = Math.Max(height, 1);
            parameters.BackBufferWidth = MaxBufferWidth;
            parameters.BackBufferHeight = MaxBufferHeight;
            parameters.BackBufferFormat = SurfaceFormat.Color;
            parameters.DepthStencilFormat = DepthFormat.None;
            parameters.DeviceWindowHandle = windowHandle;
            parameters.PresentationInterval = PresentInterval.Immediate;
            parameters.IsFullScreen = false;
			
            graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, parameters);
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            InitRasterizer();
        }

        public void InitRasterizer() {
            RasterizerState rasterizer = new RasterizerState();
            rasterizer.FillMode = FillMode.Solid;
            rasterizer.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizer;
        }

        public void SetRasterizer(RasterizerState rasterizer) {
            graphicsDevice.RasterizerState = rasterizer;
        }

        /// <summary>
        /// Current GraphicsDeviceService singleton instance.
        /// </summary>
        public static GraphicsDeviceService Instance { get; private set; }

        /// <summary>
        /// Current maximum buffer width.
        /// </summary>
        public int MaxBufferWidth { get; private set; }

        /// <summary>
        /// Current maximum buffer height.
        /// </summary>
        public int MaxBufferHeight { get; private set; }

        /// <summary>
        /// Gets the current graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice {
            get { return graphicsDevice; }
        }

        /// <summary>
        /// Gets a reference to the singleton instance.
        /// </summary>
        public static GraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height) {
            // Increment the "how many controls sharing the device" reference count.
            if(Interlocked.Increment(ref referenceCount) == 1) {
                // If this is the first control to start using the device, we must create the singleton instance.
                Instance = new GraphicsDeviceService(windowHandle, width, height);
            }
            return Instance;
        }

        /// <summary>
        /// Releases a reference to the singleton instance.
        /// </summary>
        public void Release(bool disposing) {
            // Decrement the "how many controls sharing the device" reference count.
            if(Interlocked.Decrement(ref referenceCount) == 0) {
                // If this is the last control to finish using the
                // device, we should dispose the singleton instance.
                if(disposing) {
                    if(DeviceDisposing != null) {
                        DeviceDisposing(this, EventArgs.Empty);
                    }
                    graphicsDevice.Dispose();
                }
                graphicsDevice = null;
            }
        }

        public void ResetMaxBufferSize() {
            MaxBufferWidth = 1;
            MaxBufferHeight = 1;
        }

        public void ResetDevice(int width, int height) {
            if(DeviceResetting != null) {
                DeviceResetting(this, EventArgs.Empty);
            }

            MaxBufferWidth = Math.Max(MaxBufferWidth, width);
            MaxBufferHeight = Math.Max(MaxBufferHeight, height);
            parameters.BackBufferWidth = MaxBufferWidth;
            parameters.BackBufferHeight = MaxBufferHeight;

            graphicsDevice.Reset(parameters);

            if(DeviceReset != null) {
                DeviceReset(this, EventArgs.Empty);
            }
        }
    }
}