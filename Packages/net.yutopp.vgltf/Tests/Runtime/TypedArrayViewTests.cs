//
// Copyright (c) 2021- yutopp (yutopp@gmail.com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at  https://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace VGltf.UnitTests
{
    public sealed class TypedArrayViewTest
    {
        [Test]
        public void Vec2UIntNoStrideTest()
        {
            // 0   4   8   12  16
            // |---|---|---|---|
            // [ x | y I x | y ] : Vec2<uint32>
            //            
            var buffer = new byte[] {
                /* [0]x */ 0x00, 0x01, 0x02, 0x03, /* [0]y */ 0x04, 0x05, 0x06, 0x07,
                /* [1]x */ 0x10, 0x11, 0x12, 0x13, /* [1]y */ 0x14, 0x15, 0x16, 0x17,
            };

            var view = new TypedArrayView<Int32>(new ArraySegment<byte>(buffer), 8 /* stride */ , 4 /* uint32 = 4 */, 2 /* VEC2 = 2 */, 2);

            var compositedResult = view.GetCompositedEnumerable(xs => xs.ToArray()).ToArray();
            Assert.That(compositedResult.Length, Is.EqualTo(2));
            Assert.That(compositedResult[0], Is.EquivalentTo(new Int32[] { 0x03020100, 0x07060504 }));
            Assert.That(compositedResult[1], Is.EquivalentTo(new Int32[] { 0x13121110, 0x17161514 }));

            var result = view.GetCompositedEnumerable(xs => xs.ToArray()).SelectMany(x => x).ToArray();
            Assert.That(result.Length, Is.EqualTo(4));
            Assert.That(result[0], Is.EqualTo(0x03020100));
            Assert.That(result[1], Is.EqualTo(0x07060504));
            Assert.That(result[2], Is.EqualTo(0x13121110));
            Assert.That(result[3], Is.EqualTo(0x17161514));
        }
    }
}
