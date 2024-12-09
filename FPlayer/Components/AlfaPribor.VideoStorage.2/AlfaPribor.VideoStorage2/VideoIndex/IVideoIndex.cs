using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage2
{
    /// <summary>Интерфейс индекса видеоданных</summary>
    /// <remarks>Предназначен для ускорения работы с видеоданными</remarks>
    public interface IVideoIndex : IVideoInterface
    {
        /// <summary>Информация о видеопотоках</summary>
        IList<VideoStreamInfo> StreamInfoList { get; }

        /// <summary>Дата и время команды начала записи видеопотоков</summary>
        DateTime RecordStarted { get; }

        /// <summary>Дата и время команды окончания записи видеопотоков</summary>
        DateTime RecordFinished { get; }

        /// <summary>Получить метку времени самого первого кадра (по всем видеопотокам)</summary>
        /// <param name="delta">Время от начала записи, в пределах которого должен находится первый кадр</param>
        /// <returns>Метка времени или минус 1, если не найдено</returns>
        /// <remarks>Смещение от команды начала записи видеопотоков (мсек)</remarks>
        int GetStartTime(int delta);

        /// <summary>Получить метку времени самого последнего кадра (по всем видеопотокам)</summary>
        /// <param name="delta">Допуск на разность меток времени кадров соседних каналов</param>
        /// <returns>Метка времени или минус 1, если не найдено</returns>
        /// <remarks>Смещение от команды окончания записи видеопотоков (мсек)</remarks>
        int GetFinishTime(int delta);

        /// <summary>Получить метку времени следующего кадра (мсек)</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если следующий кадр не найден</returns>
        /// <remarks>
        /// Число миллисекунд, прошедших от команды начала записи видеопотоков до начала видеоданных кадра
        /// ,следующего за кадром, поступившим ко времени current_time (мсек) после получения команды
        /// начала записи видеопотоков
        /// </remarks>
        int GetNextFrameTime(int cam_id, int current_time);

        /// <summary>Получить метку времени следующего кадра (мсек) для всех видеопотоков</summary>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если следующий кадр не найден</returns>
        /// <remarks>
        /// Число миллисекунд, прошедших от команды начала записи видеопотоков до начала видеоданных кадра
        /// ,следующего за кадром, поступившим ко времени current_time (мсек) после получения команды
        /// начала записи видеопотоков
        /// </remarks>
        int GetNextFrameTime(int current_time);

        /// <summary>Получить метку времени предыдущего кадра</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если кадр не найден</returns>
        /// <remarks>
        /// Число миллисекунд, прошедших от команды начала записи видеопотоков до начала видеоданных кадра
        /// ,идущего перед кадром, поступившим ко времени current_time (мсек) после получения команды
        /// начала записи видеопотоков
        /// </remarks>
        int GetPrevFrameTime(int cam_id, int current_time);

        /// <summary>Получить метку времени предыдущего кадра для всех видеопотоков</summary>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если кадр не найден</returns>
        /// <remarks>
        /// Число миллисекунд, прошедших от команды начала записи видеопотоков до начала видеоданных кадра
        /// ,идущего перед кадром, поступившим ко времени current_time (мсек) после получения команды
        /// начала записи видеопотоков
        /// </remarks>
        int GetPrevFrameTime(int current_time);

        /// <summary>Получить метку времени начала кадра (мсек)</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Метка времени или минус 1, если кадр не найден</returns>
        int GetFrameTime(int cam_id, int current_time);

        /// <summary>Получить номер кадра в видеопотоке</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <param name="current_time">Метка времени текущего кадра</param>
        /// <returns>Индекс кадра или минус 1, если кадр не найден</returns>
        int GetFrameIndex(int cam_id, int current_time);

        /// <summary>Получить общее количество кадров в видеопотоке</summary>
        /// <param name="cam_id">Номер камеры / видеопотока</param>
        /// <returns>Количество кадров в видеопотоке или минус 1,
        /// если видеопоток с заданным идентификатором не существует
        /// </returns>
        int GetFramesCount(int cam_id);

    }
}
