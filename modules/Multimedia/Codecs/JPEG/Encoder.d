// Experiment based on 
// https://github.com/PistonDevelopers/image/blob/master/src/jpeg/encoder.rs

// Markers ----------------------------------------------------
let SOF0 : u8 = 0xC0 // Baseline DCT
let DHT  : u8 = 0xC4 // Huffman Tables
let SOI  : u8 = 0xD8 // Start of Image (standalone)
let EOI  : u8 = 0xD9 // End of image (standalone)
let SOS  : u8 = 0xDA // Start of Scan
let DQT  : u8 = 0xDB // Quantization Tables
let APP0 : u8 = 0xE0 // Application segments start and end

// table K.1
let LUMA_QTABLE = Array<u8> {
    16, 11, 10, 16,  24,  40,  51,  61,
    12, 12, 14, 19,  26,  58,  60,  55,
    14, 13, 16, 24,  40,  57,  69,  56,
    14, 17, 22, 29,  51,  87,  80,  62,
    18, 22, 37, 56,  68, 109, 103,  77,
    24, 35, 55, 64,  81, 104, 113,  92,
    49, 64, 78, 87, 103, 121, 120, 101,
    72, 92, 95, 98, 112, 100, 103,  99,
}

// table K.2
let CHROMA_QTABLE = Array<u8> {
    17, 18, 24, 47, 99, 99, 99, 99,
    18, 21, 26, 66, 99, 99, 99, 99,
    24, 26, 56, 99, 99, 99, 99, 99,
    47, 66, 99, 99, 99, 99, 99, 99,
    99, 99, 99, 99, 99, 99, 99, 99,
    99, 99, 99, 99, 99, 99, 99, 99,
    99, 99, 99, 99, 99, 99, 99, 99,
    99, 99, 99, 99, 99, 99, 99, 99
}

// table K.3
let LUMA_DC_CODE_LENGTHS: = Array<u8> {
    0x00, 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01,
    0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
}

let LUMA_DC_VALUES = Array<u8> {
    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
    0x08, 0x09, 0x0A, 0x0B
}

// table K.4
let CHROMA_DC_CODE_LENGTHS = Array<u8> {
    0x00, 0x03, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
    0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00
}

let CHROMA_DC_VALUES = Array<u8> {
    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
    0x08, 0x09, 0x0A, 0x0B
}

// table k.5
let LUMA_AC_CODE_LENGTHS = Array<u8> {
    0x00, 0x02, 0x01, 0x03, 0x03, 0x02, 0x04, 0x03,
    0x05, 0x05, 0x04, 0x04, 0x00, 0x00, 0x01, 0x7D
}

let LUMA_AC `VALUES = new Array<u8> {
    0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07,
    0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xA1, 0x08, 0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0,
    0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0A, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28,
    0x29, 0x2A, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49,
    0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69,
    0x6A, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89,
    0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7,
    0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5,
    0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2,
    0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8,
    0xF9, 0xFA,
}

// table k.6
let CHROMA `AC` CODE `LENGTHS = new Array<u8> {
    0x00, 0x02, 0x01, 0x02, 0x04, 0x04, 0x03, 0x04,
    0x07, 0x05, 0x04, 0x04, 0x00, 0x01, 0x02, 0x77,
}

let CHROMA `AC `VALUES = new Array<u8> {
    0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21, 0x31, 0x06, 0x12, 0x41, 0x51, 0x07, 0x61, 0x71,
    0x13, 0x22, 0x32, 0x81, 0x08, 0x14, 0x42, 0x91, 0xA1, 0xB1, 0xC1, 0x09, 0x23, 0x33, 0x52, 0xF0,
    0x15, 0x62, 0x72, 0xD1, 0x0A, 0x16, 0x24, 0x34, 0xE1, 0x25, 0xF1, 0x17, 0x18, 0x19, 0x1A, 0x26,
    0x27, 0x28, 0x29, 0x2A, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48,
    0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68,
    0x69, 0x6A, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87,
    0x88, 0x89, 0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5,
    0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3,
    0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA,
    0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8,
    0xF9, 0xFA,
}

let DCCLASS: u8 = 0
let ACCLASS: u8 = 1

let LUMADESTINATION   : u8 = 0
let CHROMADESTINATION : u8 = 1
let LUMAID            : u8 = 1
let CHROMABLUEID      : u8 = 2
let CHROMAREDID       : u8 = 3

/// The permutation of dct coefficients.
let UNZIGZAG = Array<u8> {
    0,  1,  8, 16,  9,  2,  3, 10,
    17, 24, 32, 25, 18, 11,  4,  5,
    12, 19, 26, 33, 40, 48, 41, 34,
    27, 20, 13,  6,  7, 14, 21, 28,
    35, 42, 49, 56, 57, 50, 43, 36,
    29, 22, 15, 23, 30, 37, 44, 51,
    58, 59, 52, 45, 38, 31, 39, 46,
    53, 60, 61, 54, 47, 55, 62, 63,
}

JPEGComponent struct {
    id      : u8,
    h       : u8, // Horizontal sample factor
    v       : u8, // Vertical sample factor
    tq      : u8, // The quantization table selector
    dcTable : u8, // Index to the Huffman DC Table
    acTable : u8, // Index to the AC Huffman Table
    dcPred  : i32 // The dc prediction of the component
}

BitWriter impl for JPEGEncoder {
    writeBits(this self, bits: u16, size: u8) {
        if size == 0 {
            return Ok(())
        }

        accumulator |= (bits as u32) << (32 - (nbits + size))
        nbits += size

        while nbits >= 8 {
            let byte = (accumulator & (0xFFFFFFFFu32 << 24)) >> 24
            
            w.writeAll([byte as u8])

            if byte == 0xFF {
                w.writeAll([ 0x00 ])
            }

            nbits -= 8
            accumulator <<= 8
        }

        return Ok(())
    }

    padByte() {
      writeBits(0x7F, 7)
    }

    huffmanEncode(val: u8, table: Span<(u8, u16)>) {
      let (size, code) = table[val]

      if size > 16 {
        throw Error("bad huffman value")
      }

      writeBits(code, size)
    }

    writeBlock(
        block   : Span<i32>,
        prevdc  : i32,
        dctable : Span<(u8, u16)>,
        actable : Span<(u8, u16)>) {

        // Differential DC encoding
        let dcval = block[0]
        let diff  = dcval - prevdc
        let (size, value) = encodeCoefficient(diff)

        huffmanEncode(size, dctable)
        writeBits(value, size)

        // Figure F.2
        var zeroRun = 0
        var k = 0

        while true {
            k += 1;

            if block[UNZIGZAG[k]] == 0 {
                if k == 63 {
                    huffmanEncode(0x00, actable)
                    break
                }

                zeroRun += 1
            } 
            else {
                while zero_run > 15 {
                    huffmanEncode(0xF0, actable)
                    zeroRun -= 16
                }

                let (size, value) = encodeCoefficient(block[UNZIGZAG[k]])
                let symbol = (zero_run << 4) | size

                huffmanEncode(symbol, actable)
                writeBits(value, size)

                zeroRun = 0

                if k == 63 {
                    break
                }
            }
        }

        Ok(dcval)
    }

    writeSegment(this self, marker: u8, data: Option<[u8]>)  {
        w.writeAll(ref [0xFF])
        w.writeAll(ref [marker])

        if let Some(b) = data {
            w.writeU16:<BigEndian>(b.len() as u16 + 2)
            w.writeAll(ref b)
        }
    }
}

JPEGEncoder class {
    let writer            : BitWriter
    let components        : [] JPEGComponent
    let tables            : [] u8,
    let luma   `dctable   : [] (u8, u16)
    let luma   `actable   : [] (u8, u16) 
    let chroma `dctable   : [] (u8, u16) 
    let chroma `actable   : [] (u8, u16)
}

JPEGEncoder impl {
    from (w: &mut W, quality: u8 = 75) {
        let luma `dctable = buildHuff `lut(LUMA_DC_CODE_LENGTHS, LUMA_DC_VALUES)
        let luma `actable = buildHuff `lut(LUMA_AC_CODE_LENGTHS, LUMA_AC_VALUES)

        let chroma `dctable = buildHuff `lut(CHROMA_DC_CODE_LENGTHS, CHROMA_DC_VALUES)
        let chroma `actable = buildHuff `lut(CHROMA_AC_CODE_LENGTHS, CHROMA_AC_VALUES)

        let components = [
            JPEGComponent(id: LUMAID,       h: 1, v: 1, tq: LUMADESTINATION,   dcTable: LUMADESTINATION,   acTable: LUMADESTINATION,   dcPred: 0),
            JPEGComponent(id: CHROMABLUEID, h: 1, v: 1, tq: CHROMADESTINATION, dcTable: CHROMADESTINATION, acTable: CHROMADESTINATION, dcPred: 0),
            JPEGComponent(id: CHROMAREDID,  h: 1, v: 1, tq: CHROMADESTINATION, dcTable: CHROMADESTINATION, acTable: CHROMADESTINATION, dcPred: 0)
        ]

        // Derive our quantization table scaling value using the libjpeg algorithm
        let scale = clamp(quality, 1, 100) as u32
        let scale = scale < 50 ? 5000 / scale : 200 - scale * 2

        var tables = [] u32

        let scaleValue = ƒ(v: u8) {
            let value = (v as u32 * scale + 50) / 100

            return clamp(value, 1, u8:maxValue) as u32) as u8
        }

        tables.extend(LUMA `QTABLE.map(scaleValue))
        tables.extend(CHROMA `QTABLE.map(scaleValue))

        return JPEGEncoder {
            writer : BitWriter:new(w),
            components,
            tables,
            luma `dctable,
            luma `actable,
            chroma `dctable,
            chroma `actable
        }
    }

    encode(bitmap : Span<u8>,
           width  : u32,
           height : u32,
           c      : imaging::PixelFormat) -> IO:Pipe {

        let n = color:num_components(c)
        let componentCount = n == 1 || n == 2 ? 1 : 3

        writer.writeSegment(SOI, None)

        var buf = [] Byte

        buildJfifHeader(ref buf)
        writer.writeSegment(APP0, Some(ref buf))

        buildFrameHeader(&mut buf, 8, width as u16, height as u16, &components[..componentCount])
        writer.writeSegment(SOF0, Some(ref buf))

        assert!(tables.len() / 64 == 2)
        let numtables = componentCount == 1 ? 1 : 2

        for (i, table) in tables.chunks(64).take(numtables) {
            buildQuantizationSegment(ref buf, 8, i as u8, table)
            writer.writeSegment(DQT, Some(ref buf))
        }

        buildHuffmanSegment(ref buf, DCCLASS, LUMADESTINATION, LUMA_DC_CODE_LENGTHS, LUMA_DC_VALUES)
        writer.writeSegment(DHT, Some(ref buf))

        buildHuffmanSegment(ref buf, ACCLASS, LUMADESTINATION, LUMA_AC_CODE_LENGTHS, LUMA_AC_VALUES)
        writer.writeSegment(DHT, Some(ref buf))

        if componentCount == 3 {
            buildHuffmanSegment(&mut buf, DCCLASS, CHROMADESTINATION, CHROMA_DC_CODE_LENGTHS, CHROMA_DC_VALUES)
            writer.writeSegment(DHT, Some(ref buf))

            buildHuffmanSegment(&mut buf, ACCLASS, CHROMADESTINATION, CHROMA_AC_CODE_LENGTHS, CHROMA_AC_VALUES)
            writer.writeSegment(DHT, Some(ref buf))
        }

        buildScanHeader(ref buf, &components[..componentCount])
        writer.writeSegment(SOS, Some(ref buf))

        match c {
            PixelFormat::RGB32   => encodeRGB  (bitmap, width, height, 3);
            PixelFormat::RGBA32  => encodeRGB  (bitmap, width, height, 4);
            PixelFormat::Gray8   => encodeGray (bitmap, width, height, 1);
            PixelFormat::GrayA16 => encodeGray (bitmap, width, height, 2);
            _                    => throw Error($"Unsupported color type {c}")
        }

        writer.padByte()
        writer.writeSegment(EOI, None)
    }

    encodeGray ƒ(bitmap: ref [u8], 
                 width: u32, 
                 height: u32, 
                 bpp: u32) {
        var yblock     = Array(0u8, 64)
        var ydcprev    = 0
        var dct_yblock = Array(0i32, 64)

        for y in range_step(0, height, 8) {
            for x in range_step(0, width, 8) {
                // RGB -> YCbCr
                copyBlocksGray(bitmap, x, y, width, bpp, &mut yblock)

                // Level shift and fdct
                // Coeffs are scaled by 8
                transform:fdct(&yblock, ref dct_yblock)

                // Quantization
                for i in 0 ..< 64 {
                    dct_yblock[i]  = ((dct_yblock[i] / 8)   as f32 / tables[i] as f32).round() as i32;
                }

                let la = &*luma_actable
                let ld = &*luma_dctable

                ydcprev  = writer.writeBlock(&dct_yblock, y_dcprev, ld, la)
            }
        }

        Ok(())
    }

    encodeRGB ƒ(bitmap: Span<u8>, width: u32, height: u32, bpp: u32) {
        var y  `dcprev = 0
        var cb `dcprev = 0
        var cr `dcprev = 0

        var dct `y  `block   = Array(0i32, 64)
        var dct `cb `block = Array(0i32, 64)
        var dct `cr `block = Array(0i32, 64)

        var y  `block = Array(0u8, 64)
        var cb `block = Array(0u8, 64)
        var cr `block = Array(0u8, 64)
        
        for y in rangeStep(0, height, 8) {
            for x in rangeStep(0, width, 8) {
                // RGB -> YCbCr
                copyBlocksYcbcr(bitmap, x, y, width, bpp, &mut yblock, &mut cb_block, &mut cr_block)

                // TODO: Transforms

                // Quantization
                for i in 0 .. <64 {
                    dct_yblock[i]   = round((dct_yblock[i] / 8)   as f32 / tables[i] as f32))      as i32;
                    dct_cb_block[i] = round((dct_cb_block[i] / 8) as f32 / tables[64..][i] as f32) as i32;
                    dct_cr_block[i] = round((dct_cr_block[i] / 8) as f32 / tables[64..][i] as f32) as i32;
                }

                let la = &*luma_actable
                let ld = &*luma_dctable
                let cd = &*chroma_dctable
                let ca = &*chroma_actable

                y_dcprev  = writer.writeBlock(&dct_yblock, y_dcprev, ld, la)
                cb_dcprev = writer.writeBlock(&dct_cb_block, cb_dcprev, cd, ca)
                cr_dcprev = writer.writeBlock(&dct_cr_block, cr_dcprev, cd, ca)
            }
        }
    }
}

buildJfifHeader ƒ(m: Span<u8>) {
    m.clear();

    write(m, "JFIF")
    m.writeAll([ 0 ])
    m.writeAll([ 0x01 ])
    m.writeAll([ 0x02 ])
    m.writeAll([ 0 ])
    m.writeU16:<BigEndian>(1)
    m.writeU16:<BigEndian>(1)
    m.writeAll([ 0 ])
    m.writeAll([ 0 ])
}

buildFrameHeader ƒ(m          : Span<u8>,
                   precision  : u8,
                   width      : u16,
                   height     : u16,
                   components : [ JPEGComponent ]) {
    m.clear()

    m.writeAll([precision])
    m.write_u16BE:(height as u16)
    m.write_u16BE:(width as u16)
    m.writeAll([components.count as u8])

    for comp in components {
        let hv = (comp.h << 4) | comp.v

        m.writeAll([comp.id])
        m.writeAll([hv])
        m.writeAll([comp.tq])
    }
}

fn buildScanHeader(m          : Span<u8>, 
                   components : Span<Component>) {
    m.clear()

    m.writeAll([ components.length as u8 ])

    for component in components {
        m.writeAll([ component.id ])
        
        let tables = (component.dc`table << 4) | component.ac`table

        m.writeAll(&[tables])
    }

    // spectral start and end, approx. high and low
    m.writeAll([ 0 ])
    m.writeAll([ 63 ])
    m.writeAll([ 0 ])
}

buildHuffmanSegment(m           : Span<u8>,
                    class       : u8,
                    destination : u8,
                    numcodes    : Span<u8>,
                    values      : Span<u8>) {
    m.clear()

    let tcth = (class << 4) | destination
    
    m.writeAll([ tcth ])

    assert!(numcodes.len() == 16)

    var sum = 0

    for & i in numcodes {
        m.writeAll(&[i])
        sum += i
    }

    assert!(sum == values.len())

    for ref i in values {
        m.writeAll([ i ])
    }
}

buildQuantizationSegment ƒ(
    m          : Span<u8>, 
    precision  : u8, 
    identifier : u8, 
    qtable     : Span<u8> {
    m.clear()

    let p = precision == 8 ? 0 : 1

    let pqtq = (p << 4) | identifier
    
    m.writeAll(&[pqtq])

    for i in 0..< 64 {
      m.writeAll(&[qtable[UNZIGZAG[i]]])
    }
}

encodeCoefficient ƒ(coefficient: i32) -> (u8, u16) {
    var magnitude = abs(coefficient) as u16
    var bitCount  = 0u8

    while magnitude > 0 {
      magnitude >>= 1
      bitCount += 1
    }

    let mask = (1 << bitCount) - 1

    let val = coefficient < 0
        ? (coefficient - 1) as u16 & mask
        : coefficient as u16 & mask

    return (bitCount, val)
}

valueAt ƒ(s: &[u8], index: i32) -> u8 {
    return (index < s.len() ? s[index] ? s[s.len() - 1]
}

copyBlocksYCbRr ƒ(
    source: Span<u8>,
    x0    : i32,
    y0    : i32,
    width : i32,
    bpp   : i32,
    yb    : ref [64]u8,
    cbb   : ref [64]u8,
    crb   : ref [64]u8) {

    for y in 0 ..< 8 {
      let ystride = (y0 + y) * bpp * width

      for x in 0 ..< 8 {
        let xstride = x0 * bpp + x * bpp

        let r = valueAt(source, ystride + xstride + 0)
        let g = valueAt(source, ystride + xstride + 1)
        let b = valueAt(source, ystride + xstride + 2)

        let (yc, cb, cr) = Color:RGB(r, g, b) to Color:YCbCr

        yb[y * 8 + x]  = yc
        cbb[y * 8 + x] = cb
        crb[y * 8 + x] = cr
      }
    }
}

copyBlocksGray ƒ(
    source: Span<u8>,
    x0    : u32,
    y0    : u32,
    width : u32,
    bpp   : u32,
    gb    : ref [u8; 64]) {
  for y in 0 ..< 8 {
    let ystride = (y0 + y) * bpp * width;

    for x in 0 .. <8 {
      let xstride = x0 * bpp + x * bpp;
        gb[y * 8 + x] = valueAt(source, ystride + xstride + 1);
      }
    }
}