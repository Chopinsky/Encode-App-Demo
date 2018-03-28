using EncodeDemo;
using EncodeTestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EncodeServiceTest
{
    [TestClass()]
    public class IsAlphanumericTest
    {
        [TestMethod()]
        public void EnglishCharIsAlphanumericTest()
        {
            Assert.IsTrue(EncodeService.IsAlphanumeric('a'));
        }

        [TestMethod()]
        public void CapitalCharIsAlphanumericTest()
        {
            Assert.IsTrue(EncodeService.IsAlphanumeric('A'));
        }

        [TestMethod()]
        public void NumIsAlphanumericTest()
        {
            Assert.IsTrue(EncodeService.IsAlphanumeric('1'));
        }

        [TestMethod()]
        public void ControlCharIsNotAlphanumericTest()
        {
            var ch = (char)0x0008;  // backspace
            Assert.IsFalse(EncodeService.IsAlphanumeric(ch));

            ch = (char)0x01B;       // escape
            Assert.IsFalse(EncodeService.IsAlphanumeric(ch));
        }

        [TestMethod()]
        public void PunctIsNotAlphanumericTest()
        {
            Assert.IsFalse(EncodeService.IsAlphanumeric('~'));
        }

        [TestMethod()]
        public void SpaceIsNotAlphanumericTest()
        {
            Assert.IsFalse(EncodeService.IsAlphanumeric(' '));
        }

        [TestMethod()]
        public void MathSymbIsNotAlphanumericTest()
        {
            var ch = (char)0x0E4;       // sigma
            Assert.IsFalse(EncodeService.IsAlphanumeric(ch));
        }

        [TestMethod()]
        public void ChineseCharIsNotAlphanumericTest()
        {
            var ch = '我';              // Chinese character
            Assert.IsFalse(EncodeService.IsAlphanumeric(ch));
        }

        [TestMethod()]
        public void CryllicCharIsNotAlphanumericTest()
        {
            var ch = (char)0x0410;      // Cyrillic character 'A'
            Assert.IsFalse(EncodeService.IsAlphanumeric(ch));
        }
    }

    [TestClass()]
    public class GenShowMapTableTest
    {
        private const string mockTable = 
            "a=A,\tb=B,\tc=C,\td=D,\te=E,\tf=F,\tg=G,\th=H,\ti=I,\tj=J,\r\nk=K,\tl=L,\tm=M,\tn=N,\to=O,\tp=P,\tq=Q,\tr=R,\ts=S,\tt=T,\r\nu=U,\tv=V,\tw=W,\tx=X,\ty=Y,\tz=Z,\tA=a,\tB=b,\tC=c,\tD=d,\r\nE=e,\tF=f,\tG=g,\tH=h,\tI=i,\tJ=j,\tK=k,\tL=l,\tM=m,\tN=n,\r\nO=o,\tP=p,\tQ=q,\tR=r,\tS=s,\tT=t,\tU=u,\tV=v,\tW=w,\tX=x,\r\nY=y,\tZ=z.";

        [TestMethod()]
        public void GenCorrectShowMapTableTest()
        {
            IEncodeProvider provider = new MockEncodeProvider();
            string table = EncodeService.GenerateShowMapTable(provider);

            Assert.AreEqual(table, mockTable);
        }

        [TestMethod()]
        public void NullProviderGenShowMapTableTest()
        {
            IEncodeProvider provider = null;
            string table = EncodeService.GenerateShowMapTable(provider);

            Assert.AreEqual(table, ">> No encoding provider is set <<");
        }

        [TestMethod()]
        public void ProviderNullTableGenShowMapTableTest()
        {
            IEncodeProvider provider = new MockEncodeProvider(false);
            string table = EncodeService.GenerateShowMapTable(provider);

            Assert.AreEqual(table, ">> No encoding map is provided <<");
        }
    }

    [TestClass()]
    public class EncodTest
    {
        private IEncodeProvider provider = new MockEncodeProvider();

        [TestMethod()]
        public void NullProviderEncodeTest()
        {
            string encoded = EncodeService.Encode(null, "new", "old");
            Assert.AreEqual("new", encoded);

            encoded = EncodeService.Encode(null, "", "old");
            Assert.AreEqual("", encoded);

            encoded = EncodeService.Encode(null, "   ", "old");
            Assert.AreEqual("   ", encoded);
        }

        [TestMethod()]
        public void OutOfRangeEncodeTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                // add OOR
                string encoded = EncodeService.Encode(provider, "oldish", "old", 4, 3, 0);
            });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                // remove OOR
                string encoded = EncodeService.Encode(provider, "ol", "old", 4, 2, 3);
            });

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                // remove OOR
                string encoded = EncodeService.Encode(provider, "old", "old", 3, 0, 4);
            });
        }

        [TestMethod()]
        public void NullOrEmptyInputEncodeTest()
        {
            string encoded = EncodeService.Encode(provider, null, "old");
            Assert.IsNull(encoded);

            encoded = EncodeService.Encode(provider, "", "old");
            Assert.AreEqual("", encoded);

            encoded = EncodeService.Encode(provider, "  \t \r\n   ", "old");
            Assert.AreEqual("  \t \r\n   ", encoded);
        }

        [TestMethod()]
        public void BlankLastEncodeTest()
        {
            string encoded = EncodeService.Encode(provider, "n", "", 0, 1, 0);
            Assert.AreEqual("N", encoded);

            encoded = EncodeService.Encode(provider, "new", "");
            Assert.AreEqual("NEW", encoded);

            encoded = EncodeService.Encode(provider, "new", "", 4, 0, 2);  // out of range, but okay since we run full update
            Assert.AreEqual("NEW", encoded);
        }

        [TestMethod()]
        public void EncodeWholeInputTest()
        {
            string encoded = EncodeService.Encode(provider, "abc,123,XYZ");  // full update
            Assert.AreEqual("ABC,123,xyz", encoded);
        }

        [TestMethod()]
        public void OnlyAddEncodeTest()
        {
            string encoded = EncodeService.Encode(provider, "abc", "AB", 2, 1);
            Assert.AreEqual("ABC", encoded);

            encoded = EncodeService.Encode(provider, "abc", "AC", 1, 1);
            Assert.AreEqual("ABC", encoded);

            encoded = EncodeService.Encode(provider, "abc", "C", 0, 2);
            Assert.AreEqual("ABC", encoded);
            
            encoded = EncodeService.Encode(provider, "abc", "", 0, 3);
            Assert.AreEqual("ABC", encoded);
        }

        [TestMethod()]
        public void OnlyRemoveEncodeTest()
        {
            string encoded = EncodeService.Encode(provider, "abc", "ABCD", 3, 0, 1);
            Assert.AreEqual("ABC", encoded);

            encoded = EncodeService.Encode(provider, "ac", "ABC", 1, 0, 1);
            Assert.AreEqual("AC", encoded);

            encoded = EncodeService.Encode(provider, "c", "ABC", 0, 0, 2);
            Assert.AreEqual("C", encoded);

            encoded = EncodeService.Encode(provider, "", "ABC", 0, 0, 3);
            Assert.AreEqual("", encoded);
        }

        [TestMethod()]
        public void InPlaceUpdateEncodeTest()
        {
            string encoded = EncodeService.Encode(provider, "abc,123", "Whatever this was");  // full update if not giving specific positions to update
            Assert.AreEqual("ABC,123", encoded);

            encoded = EncodeService.Encode(provider, "a123bc", "Z123bc", 0, 1, 1);
            Assert.AreEqual("A123bc", encoded);

            encoded = EncodeService.Encode(provider, "abc123XYZ", "XYZ123bc", 0, 2, 1);
            Assert.AreEqual("ABYZ123bc", encoded);
        }

        [TestMethod()]
        public void MixedCharInputEncodeTest()
        {
            IEncodeProvider provider = new MockEncodeProvider();
            string encoded = EncodeService.Encode(provider, "a測試bc");
            Assert.AreEqual("A測試BC", encoded);

            encoded = EncodeService.Encode(provider, "a,b\r\n   c");
            Assert.AreEqual("A,B\r\n   C", encoded);

            encoded = EncodeService.Encode(provider, "abcXYZ");
            Assert.AreEqual("ABCxyz", encoded);
        }
    }
}
