using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

using OpenTK;
using OpenTK.Graphics;

namespace Hailstone
{
    /// <summary>
    /// Contains values for various program settings in a format that can be serialized and
    /// viewed with a property grid.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The current program settings.
        /// </summary>
        public static Settings Current = Settings.Default;

        /// <summary>
        /// Gets the default program settings.
        /// </summary>
        public static Settings Default
        {
            get
            {
                return new Settings
                {
                    CameraZoomDamping = 0.01,
                    CameraMovementDamping = 0.01,
                    CameraZoomMovement = 0.95,
                    CameraMinZoom = -10.0,
                    CameraMaxZoom = 0.0,
                    CameraZoomSpeed = 1.0,

                    StoneDamping = 0.1,
                    StoneNumberSize = 0.3,
                    StoneNumberColor = new Color(0.9, 1.0, 1.0, 1.0),
                    StoneFillColor = new ExtendedColor
                    {
                        Normal = new Color(0.4, 0.7, 1.0, 1.0),
                        Selected = new Color(0.8, 0.4, 0.4, 1.0),
                        Glow = new Color(0.9, 0.6, 0.6, 1.0)
                    },
                    StoneBorderColor = new ExtendedColor
                    {
                        Normal = new Color(0.9, 1.0, 0.9, 1.0),
                        Selected = new Color(1.0, 0.5, 0.4, 1.0),
                        Glow = new Color(1.0f, 1.0, 0.9, 1.0)
                    },
                    StoneHighlightColor = new ExtendedColor
                    {
                        Normal = new Color(0.8, 1.0, 1.0, 1.0),
                        Selected = new Color(1.0, 0.5, 0.3, 1.0),
                        Glow = new Color(1.0, 0.8, 0.8, 1.0)
                    },
                    StonePulseLength = 10.0,
                    StonePulseSpeed = 0.3,

                    StoneIntroductionPressureThreshold = 0.5,
                    StoneIntroductionPressureExpansion = 1.05,
                    StoneIntroductionTimeout = 5.0,
                    StoneIntroductionSpeed = 0.1,

                    LinkWidth = 0.07,
                    LinkMinimumArrowLength = 0.8,
                    LinkTargetLength = 2.6,

                    BackgroundColor = new Color(0.5, 0.7, 0.9, 1.0)
                };
            }
        }

        [Category("Camera")]
        [DisplayName("Zoom Damping")]
        [Description("The damping factor applied to camera zooming")]
        public double CameraZoomDamping { get; set; }

        [Category("Camera")]
        [DisplayName("Movement Damping")]
        [Description("The damping factor applied to the camera's lateral movement")]
        public double CameraMovementDamping { get; set; }

        [Category("Camera")]
        [DisplayName("Zoom Movement")]
        [Description("The amount of lateral movement created while zooming")]
        public double CameraZoomMovement { get; set; }

        [Category("Camera")]
        [DisplayName("Minimum Zoom Level")]
        [Description("The minimum zoom level for the camera. Smaller values allow a bigger viewing area")]
        public double CameraMinZoom { get; set; }

        [Category("Camera")]
        [DisplayName("Maximum Zoom Level")]
        [Description("The maximum zoom level for the camera. Larger values allow a smaller viewing area")]
        public double CameraMaxZoom { get; set; }

        [Category("Camera")]
        [DisplayName("Zoom Speed")]
        [Description("The speed multiplier applied to camera zooming")]
        public double CameraZoomSpeed { get; set; }

        [Category("Stone")]
        [DisplayName("Movement Damping")]
        [Description("The damping factor applied to stone movement")]
        public double StoneDamping { get; set; }

        [Category("Stone")]
        [DisplayName("Number Size")]
        [Description("The size of the numbers on stones")]
        public double StoneNumberSize { get; set; }

        [Category("Stone")]
        [DisplayName("Number Color")]
        [Description("The color of the numbers on stones")]
        public Color StoneNumberColor { get; set; }

        [Category("Stone")]
        [DisplayName("Border Color")]
        [Description("The color of the border on stones")]
        public ExtendedColor StoneBorderColor { get; set; }

        [Category("Stone")]
        [DisplayName("Fill Color")]
        [Description("The fill color on stones")]
        public ExtendedColor StoneFillColor { get; set; }

        [Category("Stone")]
        [DisplayName("Highlight Color")]
        [Description("The color of stones when zoomed out")]
        public ExtendedColor StoneHighlightColor { get; set; }

        [Category("Stone")]
        [DisplayName("Pulse Length")]
        [Description("The amount of stones in the pulse of a selected chain")]
        public double StonePulseLength { get; set; }

        [Category("Stone")]
        [DisplayName("Pulse Speed")]
        [Description("The rate at which selected stones pulse")]
        public double StonePulseSpeed { get; set; }

        [Category("Stone Introduction")]
        [DisplayName("Pressure Threshold")]
        [Description("The maximum local pressure before a new stone may be introduced")]
        public double StoneIntroductionPressureThreshold { get; set; }

        [Category("Stone Introduction")]
        [DisplayName("Pressure Damping")]
        [Description("The expansion factor applied to \"Pressure Threshold\" over time for a given stone")]
        public double StoneIntroductionPressureExpansion { get; set; }

        [Category("Stone Introduction")]
        [DisplayName("Timeout")]
        [Description("The time allowed for a stone to be introduced before it is discarded")]
        public double StoneIntroductionTimeout { get; set; }

        [Category("Stone Introduction")]
        [DisplayName("Initial Speed")]
        [Description("The initial speed given to introduced stones")]
        public double StoneIntroductionSpeed { get; set; }

        [Category("Link")]
        [DisplayName("Link Width")]
        [Description("The width of the links between stones")]
        public double LinkWidth { get; set; }

        [Category("Link")]
        [DisplayName("Minimum Arrow Length")]
        [Description("The minimum length of a link before an arrow appears")]
        public double LinkMinimumArrowLength { get; set; }

        [Category("Link")]
        [DisplayName("Link Target Length")]
        [Description("The ideal length for links between stones, as a factor of radius")]
        public double LinkTargetLength { get; set; }

        [Category("Background")]
        [DisplayName("Background Color")]
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// A color type for use in settings.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Color
        {
            public Color()
            {

            }

            public Color(double R, double G, double B, double A)
            {
                this.R = R;
                this.G = G;
                this.B = B;
                this.A = A;
            }

            [Description("The red component of this color, from 0.0 to 1.0")]
            public double R { get; set; }

            [Description("The green component of this color, from 0.0 to 1.0")]
            public double G { get; set; }

            [Description("The blue component of this color, from 0.0 to 1.0")]
            public double B { get; set; }

            [Description("The alpha (opacity) component of this color, from 0.0 to 1.0")]
            public double A { get; set; }

            public override string ToString()
            {
                return String.Format("Color", this.R, this.G, this.B, this.A);
            }

            public static implicit operator Color4(Color Source)
            {
                return new Color4((float)Source.R, (float)Source.G, (float)Source.B, (float)Source.A);
            }
        }

        /// <summary>
        /// A color that can vary depending on selection status.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class ExtendedColor
        {
            [Description("The visible color when the associated object is not selected")]
            public Color Normal { get; set; }

            [Description("The visible color when the associated object is selected, but not glowing")]
            public Color Selected { get; set; }

            [Description("The visible color when the associated object is selected and glowing")]
            public Color Glow { get; set; }

            public override string ToString()
            {
                return String.Format("Extended Color");
            }

            /// <summary>
            /// Gets the unselected form of this color.
            /// </summary>
            public Color4 GetUnselected()
            {
                return this.Normal;
            }

            /// <summary>
            /// Gets the selected form of this color.
            /// </summary>
            public Color4 GetSelected(float Glow)
            {
                return ((Color4)this.Selected).Mix(Glow, this.Glow);
            }
        }
    }
}