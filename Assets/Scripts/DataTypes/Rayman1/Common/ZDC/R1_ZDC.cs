﻿using System.IO;
using R1Engine.Serialize;

namespace R1Engine
{
    public class R1_ZDC
    {
        private static byte[] PC_Type_ZDC => new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x10, 0x02, 0x08, 0x03, 0x08, 0x04, 0x10, 0x06, 0x08, 0x00, 0x00,
            0x07, 0x10, 0x09, 0x08, 0x0A, 0x10, 0x0C, 0x08, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x0D, 0x08, 0x00, 0x00, 0x0E, 0x08, 0x0F, 0x08, 0x00, 0x00, 0x00, 0x00,
            0x10, 0x08, 0x11, 0x08, 0x00, 0x00, 0x00, 0x00, 0x12, 0x08, 0x13, 0x08,
            0x14, 0x08, 0x00, 0x00, 0x00, 0x00, 0x15, 0x08, 0x00, 0x00, 0x16, 0x08,
            0x00, 0x00, 0x00, 0x00, 0x17, 0x20, 0x1B, 0x08, 0x1C, 0x08, 0x1D, 0x08,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x1E, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x08, 0x00, 0x00, 0x00, 0x00,
            0x20, 0x08, 0x21, 0x08, 0x22, 0x10, 0x00, 0x00, 0x00, 0x00, 0x24, 0x10,
            0x26, 0x08, 0x00, 0x00, 0x00, 0x00, 0x27, 0x08, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x28, 0x08, 0x29, 0x08, 0x2A, 0x08, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x08,
            0x2C, 0x18, 0x00, 0x00, 0x2F, 0x18, 0x00, 0x00, 0x00, 0x00, 0x32, 0x20,
            0x36, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x37, 0x08,
            0x00, 0x00, 0x38, 0x08, 0x00, 0x00, 0x39, 0x08, 0x3A, 0x08, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x3B, 0x08, 0x3C, 0x08, 0x3D, 0x08, 0x00, 0x00,
            0x3E, 0x40, 0x00, 0x00, 0x46, 0x08, 0x47, 0x08, 0x48, 0x08, 0x00, 0x00,
            0x49, 0x08, 0x4A, 0x18, 0x4D, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x4F, 0x08, 0x00, 0x00, 0x00, 0x00, 0x50, 0x08,
            0x00, 0x00, 0x00, 0x00, 0x51, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x52, 0x08, 0x00, 0x00, 0x53, 0x20, 0x00, 0x00, 0x00, 0x00,
            0x57, 0x38, 0x5E, 0x08, 0x5F, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x60, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x08,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x62, 0x10, 0x00, 0x00, 0x64, 0x08,
            0x65, 0x08, 0x00, 0x00, 0x66, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x67, 0x10, 0x69, 0x08, 0x6A, 0x10, 0x00, 0x00, 0x6C, 0x10, 0x6E, 0x10,
            0x70, 0x10, 0x00, 0x00, 0x72, 0x08, 0x73, 0x10, 0x75, 0x10, 0x77, 0x08,
            0x00, 0x00, 0x78, 0x10, 0x7A, 0x08, 0x7B, 0x08, 0x00, 0x00, 0x7C, 0x08,
            0x7D, 0x28, 0x82, 0x10, 0x84, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x86, 0x08,
            0x00, 0x00, 0x00, 0x00, 0x87, 0x08, 0x00, 0x00, 0x00, 0x00, 0x88, 0x08,
            0x89, 0x08, 0x00, 0x00, 0x8A, 0x08, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x8C, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x90, 0x30, 0x96, 0x08, 0x97, 0x10, 0x99, 0x10, 0x9B, 0x20,
            0x00, 0x00, 0x9F, 0x08, 0xA0, 0x08, 0xA1, 0x10, 0xA3, 0x10, 0xA5, 0x08,
            0xA6, 0x08, 0x00, 0x00, 0xA7, 0x08, 0x00, 0x00, 0x00, 0x00, 0xA8, 0x10,
            0xAA, 0x10, 0xAC, 0x10, 0xAE, 0x10, 0xB0, 0x10, 0xB2, 0x28, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xB7, 0x08, 0x00, 0x00, 0xB8, 0x08, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0xF7,
            0x01, 0x00, 0x94, 0x1D, 0x05, 0x00, 0xA4, 0xEE, 0x01, 0x00, 0x80, 0x1E,
            0x05, 0x00, 0x30, 0x05
        };

        private static byte[] PC_ZDCTable => new byte[]
        {
            0x54, 0x00, 0x2C, 0x00, 0x0D, 0x11, 0x04, 0xFF, 0x03, 0x00, 0x04, 0x00,
            0x03, 0x0D, 0x03, 0x00, 0x0C, 0x00, 0x0B, 0x00, 0x10, 0x10, 0x03, 0x04,
            0x07, 0x00, 0x06, 0x00, 0x1A, 0x19, 0x01, 0xFF, 0x02, 0x00, 0x01, 0x00,
            0x08, 0x0D, 0x03, 0x02, 0x4C, 0x00, 0x3F, 0x00, 0x0A, 0x16, 0x01, 0xFF,
            0x05, 0x00, 0x08, 0x00, 0x0C, 0x08, 0x03, 0x00, 0x06, 0x00, 0xF6, 0xFF,
            0x0C, 0x23, 0x03, 0x02, 0x01, 0x00, 0x07, 0x00, 0x15, 0x05, 0x03, 0x04,
            0x47, 0x00, 0x31, 0x00, 0x12, 0x06, 0x01, 0xFF, 0x06, 0x00, 0xF6, 0xFF,
            0x0C, 0x23, 0x03, 0x02, 0x01, 0x00, 0x07, 0x00, 0x15, 0x05, 0x03, 0x04,
            0x47, 0x00, 0x31, 0x00, 0x12, 0x06, 0x01, 0xFF, 0xEB, 0xFF, 0xED, 0xFF,
            0x40, 0x40, 0x03, 0x00, 0x69, 0x00, 0xA8, 0x00, 0x30, 0x3C, 0x01, 0xFF,
            0x61, 0x00, 0x81, 0x00, 0x02, 0x02, 0x01, 0xFF, 0x08, 0x00, 0x06, 0x00,
            0x14, 0x14, 0x03, 0x00, 0x06, 0x00, 0x05, 0x00, 0x0A, 0x0B, 0x03, 0x00,
            0x4A, 0x00, 0x49, 0x00, 0x0D, 0x0E, 0x01, 0xFF, 0x09, 0x00, 0x0A, 0x00,
            0x10, 0x10, 0x03, 0x00, 0x0E, 0x00, 0x01, 0x00, 0x0E, 0x2C, 0x03, 0x00,
            0x09, 0x00, 0x0A, 0x00, 0x10, 0x10, 0x03, 0x00, 0x08, 0x00, 0x06, 0x00,
            0x14, 0x14, 0x03, 0x00, 0x40, 0x00, 0x30, 0x00, 0x20, 0x20, 0x01, 0xFF,
            0x00, 0x00, 0x00, 0x00, 0x10, 0x10, 0x03, 0x05, 0x11, 0x00, 0x01, 0x00,
            0x09, 0x09, 0x03, 0x07, 0x18, 0x00, 0x40, 0x00, 0x70, 0x60, 0x01, 0xFF,
            0x0B, 0x00, 0x0A, 0x00, 0x10, 0x10, 0x03, 0x00, 0x10, 0x00, 0x1A, 0x00,
            0x30, 0x26, 0x03, 0x00, 0x0B, 0x00, 0x0B, 0x00, 0x10, 0x10, 0x03, 0x00,
            0x01, 0x00, 0xF9, 0xFF, 0x0C, 0x22, 0x03, 0x02, 0x08, 0x00, 0x06, 0x00,
            0x14, 0x14, 0x03, 0x00, 0x01, 0x00, 0xF8, 0xFF, 0x0D, 0x1E, 0x03, 0x00,
            0x06, 0x00, 0x03, 0x00, 0x0B, 0x02, 0x03, 0x00, 0x11, 0x00, 0x0D, 0x00,
            0x3A, 0x4C, 0x03, 0x02, 0x0F, 0x00, 0x0E, 0x00, 0x17, 0x3B, 0x03, 0x05,
            0x3F, 0x00, 0x4D, 0x00, 0x15, 0x0A, 0x04, 0xFF, 0x08, 0x00, 0x05, 0x00,
            0x12, 0x17, 0x03, 0x03, 0x07, 0x00, 0x0E, 0x00, 0x06, 0x07, 0x03, 0x00,
            0x00, 0x00, 0x02, 0x00, 0xD0, 0x06, 0x03, 0x03, 0x4F, 0x00, 0x34, 0x00,
            0x06, 0x13, 0x01, 0xFF, 0x47, 0x00, 0x38, 0x00, 0x13, 0x12, 0x01, 0xFF,
            0x47, 0x00, 0x38, 0x00, 0x13, 0x12, 0x01, 0xFF, 0x4B, 0x00, 0x3A, 0x00,
            0x0C, 0x0C, 0x01, 0xFF, 0x06, 0x00, 0x04, 0x00, 0x18, 0x19, 0x06, 0x04,
            0x02, 0x00, 0x04, 0x00, 0x12, 0x0E, 0x03, 0x05, 0x08, 0x00, 0x08, 0x00,
            0x31, 0x33, 0x03, 0x06, 0x12, 0x00, 0x09, 0x00, 0x10, 0x10, 0x03, 0x01,
            0x13, 0x00, 0x02, 0x00, 0x0E, 0x44, 0x03, 0x03, 0xFD, 0xFF, 0xFE, 0xFF,
            0x0B, 0x07, 0x03, 0x0A, 0x0A, 0x00, 0x06, 0x00, 0x10, 0x13, 0x03, 0x00,
            0x0B, 0x00, 0x07, 0x00, 0x0E, 0x11, 0x03, 0x01, 0x0A, 0x00, 0x08, 0x00,
            0x11, 0x10, 0x03, 0x02, 0x18, 0x00, 0x18, 0x00, 0x36, 0x35, 0x03, 0x04,
            0x4C, 0x00, 0x3C, 0x00, 0x08, 0x09, 0x01, 0xFF, 0x15, 0x00, 0x14, 0x00,
            0x38, 0x38, 0x03, 0x00, 0x3F, 0x00, 0x28, 0x00, 0x2B, 0x2D, 0x01, 0xFF,
            0x7B, 0x00, 0x82, 0x00, 0x2B, 0x2D, 0x01, 0xFF, 0xA8, 0x00, 0x8C, 0x00,
            0x20, 0x30, 0x01, 0xFF, 0x02, 0x00, 0xF7, 0xFF, 0x08, 0x1C, 0x03, 0x02,
            0x02, 0x00, 0xF7, 0xFF, 0x08, 0x1C, 0x03, 0x02, 0x02, 0x00, 0xF7, 0xFF,
            0x08, 0x1C, 0x03, 0x02, 0x0A, 0x00, 0x06, 0x00, 0x13, 0x0B, 0x03, 0x00,
            0x0A, 0x00, 0x06, 0x00, 0x13, 0x0B, 0x03, 0x01, 0x02, 0x00, 0x14, 0x00,
            0x20, 0x25, 0x03, 0x02, 0x0B, 0x00, 0x0D, 0x00, 0x3B, 0x0C, 0x03, 0x03,
            0x12, 0x00, 0x0E, 0x00, 0x29, 0x29, 0x03, 0x0E, 0x12, 0x00, 0x0E, 0x00,
            0x29, 0x29, 0x03, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x13, 0x20, 0x06, 0x0A,
            0x04, 0x00, 0x00, 0x00, 0x13, 0x20, 0x06, 0x0B, 0x4A, 0x00, 0x49, 0x00,
            0x0D, 0x0E, 0x01, 0xFF, 0x46, 0x00, 0x44, 0x00, 0x0E, 0x0C, 0x01, 0xFF,
            0x0F, 0x00, 0x03, 0x00, 0x0C, 0x2A, 0x03, 0x00, 0x6D, 0x00, 0x33, 0x00,
            0x36, 0x38, 0x01, 0xFF, 0x6F, 0x00, 0x98, 0x00, 0x33, 0x17, 0x01, 0xFF,
            0x4F, 0x00, 0x53, 0x00, 0x74, 0x26, 0x01, 0xFF, 0x32, 0x00, 0x72, 0x00,
            0x34, 0x0F, 0x01, 0xFF, 0x47, 0x00, 0x8C, 0x00, 0x36, 0x15, 0x01, 0xFF,
            0x47, 0x00, 0x8C, 0x00, 0x36, 0x15, 0x01, 0x00, 0x07, 0x00, 0x06, 0x00,
            0x1A, 0x19, 0x01, 0xFF, 0x44, 0x00, 0x35, 0x00, 0x21, 0x22, 0x01, 0xFF,
            0x04, 0x00, 0x01, 0x00, 0x10, 0x1C, 0x03, 0x02, 0x09, 0x00, 0x09, 0x00,
            0x24, 0x09, 0x03, 0x00, 0x40, 0x00, 0x30, 0x00, 0x20, 0x20, 0x01, 0xFF,
            0x00, 0x00, 0x00, 0x00, 0x10, 0x10, 0x03, 0x05, 0x11, 0x00, 0x01, 0x00,
            0x09, 0x09, 0x03, 0x07, 0x18, 0x00, 0x40, 0x00, 0x70, 0x60, 0x01, 0xFF,
            0x03, 0x00, 0x03, 0x00, 0x0C, 0x0C, 0x03, 0x04, 0x07, 0x00, 0x04, 0x00,
            0x0D, 0x0B, 0x03, 0x06, 0x04, 0x00, 0x03, 0x00, 0x11, 0x11, 0x03, 0x07,
            0x0C, 0x00, 0x0B, 0x00, 0x1F, 0x1C, 0x03, 0x0B, 0x0F, 0x00, 0x14, 0x00,
            0x16, 0x2F, 0x03, 0x0C, 0x0D, 0x00, 0xFA, 0xFF, 0x35, 0x21, 0x03, 0x10,
            0x06, 0x00, 0x07, 0x00, 0x1E, 0x0E, 0x03, 0x11, 0x47, 0x00, 0x31, 0x00,
            0x12, 0x06, 0x01, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x4B, 0x50, 0x03, 0x02,
            0x4F, 0x00, 0x34, 0x00, 0x06, 0x13, 0x01, 0xFF, 0x07, 0x00, 0x05, 0x00,
            0x13, 0x13, 0x01, 0xFF, 0x02, 0x00, 0x01, 0x00, 0x08, 0x0D, 0x03, 0x02,
            0x4C, 0x00, 0x3F, 0x00, 0x0A, 0x16, 0x01, 0xFF, 0x07, 0x00, 0x06, 0x00,
            0x1A, 0x19, 0x01, 0xFF, 0x00, 0x00, 0x02, 0x00, 0xD0, 0x06, 0x03, 0x03,
            0x04, 0x00, 0x04, 0x00, 0x10, 0x10, 0x03, 0x00, 0xFB, 0xFF, 0xF4, 0xFF,
            0x1B, 0x2A, 0x03, 0x05, 0x05, 0x00, 0x05, 0x00, 0x11, 0x10, 0x03, 0x06,
            0x02, 0x00, 0xFF, 0xFF, 0x02, 0x1B, 0x03, 0x01, 0x47, 0x00, 0x0B, 0x00,
            0x0E, 0x6A, 0x04, 0xFF, 0x04, 0x00, 0xFA, 0xFF, 0x02, 0x16, 0x03, 0x03,
            0x47, 0x00, 0x0B, 0x00, 0x0E, 0x6A, 0x04, 0xFF, 0x04, 0x00, 0xFA, 0xFF,
            0x02, 0x16, 0x03, 0x03, 0x47, 0x00, 0x0B, 0x00, 0x0E, 0x6A, 0x04, 0xFF,
            0x04, 0x00, 0xFA, 0xFF, 0x02, 0x16, 0x03, 0x03, 0x11, 0x00, 0x0D, 0x00,
            0x3A, 0x4C, 0x03, 0x02, 0x0F, 0x00, 0x0E, 0x00, 0x17, 0x3B, 0x03, 0x05,
            0x4F, 0x00, 0x34, 0x00, 0x06, 0x13, 0x01, 0xFF, 0x11, 0x00, 0x0D, 0x00,
            0x3A, 0x4C, 0x03, 0x02, 0x0F, 0x00, 0x0E, 0x00, 0x17, 0x3B, 0x03, 0x05,
            0xFB, 0xFF, 0xF4, 0xFF, 0x1B, 0x2A, 0x03, 0x05, 0x05, 0x00, 0x05, 0x00,
            0x11, 0x10, 0x03, 0x06, 0x4F, 0x00, 0x34, 0x00, 0x06, 0x13, 0x01, 0xFF,
            0x12, 0x00, 0x07, 0x00, 0x20, 0x1D, 0x03, 0x02, 0x07, 0x00, 0x00, 0x00,
            0x1A, 0x17, 0x03, 0x05, 0x09, 0x00, 0x0B, 0x00, 0x21, 0x0D, 0x01, 0xFF,
            0x09, 0x00, 0x0B, 0x00, 0x21, 0x0D, 0x01, 0xFF, 0xFA, 0xFF, 0x7F, 0xFF,
            0x16, 0x8B, 0x03, 0x03, 0x10, 0x00, 0x04, 0x00, 0x08, 0x08, 0x03, 0x00,
            0xF8, 0xFF, 0x04, 0x00, 0x08, 0x08, 0x03, 0x01, 0x00, 0x00, 0x04, 0x00,
            0x08, 0x08, 0x03, 0x02, 0x04, 0x00, 0x04, 0x00, 0x08, 0x08, 0x03, 0x04,
            0x04, 0x00, 0x04, 0x00, 0x08, 0x08, 0x03, 0x05, 0x04, 0x00, 0x0C, 0x00,
            0x17, 0x06, 0x03, 0x00, 0x07, 0x00, 0x0C, 0x00, 0x18, 0x08, 0x03, 0x01,
            0x04, 0x00, 0x0C, 0x00, 0x17, 0x06, 0x03, 0x00, 0x07, 0x00, 0x0C, 0x00,
            0x18, 0x08, 0x03, 0x01, 0x0E, 0x00, 0x05, 0x00, 0x5D, 0x0C, 0x03, 0x08,
            0x03, 0x00, 0x02, 0x00, 0x16, 0x05, 0x03, 0x01, 0x0C, 0x00, 0xD8, 0xFF,
            0x3C, 0x99, 0x01, 0xFF, 0x48, 0x00, 0xD8, 0xFF, 0x3C, 0x99, 0x01, 0xFF,
            0x69, 0x00, 0xA8, 0x00, 0x30, 0x3C, 0x01, 0xFF, 0xA8, 0x00, 0x8C, 0x00,
            0x20, 0x30, 0x01, 0xFF, 0x0A, 0x00, 0x06, 0x00, 0x15, 0x10, 0x03, 0x00,
            0x05, 0x00, 0x04, 0x00, 0x0C, 0x0C, 0x03, 0x03, 0x06, 0x00, 0x04, 0x00,
            0x0C, 0x0C, 0x03, 0x06, 0x0B, 0x00, 0x07, 0x00, 0x17, 0x12, 0x03, 0x08,
            0x07, 0x00, 0x02, 0x00, 0x10, 0x09, 0x03, 0x02, 0x0C, 0x00, 0x07, 0x00,
            0x2F, 0x0F, 0x03, 0x04, 0x0F, 0x00, 0x07, 0x00, 0x38, 0x11, 0x03, 0x05,
            0x04, 0x00, 0x02, 0x00, 0x06, 0x32, 0x03, 0x07, 0x0A, 0x00, 0x16, 0x00,
            0x38, 0x07, 0x03, 0x08, 0x16, 0x00, 0x12, 0x00, 0x16, 0x32, 0x03, 0x06,
            0x10, 0x00, 0x02, 0x00, 0x2B, 0x1E, 0x03, 0x01, 0xFB, 0xFF, 0xF4, 0xFF,
            0x1B, 0x2A, 0x03, 0x05, 0x05, 0x00, 0x05, 0x00, 0x11, 0x10, 0x03, 0x06,
            0xFB, 0xFF, 0xF4, 0xFF, 0x1B, 0x2A, 0x03, 0x05, 0x05, 0x00, 0x05, 0x00,
            0x11, 0x10, 0x03, 0x06, 0x40, 0x00, 0x30, 0x00, 0x20, 0x20, 0x01, 0xFF,
            0x00, 0x00, 0x00, 0x00, 0x10, 0x10, 0x03, 0x05, 0x11, 0x00, 0x01, 0x00,
            0x09, 0x09, 0x03, 0x07, 0x18, 0x00, 0x40, 0x00, 0x70, 0x60, 0x01, 0xFF,
            0x02, 0x00, 0x02, 0x00, 0x02, 0x28, 0x03, 0x01, 0x01, 0x00, 0x01, 0x00,
            0x1F, 0x02, 0x03, 0x01, 0xF3, 0xFF, 0xF3, 0xFF, 0x30, 0x2B, 0x03, 0x03,
            0x05, 0x00, 0x02, 0x00, 0x1E, 0x11, 0x03, 0x07, 0xF3, 0xFF, 0xF3, 0xFF,
            0x30, 0x2B, 0x03, 0x03, 0x05, 0x00, 0x02, 0x00, 0x1E, 0x11, 0x03, 0x07,
            0x0D, 0x00, 0x04, 0x00, 0x07, 0x08, 0x03, 0x08, 0x00, 0x00, 0x50, 0x00,
            0x80, 0x01, 0x01, 0xFF, 0x40, 0x00, 0x00, 0x00, 0x20, 0x80, 0x01, 0xFF,
            0x06, 0x00, 0x05, 0x00, 0x07, 0x11, 0x03, 0x01, 0x07, 0x00, 0xF8, 0xFF,
            0x09, 0x22, 0x03, 0x03, 0x47, 0x00, 0x21, 0x00, 0x0E, 0x5F, 0x04, 0xFF,
            0x04, 0x00, 0x04, 0x00, 0x02, 0x13, 0x03, 0x02, 0x47, 0x00, 0x21, 0x00,
            0x0E, 0x5F, 0x04, 0xFF, 0x04, 0x00, 0x04, 0x00, 0x02, 0x13, 0x03, 0x02,
            0x47, 0x00, 0x21, 0x00, 0x0E, 0x5F, 0x04, 0xFF, 0x04, 0x00, 0x04, 0x00,
            0x02, 0x13, 0x03, 0x02, 0x54, 0x00, 0x2A, 0x00, 0x0D, 0x62, 0x04, 0xFF,
            0x05, 0x00, 0x01, 0x00, 0x02, 0x13, 0x03, 0x00, 0x10, 0x00, 0x04, 0x00,
            0x08, 0x08, 0x03, 0x00, 0xF8, 0xFF, 0x04, 0x00, 0x08, 0x08, 0x03, 0x01,
            0x00, 0x00, 0x04, 0x00, 0x08, 0x08, 0x03, 0x02, 0x04, 0x00, 0x04, 0x00,
            0x08, 0x08, 0x03, 0x04, 0x04, 0x00, 0x04, 0x00, 0x08, 0x08, 0x03, 0x05,
            0x0B, 0x00, 0xBA, 0x00, 0x38, 0x22, 0x01, 0xFF, 0x10, 0x00, 0xE8, 0xFF,
            0x02, 0x51, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00
        };

        public R1_TypeZDC[] TypeZDC { get; set; }
        public R1_ZDCData[] ZDCTable { get; set; }

        public void Serialize(SerializerObject s)
        {
            Pointer typeOffset;
            int typeLength;
            Pointer tableOffset;
            int tableLength;

            switch (s.GameSettings.EngineVersion)
            {
                // TODO: Read from PS1 executables
                case EngineVersion.R1_PS1:
                case EngineVersion.R1_PS1_JP:
                case EngineVersion.R1_Saturn:
                case EngineVersion.R1_PS1_JPDemoVol3:
                case EngineVersion.R1_PS1_JPDemoVol6:
                    return;

                // For GBA and DSi we could read directly from the ROM data
                case EngineVersion.R1_PC:
                case EngineVersion.R1_PocketPC:
                case EngineVersion.R1_GBA:
                case EngineVersion.R1_DSi:

                    typeLength = 272;
                    tableLength = 200;

                    var typeKey = $"{nameof(PC_Type_ZDC)}";

                    if (!s.Context.FileExists(typeKey))
                    {
                        var typeStream = new MemoryStream(PC_Type_ZDC);
                        s.Context.AddFile(new StreamFile(typeKey, typeStream, s.Context));
                    }

                    typeOffset = s.Context.GetFile(typeKey).StartPointer;

                    var tableKey = $"{nameof(PC_ZDCTable)}";

                    if (!s.Context.FileExists(tableKey))
                    {
                        var tableStream = new MemoryStream(PC_ZDCTable);
                        s.Context.AddFile(new StreamFile(tableKey, tableStream, s.Context));
                    }

                    tableOffset = s.Context.GetFile(tableKey).StartPointer;

                    break;

                // TODO: Get EDU and KIT tables
                case EngineVersion.R1_PC_Kit:
                case EngineVersion.R1_PC_Edu:
                case EngineVersion.R1_PS1_Edu:
                    return;

                default:
                    return;
            }

            TypeZDC = s.DoAt(typeOffset, () => s.SerializeObjectArray<R1_TypeZDC>(TypeZDC, typeLength, name: nameof(TypeZDC)));
            ZDCTable = s.DoAt(tableOffset, () => s.SerializeObjectArray<R1_ZDCData>(ZDCTable, tableLength, name: nameof(ZDCTable)));
        }
    }
}