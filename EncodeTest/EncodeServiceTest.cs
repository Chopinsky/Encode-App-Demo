using EncodeDemo;
using EncodeTestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        [TestMethod()]
        public void NullProviderEncodeTest()
        {
            string encoded = EncodeService.Encode(null, null);
            Assert.IsNull(encoded);

            encoded = EncodeService.Encode("content", null);
            Assert.AreEqual("content", encoded);

            encoded = EncodeService.Encode("   ", null);
            Assert.AreEqual("   ", encoded);
        }

        [TestMethod()]
        public void ProviderNullOrEmptyInputEncodeTest()
        {
            IEncodeProvider provider = new MockEncodeProvider();
            string encoded = EncodeService.Encode(null, provider);
            Assert.IsNull(encoded);

            encoded = EncodeService.Encode("", provider);
            Assert.AreEqual("", encoded);

            encoded = EncodeService.Encode("   ", provider);
            Assert.AreEqual("   ", encoded);

            encoded = EncodeService.Encode("\t  \r\n   ", provider);
            Assert.AreEqual("\t  \r\n   ", encoded);
        }

        [TestMethod()]
        public void ProviderNonNullInputEncodeTest()
        {
            IEncodeProvider provider = new MockEncodeProvider();
            string encoded = EncodeService.Encode("abc", provider);
            Assert.AreEqual("ABC", encoded);

            encoded = EncodeService.Encode("a123bc", provider);
            Assert.AreEqual("A123BC", encoded);
        }

        [TestMethod()]
        public void ProviderMixedInputEncodeTest()
        {
            IEncodeProvider provider = new MockEncodeProvider();
            string encoded = EncodeService.Encode("a測試bc", provider);
            Assert.AreEqual("A測試BC", encoded);

            encoded = EncodeService.Encode("a,b\r\n   c", provider);
            Assert.AreEqual("A,B\r\n   C", encoded);

            encoded = EncodeService.Encode("abcXYZ", provider);
            Assert.AreEqual("ABCxyz", encoded);
        }
    }
}
