using PDFiumCore;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApplePDF.PdfKit
{
    public class PdfSegmentPath
    {
        public PdfSegmentFlag Type;
        /// <summary>
        /// If is <see cref="PdfBezierSegmentPath"/>, this is End point of Bezier.
        /// </summary>
        public PointF Position;
        public bool IsCloseToStart;

        /// <summary>
        /// 产生绘制矩形的路径.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static List<PdfSegmentPath> GenerateRectSegments(float x, float y, float w, float h)
        {
            var path = new List<PdfSegmentPath>();
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_MOVETO, Position = new PointF(x, y) });
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x + w, y) });
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x + w, y - h) });
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x, y - h) });
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x, y) });
            return path;
        }

        /// <summary>
        /// 产生绘制圆角矩形的路径.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static List<PdfSegmentPath> GenerateRoundRectSegments(float x, float y, float w, float h, float radius)
        {
            var path = new List<PdfSegmentPath>();
            path.AddRange(GenerateArcSegments(x + radius, y - radius, radius, 90, 180));//左上角圆弧
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x, y - h + radius) });
            path.AddRange(GenerateArcSegments(x + radius, y - h + radius, radius, 180, 270));//左下角圆弧
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x + w - radius, y - h) });
            path.AddRange(GenerateArcSegments(x + w - radius, y - h + radius, radius, 270, 360));//右下角圆弧
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x + w, y - radius) });
            path.AddRange(GenerateArcSegments(x + w - radius, y - radius, radius, 0, 90));//右上角圆弧
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x + radius, y) });
            return path;
        }

        /// <summary>
        /// 产生绘制三角形的路径.
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="pointC"></param>
        /// <returns></returns>
        public static List<PdfSegmentPath> GenerateTriangleSegments(PointF pointA, PointF pointB, PointF pointC)
        {
            var path = new List<PdfSegmentPath>();
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_MOVETO, Position = pointA });
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = pointB });
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = pointC });
            path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = pointA });
            return path;
        }

        /// <summary>
        /// 用来计算绘制圆形贝塞尔曲线控制点的位置的常数
        /// </summary>
        const float C = 0.552284749831f;
        /// <summary>
        /// 产生绘制圆的路径.
        /// 其使用贝塞尔曲线模拟圆,模拟圆原理参考:https://www.jianshu.com/p/5198d8aa80c1
        /// </summary>
        /// <param name="mCircleRadius"></param>
        /// <returns></returns>
        public static List<PdfSegmentPath> GenerateCircleSegments(float mCircleRadius = 100, float centerX = 0, float centerY = 0)
        {
            var path = new List<PdfSegmentPath>();
            float[] mData = new float[8];               // 顺时针记录绘制圆形的四个数据点
            float[] mCtrl = new float[16];              // 顺时针记录绘制圆形的八个控制点

            void initData()
            {
                // 初始化数据点
                mData[0] = centerX;
                mData[1] = centerY + mCircleRadius;

                mData[2] = centerX + mCircleRadius;
                mData[3] = centerY;

                mData[4] = centerX;
                mData[5] = centerY - mCircleRadius;

                mData[6] = centerX - mCircleRadius;
                mData[7] = centerY;

                // 初始化控制点
                float mDifference = mCircleRadius * C;  //圆形的控制点与数据点的差值
                mCtrl[0] = mData[0] + mDifference;
                mCtrl[1] = mData[1];

                mCtrl[2] = mData[2];
                mCtrl[3] = mData[3] + mDifference;

                mCtrl[4] = mData[2];
                mCtrl[5] = mData[3] - mDifference;

                mCtrl[6] = mData[4] + mDifference;
                mCtrl[7] = mData[5];

                mCtrl[8] = mData[4] - mDifference;
                mCtrl[9] = mData[5];

                mCtrl[10] = mData[6];
                mCtrl[11] = mData[7] - mDifference;

                mCtrl[12] = mData[6];
                mCtrl[13] = mData[7] + mDifference;

                mCtrl[14] = mData[0] - mDifference;
                mCtrl[15] = mData[1];
            }

            void onDraw(List<PdfSegmentPath> path)
            {
                // 绘制贝塞尔曲线
                path.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_MOVETO, Position = new PointF(mData[0], mData[1]) });

                path.Add(new PdfBezierSegmentPath() { ControlPoint1 = new PointF(mCtrl[0], mCtrl[1]), ControlPoint2 = new PointF(mCtrl[2], mCtrl[3]), Position = new PointF(mData[2], mData[3]) });
                path.Add(new PdfBezierSegmentPath() { ControlPoint1 = new PointF(mCtrl[4], mCtrl[5]), ControlPoint2 = new PointF(mCtrl[6], mCtrl[7]), Position = new PointF(mData[4], mData[5]) });
                path.Add(new PdfBezierSegmentPath() { ControlPoint1 = new PointF(mCtrl[8], mCtrl[9]), ControlPoint2 = new PointF(mCtrl[10], mCtrl[11]), Position = new PointF(mData[6], mData[7]) });
                path.Add(new PdfBezierSegmentPath() { ControlPoint1 = new PointF(mCtrl[12], mCtrl[13]), ControlPoint2 = new PointF(mCtrl[14], mCtrl[15]), Position = new PointF(mData[0], mData[1]) });
            }

            initData();
            onDraw(path);
            return path;
        }

        /// <summary>
        /// 产生绘制圆弧的路径, 请确保startAngle小于endAngle, 即绘制的计算是逆时针的.
        /// 其使用贝塞尔曲线模拟圆弧, 模拟圆弧有原理参考:https://www.e-planet.cn/news_page-101.html
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <returns></returns>
        public static List<PdfSegmentPath> GenerateArcSegments(float centerX, float centerY, float radius, float startAngle, float endAngle)
        {
            var paths = new List<PdfSegmentPath>();
            //平方
            float Square(float x)
            {
                return x * x;
            }

            //已知圆心和半径, 有圆方程 (x-cx)²+(y-cy)²=R²
            var cx = centerX;
            var cy = centerY;
            var R = radius;

            //参考Windows图形编程8.6.3,贝塞尔绘制半圆会不精确,大于1/4圆的我们先切割
            if (endAngle - startAngle >= 90)
            {
                int splitAngle = 90;
                var count = (endAngle - startAngle) / splitAngle;
                int countInt = (int)count;
                for (int index = 0; index < countInt; index++)
                {
                    paths.AddRange(GetArcSegments(startAngle + index * splitAngle, startAngle + (index + 1) * splitAngle));
                }
                if (countInt < count)//除以90后剩余的
                    paths.AddRange(GetArcSegments(startAngle + countInt * splitAngle, endAngle));
            }
            else
            {
                paths.AddRange(GetArcSegments(startAngle, endAngle));
            }

            List<PdfSegmentPath> GetArcSegments(double startAngle, double endAngle)
            {
                //已知角度为startAngle, 设过圆上某点(x1,y1),有切线方程 (x1-cx)(x-cx)+(y1-cy)(y-cy)=R², 某点位置有 x1=cx+R*cos(startAngle), y1=cy+R*sin(startAngle)
                double startRadian = startAngle / 180 * Math.PI;//角度转弧度
                double endRadian = endAngle / 180 * Math.PI;
                var x1 = cx + (float)(R * Math.Cos(startRadian));//端点
                var y1 = cy + (float)(R * Math.Sin(startRadian));
                var x2 = cx + (float)(R * Math.Cos(endRadian));
                var y2 = cy + (float)(R * Math.Sin(endRadian));
                float controlLineLength = 4f / 3 * (float)Math.Tan((endRadian - startRadian) / 4) * R;//贝塞尔画圆最优解推导出的控制点到端点的距离公式
                float x01;//控制点1坐标
                float y01;
                if (Math.Abs(y1 - cy) < 0.00001)//是否相等
                {
                    if (startAngle == 0 || startAngle % 360 == 0)//点和中心同x,在中心左边,控制点在上面
                    {
                        x01 = x1;
                        y01 = y1 + controlLineLength;
                    }
                    else//点和中心同x,在中心右边,控制点在下面
                    {
                        x01 = x1;
                        y01 = y1 - controlLineLength;
                    }
                }
                else
                {
                    //设该点的控制点坐标(x0,y0),有 (x1-cx)(x0-cx)+(y1-cy)(y0-cy)=R², (x0-x1)²+(y0-y1)² = (4/3tan((endAngle-startAngle)/4)))², 可求得控制点1
                    //带入计算,利用一元二次方程求根公式
                    var temp = Square(R) + (x1 - cx) * cx - Square(y1 - cy);
                    var a = 1 + Square(x1 - cx) / Square(y1 - cy);
                    var b = (-2) * x1 - 2 * temp * (x1 - cx) / Square(y1 - cy);
                    var c = Square(x1) + Square(temp) / Square(y1 - cy) - Square(controlLineLength);
                    if ((y1 - cy) > 0.00001)//相比中心点上下
                    {
                        x01 = (-1 * b - (float)Math.Sqrt(Square(b) - 4 * a * c)) / (2 * a);
                    }
                    else
                    {
                        x01 = (-1 * b + (float)Math.Sqrt(Square(b) - 4 * a * c)) / (2 * a);
                    }
                    y01 = (Square(R) - (x1 - cx) * (x01 - cx)) / (y1 - cy) + cy;
                }
                //计算另一个控制点,使用对称性计算
                //计算对称的线Ax+By+C=0
                //(y-cy)/(x-cx)= tan(startAngle+(endAngle-startangle)/2)
                var A = (float)Math.Tan(startRadian + (endRadian - startRadian) / 2);
                var B = -1;
                var C = -1 * cx * (float)Math.Tan(startRadian + (endRadian - startRadian) / 2) + cy;
                var controlPoint1X = x01;
                var controlPoint1Y = y01;
                //对称点公式, 参考https://blog.csdn.net/changbaolong/article/details/7414796
                var controlPoint2X = ((B * B - A * A) * controlPoint1X - 2 * A * B * controlPoint1Y - 2 * A * C) / (A * A + B * B);
                var controlPoint2Y = (-2 * A * B * controlPoint1X + (A * A - B * B) * controlPoint1Y - 2 * B * C) / (A * A + B * B);

                var paths = new List<PdfSegmentPath>();
                paths.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_MOVETO, Position = new PointF(x1, y1) });
                paths.Add(new PdfBezierSegmentPath()
                {
                    ControlPoint1 = new PointF(controlPoint1X, controlPoint1Y),
                    ControlPoint2 = new PointF(controlPoint2X, controlPoint2Y),
                    Position = new PointF(x2, y2)
                });
                //绘制控制线
                /*paths.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_MOVETO, Position = new PointF(x1, y1) });
                paths.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(controlPoint1X, controlPoint1Y) });
                paths.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(controlPoint2X, controlPoint2Y) });
                paths.Add(new PdfSegmentPath() { Type = PdfSegmentFlag.FPDF_SEGMENT_LINETO, Position = new PointF(x2, y2) });
                */
                return paths;
            }

            return paths;
        }
    }

    /// <summary>
    /// For <see cref="fpdf_edit.FPDFPathBezierTo"/>
    /// </summary>
    public class PdfBezierSegmentPath : PdfSegmentPath
    {
        public PointF ControlPoint1;
        public PointF ControlPoint2;

        public PdfBezierSegmentPath()
        {
            Type = PdfSegmentFlag.FPDF_SEGMENT_BEZIERTO;
        }
    }
}
