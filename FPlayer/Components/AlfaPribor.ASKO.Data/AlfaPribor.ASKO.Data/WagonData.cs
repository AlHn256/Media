using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Данные вагона</summary>
   
    public class WagonData: IEquatable<WagonData>
    {

        #region Variables

        /// <summary>Идентификатор вагона</summary>
        int _WagId = 0;
        /// <summary>Идентификатор поезда/отцепа</summary>
        int _TrainId = 0;
        /// <summary>Порядковый номер пересечения вагона в составе</summary>
        int _Sn = 0;
        /// <summary>Порядковый номер вагона в составе</summary>
        int _SnSost = 0;
        /// <summary>Тип вагона</summary>
        int _Loco = 0;
        /// <summary>Инвентарный номер вагона</summary>
        string _InvNumber = string.Empty;
        /// <summary>Инвентарный номер вагона по натурному листу</summary>
        string _InvNumberByNL = string.Empty;
        /// <summary>Метка времени начала вагона (мс)</summary>
        int _TimeSpanBegin = 0;
        /// <summary>Метка времени окончания вагона (мс)</summary>
        int _TimeSpanEnd = 0;
        /// <summary>Метка времени начала вагона (мс) по данным БС</summary>
        int _TimeSpanBeginBS = 0;
        /// <summary>Метка времени окончания вагона (мс) по данным БС</summary>
        int _TimeSpanEndBS = 0;
        /// <summary>Скорость в начале</summary>
        int _SpeedBegin = 0;
        /// <summary>Скорость</summary>
        int _SpeedEnd = 0;
        /// <summary>Направление в начале</summary>
        int _DirectionBegin = 0;
        /// <summary>Направление в конце</summary>
        int _DirectionEnd = 0;
        /// <summary>Негабариты</summary>
        int _Ngb = 0;
        /// <summary>Вагон маркирован</summary>
        bool _Mark = false;
        /// <summary>Запрет ставить в состав</summary>
        bool _Banned = false;
        /// <summary>Комментарий</summary>
        string _Comment = string.Empty;
        /// <summary>Дата/время изменения данных вагона</summary>
        DateTime _TimeChanged;
        /// <summary>Дата/время прохождения вагона</summary>
        DateTime _Time;
        /// <summary>Признак остановки поезда на вагоне</summary>
        int _Stop = 0;

        /// <summary>Номер вагона для отображения в </summary>
        int _SnShow = 0;

        //Распознавание

        /// <summary>Распознанный номер</summary>
        string _OcrInv = "";
        /// <summary>Тип распознанного номера</summary>
        int _OcrType = 0;
        /// <summary>Вероятность распознанного номера</summary>
        int _OcrAccuracy = 0;
        /// <summary>Проверка оператором</summary>
        int _OcrCheck;
        /// <summary>Идентификатор поезда в системе аскин</summary>
        int _OcrAskinTrainID;

        #endregion

        #region Основные данные вагона

        /// <summary>Порядковый номер пересечения вагона в составе</summary>
       
        [XmlElement("sn")]
        public int Sn
        {
            get { return _Sn; }
            set { _Sn = value; }
        }

        /// <summary>Порядковый номер вагона в составе</summary>
       
        [XmlElement("sn_sost")]
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

        /// <summary>Идентификатор вагона</summary>
       
        [XmlElement("wagon_id")]
        public int WagId
        {
            get { return _WagId; }
            set { _WagId = value; }
        }

        /// <summary>Тип вагона</summary>
       
        [XmlElement("loco")]
        public int Loco
        {
            get { return _Loco; }
            set { _Loco = value; }
        }

        /// <summary>Инвентарный номер вагона (введенный оператором)</summary>
       
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

        /// <summary>Метка времени начала вагона (мс)</summary>
       
        [XmlElement("time_span_begin")]
        public int TimeSpanBegin
        {
            get { return _TimeSpanBegin; }
            set { _TimeSpanBegin = value; }
        }

        /// <summary>Метка времени окончания вагона (мс)</summary>
       
        [XmlElement("time_span_end")]
        public int TimeSpanEnd
        {
            get { return _TimeSpanEnd; }
            set { _TimeSpanEnd = value; }
        }

        /// <summary>Метка времени начала вагона (мс) по данным БС</summary>
       
        [XmlElement("time_span_begin_bs")]
        public int TimeSpanBeginBS
        {
            get { return _TimeSpanBeginBS; }
            set { _TimeSpanBeginBS = value; }
        }

        /// <summary>Метка времени окончания вагона (мс) по данным БС</summary>
       
        [XmlElement("time_span_end_bs")]
        public int TimeSpanEndBS
        {
            get { return _TimeSpanEndBS; }
            set { _TimeSpanEndBS = value; }
        }

        /// <summary>Скорость в начале</summary>
       
        [XmlElement("speed_begin")]
        public int SpeedBegin
        {
            get { return _SpeedBegin; }
            set { _SpeedBegin = value; }
        }

        /// <summary>Скорость</summary>
       
        [XmlElement("speed_end")]
        public int SpeedEnd
        {
            get { return _SpeedEnd; }
            set { _SpeedEnd = value; }
        }

        /// <summary>Направление в начале</summary>
       
        [XmlElement("direction_begin")]
        public int DirectionBegin
        {
            get { return _DirectionBegin; }
            set { _DirectionBegin = value; }
        }

        /// <summary>Направление в конце</summary>
       
        [XmlElement("direction_end")]
        public int DirectionEnd
        {
            get { return _DirectionEnd; }
            set { _DirectionEnd = value; }
        }

        /// <summary>Признак остановки (0 - нормальное движение, 1 - остановка)</summary>
        [XmlElement("stop")]
        public int Stop
        {
            get { return _Stop; }
            set { _Stop = value; }
        }
               
        /// <summary>Негабариты, 
        /// Маска зон для обычного режима
        /// Маска расширенных областей для расширенного режима</summary>
       
        [XmlElement("ngb")]
        public int Ngb
        {
            get { return _Ngb; }
            set { _Ngb = value; }
        }

        /// <summary>Вагон маркирован</summary>
       
        [XmlElement("mark")]
        public bool Mark
        {
            get { return _Mark; }
            set { _Mark = value; }
        }

        /// <summary>Запрет ставить в состав</summary>
       
        [XmlElement("banned")]
        public bool Banned
        {
            get { return _Banned; }
            set { _Banned = value; }
        }
        
        /// <summary>Комментарий</summary>
       
        [XmlElement("comment")]
        public string Comment
        {
            get { return _Comment; }
            set { _Comment = value; }
        }

        /// <summary>Дата и время изменения</summary>
       
        [XmlElement("timechanged")]
        public DateTime TimeChanged
        {
            get { return _TimeChanged; }
            set { _TimeChanged = value; }
        }

        /// <summary>Дата и время прохождения вагона (состава)</summary>
       
        [XmlElement("time")]
        public DateTime Time
        {
            get { return _Time; }
            set { _Time = value; }
        }

        /// <summary>Дата и время прохождения вагона (состава)</summary>
       
        [XmlElement("sn_show")]
        public int  SnShow
        {
            get { return _SnShow; }
            set { _SnShow = value; }
        }



        #endregion

        #region Данные о грузе

        /// <summary>Продольное смещение центра масс груза (по оси X)</summary>
        [XmlElement("length_shift")]
       
        public int ShiftX { get; set; }

        /// <summary>Поперечное смещение центра масс груза (по оси Y)</summary>
        [XmlElement("width_shift")]
       
        public int ShiftY { get; set; }

        /// <summary>Высота центра масс груза головки рельс (по оси Z)</summary>
        [XmlElement("v_shift")]
       
        public int ShiftZ { get; set; }

        /// <summary>Объем груза</summary>
        [XmlElement("cargo_volume")]
       
        public int CargoVolume { get; set; }

        /// <summary>Итоговая длина груза</summary>
        [XmlElement("cargo_length")]
       
        public int CargoLength { get; set; }

        /// <summary>Средняя ширина груза</summary>
        [XmlElement("cargo_width")]
       
        public int CargoWidth { get; set; }

        /// <summary>Средняя высота груза</summary>
        [XmlElement("cargo_height")]
       
        public int CargoHeight { get; set; }

        /// <summary>Максимальная ширина груза</summary>
        [XmlElement("cargo_width_max")]
       
        public int CargoWidthMax { get; set; }

        /// <summary>Максимальная высота груза</summary>
        [XmlElement("cargo_height_max")]
       
        public int CargoHeightMax { get; set; }

        /// <summary>Наличие груза</summary>
        [XmlElement("cargo_exist")]
       
        public bool CargoExist { get; set; }

        /// <summary>Признак очистки вагона</summary>
        [XmlElement("cargo_clear")]
       
        public bool CargoClear { get; set; }

        /// <summary>Тип груза
        /// (0 - неизвестен / 1 – навалочный / 2 – тарно-штучный / 3 - наливной)</summary>
        [XmlElement("cargo_type")]
       
        public int CargoType{ get; set; }

        /// <summary>Тип вагона</summary>
        [XmlElement("wagon_type")]
       
        public int WagonType { get; set; }

        /// <summary>Модель вагона</summary>
        [XmlElement("wagon_model")]
       
        public string WagonModel { get; set; }

        /// <summary>Уровень пола</summary>
        [XmlElement("floor_height")]
       
        public int FloorLevel { get; set; }
        
        /// <summary>Несоответствие по грузу</summary>
        [XmlElement("cargo_ngbt")]
       
        public bool CargoNGBT { get; set; }

        /// <summary>Несоответствие по грузу</summary>
        [XmlElement("cargo_max_height_ngbt")]
       
        public bool CargMaxHeightNGBT { get; set; }

        #endregion

        #region Данные сравнения грузов

        /// <summary>Идентификатор удалённого сервера</summary>
        [XmlElement("server_id")]
       
        public int RemoteServerId { get; set; }

        /// <summary>Идентификатор поезда на удалённом сервере</summary>
        [XmlElement("remote_train_id")]
       
        public int RemoteTrainId { get; set; }

        /// <summary>Номер поезда на удалённом сервере</summary>
        [XmlElement("remote_train_number")]
       
        public string RemoteTrainNumber { get; set; }

        /// <summary>Индекс поезда на удалённом сервере</summary>
        [XmlElement("remote_train_index")]
       
        public string RemoteTrainIndex { get; set; }

        /// <summary>Серийный номер вагона на удалённом сервере</summary>
        [XmlElement("remote_wagon_sn")]
       
        public int RemoteWagonSn { get; set; }

        /// <summary>Идентификатор вагона на удалённом сервере</summary>
        [XmlElement("remote_wagon_id")]
       
        public int RemoteWagonId { get; set; }

        /// <summary>Дата прохождения вагона на удалённом сервере</summary>
        [XmlElement("remote_wagon_date")]
       
        public DateTime RemoteWagonDate { get; set; }

        /// <summary>Смещение груза по оси Х</summary>
        [XmlElement("diff_shift_x")]
       
        public int DiffShiftX { get; set; }

        /// <summary>Смещение груза по оси Y</summary>
        [XmlElement("diff_shift_y")]
       
        public int DiffShiftY { get; set; }

        /// <summary>Смещение груза по оси Z</summary>
        [XmlElement("diff_shift_z")]
       
        public int DiffShiftZ { get; set; }

        /// <summary>Изменение объёма груза</summary>
        [XmlElement("diff_volume")]
       
        public int DiffVolume { get; set; }

        /// <summary>Изменение ширины груза</summary>
        [XmlElement("diff_width")]
       
        public int DiffWidth { get; set; }

        /// <summary>Смещение высоты груза</summary>
        [XmlElement("diff_height")]
       
        public int DiffHeght { get; set; }

        /// <summary>Изменение длины груза</summary>
        [XmlElement("diff_length")]
       
        public int DiffLength { get; set; }

        /// <summary>Изменение максимаьлной ширины груза</summary>
        [XmlElement("diff_width_max")]
       
        public int DiffWidthMax { get; set; }

        /// <summary>Изменнение максимальной высоты груза</summary>
        [XmlElement("diff_height_max")]
       
        public int DiffHeightMax { get; set; }

        /// <summary>Изменение флага наличия груза</summary>
        [XmlElement("diff_cargo_exist")]
       
        public bool DiffCargoExists { get; set; }

        /// <summary>Изменение флага очистки вагона</summary>
        [XmlElement("diff_cargo_clear")]
       
        public bool DiffCargoClear { get; set; }

        /// <summary>Флаг использования приведённого сравнения</summary>
        [XmlElement("is_relation_model")]
       
        public bool IsRelationModel { get; set; }

        /// <summary>Изменение скорости движения вагона</summary>
        [XmlElement("diff_speed")]
       
        public float DiffSpeed { get; set; }

        /// <summary>Флаг валидности, полученных данных сравнения груза. Результат поиска и расчёта 
        /// Значения: 0 - найден и расчитан, осальное либо расчёт не проводился, либо ошибки при определении соответствия типов</summary>
        //Правильное название RemoteSearchState
        [XmlElement("valid")]
       
        public int RemoteValid { get; set; }

        /// <summary>Дата выполнения сравнения груза, если не совпадает с RemoteWagonDate, значит сравнение ваполнялось повторно</summary>
        [XmlElement("compare_date")]
       
        public DateTime CompareDate { get; set; }

        /// <summary>Идентификатор оператора, инициировавшего сравнение</summary>
        [XmlElement("operator_id")]
       
        public int OperatorID { get; set; }

        #endregion
        
        #region Данные АСКИН

        /// <summary>Инвентарный номер вагона</summary>
        [XmlElement("ocr_inv")]
       
        public string Ocr_InvNumber
        {
            get { return _OcrInv; }
            set { _OcrInv = value; }
        }

        /// <summary>Тип достоверности номера</summary>
        [XmlElement("ocr_type")]
       
        public int Ocr_InvType
        {
            get { return _OcrType; }
            set { _OcrType = value; }
        }

        /// <summary>Тип достоверности номера</summary>
        [XmlElement("ocr_accuracy")]
       
        public int Ocr_Accuracy
        {
            get { return _OcrAccuracy; }
            set { _OcrAccuracy = value; }
        }

        /// <summary>Проверка оператором</summary>
        [XmlElement("ocr_check")]
       
        public int Ocr_UserCheck
        {
            get { return _OcrCheck; }
            set { _OcrCheck = value; }
        }

        /// <summary>Идентификатор поезда в системе АСКИН</summary>
        [XmlElement("ocr_train_Id")]
       
        public int Ocr_TrainId
        {
            get { return _OcrAskinTrainID; }
            set { _OcrAskinTrainID = value; }
        }

        #endregion

        #region Расширенные негабариты

        /// <summary>Маска зонального негабарита</summary>
        [XmlElement("ngb_zonal")]
       
        public int NgbZonal { get; set; }

        /// <summary>Маска основного габарита погрузки</summary>
        [XmlElement("ngb_main")]
       
        public int NgbBase { get; set; }

        /// <summary>Маска льготного габарита погрузки</summary>
        [XmlElement("ngb_soft")]
       
        public int NgbSoft { get; set; }

        /// <summary>Маска 1 степени негабаритности груза</summary>
        [XmlElement("ngb_grade1")]
       
        public int NgbGrade1 { get; set; }

        /// <summary>Маска 2 степени негабаритности груза</summary>
        [XmlElement("ngb_grade2")]
       
        public int NgbGrade2 { get; set; }

        /// <summary>Маска 3 степени негабаритности груза</summary>
        [XmlElement("ngb_grade3")]
       
        public int NgbGrade3 { get; set; }

        /// <summary>Маска 4 степени негабаритности груза</summary>
        [XmlElement("ngb_grade4")]
       
        public int NgbGrade4 { get; set; }

        /// <summary>Маска 5 степени негабаритности груза</summary>
        [XmlElement("ngb_grade5")]
       
        public int NgbGrade5 { get; set; }

        /// <summary>Маска 6 степени негабаритности груза</summary>
        [XmlElement("ngb_grade6")]
       
        public int NgbGrade6 { get; set; }
        
        /// <summary>Маска сверхнегабаритности груза</summary>
        [XmlElement("ngb_gradeEx")]
       
        public int NgbGradeEx { get; set; }
        
        /// <summary>Маска статического габарита Т</summary>
        [XmlElement("ngb_static_T")]
       
        public int NgbStatic_T { get; set; }
        
        /// <summary>Маска статического габарита Тпр</summary>
        [XmlElement("ngb_static_Tpr")]
       
        public int NgbStatic_Tpr { get; set; }

        /// <summary>Маска статического габарита 1-Т</summary>
        [XmlElement("ngb_static_1T")]
       
        public int NgbStatic_1T { get; set; }

        /// <summary>Маска статического габарита Тц</summary>
        [XmlElement("ngb_static_Tc")]
       
        public int NgbStatic_Tc { get; set; }
        
        /// <summary>Маска статического габарита 1-ВМ</summary>
        [XmlElement("ngb_static_1VM")]
       
        public int NgbStatic_1VM { get; set; }
        
        /// <summary>Маска статического габарита 0-ВМ</summary>
        [XmlElement("ngb_static_0VM")]
       
        public int NgbStatic_0VM { get; set; }

        /// <summary>Маска статического габарита 02-ВМ</summary>
        [XmlElement("ngb_static_2VM")]
       
        public int NgbStatic_02VM { get; set; }
        
        /// <summary>Маска статического габарита 03-ВМ</summary>
        [XmlElement("ngb_static_3VM")]
       
        public int NgbStatic_03VM { get; set; }

        /// <summary>Маска габарита приближения строений</summary>
        [XmlElement("ngb_static_build")]
       
        public int NgbBuild { get; set; }

        #endregion

        #region Модели вагонов

        /// <summary>Код вагона габарита подвижного состава (в натурном листе)</summary>
        [XmlElement("ngb_code")]
       
        public int NgbCode { get; set; }

        /// <summary>Модель вагона (в натурном листе)</summary>
        [XmlElement("model")]
       
        public string Model { get; set; }

        #endregion


        /// <summary>Пустой конструктор</summary>
        public WagonData() {
            RemoteValid = -1;
        }

        /// <summary>Конструктор копирования</summary>
        /// <param name="other">Объект, с которого производиться копирование</param>
        /// <exception cref="System.ArgumentNullException">Не задан объект для копирования</exception>
        public WagonData(WagonData other):this()
        {
            if (other == null)
            {
                throw new ArgumentNullException();
            }
            _WagId = other._WagId;
            _TrainId = other._TrainId;
            _Sn = other._Sn;
            _InvNumber = other._InvNumber;
            _InvNumberByNL = other._InvNumberByNL;
            _TimeSpanEnd = other._TimeSpanEnd;
            _TimeSpanBegin = other._TimeSpanBegin;
            _SpeedEnd = other._SpeedEnd;
            _DirectionEnd = other._DirectionEnd;
            _Ngb = other._Ngb;
            _Mark = other._Mark;
            _Banned = other._Banned;
            _Comment = other._Comment;
            _TimeChanged = other.TimeChanged;
            _Time = other._Time;
        }

        /// <summary>Конструктор</summary>
        /// <param name="wagon_id">Идентификатор записи в базе</param>
        /// <param name="train_id">Идентификатор поезда/ отцепа в СБВ УВГ</param>
        /// <param name="wagon_sn">Последовательный номер вагона</param>
        public WagonData(int wagon_id, int train_id, int sn):this()
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
            if (other == null) return false;
            return
                (_WagId == other._WagId) &&
                (_TrainId == other._TrainId) &&
                (_Sn == other._Sn) &&
                (_Loco == other._Loco) &&
                (_InvNumber == other._InvNumber) &&
                (_InvNumberByNL == other._InvNumberByNL) &&
                (_TimeSpanEnd == other._TimeSpanEnd) &&
                (_TimeSpanBegin == other._TimeSpanBegin) &&
                (_SpeedEnd == other._SpeedEnd) &&
                (_DirectionEnd == other._DirectionEnd) &&
                (_Ngb == other._Ngb) &&
                (_Mark == other._Mark) &&
                (_Banned == other._Banned) &&
                (_Comment == other._Comment) &&
                (_TimeChanged == other._TimeChanged) &&
                (_Time == other._Time);
        }

        #endregion

    }
}
