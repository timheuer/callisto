// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

using System;
#if NETFX_CORE
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
#endif

namespace Callisto.Controls.Common
{
    public static class MatrixEx
    {
        public static Matrix Translate(this Matrix matrix, double offsetX, double offsetY)
        {
            matrix.OffsetX += offsetX;
            matrix.OffsetY += offsetY;
            return matrix;
        }

        public static Matrix Scale(this Matrix matrix, double scaleX, double scaleY)
        {
            return Multiply(matrix, new Matrix(scaleX, 0d, 0d, scaleY, 0d, 0d));
        }

        public static Matrix Rotate(this Matrix matrix, double angle)
        {
            angle = (angle % 360d) / 180d * Math.PI;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            return Multiply(matrix, new Matrix(cos, sin, -sin, cos, 0d, 0d));
        }

        public static Matrix RotateAt(this Matrix matrix, double angle, double centerX, double centerY)
        {
            angle = (angle % 360d) / 180d * Math.PI;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var offsetX = centerX * (1d - cos) + centerY * sin;
            var offsetY = centerY * (1d - cos) - centerX * sin;
            return Multiply(matrix, new Matrix(cos, sin, -sin, cos, offsetX, offsetY));
        }

        public static Matrix Invert(this Matrix matrix)
        {
            var determinant = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
            return new Matrix(
                matrix.M22 / determinant, -matrix.M12 / determinant,
                -matrix.M21 / determinant, matrix.M11 / determinant,
                (matrix.M21 * matrix.OffsetY - matrix.M22 * matrix.OffsetX) / determinant,
                (matrix.M12 * matrix.OffsetX - matrix.M11 * matrix.OffsetY) / determinant);
        }

        public static Matrix Multiply(this Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(
                matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21,
                matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22,
                matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21,
                matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22,
                (matrix2.M11 * matrix1.OffsetX + matrix2.M21 * matrix1.OffsetY) + matrix2.OffsetX,
                (matrix2.M12 * matrix1.OffsetX + matrix2.M22 * matrix1.OffsetY) + matrix2.OffsetY);
        }
    }
}
