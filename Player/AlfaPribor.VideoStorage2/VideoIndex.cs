using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoStorage
{
    /// <summary>
    /// Индекс видеоданных
    /// </summary>
    //public class VideoIndex : IVideoIndex
    //{
    //    #region Fields

    //    /// <summary>
    //    /// Количество предполагаемых видеопотоков по умолчанию
    //    /// </summary>
    //    private const int DefaultStreamsCount = 4;

    //    /// <summary>
    //    /// Количество предполагаемых индексов на один видеопоток
    //    /// </summary>
    //    private const int DefaultIndexesPerStream = 1000;

    //    /// <summary>
    //    /// Коллекция видеоиндексов от различных видеоканалов
    //    /// </summary>
    //    private List<List<SingleStreamFrameIndex>>
    //        _MultiStreamFrameIndexes = new List<List<SingleStreamFrameIndex>>(DefaultStreamsCount);
        
    //    /// <summary>
    //    /// Идентификатор поезда или видеоданных
    //    /// </summary>
    //    private int _Id;

    //    /// <summary>
    //    /// Идентификатор раздела хранилища
    //    /// </summary>
    //    private int _PartitionId;
        
    //    /// <summary>
    //    /// Статус интерфейса
    //    /// </summary>
    //    private VideoStorageIntStat _Status;

    //    /// <summary>
    //    /// Ссылка на коллекцию данных с информацией об имеющихся видеопотоках
    //    /// </summary>
    //    private IList<VideoStreamInfo> _VideoStreamInfoList;

    //    #endregion

    //    #region Methods

    //    /// <summary>
    //    /// Инициализирует индекс видеоданных заданными идентификаторами видеоданных и раздела
    //    /// </summary>
    //    /// <param name="id">Идентификатор видеоданных</param>
    //    /// <param name="part_id">Идентификатор раздела, содержащего видеоданные</param>
    //    public VideoIndex(int id, int part_id)
    //    {
    //        //_Id = id;
    //        //_PartitionId = part_id;
    //        //_RecordStarted = DateTime.MinValue;
    //        //_RecordFinished = DateTime.MinValue;
    //        //_Status = VideoStorageIntStat.Ok;
    //    }

    //    /// <summary>
    //    /// Используется при создании индекса видеоданных в случае, если статус интерфейса индекса заведомо известен
    //    /// (например, индекс не существует)
    //    /// </summary>
    //    /// <param name="id">Идентификатор видеоданных</param>
    //    /// <param name="part_id">Идентификатор раздела, содержащего видеоданные</param>
    //    /// <param name="status">Статус интерфейса, обеспечивающего работу с индексом видеоданных</param>
    //    public VideoIndex(int id, int part_id, VideoStorageIntStat status)
    //    {
    //        //_Id = id;
    //        //_PartitionId = part_id;
    //        //_RecordStarted = DateTime.MinValue;
    //        //_RecordFinished = DateTime.MinValue;
    //        //_Status = status;
    //    }

    //    /// <summary>
    //    /// Инициализирует индекс видеоданных с учетом колличества создаваемых видеопотоков
    //    /// </summary>
    //    /// <param name="id">Идентификатор видеоданных</param>
    //    /// <param name="part_id">Идентификатор раздела, содержащего видеоданные</param>
    //    /// <param name="streams">Информация о видеопотоках, содержащихся в индексе видеоданных</param>
    //    /// <exception cref="System.ArgumentNullException">
    //    /// Значение параметра streams равно null
    //    /// </exception>
    //    public VideoIndex(int id, int part_id, IList<VideoStreamInfo> streams)
    //        : this(id, part_id)
    //    {
    //        if (streams == null)
    //            throw new ArgumentNullException("streams");

    //        _MultiStreamFrameIndexes.Capacity = streams.Count;
    //        for (int i = 0; i < streams.Count; ++i)
    //        {
    //            _MultiStreamFrameIndexes.Add(new List<SingleStreamFrameIndex>(DefaultIndexesPerStream));
    //        }
    //        _VideoStreamInfoList = streams;
    //    }

    //    #endregion

    //    #region IVideoIndex Members

    //    /// <summary>
    //    /// Коллекция данных с информацией об имеющихся видеопотоках
    //    /// </summary>
    //    public IList<VideoStreamInfo> StreamInfoList
    //    {
    //        get { return _VideoStreamInfoList; }
    //    }

    //    //public DateTime RecordStarted
    //    //{
    //    //    get { return _RecordStarted; }
    //    //}

    //    //public DateTime RecordFinished
    //    //{
    //    //    get { return _RecordFinished; }
    //    //}

    //    public int GetStartTime(int delta)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int GetFinishTime(int delta)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int GetNextFrameTime(int cam_id, int current_time)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int GetPrevFrameTime(int cam_id, int current_time)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion

    //    #region IVideoInterface Members

    //    /// <summary>
    //    /// Идентификатор поезда или видеоданных
    //    /// </summary>
    //    public string Id
    //    {
    //        get { return _Id; }
    //    }

    //    /// <summary>
    //    /// Идентификатор раздела хранилища
    //    /// </summary>
    //    public int PartitionId
    //    {
    //        get { return _PartitionId; }
    //    }

    //    /// <summary>
    //    /// Статус интерфейса
    //    /// </summary>
    //    public VideoStorageIntStat Status
    //    {
    //        get { return _Status; }
    //    }

    //    #endregion
    //}
}
