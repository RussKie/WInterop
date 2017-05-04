﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WInterop.Gdi;
using WInterop.Gdi.Types;

namespace WInterop.Extensions.WindowExtensions
{
    public static partial class DeviceContextExtensions
    {
        public static int GetDeviceCapability(this DeviceContext deviceContext, DeviceCapability capability)
            => GdiMethods.GetDeviceCapability(deviceContext, capability);
        public static GdiObjectHandle SelectObject(this DeviceContext deviceContext, GdiObjectHandle @object)
            => GdiMethods.SelectObject(deviceContext, @object);
        public static bool TextOut(this DeviceContext deviceContext, int x, int y, string text)
            => GdiMethods.TextOut(deviceContext, x, y, text);
        public static unsafe bool TextOut(this DeviceContext deviceContext, int x, int y, char* text, int length)
            => GdiMethods.TextOut(deviceContext, x, y, text, length);
        public static unsafe int DrawText(this DeviceContext deviceContext, string text, RECT rect, TextFormat format)
            => GdiMethods.DrawText(deviceContext, text, rect, format);
        public static TextAlignment SetTextAlignment(this DeviceContext deviceContext, TextAlignment alignment)
            => GdiMethods.SetTextAlignment(deviceContext, alignment);
        public static bool GetTextMetrics(this DeviceContext deviceContext, out TEXTMETRIC metrics)
            => GdiMethods.GetTextMetrics(deviceContext, out metrics);
        public static bool MoveTo(this DeviceContext deviceContext, int x, int y)
            => GdiMethods.MoveTo(deviceContext, x, y);
        public static bool LineTo(this DeviceContext deviceContext, int x, int y)
            => GdiMethods.LineTo(deviceContext, x, y);
        public static BackgroundMode SetBackgroundMode(this DeviceContext deviceContext, BackgroundMode mode)
            => GdiMethods.SetBackgroundMode(deviceContext, mode);
        public static PolyFillMode GetPolyFillMode(this DeviceContext deviceContext)
            => GdiMethods.GetPolyFillMode(deviceContext);
        public static PolyFillMode SetPolyFillMode(this DeviceContext deviceContext, PolyFillMode fillMode)
            => GdiMethods.SetPolyFillMode(deviceContext, fillMode);
        public unsafe static bool Polygon(this DeviceContext deviceContext, params POINT[] points)
            => GdiMethods.Polygon(deviceContext, points);
        public static bool Polyline(this DeviceContext deviceContext, params POINT[] points)
            => GdiMethods.Polyline(deviceContext, points);
        public static bool Rectangle(this DeviceContext deviceContext, int left, int top, int right, int bottom)
            => GdiMethods.Rectangle(deviceContext, left, top, right, bottom);
        public static bool Rectangle(this DeviceContext deviceContext, RECT rectangle) => GdiMethods.Rectangle(deviceContext, rectangle);
        public static bool FillRectangle(this DeviceContext deviceContext, RECT rectangle, BrushHandle brush) => GdiMethods.FillRectangle(deviceContext, rectangle, brush);
        public static bool Ellipse(this DeviceContext deviceContext, int left, int top, int right, int bottom)
            => GdiMethods.Ellipse(deviceContext, left, top, right, bottom);
        public static bool RoundRectangle(this DeviceContext deviceContext, int left, int top, int right, int bottom, int cornerWidth, int cornerHeight)
            => GdiMethods.RoundRectangle(deviceContext, left, top, right, bottom, cornerWidth, cornerHeight);
        public static bool PolyBezier(this DeviceContext deviceContext, params POINT[] points)
            => GdiMethods.PolyBezier(deviceContext, points);
        public static bool DeviceToLogical(this DeviceContext deviceContext, params POINT[] points) => GdiMethods.DeviceToLogical(deviceContext, points);
        public static bool LogicalToDevice(this DeviceContext deviceContext, params POINT[] points) => GdiMethods.LogicalToDevice(deviceContext, points);
        public static void SetViewportOrigin(this DeviceContext deviceContext, int x, int y) => GdiMethods.SetViewportOrigin(deviceContext, x, y);
        public static bool SetPixel(this DeviceContext deviceContext, int x, int y, COLORREF color) => GdiMethods.SetPixel(deviceContext, x, y, color);


        public static RegionType SelectClippingRegion(this DeviceContext deviceContext, RegionHandle region) => GdiMethods.SelectClippingRegion(deviceContext, region);
        public static RegionType CombineRegion(this RegionHandle destination, RegionHandle sourceOne, RegionHandle sourceTwo, CombineRegionMode mode) =>
            GdiMethods.CombineRegion(destination, sourceOne, sourceTwo, mode);
    }
}
