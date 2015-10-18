using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;
using MSBrush = System.Drawing.Brush;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;
using XNAColor = Microsoft.Xna.Framework.Color;

namespace WinformXNA {
    [DefaultEvent("Render")]
    public abstract class GraphicsDeviceControl: Control {
        public delegate void RenderHandler(GraphicsDevice g, SpriteBatch s);

        [Description("Occurs when the XNAPanel needs to be rendered"), Category("CatBehavior")]
        public event RenderHandler Render;

        // However many GraphicsDeviceControl instances you have, they all share the same underlying GraphicsDevice, managed by this helper service.
        protected GraphicsDeviceService graphicsDeviceService;
        protected SpriteBatch spriteBatch;
        protected ContentManager content;
        protected ServiceContainer services;
        protected Viewport viewport;
        private XNARectangle surfaceRectangle;
        protected bool canUpdate;
        private bool fixedViewport;

        public GraphicsDeviceControl() {
            BackColor = Color.CornflowerBlue;
            this.SizeChanged += new EventHandler(GraphicsDeviceControl_SizeChanged);

			// Hack to trigger the OnCreateControl() method. From here we can detect if the application is in design mode.
			// Design mode cannot be detected from a constructor. Donno how to implement a proper architecture for this atm.
			this.Handle.ToString();
        }

        #region Properties
        public bool EnableDebugDraw { get; set; }

        public GraphicsDevice GraphicsDevice {
            get { return graphicsDeviceService.GraphicsDevice; }
        }

        public SpriteBatch SpriteBatch {
            get { return spriteBatch; }
        }

        public ContentManager Content {
            get { return content; }
        }

        /// <summary>
        /// Gets an IServiceProvider containing our IGraphicsDeviceService. This can be used with components such as the ContentManager, which use this service to look up the GraphicsDevice.
        /// </summary>
        public ServiceContainer Services {
            get { return services; }
        }

        public Viewport Viewport {
            get { return viewport; }
        }
        #endregion

        #region Events & Drawing logic
        protected override void OnCreateControl() {
            if(!DesignMode) {
                services = new ServiceContainer();
                graphicsDeviceService = GraphicsDeviceService.AddRef(Handle, Width, Height);
                services.AddService<IGraphicsDeviceService>(graphicsDeviceService); // Register the service, so components like ContentManager can find it.
                spriteBatch = new SpriteBatch(GraphicsDevice);
                content = new ContentManager(services, "Content");
                viewport = new Viewport();
                surfaceRectangle = new XNARectangle(0, 0, Width, Height);
                canUpdate = true;

                Initialize();
            }
            base.OnCreateControl();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent) {
        }

        protected override void OnPaint(PaintEventArgs e) {
            if(graphicsDeviceService != null && Render != null) {
                if(canUpdate) {
                    BeginDraw();
                    OnDraw();
                    EndDraw();
                } else {
                    e.Graphics.Clear(Color.Black);
                }
            } else {
                PaintUsingSystemDrawing(e.Graphics, Text + "\n\n" + GetType());
            }
        }

        private void BeginDraw() {
            switch(GraphicsDevice.GraphicsDeviceStatus) {
                case GraphicsDeviceStatus.Lost:
                    return;

                case GraphicsDeviceStatus.NotReset:
                    ResetDevice();
                    break;

                default:
                    if(Width > graphicsDeviceService.MaxBufferWidth || Height > graphicsDeviceService.MaxBufferHeight) {
                        ResetDevice();
                    }
                    break;
            }

            // Many GraphicsDeviceControl instances can be sharing the same GraphicsDevice. The device backbuffer will be resized to fit the largest of these controls.
            // But what if we are currently drawing a smaller control? To avoid unwanted stretching, we set the viewport to only use the top left portion of the full backbuffer.
            if(!fixedViewport) {
                viewport.X = 0;
                viewport.Y = 0;
                viewport.Width = Width;
                viewport.Height = Height;
                viewport.MinDepth = 0;
                viewport.MaxDepth = 1;
            }

            GraphicsDevice.Viewport = viewport;
        }

        /// <summary>
        /// Ends drawing the control. This is called after derived classes have finished their Draw method, and is responsible for presenting
        /// the finished image onto the screen, using the appropriate WinForms control handle to make sure it shows up in the right place.
        /// </summary>
        private void EndDraw() {
            try {
                GraphicsDevice.Present(surfaceRectangle, null, this.Handle);
            } catch {
                // Present might throw if the device became lost while we were drawing. The lost device will be handled by the next BeginDraw, so we just swallow the exception.
            }
        }

        protected virtual void OnDraw() {
            Render(GraphicsDevice, spriteBatch);
            if(EnableDebugDraw) {
                DebugDraw();
            }
        }

        private void ResetDevice() {
            try {
                graphicsDeviceService.ResetDevice(Width, Height);
            } catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Captures the screen onto a Texture2D.
        /// Note: This is not a copy of the current backbuffer (displayed screen image).
        /// This is instead an entirely new rendering of the screen onto a Texture2D which is relativly slow. So performance wise, it's not something you'll want to do each frame.
        /// </summary>
        public Texture2D CaptureScreen() {
            if(graphicsDeviceService != null && Render != null) {
				bool debugDraw = EnableDebugDraw;
				EnableDebugDraw = false;

                BeginDraw();

                using(RenderTarget2D renderTarget = new RenderTarget2D(GraphicsDevice, Width, Height)) {
                    GraphicsDevice.SetRenderTarget(renderTarget);
                    OnDraw();
                    GraphicsDevice.SetRenderTarget(null);
					EnableDebugDraw = debugDraw;

                    // Copy the rendertargets color data into a color array.
                    XNAColor[] colors = new XNAColor[Width * Height];
                    renderTarget.GetData(colors);

                    // Set the copied color data to the new Texture2D.
                    // This makes sure that the screenshot doesn't get lost with the rendertarget. This also prevents memory leaks as the rendertarget can now safely be disposed.
                    Texture2D texture = new Texture2D(GraphicsDevice, Width, Height);
                    texture.SetData(colors);

                    return texture;
                }
            }

            return null;
        }

        /// <summary>
        /// If we do not have a valid graphics device (for instance if the device is lost,
        /// or if we are running inside the Form designer), we must use regular System.Drawing method to display a status message.
        /// </summary>
        protected virtual void PaintUsingSystemDrawing(Graphics graphics, string text) {
            graphics.Clear(BackColor);

            using(MSBrush brush = new SolidBrush(Color.Black)) {
                using(StringFormat format = new StringFormat()) {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    graphics.DrawString(text, Font, brush, ClientRectangle, format);
                }
            }
        }

        private void GraphicsDeviceControl_SizeChanged(object sender, EventArgs e) {
            if(graphicsDeviceService != null) {
                surfaceRectangle = new XNARectangle(0, 0, Width, Height);
                graphicsDeviceService.ResetMaxBufferSize();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the viewport.
        /// </summary>
        public void SetViewport(Viewport v) {
            viewport = v;
            fixedViewport = true;
        }

        /// <summary>
        /// Sets the viewport.
        /// </summary>
        public void SetViewport(int x, int y, int width, int height) {
            viewport.X = x;
            viewport.Y = y;
            viewport.Width = width;
            viewport.Height = height;
            fixedViewport = true;
        }

        /// <summary>
        /// Sets the viewport to always be the same as the size of the panel.
        /// </summary>
        public void SetViewportOff() {
            fixedViewport = false;
        }

        /// <summary>
        /// Disables redrawing of the control.
        /// </summary>
        public void BeginUpdate() {
            canUpdate = false;
        }

        /// <summary>
        /// Enables redrawing of the control.
        /// </summary>
        public void EndUpdate() {
            canUpdate = true;
        }

        protected override void Dispose(bool disposing) {
            if(graphicsDeviceService != null) {
                graphicsDeviceService.Release(disposing);
                graphicsDeviceService = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Virtual methods
        /// <summary>
        /// Called after the GraphicsDeviceControl is done initializing. Deriving classes can override this to initialize their own code.
        /// </summary>
        protected virtual void Initialize() {
        }

        protected virtual void DebugDraw() {
        }
        #endregion
    }
}