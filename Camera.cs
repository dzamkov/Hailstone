using System;
using System.Collections.Generic;

namespace Hailstone
{
    /// <summary>
    /// Represents a user-controllable view.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The point at the center of the camera view.
        /// </summary>
        public Vector Center = Vector.Zero;

        /// <summary>
        /// The movement velocity of the camera in viewspace.
        /// </summary>
        public Vector Velocity = Vector.Zero;

        /// <summary>
        /// The zoom level of the camera.
        /// </summary>
        public double Zoom = 0.0;

        /// <summary>
        /// The change in zoom level, per second.
        /// </summary>
        public double ZoomVelocity = 0.0;

        /// <summary>
        /// Gets the extent of the camera's view away from the center.
        /// </summary>
        public double Extent
        {
            get
            {
                return Math.Exp(-this.Zoom);
            }
        }

        /// <summary>
        /// Gets the transform from viewspace to worldspace using this camera.
        /// </summary>
        public Transform Transform
        {
            get
            {
                double extent = this.Extent;
                return new Transform(this.Center, new Vector(extent, 0.0), new Vector(0.0, extent));
            }
        }

        /// <summary>
        /// Updates the state of the camera by the given time in seconds.
        /// </summary>
        public void Update(double Time)
        {
            double extent = this.Extent;
            this.Center += this.Velocity * (extent * Time);
            this.Velocity *= Math.Pow(Settings.Current.CameraMovementDamping, Time);
            this.Zoom += this.ZoomVelocity * Time;
            this.ZoomVelocity *= Math.Pow(Settings.Current.CameraZoomDamping, Time);
            double minzoom = Settings.Current.CameraMinZoom;
            if (this.Zoom < minzoom)
            {
                this.Zoom = minzoom;
                this.ZoomVelocity = 0.0;
            }
            double maxzoom = Settings.Current.CameraMaxZoom;
            if (this.Zoom > maxzoom)
            {
                this.Zoom = maxzoom;
                this.ZoomVelocity = 0.0;
            }
        }

        /// <summary>
        /// Zooms the camera to the given target point by the given amount.
        /// </summary>
        public void ZoomTo(double Amount, Vector Target)
        {
            Amount *= Settings.Current.CameraZoomSpeed;
            double extent = this.Extent;
            Vector dif = Target - this.Center;
            this.ZoomVelocity += Amount;
            this.Velocity += dif * (Amount * Settings.Current.CameraZoomMovement / extent);
        }
    }
}
