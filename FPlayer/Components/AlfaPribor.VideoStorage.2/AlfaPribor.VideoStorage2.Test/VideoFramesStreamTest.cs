using AlfaPribor.VideoStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System;
using AlfaPribor.Streams;

namespace AlfaPribor.VideoStorage.Test
{
    
    
    /// <summary>
    ///Это класс теста для VideoFramesStreamTest, в котором должны
    ///находиться все модульные тесты VideoFramesStreamTest
    ///</summary>
    [TestClass()]
    public class VideoFramesStreamTest
    {

        private TestContext testContextInstance;

        /// <summary>
        /// Генератор идентификоторов видеопотока
        /// </summary>
        private int StreamIdGenerator = 0;

        /// <summary>
        /// Генератор меток времени для видеокадров
        /// </summary>
        private int TimeStampGenerator = 100;


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
        /// Создает файл для хранения видеокадров
        /// </summary>
        /// <returns>Поток для записи/чтения видеокадров</returns>
        private Stream CreateStream()
        {
            Stream result = null;
            try
            {
                string path = TestContext.TestDir + "\\VideoFramesStream";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                result = new FileStream(path + "\\test.frames", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch { }
            return result;
        }

        /// <summary>
        /// Создает файл для хранения видеокадров
        /// </summary>
        /// <returns>Поток для записи/чтения видеокадров</returns>
        private Stream CreateStream(string file_name)
        {
            Stream result = null;
            try
            {
                string path = TestContext.TestDir + "\\VideoFramesStream";
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
        /// Создает и инициирует объект с данными видеокадра. Бинарные данные у кадра не создаются.
        /// </summary>
        /// <returns>Возвращает объект с данными пустого видеокадра</returns>
        private VideoFrame GenerateEmptyFrame()
        {
            VideoFrame result = new VideoFrame(StreamIdGenerator, TimeStampGenerator, "image/jpeg", null);
            StreamIdGenerator += 1;
            TimeStampGenerator += 10;
            return result;
        }

        /// <summary>
        /// Создает и инициирует объект с данными видеокадра.
        /// </summary>
        /// <param name="video_size">Длина бинарных данных видеокадра</param>
        /// <returns>Возвращает объект с данными видеокадра</returns>
        private VideoFrame GenerateFrame(int video_size)
        {
            VideoFrame result = GenerateEmptyFrame();
            byte[] video_data = new byte[video_size];
            for (int i = 0; i < video_data.Length; i++)
            {
                video_data[i] = 0xFF;
            }
            result.FrameData = video_data;
            return result;
        }

        /// <summary>
        /// Создает и инициирует массив объектов с данными видеокадров
        /// </summary>
        /// <param name="count">Количество элементов массива видеокадров</param>
        /// <param name="video_size">Длина бинарных данных видеокадра</param>
        /// <returns>Возвращает ссылку на массив с данными видеокадров</returns>
        private VideoFrame[] GenerateFrames(int count, int video_size)
        {
            VideoFrame[] result = new VideoFrame[count];
            for (int i = 0; i < count; ++i)
            {
                result[i] = GenerateFrame(video_size);
            }
            return result;
        }

        /// <summary>
        ///Тест для RecordStarted
        ///</summary>
        [TestMethod()]
        public void RecordStartedTest()
        {
            using (Stream stream = CreateStream("test.single_frame.dat"))
            {
                VideoFramesStream target = new VideoFramesStream(stream);
                DateTime expected = DateTime.Now;
                DateTime actual;
                target.RecordStarted = expected;
                actual = target.RecordStarted;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для RecordFinished
        ///</summary>
        [TestMethod()]
        public void RecordFinishedTest()
        {
            using (Stream stream = CreateStream("test.single_frame.dat"))
            {
                VideoFramesStream target = new VideoFramesStream(stream);
                DateTime expected = DateTime.Now;
                DateTime actual;
                target.RecordFinished = expected;
                actual = target.RecordFinished;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для Boundary
        ///</summary>
        [TestMethod()]
        public void BoundaryTest()
        {
            VideoFramesStream target;
            using (target = new VideoFramesStream(CreateStream()))
            {
                byte[] actual = target.Boundary;
                Assert.IsNotNull(actual);
            }
            try
            {
                byte[] actual = target.Boundary;
                Assert.Fail("Обращение к свойству уничтоженного объекта! Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        ///Тест для AverageFrameSize
        ///</summary>
        [TestMethod()]
        public void AverageFrameSizeTest()
        {
            Stream stream = CreateStream();
            VideoFramesStream target = new VideoFramesStream(stream);
            int expected = 4096;
            int actual;
            target.AverageFrameSize = expected;
            actual = target.AverageFrameSize;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для WriteStreamInfo
        ///</summary>
        [TestMethod()]
        public void WriteStreamInfoTest()
        {
            using (Stream stream = new VideoFramesStream(CreateStream()))
            {
                PrivateObject param0 = new PrivateObject(stream);
                VideoFramesStream_Accessor target = new VideoFramesStream_Accessor(param0);
                VideoFrame[] frames = GenerateFrames(3, 5);
                int bytes_wrote = 0;
                target.Write(frames, 0, 3, ref bytes_wrote);
                IList<VideoStreamInfo> info = null;
                try
                {
                    target.WriteStreamInfo(info);
                    Assert.Fail("Не задан массив данных с информацией о видеопотоках! должно генерироваться исключение System.ArgumentNullException.");
                }
                catch (ArgumentNullException) { }
                info = new List<VideoStreamInfo>(3);
                foreach (VideoFrame frame in frames)
                {
                    info.Add(new VideoStreamInfo(frame.CameraId, frame.ContentType.ToString()));
                }
                long expected = stream.Position;
                Assert.IsTrue(target.WriteStreamInfo(info));
                long actual = stream.Position;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для WriteFrame
        ///</summary>
        [TestMethod()]
        public void WriteFrameTest()
        {
            using (Stream stream = CreateStream("test.single_frame.frames"))
            {
                VideoFramesStream target = new VideoFramesStream(stream);
                VideoFrame frame = GenerateFrame(5);
                int actual = target.WriteFrame(frame);
                if (actual <= 0)
                {
                    Assert.Fail("Ошибка записи видеокадров в поток!");
                }
            }
        }

        /// <summary>
        ///Тест для Write
        ///</summary>
        [TestMethod()]
        public void WriteTest()
        {
            using (Stream stream = CreateStream("test.without_stream_info.dat"))
            {
                VideoFramesStream target = new VideoFramesStream(stream);
                VideoFrame[] frames = null;
                int offset = 0;
                int count = 0;
                int bytes_wrote = 0;
                int actual;
                try
                {
                    actual = target.Write(frames, offset, count, ref bytes_wrote);
                    Assert.Fail("Не задан массив видеокадров для записи в поток. Должно генерироваться исключение System.ArgumentNullException!");
                }
                catch (ArgumentNullException) { }
                frames = GenerateFrames(4,5);
                offset = -1;
                try
                {
                    actual = target.Write(frames, offset, count, ref bytes_wrote);
                    Assert.Fail("Параметр offset задан неверно. Должно генерироваться исключение System.ArgumentOutOfRangeException!");
                }
                catch (ArgumentOutOfRangeException) { }
                offset = 1;
                count = 4;
                try
                {
                    actual = target.Write(frames, offset, count, ref bytes_wrote);
                    Assert.Fail("Недопустимое сочетание значений параметров offset и count. Должно генерироваться исключение System.ArgumentException!");
                }
                catch (ArgumentException) { }
                count = 3;
                actual = target.Write(frames, offset, count, ref bytes_wrote);
                Assert.AreEqual(actual, count);
            }
        }

        /// <summary>
        ///Тест для SetLength
        ///</summary>
        [TestMethod()]
        public void SetLengthTest()
        {
            Stream stream = CreateStream("test.empty.dat");
            VideoFramesStream target = new VideoFramesStream(stream);
            long value = 5;
            target.SetLength(value);
            long actual = target.Length;
            if (actual <= 0)
            {
                Assert.Fail("Не изменилась длина потока!");
            }
        }

        /// <summary>
        ///Тест для ReadStreamInfo
        ///</summary>
        [TestMethod()]
        public void ReadStreamInfoTest()
        {
            WriteStreamInfoTest();

            VideoFramesStream target = new VideoFramesStream(CreateStream());
            IList<VideoStreamInfo> info = null;
            bool expected = true;
            bool actual;
            try
            {
                actual = target.ReadStreamInfo(info);
                Assert.Fail("Не задана ссылка на список, принимающий считанные данные! Должно генерироваться исключение System.ArgumentNullException.");
            }
            catch (ArgumentNullException) { }
            info = new List<VideoStreamInfo>();
            long pos = target.Position;
            actual = target.ReadStreamInfo(info);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(pos, target.Position);
        }

        /// <summary>
        ///Тест для ReadFrame
        ///</summary>
        [TestMethod()]
        public void ReadFrameTest()
        {
            WriteFrameTest();

            Stream stream = CreateStream("test.single_frame.frames");
            VideoFramesStream target = new VideoFramesStream(stream);
            VideoFrame frame;
            int actual = target.ReadFrame(out frame);
            if (actual <= 0)
            {
                Assert.Fail("Не удалось прочитать видеокадр из потока!");
            }
        }

        /// <summary>
        ///Тест для Read
        ///</summary>
        [TestMethod()]
        public void ReadTest()
        {
            WriteTest();

            VideoFramesStream target = new VideoFramesStream(CreateStream("test.without_stream_info.dat"));
            VideoFrame[] frames = null;
            int offset = 0;
            int count = 0;
            int bytes_readed = 0;
            int expected = 3;
            int actual;
            try
            {
                actual = target.Read(frames, offset, count, ref bytes_readed);
                Assert.Fail("Не задана ссылка на массив, принимающий данные прочитанных видеокадров. Должно генерироваться исключение System.ArgumentNullException.");
            }
            catch (ArgumentNullException) { }
            frames = new VideoFrame[3];
            offset = -1;
            try
            {
                actual = target.Read(frames, offset, count, ref bytes_readed);
                Assert.Fail("Не верное значение параметра offset. Должно генерироваться исключение System.ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException) { }
            offset = 2;
            count = 3;
            try
            {
                actual = target.Read(frames, offset, count, ref bytes_readed);
                Assert.Fail("Недопустимое сочетание значений параметров offset и count. Должно генерироваться исключение System.ArgumentException.");
            }
            catch (ArgumentException) { }
            offset = 0;
            actual = target.Read(frames, offset, count, ref bytes_readed);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для DoWriteStreamInfo
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DoWriteStreamInfoTest()
        {
            using (Stream stream = CreateStream("test.with_stream_info.dat"))
            {
                PrivateObject param0 = new PrivateObject(new VideoFramesStream(stream));
                VideoFramesStream_Accessor target = new VideoFramesStream_Accessor(param0);
                VideoFrame[] frames = GenerateFrames(3, 5);
                IList<VideoStreamInfo> info = null;
                try
                {
                    target.DoWriteStreamInfo(info);
                    Assert.Fail("Не задан массив данных с информацией о видеопотоках! должно генерироваться исключение System.ArgumentNullException.");
                }
                catch (ArgumentNullException) { }
                info = new List<VideoStreamInfo>(3);
                foreach (VideoFrame frame in frames)
                {
                    info.Add(new VideoStreamInfo(frame.CameraId, frame.ContentType.ToString()));
                }
                target.DoWriteStreamInfo(info);
            }
        }

        /// <summary>
        ///Тест для DoWriteHeader
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DoWriteHeaderTest()
        {
            using(VideoFramesStream stream = new VideoFramesStream(CreateStream("test.header.dat")))
            {
                PrivateObject param0 = new PrivateObject(stream);
                VideoFramesStream_Accessor target = new VideoFramesStream_Accessor(param0);
                bool expected = true;
                bool actual;
                actual = target.DoWriteHeader();
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для DoWriteFrame
        ///</summary>
        [TestMethod()]
        public void DoWriteFrameTest()
        {
            using (VideoFramesStream stream = new VideoFramesStream(CreateStream("test.2_frames.dat")))
            {
                VideoFrame frame = GenerateEmptyFrame();
                PrivateObject param0 = new PrivateObject(stream);
                VideoFramesStream_Accessor target = new VideoFramesStream_Accessor(param0);
                try
                {
                    target.DoWriteFrame(null);
                    Assert.Fail("Отсутствует ссылка на объект с данными. Должно генерироваться исключение System.ArgumentNullException.");
                }
                catch (ArgumentNullException) { }
                // записываем пустой кадр (без видеоданных)
                int actual = target.DoWriteFrame(frame);
                if (actual <= 0)
                {
                    Assert.Fail("Не могу записать видеокадр в поток!");
                }
                frame.FrameData = new byte[5] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                // записываем кадр c данными
                actual = target.DoWriteFrame(frame);
                if (actual <= 0)
                {
                    Assert.Fail("Не могу записать видеокадр в поток!");
                }
            }
        }

        /// <summary>
        ///Тест для DoReadStreamInfo
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DoReadStreamInfoTest()
        {
            DoWriteStreamInfoTest();

            VideoFramesStream stream = new VideoFramesStream(CreateStream("test.with_stream_info.dat"));
            PrivateObject param0 = new PrivateObject(stream);
            VideoFramesStream_Accessor target = new VideoFramesStream_Accessor(stream);
            IList<VideoStreamInfo> info = null;
            bool expected = true;
            bool actual;
            try
            {
                actual = target.DoReadStreamInfo(info);
                Assert.Fail("Не задана ссылка на список, принимающий считанные данные! Должно генерироваться исключение System.ArgumentNullException.");
            }
            catch (ArgumentNullException) { }
            info = new List<VideoStreamInfo>();
            actual = target.DoReadStreamInfo(info);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для DoReadHeader
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DoReadHeaderTest()
        {
            DoWriteHeaderTest();
            VideoFramesStream stream = new VideoFramesStream(CreateStream("test.header.dat"));
            PrivateObject param0 = new PrivateObject(stream);
            VideoFramesStream_Accessor target = new VideoFramesStream_Accessor(param0);
            bool expected = true;
            bool actual;
            actual = target.DoReadHeader();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для DoReadFrame
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DoReadFrameTest()
        {
            DoWriteFrameTest();

            VideoFramesStream stream = new VideoFramesStream(CreateStream("test.2_frames.dat"));
            PrivateObject param0 = new PrivateObject(stream);
            VideoFramesStream_Accessor target = new VideoFramesStream_Accessor(param0);
            VideoFrame frame;
            int actual = target.DoReadFrame(out frame);
            if (actual <= 0)
            {
                Assert.Fail("Не удалось прочитать видеокадр из потока!");
            }
            actual = target.DoReadFrame(out frame);
            if (actual <= 0)
            {
                Assert.Fail("Не удалось прочитать видеокадр из потока!");
            }
        }

        /// <summary>
        ///Тест для Dispose
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AlfaPribor.VideoStorage.dll")]
        public void DisposeTest()
        {
            VideoFramesStream stream = new VideoFramesStream(CreateStream());
            PrivateObject param0 = new PrivateObject(stream);
            VideoFramesStream_Accessor target = new VideoFramesStream_Accessor(param0);
            bool disposing = false;
            target.Dispose(disposing);
            try
            {
                long len = stream.Length;
                Assert.Fail("Обращение к свойству уничтоженного объекта! Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch (ObjectDisposedException) { }
            disposing = true;
            target.Dispose(disposing);
            try
            {
                byte[] data = stream.Boundary;
                Assert.Fail("Обращение к свойству уничтоженного объекта! Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch (ObjectDisposedException) { }
            try
            {
                DateTime dt = stream.RecordStarted;
                Assert.Fail("Обращение к свойству уничтоженного объекта! Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch (ObjectDisposedException) { }
            try
            {
                DateTime dt = stream.RecordFinished;
                Assert.Fail("Обращение к свойству уничтоженного объекта! Должно генерироваться исключение System.ObjectDisposedException.");
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        ///Тест для CheckHeader
        ///</summary>
        [TestMethod()]
        public void CheckHeaderTest()
        {
            DoWriteHeaderTest();

            Stream stream = CreateStream("test.header.dat");
            VideoFramesStream target = new VideoFramesStream(stream);
            StreamHeaderState expected = StreamHeaderState.Ok;
            StreamHeaderState actual;
            actual = target.CheckHeader();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Тест для Конструктор VideoFramesStream
        ///</summary>
        [TestMethod()]
        public void VideoFramesStreamConstructorTest()
        {
            Stream stream = null;
            VideoFramesStream target;
            try
            {
                target = new VideoFramesStream(stream);
                Assert.Fail("Не задан целевой поток! Должно гененрироваться исключение System.ArgumentNullException.");
            }
            catch (ArgumentNullException) { }
            stream = CreateStream();
            target = new VideoFramesStream(stream);
        }
    }
}
