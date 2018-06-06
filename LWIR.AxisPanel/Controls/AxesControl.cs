using SharpGL;
using SharpGL.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LWIR.NET.Entity;
using LWIR.NET.Utility;

namespace LWIR.AxisPanel.Controls
{
    public class AxesControl : Control
    {
        /// <summary>
        /// OpenGL control for drawing
        /// </summary>
        private OpenGLControl glCtrl = null;

        /// <summary>
        /// is pan enabled
        /// </summary>
        private bool isPanEnabled = false;

        /// <summary>
        /// A point when left button down
        /// </summary>
        private Point startPoint = new Point(0, 0);

        /// <summary>
        /// backup minX
        /// </summary>
        private DateTime oldMinX;

        /// <summary>
        /// backup maxX
        /// </summary>
        private DateTime oldMaxX;

        static AxesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxesControl), new FrameworkPropertyMetadata(typeof(AxesControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            glCtrl = GetTemplateChild("PART_GL") as OpenGLControl;

            if (glCtrl != null)
            {
                glCtrl.OpenGLDraw += glCtrl_OpenGLDraw;
                glCtrl.OpenGLInitialized += glCtrl_OpenGLInitialized;
            }

            this.Background = this.AxisBackground;
            this.MouseLeftButtonDown += AxesControl_MouseLeftButtonDown;
            this.MouseLeftButtonUp += AxesControl_MouseLeftButtonUp;
            this.MouseMove += AxesControl_MouseMove;
            this.MouseWheel += AxesControl_MouseWheel;
        }

        /// <summary>
        /// wheel to zoom out/in the x-axis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AxesControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!IsRollEnabled)
            {
                return;
            }

            var oldMinX = MinX;
            var oldMaxX = MaxX;

            var curPoint = e.GetPosition(this);
            var curTime = GetTimeFromX(curPoint.X, oldMinX, oldMaxX);

            if (e.Delta > 0)
            {
                // zoom in
                var tempMinX = oldMinX.AddMinutes(1);
                var tempMaxX = oldMaxX.AddMinutes(-1);

                if (tempMaxX <= tempMinX)
                {
                    //disabled zoom in
                    return;
                }

                // Translate the current time to the location of the mouse
                var curTimeReferToNewAxis = GetTimeFromX(curPoint.X, tempMinX, tempMaxX);
                var translateSpan = curTimeReferToNewAxis - curTime;
                MinX = tempMinX - translateSpan;
                MaxX = tempMaxX - translateSpan;
            }
            else
            {
                // zoom out
                var tempMinX = oldMinX.AddMinutes(-1);
                var tempMaxX = oldMaxX.AddMinutes(1);
                var timeSpan = tempMaxX - tempMinX;

                if (timeSpan.TotalDays > 24)
                {
                    return;
                }

                var curTimeReferToNewAxis = GetTimeFromX(curPoint.X, tempMinX, tempMaxX);
                var translateSpan = curTimeReferToNewAxis - curTime;

                MinX = tempMinX - translateSpan;
                MaxX = tempMaxX - translateSpan;
            }
        }

        /// <summary>
        /// disabled the pan when left button up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AxesControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isPanEnabled = false;
        }

        /// <summary>
        /// enabled the pan when left button down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AxesControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(this);
            oldMinX = MinX;
            oldMaxX = MaxX;
            isPanEnabled = true;
        }

        /// <summary>
        /// drag the axis by mouse, relative to the old axis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AxesControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isPanEnabled)
            {
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            this.Cursor = this.Cursor != Cursors.Hand ? Cursors.Hand : this.Cursor;

            var curPoint = e.GetPosition(this);
            var curTime = GetTimeFromX(curPoint.X, oldMinX, oldMaxX);
            var oldTime = GetTimeFromX(startPoint.X, oldMinX, oldMaxX);
            var timeSpan = curTime - oldTime;
            MinX = oldMinX - timeSpan;
            MaxX = oldMaxX - timeSpan;
        }


        /// <summary>
        /// Initial OpenGL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void glCtrl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            args.OpenGL.LineWidth(1.0f);
            //args.OpenGL.Enable(OpenGL.GL_POINT_SMOOTH);
            //args.OpenGL.Enable(OpenGL.GL_LINE_SMOOTH);
            //args.OpenGL.Enable(OpenGL.GL_POLYGON_SMOOTH);

            //args.OpenGL.Hint(OpenGL.GL_POINT_SMOOTH, OpenGL.GL_NICEST);
            //args.OpenGL.Hint(OpenGL.GL_POINT_SMOOTH, OpenGL.GL_NICEST);
            //args.OpenGL.Hint(OpenGL.GL_POINT_SMOOTH, OpenGL.GL_NICEST);
        }

        /// <summary>
        /// Draw somthing in GL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void glCtrl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            // Get the OpenGL instance that's been passed to us.
            OpenGL gl = args.OpenGL;

            // Load and clear the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            // Load the modelview.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            //  Clear the color and depth buffers.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            // Draw display panel
            gl.Viewport(0, 0, gl.RenderContextProvider.Width, gl.RenderContextProvider.Height);
            gl.Ortho2D(0, gl.RenderContextProvider.Width, 0, gl.RenderContextProvider.Height);

            var glColor = AxisBackground.GetRGBAf();
            gl.Color(glColor[0], glColor[1], glColor[2]);
            gl.Rect(0, 0, gl.RenderContextProvider.Width, gl.RenderContextProvider.Height);

            gl.PushMatrix();
            // Draw Axis
            DrawAxis(gl, gl.RenderContextProvider.Width, gl.RenderContextProvider.Height);

            // Draw Data
            DrawData(gl, gl.RenderContextProvider.Width, gl.RenderContextProvider.Height);
            gl.PopMatrix();

            //  Flush OpenGL.
            gl.Flush();
        }

        /// <summary>
        /// Draw axis with all lines
        /// </summary>
        /// <param name="gl"></param>
        private void DrawAxis(OpenGL gl, double width, double height)
        {
            //Get the axis color, secondary lines color and ticks color
            var axisLineColor = AxisBrush.GetRGBAf();
            var axisSecondaryLineColor = SecondaryGridBrush.GetRGBAf();
            var ticksColor = TicksBrush.GetRGBAf();
            var ticksScaleColor = TicksScaleBrush.GetRGBAf();

            // Get length of Tick line
            double tickWidth = 5;

            #region Draw YTicks and labels
            // Ticks line of axis-Y and ticks
            double gapY = height / (YGapCount + 1);
            double realValueOfGapY = (MaxY - MinY) / (YGapCount + 1);
            for (int num = 0; num <= YGapCount + 1; num++)
            {
                double posX = tickWidth;
                double posY = gapY * num;

                // Draw secondary line
                if (IsSecondaryLineVisible)
                {
                    gl.PushAttrib(OpenGL.GL_ENABLE_BIT);

                    if (IsDotDash)
                    {
                        gl.LineStipple(1, 0xAAAA);
                        gl.Enable(OpenGL.GL_LINE_STIPPLE);
                    }
                    else
                    {
                        gl.Disable(OpenGL.GL_LINE_STIPPLE);
                    }

                    gl.Color(axisSecondaryLineColor[0], axisSecondaryLineColor[1], axisSecondaryLineColor[2]);// will be chaged by drawText if without it
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Vertex(0.0d, posY);
                    gl.Vertex(width, posY);
                    gl.End();
                    gl.PopAttrib();
                }

                // Draw ticks line
                gl.Color(ticksScaleColor[0], ticksScaleColor[1], ticksScaleColor[2]);// will be chaged by drawText if without it
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(0.0d, posY);
                gl.Vertex(tickWidth, posY);
                gl.End();

                // Draw ticks
                string dispText = (num * realValueOfGapY + MinY).ToString("f2");
                double textWidth = 0, textHeight = 0;
                GetSizeOfText(gl, dispText, ref textWidth, ref textHeight);

                double textPosX = 2 * tickWidth;
                double textPosY = num == YGapCount + 1 ? posY - textHeight : (num == 0 ? posY + 2 : posY);

                DrawString(gl, dispText, (int)textPosX, (int)textPosY, ticksColor);
            }
            #endregion

            #region Draw XTicks and labels
            // Ticks line of axis-X and ticks
            double gapX = width / (XGapCount + 1);
            double realValueOfGapX = (MaxX - MinX).TotalMilliseconds / (XGapCount + 1);// Milliseconds in one gap
            for (int num = 0; num <= XGapCount + 1; num++)
            {
                double posX = num * gapX;

                // Draw secondary line
                if (IsSecondaryLineVisible)
                {
                    gl.PushAttrib(OpenGL.GL_ENABLE_BIT);

                    if (IsDotDash)
                    {
                        gl.LineStipple(1, 0xAAAA);
                        gl.Enable(OpenGL.GL_LINE_STIPPLE);
                    }
                    else
                    {
                        gl.Disable(OpenGL.GL_LINE_STIPPLE);
                    }

                    gl.Color(axisSecondaryLineColor[0], axisSecondaryLineColor[1], axisSecondaryLineColor[2]);// will be chaged by drawText if without it
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Vertex(posX, 0.0d);
                    gl.Vertex(posX, height);
                    gl.End();
                    gl.PopAttrib();
                }

                // Draw ticks line
                gl.Color(ticksScaleColor[0], ticksScaleColor[1], ticksScaleColor[2]);// will be chaged by drawText if without it
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(posX, 0.0d);
                gl.Vertex(posX, tickWidth);
                gl.End();

                // Draw text
                string dispText = (MinX + TimeSpan.FromMilliseconds(realValueOfGapX * num)).ToString("HH:mm:ss");
                double textWidth = 0, textHeight = 0;
                GetSizeOfText(gl, dispText, ref textWidth, ref textHeight);

                double textPosX = num == XGapCount + 1 ? posX - textWidth - 5 : posX - textWidth / 2;
                double textPosY = tickWidth + textHeight / 2;

                DrawString(gl, dispText, (int)textPosX, (int)textPosY, ticksColor);
            }
            #endregion

            #region Draw Axis-X and Axis-Y
            // Left line of axis-Y
            gl.Color(axisLineColor[0], axisLineColor[1], axisLineColor[2]);
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(0.0d, 0.0d);
            gl.Vertex(0.0d, height);
            gl.End();

            // Draw right line when the axis is boxed
            if (IsBoxed)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(width, 0.0d);
                gl.Vertex(width, height);
                gl.End();
            }

            // Left line of axis-Y
            gl.Color(axisLineColor[0], axisLineColor[1], axisLineColor[2]);
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(0.0d, 0.0d);
            gl.Vertex(width, 0.0d);
            gl.End();

            // Draw right line when the axis is boxed
            if (IsBoxed)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(0.0d, height);
                gl.Vertex(width, height);
                gl.End();
            }
            #endregion
        }

        /// <summary>
        /// Draw data curves
        /// </summary>
        /// <param name="gl"></param>
        private void DrawData(OpenGL gl, double width, double height)
        {
            #region draw curves

            for (int index = 0; index < CurvesList.Count; index++)
            {
                var curveColor = CurvesList[index].CurveBrush.GetRGBAf();

                gl.Color(curveColor[0], curveColor[1], curveColor[2]);
                gl.Begin(OpenGL.GL_LINE_STRIP);
                var minValueList = CurvesList[index].DataPoints.Where(x => x.X <= MinX);
                var maxValueList = CurvesList[index].DataPoints.Where(x => x.X >= MaxX);
                var minValue = minValueList.Count() > 0 ? minValueList.Max(x => x.X) : MinX;
                var maxValue = maxValueList.Count() > 0 ? maxValueList.Min(x => x.X) : MaxX;

                for (int num = 0; num < CurvesList[index].DataPoints.Count; num++)
                {
                    var tempPoint = CurvesList[index].DataPoints[num];

                    if (tempPoint.X < minValue || tempPoint.X > maxValue)
                    {
                        continue;
                    }

                    // calculate posX refer to the axis
                    var posX = GetXFromTime(tempPoint.X, MinX, MaxX);

                    // calculate posY refer to the axis
                    var posY = (tempPoint.Y - MinY) / (MaxY - MinY) * height;

                    gl.Vertex(posX, posY);
                }

                gl.End();
            }

            #endregion
        }

        /// <summary>
        /// Draw string in opengl
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="dispText"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="foreColor"></param>
        private void DrawString(OpenGL gl, string dispText, int posX, int posY, float[] foreColor)
        {
            gl.DrawText(posX, posY, foreColor[0], foreColor[1], foreColor[2], this.FontFamily.Source, (float)this.FontSize, dispText);
        }

        /// <summary>
        /// Get the size of the display text for drawing
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="dispText"></param>
        /// <param name="textWidth"></param>
        /// <param name="textHeight"></param>
        private void GetSizeOfText(OpenGL gl, string dispText, ref double textWidth, ref double textHeight)
        {
            FormattedText formattedText = new FormattedText(dispText,
                Thread.CurrentThread.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(this.FontFamily.Source),
                this.FontSize,
                this.TicksBrush);

            textWidth = formattedText.Width;
            textHeight = formattedText.Extent;
        }

        /// <summary>
        /// Get datetime from posX, relative to the minX
        /// </summary>
        /// <param name="posX"></param>
        /// <returns></returns>
        private DateTime GetTimeFromX(double posX, DateTime _minX, DateTime _maxX)
        {
            // calculate posX refer to the axis
            var totalMilliseconds = posX / this.ActualWidth * (_maxX - _minX).TotalMilliseconds;
            return _minX + TimeSpan.FromMilliseconds(totalMilliseconds);
        }

        /// <summary>
        /// Get posX from datetime, relative to the minX
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private double GetXFromTime(DateTime time, DateTime _minX, DateTime _maxX)
        {
            // calculate posX refer to the axis
            return (time - _minX).TotalMilliseconds / (double)(_maxX - _minX).TotalMilliseconds * this.ActualWidth;
        }

        #region  Data Properties

        /// <summary>
        /// Draw FPS or not
        /// </summary>
        public bool DrawFPS
        {
            get { return (bool)GetValue(DrawFPSProperty); }
            set { SetValue(DrawFPSProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawFPS.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawFPSProperty =
            DependencyProperty.Register("DrawFPS", typeof(bool), typeof(AxesControl), new PropertyMetadata(false));

        /// <summary>
        /// The minimize value of Axis-Y
        /// </summary>
        public double MinY
        {
            get { return (double)GetValue(MinYProperty); }
            set { SetValue(MinYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinYProperty =
            DependencyProperty.Register("MinY", typeof(double), typeof(AxesControl), new PropertyMetadata(0.0));

        /// <summary>
        /// The maximize value of Axis-Y
        /// </summary>
        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxYProperty =
            DependencyProperty.Register("MaxY", typeof(double), typeof(AxesControl), new PropertyMetadata(1.0));

        /// <summary>
        /// Count of gap in Y-Ticks, only two ticks when the value is 0
        /// </summary>
        public uint YGapCount
        {
            get { return (uint)GetValue(YGapCountProperty); }
            set { SetValue(YGapCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntervalY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YGapCountProperty =
            DependencyProperty.Register("YGapCount", typeof(uint), typeof(AxesControl), new PropertyMetadata(5u));

        /// <summary>
        /// The minimize value of Axis-X
        /// </summary>
        public DateTime MinX
        {
            get { return (DateTime)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinXProperty =
            DependencyProperty.Register("MinX", typeof(DateTime), typeof(AxesControl), new PropertyMetadata(DateTime.Now));

        /// <summary>
        /// The maximize value of Axis-X
        /// </summary>
        public DateTime MaxX
        {
            get { return (DateTime)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxXProperty =
            DependencyProperty.Register("MaxX", typeof(DateTime), typeof(AxesControl), new PropertyMetadata(DateTime.Now.AddMinutes(5)));

        /// <summary>
        /// Count of gap in X-Ticks, only two ticks when the value is 0
        /// </summary>
        public uint XGapCount
        {
            get { return (uint)GetValue(XGapCountProperty); }
            set { SetValue(XGapCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntervalX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XGapCountProperty =
            DependencyProperty.Register("XGapCount", typeof(uint), typeof(AxesControl), new PropertyMetadata(5u));

        /// <summary>
        /// List of all the curves
        /// </summary>
        public List<CurvesData> CurvesList
        {
            get { return (List<CurvesData>)GetValue(CurvesListProperty); }
            set { SetValue(CurvesListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurvesListProperty =
            DependencyProperty.Register("CurvesList", typeof(List<CurvesData>), typeof(AxesControl), new PropertyMetadata(new List<CurvesData>()));
        #endregion

        #region Control Properties
        /// <summary>
        /// Background for axis
        /// </summary>
        public Brush AxisBackground
        {
            get { return (Brush)GetValue(AxisBackgroundProperty); }
            set { SetValue(AxisBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisBackgroundProperty =
            DependencyProperty.Register("AxisBackground", typeof(Brush), typeof(AxesControl), new PropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(BackgroundChanged)));

        private static void BackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AxesControl ctrl = d as AxesControl;
            ctrl.Background = ctrl.AxisBackground;
        }

        /// <summary>
        /// Brush for Axis line
        /// </summary>
        public Brush AxisBrush
        {
            get { return (Brush)GetValue(AxisBrushProperty); }
            set { SetValue(AxisBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisBrushProperty =
            DependencyProperty.Register("AxisBrush", typeof(Brush), typeof(AxesControl), new PropertyMetadata(Brushes.Black));


        /// <summary>
        /// Brush for secondary lines
        /// </summary>
        public Brush SecondaryGridBrush
        {
            get { return (Brush)GetValue(SecondaryGridBrushProperty); }
            set { SetValue(SecondaryGridBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondaryGridBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondaryGridBrushProperty =
            DependencyProperty.Register("SecondaryGridBrush", typeof(Brush), typeof(AxesControl), new PropertyMetadata(Brushes.Black));


        /// <summary>
        /// Brush for Ticks
        /// </summary>
        public Brush TicksBrush
        {
            get { return (Brush)GetValue(TicksBrushProperty); }
            set { SetValue(TicksBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TicksBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TicksBrushProperty =
            DependencyProperty.Register("TicksBrush", typeof(Brush), typeof(AxesControl), new PropertyMetadata(Brushes.Black));

        public Brush TicksScaleBrush
        {
            get { return (Brush)GetValue(TicksScaleBrushProperty); }
            set { SetValue(TicksScaleBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TicksScaleBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TicksScaleBrushProperty =
            DependencyProperty.Register("TicksScaleBrush", typeof(Brush), typeof(AxesControl), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Is dot dash for secondary lines or not
        /// </summary>
        public bool IsDotDash
        {
            get { return (bool)GetValue(IsDotDashProperty); }
            set { SetValue(IsDotDashProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDotDash.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDotDashProperty =
            DependencyProperty.Register("IsDotDash", typeof(bool), typeof(AxesControl), new PropertyMetadata(false));


        /// <summary>
        /// Flag for secondary line, true to visible
        /// </summary>
        public bool IsSecondaryLineVisible
        {
            get { return (bool)GetValue(IsSecondaryLineVisibleProperty); }
            set { SetValue(IsSecondaryLineVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSecondaryLineVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSecondaryLineVisibleProperty =
            DependencyProperty.Register("IsSecondaryLineVisible", typeof(bool), typeof(AxesControl), new PropertyMetadata(false));

        /// <summary>
        /// Is boxed for axis(with top line and right line) or not
        /// </summary>
        public bool IsBoxed
        {
            get { return (bool)GetValue(IsBoxedProperty); }
            set { SetValue(IsBoxedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBoxed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBoxedProperty =
            DependencyProperty.Register("IsBoxed", typeof(bool), typeof(AxesControl), new PropertyMetadata(false));

        /// <summary>
        /// only a status for pan. enabled panning when roll disabled.
        /// </summary>
        public bool IsRollEnabled
        {
            get { return (bool)GetValue(IsRollEnabledProperty); }
            set { SetValue(IsRollEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRollEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRollEnabledProperty =
            DependencyProperty.Register("IsRollEnabled", typeof(bool), typeof(AxesControl), new PropertyMetadata(false));

        #endregion
    }
}
