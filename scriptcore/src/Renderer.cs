﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FEEngine.Math;

namespace FEEngine
{
    public class Renderer
    {
        public enum Color
        {
            RED,
            GREEN,
            BLUE,
            YELLOW,
            WHITE,
            BLACK,
            NONE
        }
        private ulong memoryAddress;
        public void RenderCharAt(Vec2<int> position, char character, Color color = Color.NONE, Color backgroundColor = Color.NONE)
        {
            RenderCharAt_Native(this.memoryAddress, position, character, color, backgroundColor);
        }
        public void RenderStringAt(Vec2<int> position, String text, Color color = Color.NONE, Color backgroundColor = Color.NONE)
        {
            RenderStringAt_Native(this.memoryAddress, position, text, color, backgroundColor);
        }
        public Vec2<int> GetBufferSize()
        {
            return GetBufferSize_Native(this.memoryAddress);
        }
        private Renderer()
        {
            this.memoryAddress = 0;
        }
        public static Renderer MakeFromMemoryAddress(ulong address)
        {
            Renderer r = new Renderer();
            r.memoryAddress = address;
            return r;
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void RenderCharAt_Native(ulong address, Vec2<int> position, char character, Color color, Color backgroundColor);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void RenderStringAt_Native(ulong address, Vec2<int> position, String text, Color color, Color backgroundColor);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern Vec2<int> GetBufferSize_Native(ulong address);
    }
}
