using Katabasis.Extended.ViewportAdapters;
using System.Drawing;
using System.Numerics;

namespace Katabasis.Extended
{
    public sealed class OrthographicCamera : Camera<Vector2>, IMovable, IRotatable
    {
        private readonly ViewportAdapter _viewportAdapter;
        private float _maximumZoom = float.MaxValue;
        private float _minimumZoom;
        private float _zoom;

        public OrthographicCamera()
            : this(new DefaultViewportAdapter())
        {
        }

        public OrthographicCamera(ViewportAdapter viewportAdapter)
        {
            _viewportAdapter = viewportAdapter;

            Rotation = 0;
            Zoom = 1;
            Origin = new Vector2(viewportAdapter.VirtualWidth/2f, viewportAdapter.VirtualHeight/2f);
            Position = Vector2.Zero;
        }

        public override Vector2 Position { get; set; }
        public override float Rotation { get; set; }
        public override Vector2 Origin { get; set; }
        public override Vector2 Center => Position + Origin;

        public override float Zoom
        {
            get => _zoom;
            set
            {
                if ((value < MinimumZoom) || (value > MaximumZoom))
                    throw new ArgumentException("Zoom must be between MinimumZoom and MaximumZoom");

                _zoom = value;
            }
        }

        public override float MinimumZoom
        {
            get => _minimumZoom;
            set
            {
                if (value < 0)
                    throw new ArgumentException("MinimumZoom must be greater than zero");

                if (Zoom < value)
                    Zoom = MinimumZoom;

                _minimumZoom = value;
            }
        }

        public override float MaximumZoom
        {
            get => _maximumZoom;
            set
            {
                if (value < 0)
                    throw new ArgumentException("MaximumZoom must be greater than zero");

                if (Zoom > value)
                    Zoom = value;

                _maximumZoom = value;
            }
        }

        public override RectangleF BoundingRectangle
        {
            get
            {
                throw new NotImplementedException("Bounding rectangle hasn't been implemented yet");
                // TODO: Implement
                //var frustum = GetBoundingFrustum();
                //var corners = frustum.GetCorners();
                //var topLeft = corners[0];
                //var bottomRight = corners[2];
                //var width = bottomRight.X - topLeft.X;
                //var height = bottomRight.Y - topLeft.Y;
                //return new RectangleF(topLeft.X, topLeft.Y, width, height);
            }
        }
        
        public override void Move(Vector2 direction)
        {
            Position += Vector2.Transform(direction, Matrix4x4.CreateRotationZ(-Rotation));
        }

        public override void Rotate(float deltaRadians)
        {
            Rotation += deltaRadians;
        }

        public override void ZoomIn(float deltaZoom)
        {
            ClampZoom(Zoom + deltaZoom);
        }

        public override void ZoomOut(float deltaZoom)
        {
            ClampZoom(Zoom - deltaZoom);
        }

        private void ClampZoom(float value)
        {
            if (value < MinimumZoom)
                Zoom = MinimumZoom;
            else
                Zoom = value > MaximumZoom ? MaximumZoom : value;
        }

        public override void LookAt(Vector2 position)
        {
            Position = position - new Vector2(_viewportAdapter.VirtualWidth/2f, _viewportAdapter.VirtualHeight/2f);
        }

        public Vector2 WorldToScreen(float x, float y)
        {
            return WorldToScreen(new Vector2(x, y));
        }

        public override Vector2 WorldToScreen(Vector2 worldPosition)
        {
            var viewport = _viewportAdapter.Viewport;
            return Vector2.Transform(worldPosition + new Vector2(viewport.X, viewport.Y), GetViewMatrix());
        }

        public Vector2 ScreenToWorld(float x, float y)
        {
            return ScreenToWorld(new Vector2(x, y));
        }

        public override Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            var viewport = _viewportAdapter.Viewport;
            Matrix4x4.Invert(GetViewMatrix(), out Matrix4x4 inverse);
            return Vector2.Transform(screenPosition - new Vector2(viewport.X, viewport.Y), inverse);
        }

        public Matrix4x4 GetViewMatrix(Vector2 parallaxFactor)
        {
            return GetVirtualViewMatrix(parallaxFactor)*_viewportAdapter.GetScaleMatrix();
        }

        private Matrix4x4 GetVirtualViewMatrix(Vector2 parallaxFactor)
        {
            return
                Matrix4x4.CreateTranslation(new Vector3(-Position*parallaxFactor, 0.0f))*
                Matrix4x4.CreateTranslation(new Vector3(-Origin, 0.0f))*
                Matrix4x4.CreateRotationZ(Rotation)*
                Matrix4x4.CreateScale(Zoom, Zoom, 1)*
                Matrix4x4.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        private Matrix4x4 GetVirtualViewMatrix()
        {
            return GetVirtualViewMatrix(Vector2.One);
        }

        public override Matrix4x4 GetViewMatrix()
        {
            return GetViewMatrix(Vector2.One);
        }

        public override Matrix4x4 GetInverseViewMatrix()
        {
            Matrix4x4.Invert(GetViewMatrix(), out Matrix4x4 inverse);
            return inverse;
        }

        private Matrix4x4 GetProjectionMatrix(Matrix4x4 viewMatrix)
        {
            var projection = Matrix4x4.CreateOrthographicOffCenter(0, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight, 0, -1, 0);
            return Matrix4x4.Multiply(viewMatrix, projection);
        }

        // TODO: Implement

        //public override BoundingFrustum GetBoundingFrustum()
        //{
        //    var viewMatrix4x4 = GetVirtualViewMatrix();
        //    var projectionMatrix4x4 = GetProjectionMatrix(viewMatrix);
        //    return new BoundingFrustum(projectionMatrix);
        //}

        //public ContainmentType Contains(Point point)
        //{
        //    return Contains(point.ToVector2());
        //}

        //public override ContainmentType Contains(Vector2 vector2)
        //{
        //    return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0));
        //}

        //public override ContainmentType Contains(Rectangle rectangle)
        //{
        //    var max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
        //    var min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
        //    var boundingBox = new BoundingBox(min, max);
        //    return GetBoundingFrustum().Contains(boundingBox);
        //}
    }
}
