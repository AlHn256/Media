using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Интерфейс для чтения видеоданных из хранилища</summary>
    public interface IVideoReader : IVideoInterface
    {
        /// <summary>Получить интерфейс индекса видеоданных</summary>
        IVideoIndex VideoIndex { get; }

        /// <summary>Прочитать кадр указанной камеры, соответствующий указанному моменту времени</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult ReadFrame(int cam_id, int time_stamp, out VideoFrame frame);

        /// <summary>Прочитать кадр указанной камеры, соответствующий указанному моменту времени</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="delta_time">Интервал времени относительно time_stamp, в пределах которого будет искаться видеокадр</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult ReadFrame(int cam_id, int time_stamp, int delta_time, out VideoFrame frame);

        /// <summary>Прочитать кадры всех каналов соответствующие указанному моменту времени</summary>
        /// <param name="time_stamp">Время от начала записи</param>
        /// <param name="frames">Список кадров</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult ReadFrames(int time_stamp, out IList<VideoFrame> frames);

        /// <summary>Прочитать кадры всех каналов соответствующие указанному моменту времени</summary>
        /// <param name="time_stamp">Время от начала записи</param>
        /// /// <param name="delta_time">Интервал времени относительно time_stamp, в пределах которого будет искаться видеокадр</param>
        /// <param name="frames">Список кадров</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult ReadFrames(int time_stamp, int delta_time, out IList<VideoFrame> frames);

        /// <summary>Прочитать первый кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult ReadFirstFrame(int cam_id, out VideoFrame frame);

        /// <summary>Прочитать последний кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult ReadLastFrame(int cam_id, out VideoFrame frame);

        /// <summary>Прочитать следующий кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="step">Инкремент номера кадра</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult ReadNextFrame(int cam_id, int step, out VideoFrame frame);

        /// <summary>Прочитать предыдущий кадр указанного видеопотока</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="step">Инкремент номера кадра</param>
        /// <param name="frame">Данные кадра</param>
        /// <returns>Результат выполнения операции</returns>
        VideoStorageResult ReadPrevFrame(int cam_id, int step, out VideoFrame frame);
    }
}
