An exploration of Mecrisp-Stellaris Forth 2.2.1 on HyTiny STM32F103.

This code runs on the "RF Node Watcher" hardware described on the [weblog][R].
It includes an ARM-based [Hy-TinySTM103T][H], an RFM69 connected over SPI, and
a small 128x64 pixel OLED display connected over I2C. It should also work on
other STM32F103 boards, see the "h" file for pin definitions.

## Installation

The board must have Mecrisp-Stellaris Forth pre-loaded, see [SourceForge][M].
This 16 KB firmware provides a nice runtime environment, everything else here
has been implemented on top in pure Forth.

There are three small "wrapper" files in this area, which include several more
substantial source files from `../flib/`:

* "h" will install a hardware abstraction layer into flash memory
* "l" installs a set of library packages on top of "h", also in flash memory
* "d" is development and testing code which requires "h" and "l" to be in flash

(with Picoterm, "^A ^S d" is very conveniently placed on the home row of keys)

Loading "h" will erase flash (everything but Mecrisp) before re-installing
itself. Similarly, loading "l" leaves Mecrisp and the "h" code intact, but
erases everything else before re-installing "l" (it then ends by loading "d").
Loading "d" is a RAM-only affair, allowing fast and frequent edit-run cycles.

In day-to-day use, "h" and "l" get loaded once and then remain on the chip,
ready for use after power-up. This creates a fairly elaborate context for
development, including bit-banged SPI and I2C drivers, as well as the OLED
and RF69 drivers. There is a complete graphics library, able to draw lines,
circles, and text, as well as a logo bitmap and an ASCII 8x8 text font.

Flash memory use is well under 32 KB for all of the above plus Mecrisp Forth.

New code and definitions can be typed in interactively, or added to "d" and
reloaded. Once the code is stable enough, it can then be moved to a new source
file and included in "l" to make it more permanent.

Next are a few examples, using definitions from "h" and "l".

_Show current GPIO settings:_

    io.all 
    PIN 0  PORT A  CRL 14114414  CRH 000004B0  IDR 00000459  ODR 0000A010 
    PIN 0  PORT B  CRL 44484444  CRH 44444444  IDR 000000DA  ODR 00000010 
    PIN 0  PORT C  CRL 44444444  CRH 44444444  IDR 00000000  ODR 00000000 
    PIN 0  PORT D  CRL 44444444  CRH 44444444  IDR 00000000  ODR 00000000 
    PIN 0  PORT E  CRL 44444444  CRH 44444444  IDR 00000000  ODR 00000000  ok.

_Show GPIO info related to a specific pin:_

    led io. 
    PIN 1  PORT A  CRL 14114414  CRH 000004B0  IDR 00000459  ODR 0000A010  ok.

_Scan the I2C bus for attached devices:_

    init-i2c i2c. 
    00: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
    10: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
    20: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
    30: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
    40: -- -- -- -- -- -- -- -- 48 -- -- -- -- -- -- --
    50: -- -- -- 53 -- -- -- -- -- -- -- -- -- -- -- --
    60: -- -- -- -- -- -- -- -- 68 -- -- -- -- -- -- --
    70: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- ok.

_Receive some RF69 test packets (stopped by keypress):_

    rfdemo 
    OK 128 24 64 
    OK 128 24 66 1 2 
    OK 128 24 67 1 2 3 
    OK 128 24 68 1 2 3 4 
    OK 128 24 69 1 2 3 4 5 
    ok.

_Receive some RF69 test packets, in HEX format (stopped by keypress):_

    rfdemox 
    OKX 801846010203040506
    OKX 80184701020304050607
    OKX 8018480102030405060708
    OKX 801849010203040506070809
    ok.

_Dump the RF69's internal register settings:_

    rf. 
        0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F 
    00: -- 10 00 02 8A 02 E1 D9 26 40 41 60 02 92 F5 20
    10: 24 9F 09 1A 40 B0 7B 9B 18 4A 42 40 80 06 5C 00
    20: 00 FF F3 00 83 00 07 D9 46 A0 00 00 00 05 88 2D
    30: 2A 00 00 00 00 00 00 D0 42 00 00 00 8F 12 00 00
    40: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00
    50: 14 C5 88 08 00 00 01 00 1B 09 55 80 70 33 CA 08 ok.

That's it for now. Enjoy!

  [R]: http://jeelabs.org/book/1545f/
  [H]: http://www.hotmcu.com/stm32f103tb-arm-cortex-m3-development-board-p-222.html
  [M]: http://mecrisp.sourceforge.net/