using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using AlfaPribor.VideoStorage;

namespace AlfaPribor.VideoStorage.ASKO.Test
{
    /// <summary>
    ///Это класс теста для VideoIndexesStreamTest, в котором должны
    ///находиться все модульные тесты VideoIndexesStreamTest
    ///</summary>
    [TestClass()]
    public class AskoVideoIndexesStreamTest
    {
        private TestContext testContextInstance;
        private const string fileName = "test.index";
        private const string writeFileName = "write_test.index";
        private Random generator = new Random();

        /// <summary>
        ///Получает или устанавливает контекст теста, в котором предоставляются
        ///сведения о текущем тестовом запуске и обеспечивается его функциональность.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Дополнительные атрибуты теста
        // 
        //При написании тестов можно использовать следующие дополнительные атрибуты:
        //
        // ClassInitialize используется для выполнения кода до запуска первого теста в классе
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //ClassCleanup используется для выполнения кода после завершения работы всех тестов в классе
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //TestInitialize используется для выполнения кода перед запуском каждого теста
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //TestCleanup используется для выполнения кода после завершения каждого теста
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private SingleStreamFrameIndex CreateTestSingleStreamFrameIndex()
        {
            SingleStreamFrameIndex result = new SingleStreamFrameIndex(
                generator.Next(4),
                generator.Next(10000),
                generator.Next(Int32.MaxValue)
            );
            return result;
        }

        private SingleStreamFrameIndex[] CreateTestSingleStreamFrameIndexes(int count)
        {
            SingleStreamFrameIndex[] result = new SingleStreamFrameIndex[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = CreateTestSingleStreamFrameIndex();
            }
            return result;
        }

        /// <summary>
        ///Тест для Конструктор AskoVideoIndexesStream
        ///</summary>
        [TestMethod()]
        public void VideoIndexesStreamConstructorTest()
        {
            Stream stream = null;
            try
            {
                AskoVideoIndexesStream target = new AskoVideoIndexesStream(stream);
                Assert.Fail("Ожидается исключение ArgumentException!");
            }
            catch (ArgumentException) { }
            using (stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                using (AskoVideoIndexesStream target = new AskoVideoIndexesStream(stream)) { }
            }
        }

        /// <summary>
        ///Тест для WriteIndex
        ///</summary>
        [TestMethod()]
        public void WriteIndexTest()
        {
            using (Stream stream = new FileStream(writeFileName, FileMode.Create, FileAccess.Write))
            {
                AskoVideoIndexesStream target = new AskoVideoIndexesStream(stream);
                SingleStreamFrameIndex index = CreateTestSingleStreamFrameIndex();
                int actual;
                try
                {
                    actual = target.WriteIndex(null);
                    Assert.Fail("Ожидается исключение ArgumentException!");
                }
                catch (ArgumentException) { }
                actual = target.WriteIndex(index);
                Assert.IsTrue(actual > 0);
            }
        }

        /// <summary>
        ///Тест для Write
        ///</summary>
        [TestMethod()]
        public void WriteTest()
        {
            using (Stream stream = new FileStream(writeFileName, FileMode.Append, FileAccess.Write))
            {
                AskoVideoIndexesStream target = new AskoVideoIndexesStream(stream);
                int idx_count = 5;
                SingleStreamFrameIndex[] indexes = CreateTestSingleStreamFrameIndexes(idx_count);
                int offset = 0;
                int count = idx_count;
                int bytes_wrote = 0;
                int actual;
                try
                {
                    target.Write(null, idx_count, 1, ref bytes_wrote);
                    Assert.Fail("Ожидается исключение ArgumentNullException!");
                }
                catch (ArgumentNullException) { }
                try
                {
                    target.Write(indexes, 1, idx_count, ref bytes_wrote);
                    Assert.Fail("Ожидается исключение ArgumentException!");
                }
                catch (ArgumentException) { }
                actual = target.Write(indexes, offset, count, ref bytes_wrote);
                Assert.IsTrue(bytes_wrote > 0);
                Assert.AreEqual(count, actual);
            }
        }

        /// <summary>
        ///Тест для Read
        ///</summary>
        [TestMethod()]
        public void ReadTest()
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                AskoVideoIndexesStream target = new AskoVideoIndexesStream(stream);
                int idx_count = 426;
                SingleStreamFrameIndex[] indexes = new SingleStreamFrameIndex[idx_count];
                int offset = 0;
                int count = 426;
                int bytes_readed = 0;
                try
                {
                    target.Read(null, idx_count, 1, ref bytes_readed);
                    Assert.Fail("Ожидается исключение ArgumentNullException!");
                }
                catch (ArgumentNullException) { }
                try
                {
                    target.Write(indexes, 1, idx_count, ref bytes_readed);
                    Assert.Fail("Ожидается исключение ArgumentException!");
                }
                catch (ArgumentException) { }
                try
                {
                    target.Write(indexes, idx_count, 1, ref bytes_readed);
                    Assert.Fail("Ожидается исключение ArgumentException!");
                }
                catch (ArgumentException) { }

                int actual = target.Read(indexes, offset, count, ref bytes_readed);
                Assert.AreEqual(actual, idx_count);
                Assert.IsTrue(bytes_readed > 0);
            }
        }

        /// <summary>
        ///Тест для ReadIndex
        ///</summary>
        [TestMethod()]
        public void ReadIndexTest()
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                AskoVideoIndexesStream target = new AskoVideoIndexesStream(stream);
                SingleStreamFrameIndex index;
                SingleStreamFrameIndex expected = new SingleStreamFrameIndex(2, 25, 202);
                int actual = target.ReadIndex(out index);
                Assert.IsTrue(expected.Equals(index));
                Assert.IsTrue(actual > 0);
            }
        }
    }
}
