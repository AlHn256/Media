using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AlfaPribor.ASKIN.Data
{

    /// <summary>Данные вагона</summary>
    public class WagonData: IEquatable<WagonData>
    {

        int _WagId = 0;
        int _RefId = 0;     //Ссылка на реальный вагон в случае виртуального "склеенного" состава
        int _RefTrain = 0;  //Сссылка на реальный поезд другого ПСЧ виртуального "склеенного" состава с другого ПСЧ
        int _RefPoint = 0;  //Номер ПСЧ виртуального "склеенного" состава с другого ПСЧ
        int _TrainId = 0;
        int _Sn = 0;
        int _SnSost = 0;
        int _Loco = 0;
        string _InvNumber = string.Empty;
        string _InvNumberByNL = string.Empty;
        int _InvType = 0;
        int _Accuracy = 0;
        int _TimeSpan = 0;
        int _TimeSpanBegin = 0;
        int _Speed;
        int _Direction;
        int _BestChannel;
        string _TotalFrames;
        string _RecognFrames;
        string _MissedFrames;

        /// <summary>Порядковый номер пересечения конца вагона</summary>
        [XmlElement("sn")]
        public int Sn
        {
            get { return _Sn; }
            set { _Sn = value; }
        }

        /// <summary>Порядковый номер вагона в составе</summary>
        [XmlElement("snsost")]
        public int SnSost
        {
            get { return _SnSost; }
            set { _SnSost = value; }
        }

        /// <summary>Идентификатор поезда/отцепа</summary>
        [XmlElement("train_id")]
        public int TrainId
        {
            get { return _TrainId; }
            set { _TrainId = value; }
        }

        /// <summary>Серийный номер вагона</summary>
        [XmlElement("wagon_id")]
        public int WagId
        {
            get { return _WagId; }
            set { _WagId = value; }
        }
        
        /// <summary>Ссылка на реальный вагон в случае виртуального "склеенного" состава</summary>
        [XmlElement("ref_id")]
        public int RefId
        {
            get { return _RefId; }
            set { _RefId = value; }
        }

        /// <summary>Ссылка на состав на другом ПСЧ в случае виртуального "склеенного" состава с другого ПСЧ</summary>
        [XmlElement("ref_train")]
        public int RefTrain
        {
            get { return _RefTrain; }
            set { _RefTrain = value; }
        }

        /// <summary>Номер ПСЧ виртуального "склеенного" состава с другого ПСЧ</summary>
        [XmlElement("ref_point")]
        public int RefPoint
        {
            get { return _RefPoint; }
            set { _RefPoint = value; }
        }

        /// <summary>Признак локомотива</summary>
        [XmlElement("loco")]
        public int Loco
        {
            get { return _Loco; }
            set { _Loco = value; }
        }

        /// <summary>Инвентарный номер вагона</summary>
        [XmlElement("inv")]
        public string InvNumber
        {
            get { return _InvNumber; }
            set { _InvNumber = value; }
        }

        /// <summary>Инвентарный номер вагона по натурному листу</summary>
        [XmlElement("inv_nl")]
        public string InvNumByNL
        {
            get { return _InvNumberByNL; }
            set { _InvNumberByNL = value; }
        }

        /// <summary>Вероятность номера</summary>
        [XmlElement("accuracy")]
        public int Accuracy
        {
            get { return _Accuracy; }
            set { _Accuracy = value; }
        }

        /// <summary>Тип достоверности номера</summary>
        [XmlElement("inv_type")]
        public int InvType
        {
            get { return _InvType; }
            set { _InvType = value; }
        }

        /// <summary>Метка времени окончания вагона (мс)</summary>
        [XmlElement("time_span")]
        public int TimeSpan
        {
            get { return _TimeSpan; }
            set { _TimeSpan = value; }
        }

        /// <summary>Метка времени начала вагона (мс)</summary>
        [XmlElement("time_span_begin")]
        public int TimeSpanBegin
        {
            get { return _TimeSpanBegin; }
            set { _TimeSpanBegin = value; }
        }

        /// <summary>Скорость</summary>
        [XmlElement("speed")]
        public int Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        /// <summary>Направление</summary>
        [XmlElement("direction")]
        public int Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        /// <summary>Канал лучшего распознавания</summary>
        [XmlElement("bestchannel")]
        public int BestChannel
        {
            get { return _BestChannel; }
            set { _BestChannel = value; }
        }

        /// <summary>Общее число распознанных кадров</summary>
        [XmlElement("totalframes")]
        public string TotalFrames
        {
            get { return _TotalFrames; }
            set { _TotalFrames = value; }
        }

        /// <summary>Число пропущенных кадров</summary>
        [XmlElement("missedframes")]
        public string MissedFrames
        {
            get { return _MissedFrames; }
            set { _MissedFrames = value; }
        }

        /// <summary>Число кадров с найденным номером</summary>
        [XmlElement("recogframes")]
        public string RecognFrames
        {
            get { return _RecognFrames; }
            set { _RecognFrames = value; }
        }

        /// <summary>Пустой конструктор</summary>
        public WagonData() { }

        /// <summary>Конструктор копирования</summary>
        /// <param name="other">Объект, с которого производиться копирование</param>
        /// <exception cref="System.ArgumentNullException">Не задан объект для копирования</exception>
        public WagonData(WagonData other)
        {
            if (other == null)
	        {
                throw new ArgumentNullException();
	        }
            _Loco = other._Loco;
            _WagId = other._WagId;
            _RefId = other._RefId;
            _RefTrain = other._RefTrain;
            _RefPoint = other._RefPoint;
            _TrainId = other._TrainId;
            _Sn = other._Sn;
            _SnSost = other._SnSost;
            _InvNumber = other._InvNumber;
            _InvNumberByNL = other._InvNumberByNL;
            _InvType = other._InvType;
            _Accuracy = other._Accuracy;
            _TimeSpan = other._TimeSpan;
            _TimeSpanBegin = other._TimeSpanBegin;
            _Speed = other._Speed;
            _Direction = other._Direction;
            _BestChannel = other._BestChannel;
            _TotalFrames = other._TotalFrames;
            _MissedFrames = other._MissedFrames;
            _RecognFrames = other.RecognFrames;
        }

        /// <summary>Конструктор</summary>
        /// <param name="wagon_id">Идентификатор записи в базе</param>
        /// <param name="train_id">Идентификатор поезда/ отцепа в СБВ УВГ</param>
        /// <param name="wagon_sn">Последовательный номер вагона</param>
        public WagonData(int wagon_id, int train_id, int sn)
        {
            _WagId = wagon_id;
            _TrainId = train_id;
            _Sn = sn;
        }

        /// <summary>Предоставляет формат представления даты/времени в виде строки</summary>
        /// <returns>Формат представления даты/времени в виде строки</returns>
        protected virtual string GetDateTimeConvFormat()
        {
            return "dd.MM.yyyy HH:mm:ss";
        }

        #region IEquatable<WagonWeightData> members

        /// <summary>Указывает, равен ли текущий объект другому объекту того же типа</summary>
        /// <param name="other">Объект, с которым происходит сравнение</param>
        /// <returns>true, если свойства объектов равны, false - в противном случае</returns>
        public bool Equals(WagonData other)
        {
            if (other == null)
            {
                return false;
            }
            return
                (_WagId == other._WagId) &&
                (_RefId == other._RefId) &&
                (_RefTrain == other._RefTrain) &&
                (_TrainId == other._TrainId) &&
                (_Sn == other._Sn) &&
                (_SnSost == other._SnSost) &&
                (_Loco == other._Loco) &&
                (_InvNumber == other._InvNumber) &&
                (_InvNumberByNL == other._InvNumberByNL) &&
                (_InvType == other._InvType) &&
                (_Accuracy == other._Accuracy) &&
                (_TimeSpan == other._TimeSpan) &&
                (_TimeSpanBegin == other._TimeSpanBegin) &&
                (_Speed == other._Speed) &&
                (_Direction == other._Direction) &&
                (_BestChannel == other._BestChannel) &&
                (_TotalFrames == other._TotalFrames) &&
                (_MissedFrames == other._MissedFrames) &&
                (_RecognFrames == other._RecognFrames);
        }

        #endregion

    }
}
