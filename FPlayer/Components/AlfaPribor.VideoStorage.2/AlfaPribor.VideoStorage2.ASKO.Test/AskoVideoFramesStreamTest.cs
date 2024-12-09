using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AlfaPribor.Streams;
using System;
using AlfaPribor.VideoStorage;
using System.Collections.Generic;

namespace AlfaPribor.VideoStorage.ASKO.Test
{
    /// <summary>
    ///Это класс теста для VideoFramesStreamTest, в котором должны
    ///находиться все модульные тесты VideoFramesStreamTest
    ///</summary>
    [TestClass()]
    public class AskoVideoFramesStreamTest
    {
        private TestContext testContextInstance;
        private const string fileName = "test.mjpg";
        private const string writeFileName = "write_test.mjpg";
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

        private VideoFrame CreateTestVideoFrame()
        {
            byte[] data = new byte[generator.Next(1024)];
            for (int i = 0; i < data.Length; i++)
			{
			    data[i] = (byte)generator.Next(256);
			}
            VideoFrame result = new VideoFrame(
                generator.Next(4),
                generator.Next(10000),
                "image/jpeg",
                data
            );
            return result;
        }

        private VideoFrame[] CreateTestVideoFrames(int count)
        {
            VideoFrame[] result = new VideoFrame[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = CreateTestVideoFrame();
            }
            return result;
        }

        /// <summary>
        ///Тест для RecordStarted
        ///</summary>
        [TestMethod()]
        public void RecordStartedTest()
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                DateTime expected = new DateTime(2010, 5, 17, 11, 1, 54);
                DateTime actual;
                Assert.AreEqual(StreamHeaderState.Ok, target.CheckHeader());
                actual = target.RecordStarted;
                Assert.AreEqual(expected, actual);
                expected = DateTime.Now;
                target.RecordStarted = expected;
                Assert.AreEqual(StreamHeaderState.Ok, target.CheckHeader());
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
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                DateTime expected = new DateTime(2010, 5, 17, 11, 2, 00);
                DateTime actual;
                Assert.AreEqual(StreamHeaderState.Ok, target.CheckHeader());
                actual = target.RecordFinished;
                Assert.AreEqual(expected, actual);
                expected = DateTime.Now;
                target.RecordFinished = expected;
                actual = target.RecordFinished;
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для CheckHeader
        ///</summary>
        [TestMethod()]
        public void CheckHeaderTest()
        {
            using (Stream stream = new FileStream(fileName,  FileMode.Open, FileAccess.Read))
            {
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                StreamHeaderState expected = StreamHeaderState.Ok;
                StreamHeaderState actual = target.CheckHeader();
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для Конструктор AskoVideoFramesStream
        ///</summary>
        [TestMethod()]
        public void VideoFramesStreamConstructorTest()
        {
            AskoVideoFramesStream target;
            try
            {
                target = new AskoVideoFramesStream(null);
                Assert.Fail("Ожидается исключение ArgumentNullException!");
            }
            catch (ArgumentNullException) { }
            using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (target = new AskoVideoFramesStream(stream)) { }
            }
        }

        /// <summary>
        ///Тест для ReadStreamInfo
        ///</summary>
        [TestMethod()]
        public void ReadStreamInfoTest()
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                IList<VideoStreamInfo> info = new List<VideoStreamInfo>();
                bool expected = true;
                bool actual = target.ReadStreamInfo(info);
                Assert.AreEqual(expected, actual);
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
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                int frames_count = 426;
                VideoFrame[] frames = new VideoFrame[frames_count];
                int offset = 0;
                int count = frames_count;
                int bytes_readed = 0;
                int expected = frames_count;
                int actual;
                try
                {
                    target.Read(null, offset, count, ref bytes_readed);
                    Assert.Fail("Ожидается исключение ArgumentNullException!");
                }
                catch (ArgumentNullException) { }
                try
                {
                    target.Read(frames, 1, frames_count, ref bytes_readed);
                    Assert.Fail("Ожидается исключение ArgumentException!");
                }
                catch (ArgumentException) { }
                try
                {
                    target.Read(frames, frames_count, 1, ref bytes_readed);
                    Assert.Fail("Ожидается исключение ArgumentException!");
                }
                catch (ArgumentException) { }

                actual = target.Read(frames, offset, count, ref bytes_readed);
                Assert.IsTrue(bytes_readed > 0);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Тест для ReadFrame
        ///</summary>
        [TestMethod()]
        public void ReadFrameTest()
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                VideoFrame frame;
                int actual = target.ReadFrame(out frame);
                Assert.IsNotNull(frame);
                Assert.IsTrue(actual > 0);
                Assert.AreEqual(frame.TimeStamp, 25);
                Assert.AreEqual(frame.CameraId, 2);
                Assert.AreEqual(frame.ContentType.ToString(), "image/jpeg");
                Assert.AreEqual(frame.FrameData.Length, 49433);
            }
        }

        /// <summary>
        ///Тест для WriteStreamInfo
        ///</summary>
        [TestMethod()]
        public void WriteStreamInfoTest()
        {
            using (Stream stream = new FileStream(writeFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                IList<VideoStreamInfo> info = new List<VideoStreamInfo>();
                info.Add(new VideoStreamInfo(0, "image/jpeg; resolution=640x480"));
                bool expected = true;
                try
                {
                    target.WriteStreamInfo(null);
                    Assert.Fail("Ожидается исключение ArgumentNullException!");
                }
                catch (ArgumentNullException) { }
                bool actual = target.WriteStreamInfo(info);
                Assert.AreEqual(expected, actual);
                IList<VideoStreamInfo> actual_info = new List<VideoStreamInfo>();
                target.ReadStreamInfo(actual_info);
                Assert.AreEqual(0, actual_info.Count);
                target.WriteFrame(new VideoFrame(0, 5, "image/jpeg; resolution=640x480", null));
                target.ReadStreamInfo(actual_info);
                Assert.IsTrue(info[0].Equals(actual_info[0]));
            }
        }

        /// <summary>
        ///Тест для WriteFrame
        ///</summary>
        [TestMethod()]
        public void WriteFrameTest()
        {
            using (Stream stream = new FileStream(writeFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                byte[] data = new byte[10];
                data.Initialize();
                VideoFrame frame = CreateTestVideoFrame();
                int actual = target.WriteFrame(frame);
                Assert.IsTrue(actual > 0);
                target.Position = 0;
                VideoFrame actual_frame;
                target.ReadFrame(out actual_frame);
                Assert.IsTrue(actual_frame.Equals(frame));
            }
        }

        /// <summary>
        ///Тест для Write
        ///</summary>
        [TestMethod()]
        public void WriteTest()
        {
            using (Stream stream = new FileStream(writeFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                AskoVideoFramesStream target = new AskoVideoFramesStream(stream);
                int frames_count = 10;
                VideoFrame[] frames = CreateTestVideoFrames(frames_count);
                int offset = 0;
                int count = frames_count;
                int bytes_wrote = 0;
                try
                {
                    target.Write(null, offset, count, ref bytes_wrote);
                }
                catch (ArgumentNullException) { }
                try
                {
                    target.Write(frames, frames_count, 1, ref bytes_wrote);
                }
                catch (ArgumentException) { }
                try
                {
                    target.Write(frames, 1, frames_count, ref bytes_wrote);
                }
                catch (ArgumentException) { }
                int actual = target.Write(frames, offset, count, ref bytes_wrote);
                Assert.AreEqual(frames_count, actual);
                Assert.IsTrue(bytes_wrote > 0);
                VideoFrame[] actual_frames = new VideoFrame[frames_count];
                target.Position = 0;
                int read_bytes = 0;
                target.Read(actual_frames, offset, count, ref read_bytes);
                bool result = true;
                for (int i = 0; i < frames_count; i++)
                {
                    if (!frames[i].Equals(actual_frames[i]))
                    {
                        result = false;
                        break;
                    }
                }
                Assert.IsTrue(result);
            }
        }
    }
}
