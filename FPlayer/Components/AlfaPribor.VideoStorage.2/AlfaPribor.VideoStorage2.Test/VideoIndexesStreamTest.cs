using AlfaPribor.VideoStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using AlfaPribor.Streams;

namespace AlfaPribor.VideoStorage.Test
{
    /// <summary>
    ///Это класс теста для VideoIndexesStreamTest, в котором должны
    ///находиться все модульные тесты VideoIndexesStreamTest
    ///</summary>
    [TestClass()]
    public class VideoIndexesStreamTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Генератор значения для идентификатора видеопотока
        /// </summary>
        private static int StreamIdGenerator = 0;

        /// <summary>
        /// Генератор значений для метки времени видеокадра
        /// </summary>
        private static int TimeStampGenerator = 10;

        /// <summary>
        /// Генератор значений для смещений видеокадров в файле с видеоданными
        /// </summary>
        private static long FileOffsetGenerator = 100;

        /// <summary>
        /// Создает файл для хранения видеоиндексов
        /// </summary>
        /// <returns>Поток для записи/чтения видеоиндексов</returns>
        private Stream CreateStream()
        {
            Stream result = null;
            try
            {
                string path = TestContext.TestDir + "\\VideoIndexesStream";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                result = new FileStream(path + "\\test.index", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch { }
            return result;
        }

        /// <summary>
        /// Создает файл для хранения видеоиндексов
        /// </summary>
        /// <returns>Поток для записи/чтения видеоиндексов</returns>
        private Stream CreateStream(string file_name)
        {
            Stream result = null;
            try
            {
                string path = TestContext.TestDir + "\\VideoIndexesStream";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                result = new FileStream(path + "\\" + file_name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch { }
            return result;
        }

        /// <summary>
        /// Создает и инициализирует массив видеоиндексов
        /// </summary>
        /// <param name="count">Количество элементов в массиве</param>
        /// <returns>Ссылка на массив видеоиндексов</returns>
        private SingleStreamFrameIndex[] GenerateIndexes(int count)
        {
            SingleStreamFrameIndex[] array = new SingleStreamFrameIndex[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = GenerateIndex();
            }
            return array;
        }

        /// <summary>
        /// Создает и инициализирует видеоиндекс
        /// </summary>
        /// <returns>Ссылка на объект с данными видеоиндекса</returns>
        private SingleStreamFrameIndex GenerateIndex()
        {
            StreamIdGenerator++;
            TimeStampGenerator += 10;
            FileOffsetGenerator += 100;
            return new SingleStreamFrameIndex(StreamIdGenerator, TimeStampGenerator, FileOffsetGenerator);
        }

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
        //ClassInitialize используется для выполнения кода до запуска первого теста в классе
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


        /// <summary>
        ///Тест для Position
        ///</summary>
        [TestMethod()]
        public void PositionTest()
        {
            using (Stream stream = CreateStream())
            {
                VideoIndexesStream target = new VideoIndexesStream(stream);
                SingleStreamFrameIndex[] indexes = new SingleStreamFrameIndex[3];
                int bytes = 0;
                indexes[0] = new SingleStreamFrameIndex(1, 1, 1);
                indexes[1] = new SingleStreamFrameIndex(2, 2, 2);
                indexes[2] = new SingleStreamFrameIndex(3, 3, 3);

                long expected = 3;
                long actual = target.Write(indexes, 0, indexes.Length, ref bytes);
                Assert.AreEqual(expected, actual);
                Assert.IsTrue(bytes > 0, "Число записанных в поток байт должно быть больше нуля!");

                expected = 1;
                target.Position = expected;
                actual = target.Position;
                Assert.AreEqual(expected, actual);

                SingleStreamFrameIndex test_index;
                target.ReadIndex(out test_index);
                if (!indexes[1].Equals(test_index))
                {
                    Assert.Fail("Записанный объект не равен прочитанному!");
                }

                expected = 0;
                target.Position = expected;
                actual = target.Position;
                Assert.AreEqual(expected, actual);

                target.ReadIndex(out test_index);
                if (!indexes[0].Equals(test_index))
                {
                    Assert.Fail("Записанный объект не равен прочитанному!");
                }

                expected = 2;
                target.Position = expected;
                actual = target.Position;
                Assert.AreEqual(expected, actual);

                target.ReadIndex(out test_index);
                if (!indexes[2].Equals(test_index))
                {
                    Assert.Fail("Записанный объект не равен прочитанному!");
                }

                expected = 3;
                target.Position = expected;
                actual = target.Position;
                Assert.AreEqual(expected, actual);

                expected = 0;
                target.Position = -1;
                actual = target.Position;
                Assert.AreEqual(expected, actual);

                expected = 4;
                target.Position = 4;
                actual = target.Position;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для Length
        ///</summary>
        [TestMethod()]
        public void LengthTest()
        {
            WriteTest();
            VideoIndexesStream target = new VideoIndexesStream(CreateStream());
            long expected = 10;
            long actual;
            actual = target.Length;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для WriteIndex
        ///</summary>
        [TestMethod()]
        public void WriteIndexTest()
        {
            using (VideoIndexesStream target = new VideoIndexesStream(CreateStream("test.single_index.index")))
            {
                SingleStreamFrameIndex index = null;
                try
                {
                    target.WriteIndex(index);
                    Assert.Fail("Не допускается принимать пустую ссылку на индекс! Должно генерироваться исключение System.ArgumentNullException.");
                }
                catch { }
                index = GenerateIndex();
                target.WriteIndex(index);

                Assert.IsTrue(target.Modified, "Признак изменения файла должен быть выставлен (Modified = TRUE)");
            }
        }

        /// <summary>
        ///Тест для Write
        ///</summary>
        [Priority(0), TestMethod()]
        public void WriteTest()
        {
            using (VideoIndexesStream target = new VideoIndexesStream(CreateStream()))
            {
                SingleStreamFrameIndex[] indexes = null;
                int offset = 0;
                int count = 0;
                int bytes = 0;
                try
                {
                    target.Write(indexes, offset, count, ref bytes);
                    Assert.Fail("Недопустимо принимать пустую ссылку на массив индексов! Должно генерироваться исключение System.ArgumentNullException.");
                }
                catch (ArgumentNullException) { }
                indexes = GenerateIndexes(10);
                offset = -1;
                count = 10;
                try
                {
                    target.Write(indexes, offset, count, ref bytes);
                    Assert.Fail("Недопустимое значение параметра offset! Должно генерироваться исключение System.ArgumentOutOfRangeException.");
                }
                catch (ArgumentOutOfRangeException) { }
                offset = 5;
                count = 10;
                try
                {
                    target.Write(indexes, offset, count, ref bytes);
                    Assert.Fail("Недопустимое сочетание параметров offset и count! Должно генерироваться исключение System.ArgumentException.");
                }
                catch (ArgumentException) { }
                offset = 0;
                count = 10;
                long expected = 10;
                long actual = target.Write(indexes, offset, count, ref bytes);

                Assert.AreEqual(expected, actual);
                Assert.IsTrue(bytes > 0, "Число записанных в поток байт должно быть больше нуля!");
                Assert.IsTrue(target.Modified, "Признак изменения потока должен быть выставлен (Modified = TRUE)");
            }
        }

        /// <summary>
        ///Тест для SetLength
        ///</summary>
        [TestMethod()]
        public void SetLengthTest()
        {
            using (Stream stream = CreateStream("test.empty_10_indexes.dat"))
            {
                VideoIndexesStream target = new VideoIndexesStream(stream);
                long value = 10;
                target.SetLength(value);
            }
        }

        /// <summary>
        ///Тест для Seek
        ///</summary>
        [TestMethod()]
        public void SeekTest()
        {
            using (Stream stream = CreateStream("test.seek.index"))
            {

                VideoIndexesStream target = new VideoIndexesStream(stream);
                SingleStreamFrameIndex[] indexes = new SingleStreamFrameIndex[3];
                indexes[0] = new SingleStreamFrameIndex(1, 1, 1);
                indexes[1] = new SingleStreamFrameIndex(2, 2, 2);
                indexes[2] = new SingleStreamFrameIndex(3, 3, 3);
                int bytes = 0;
                target.Write(indexes, 0, indexes.Length, ref bytes);

                long offset = 2;
                SeekOrigin origin = SeekOrigin.Begin;
                long expected = 2;
                long actual;
                SingleStreamFrameIndex test_index;

                actual = target.Seek(offset, origin);
                Assert.AreEqual(expected, actual);

                target.ReadIndex(out test_index);
                if (!indexes[2].Equals(test_index))
                {
                    Assert.Fail("Прочитанные данные не идентичны записаным!");
                }

                expected = 0;
                offset = -3;
                origin = SeekOrigin.End;
                actual = target.Seek(offset, origin);
                Assert.AreEqual(expected, actual);

                target.ReadIndex(out test_index);
                if (!indexes[0].Equals(test_index))
                {
                    Assert.Fail("Прочитанные данные не идентичны записаным!");
                }

                expected = 2;
                offset = 1;
                origin = SeekOrigin.Current;
                actual = target.Seek(offset, origin);
                Assert.AreEqual(expected, actual);

                target.ReadIndex(out test_index);
                if (!indexes[2].Equals(test_index))
                {
                    Assert.Fail("Прочитанные данные не идентичны записаным!");
                }

                expected = 1;
                offset = -2;
                origin = SeekOrigin.Current;
                actual = target.Seek(offset, origin);
                Assert.AreEqual(expected, actual);
                target.ReadIndex(out test_index);

                if (!indexes[1].Equals(test_index))
                {
                    Assert.Fail("Прочитанные данные не идентичны записаным!");
                }

                expected = 5;
                offset = 5;
                origin = SeekOrigin.Begin;
                actual = target.Seek(offset, origin);
                Assert.AreEqual(expected, actual);

                offset = -10;
                origin = SeekOrigin.End;
                try
                {
                    actual = target.Seek(offset, origin);
                    Assert.Fail("Невозможно переместить указатель на данные перед началом данных! Должно генерироваться исключение IOException.");
                }
                catch { }

                expected = 1;
                offset = 1;
                origin = SeekOrigin.Begin;
                actual = target.Seek(offset, origin);
                Assert.AreEqual(expected, actual);

                expected = 5;
                offset = 4;
                origin = SeekOrigin.Current;
                actual = target.Seek(offset, origin);
                Assert.AreEqual(expected, actual);

                expected = 1;
                offset = 1;
                origin = SeekOrigin.Begin;
                actual = target.Seek(offset, origin);
                Assert.AreEqual(expected, actual);

                offset = -4;
                origin = SeekOrigin.Current;
                try
                {
                    actual = target.Seek(offset, origin);
                    Assert.Fail("Невозможно переместить указатель на данные перед началом данных! Должно генерироваться исключение IOException.");
                }
                catch { }
            }
        }

        /// <summary>
        ///Тест для ReadIndex
        ///</summary>
        [TestMethod()]
        public void ReadIndexTest()
        {
            WriteIndexTest();

            VideoIndexesStream target = new VideoIndexesStream(CreateStream("test.single_index.index"));
            SingleStreamFrameIndex index;
            int actual = target.ReadIndex(out index);
            Assert.IsTrue(actual > 0, "Должно возвращаться положительное число записанных в поток байтов!");
        }

        /// <summary>
        ///Тест для Read
        ///</summary>
        [Priority(0), TestMethod()]
        public void ReadTest()
        {
            WriteTest();

            using (VideoIndexesStream target = new VideoIndexesStream(CreateStream()))
            {
                SingleStreamFrameIndex[] indexes = null;
                int offset = 0;
                int count = 0;
                int expected = 10;
                int actual;
                int bytes = 0;
                try
                {
                    actual = target.Read(indexes, offset, count, ref bytes);
                    Assert.Fail("Отсутствие ссылки на массивов индексов недопустимо! Должно генерироваться исключение System.ArgumentNullException.");
                }
                catch (ArgumentNullException) { }
                indexes = new SingleStreamFrameIndex[10];
                offset = -3;
                count = 0;
                try
                {
                    actual = target.Read(indexes, offset, count, ref bytes);
                    Assert.Fail("Недопустимое значение параметра offset! Должно генерироваться исключение System.ArgumentAutOfRangeException.");
                }
                catch (ArgumentOutOfRangeException) { }
                offset = 3;
                count = 13;
                try
                {
                    actual = target.Read(indexes, offset, count, ref bytes);
                    Assert.Fail("Недопустимое сочетание значений параметров offset и count! Должно генерироваться исключение System.ArgumentException.");
                }
                catch (ArgumentException) { }
                offset = 0;
                count = indexes.Length;
                actual = target.Read(indexes, offset, count, ref bytes);
                Assert.AreEqual(expected, actual);
                Assert.IsTrue(bytes > 0, "Число записанных в поток байт должно быть больше нуля!"); 
            }
        }

        /// <summary>
        ///Тест для DoWriteIndex
        ///</summary>
        [TestMethod()]
        public void DoWriteIndexTest()
        {
            using (VideoIndexesStream stream = new VideoIndexesStream(CreateStream("test.index.dat")))
            {
                PrivateObject param0 = new PrivateObject(stream);
                VideoIndexesStream_Accessor target = new VideoIndexesStream_Accessor(param0);
                SingleStreamFrameIndex index = null;
                try
                {
                    target.DoWriteIndex(index);
                    Assert.Fail("Отсутствие ссылки на видеоиндекс недопустимо! Должно генерироваться исключение System.ArgumentNullException.");
                }
                catch { }
                index = GenerateIndex();
                target.DoWriteIndex(index);
            }
        }

        /// <summary>
        ///Тест для DoWriteHeader
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DoWriteHeaderTest()
        {
            using (VideoIndexesStream stream = new VideoIndexesStream(CreateStream("test.header.dat")))
            {
                PrivateObject param0 = new PrivateObject(stream);
                VideoIndexesStream_Accessor target = new VideoIndexesStream_Accessor(param0);
                bool expected = true;
                bool actual;
                actual = target.DoWriteHeader();
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для DoReadIndex
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DoReadIndexTest()
        {
            DoWriteIndexTest();

            VideoIndexesStream stream = new VideoIndexesStream(CreateStream("test.index.dat"));
            PrivateObject param0 = new PrivateObject(stream);
            VideoIndexesStream_Accessor target = new VideoIndexesStream_Accessor(param0);
            SingleStreamFrameIndex index;
            int actual = target.DoReadIndex(out index);
            Assert.IsTrue(actual > 0, "Должно возвращатся положительное число записанных в поток байтов.");
        }

        /// <summary>
        ///Тест для DoReadHeader
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DoReadHeaderTest()
        {
            DoWriteHeaderTest();

            VideoIndexesStream stream = new VideoIndexesStream(CreateStream("test.header.dat"));
            PrivateObject param0 = new PrivateObject(stream);
            VideoIndexesStream_Accessor target = new VideoIndexesStream_Accessor(param0);
            bool expected = true;
            bool actual;
            actual = target.DoReadHeader();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для Dispose
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DisposeTest()
        {
            VideoIndexesStream stream = new VideoIndexesStream(CreateStream("test.disposing.index"));
            PrivateObject param0 = new PrivateObject(stream);
            VideoIndexesStream_Accessor target = new VideoIndexesStream_Accessor(param0);

            // Освобождаем неуправляемые ресурсы
            bool disposing = false;
            target.Dispose(disposing);
            try
            {
                long l = stream.Length;
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                long l = stream.Position;
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                bool value = stream.Modified;
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                int i = stream.Version;
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                byte[] b = stream.Signature;
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                byte[] buffer = new byte[16];
                int offset = 0;
                int count = 16;
                stream.Read(buffer, offset, count);
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                byte[] buffer = new byte[16];
                int offset = 0;
                int count = 16;
                stream.Write(buffer, offset, count);
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                long offset = 1;
                stream.Seek(offset, SeekOrigin.Begin);
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                long len = 1;
                stream.SetLength(len);
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                SingleStreamFrameIndex index;
                stream.ReadIndex(out index);
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                SingleStreamFrameIndex index = new SingleStreamFrameIndex(0);
                stream.WriteIndex(index);
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                SingleStreamFrameIndex[] indexes = new SingleStreamFrameIndex[3];
                int bytes = 0;
                stream.Read(indexes, 0, indexes.Length, ref bytes);
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                SingleStreamFrameIndex[] indexes = GenerateIndexes(3);
                int bytes = 0;
                stream.Write(indexes, 0, indexes.Length, ref bytes);
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
            try
            {
                stream.CheckHeader();
                Assert.Fail("Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch { }
        }

        /// <summary>
        ///Тест для CheckHeader
        ///</summary>
        [TestMethod()]
        public void CheckHeaderTest()
        {
            DoWriteHeaderTest();
            VideoIndexesStream target = new VideoIndexesStream(CreateStream("test.header.dat"));
            StreamHeaderState expected = StreamHeaderState.Ok;
            StreamHeaderState actual;
            actual = target.CheckHeader();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для конструктора VideoIndexesStream
        ///</summary>
        [Priority(0), TestMethod()]
        public void VideoIndexesStreamConstructorTest()
        {
            Stream stream = null;
            bool expected = false;
            bool actual;
            try
            {
                VideoIndexesStream target = new VideoIndexesStream(stream);
                actual = true;
            }
            catch
            {
                actual = false;
            }
            Assert.AreEqual(expected, actual,"Значение входного параметра stream = null недопустимо!");
            stream = new FileStream("test.index", FileMode.OpenOrCreate);
            expected = true;
            try
            {
                VideoIndexesStream target = new VideoIndexesStream(stream);
                actual = true;
            }
            catch
            {
                actual = false;
            }
            Assert.AreEqual(expected, actual);
        }
    }
}
