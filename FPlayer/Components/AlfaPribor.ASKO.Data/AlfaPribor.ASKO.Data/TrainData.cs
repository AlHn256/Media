using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace AlfaPribor.ASKO.Data
{

    /// <summary>Элемент списка поездов</summary>
    public class TrainData
    {

        #region Variables

        /// <summary>Идентификатор поезда в базе данных</summary>
        int id = 0;
        /// <summary>Дата время начала записи</summary>
        DateTime begin_time = DateTime.MinValue;
        /// <summary>Дата время окончания записи</summary>
        DateTime end_time = DateTime.MinValue;
        /// <summary>Направление (0 - прямое, 1 - обратное)</summary>
        int direction = 0;
        /// <summary>Идентификатор каталога видеозаписи</summary>
        int dir_id = 0;
        /// <summary>Путь каталога видеозаписи</summary>
        string dir_path = string.Empty;
        /// <summary>Число вагонов (общее)</summary>
        int wagons_count = 0;
        /// <summary>Число локомотивов</summary>
        int loco_count = 0;
        /// <summary>Номер поезда</summary>
        string train_num;
        /// <summary>Индекс поезда</summary>
        string train_index;
        /// <summary>Статус поезда</summary>
        int status;
        /// <summary>Скорость поезда</summary>
        int speed;
        /// <summary>Путь</summary>
        string way;
        /// <summary>Идентификатор пути осмотра</summary>
        int way_id = 0;
        /// <summary>Идентификатор оператора</summary>
        int op_id;
        /// <summary>Имя оператора</summary>
        string op_name;
        /// <summary>Идентификатор оператора, обработавшего состав</summary>
        int accepted;
        /// <summary>Число вагонов с негабаритами</summary>
        int wagons_ngb_count = 0;
        /// <summary>Общее число вагонов коммерческих неисправностей</summary>
        int total_ngb_count = 0;
        /// <summary>Наличие видео</summary>
        bool has_video;
        /// <summary>Инвертировать инвентарные номера из натурного листа</summary>
        bool invert_inv_num;
        /// <summary>Комментарий</summary>
        string comment;
        /// <summary>сдвиг начала записи относительно абсолютного времени</summary>
        int record_offset = 0;
        /// <summary>Наличие натурного листа</summary>
        bool is_numbers = false;
        /// <summary>Наличие моделей вагонов</summary>
        bool is_models = false;
        /// <summary>Наличие натурного листа</summary>
        bool is_ocr = false;

        #endregion

        /// <summary>Идентификатор поезда</summary>
        [XmlElement("id")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>Строка дата и время начала записи</summary>
        [XmlElement("begin")]
        public string RecordBegin
        {
            get
            {
                string result = string.Empty;
                if (begin_time != DateTime.MinValue)
                {
                    try
                    {
                        result = begin_time.ToString(GetDateTimeConvFormat());
                    }
                    catch (Exception) { }
                }
                return result;
            }
            set
            {
                begin_time = DateTime.MinValue;
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        begin_time = Convert.ToDateTime(value);
                    }
                    catch { }
                }
            }
        }

        /// <summary>Строка дата и время окончания записи</summary>
        [XmlElement("end")]
        public string RecordEnd
        {
            get
            {
                string result = string.Empty;
                if (end_time != DateTime.MinValue)
                {
                    try
                    {
                        result = end_time.ToString(GetDateTimeConvFormat());
                    }
                    catch (Exception) { }
                }
                return result;
            }
            set
            {
                end_time = DateTime.MinValue;
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        end_time = Convert.ToDateTime(value);
                    }
                    catch { }
                }
            }
        }

        /// <summary>Направление (0|1)</summary>
        [XmlElement("direction")]
        public int Direction
        {
            get { return direction; }
            set 
            { 
                direction = value != 0 ? 1 : 0; 
            }
        }

        /// <summary>Идентификатор каталога видеоархива</summary>
        [XmlIgnore]
        public int DirId
        {
            get { return dir_id; }
            set { dir_id = value; }
        }

        /// <summary>Номер поезда</summary>
       
        [XmlElement("trainnum")]
        public string TrainNum
        {
            get { return train_num; }
            set { train_num = value; }
        }

        /// <summary>Индекс поезда</summary>
        [XmlElement("trainindex")]
        public string TrainIndex
        {
            get { return train_index; }
            set { train_index = value; }
        }

        /// <summary>Путь</summary>
        [XmlElement("way")]
        public string Way
        {
            get { return way; }
            set { way  = value; }
        }

        /// <summary>Идентификатор пути осмотра</summary>
        [XmlElement("way_id")]
        public int WayId
        {
            get { return way_id; }
            set { way_id = value; }
        }

        /// <summary>Статус обработки записи поезда</summary>
        [XmlElement("status")]
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        
        /// <summary>Путь каталога видеоархива</summary>
        [XmlIgnore]
        public string DirPath
        {
            get { return dir_path; }
            set { dir_path = value; }
        }

        /// <summary>Дата и время начала записи</summary>
        [XmlElement("start")]
        public DateTime BeginTime
        {
            get { return begin_time; }
            set 
            {
                // Округляем до миллисекунд
                // Ограничение связано с форматом хранения значения в базе данных
                begin_time = new DateTime(value.Year, value.Month, value.Day, value.Hour, 
                                          value.Minute, value.Second, value.Millisecond);
            }
        }

        /// <summary>Дата и время окончания записи</summary>
        [XmlElement("stop")]
        public DateTime EndTime
        {
            get { return end_time; }
            set 
            {
                // Округляем до миллисекунд
                // Ограничение связано с форматом хранения значения в базе данных
                end_time = new DateTime(value.Year, value.Month, value.Day, value.Hour, 
                                        value.Minute, value.Second, value.Millisecond);
            }
        }

        /// <summary>Количество вагонов(общее)</summary>
        [XmlElement("wagons_count")]
        public int WagonsCount
        {
            get { return wagons_count; }
            set { wagons_count = value; }
        }

        /// <summary>Количество вагонов без локомотивов</summary>
        [XmlElement("pure_wagons_count")]
        public int PureWagonsCount
        {
            get { return wagons_count - loco_count; }
            set { loco_count = wagons_count - value; }
        }

        /// <summary>Количество локомотивов</summary>
        [XmlElement("loco_count")]
        public int LocoCount
        {
            get { return loco_count; }
            set { loco_count = value; }
        }

        /// <summary>Количество вагонов с негабаритами</summary>
        [XmlElement("wagong_ngb_count")]
        public int WagonsNgbCount
        {
            get { return wagons_ngb_count; }
            set { wagons_ngb_count = value; }
        }

        /// <summary>Общее число негабаритов</summary>
        [XmlElement("total_ngb_count")] 
        public int TotalNgbCount
        {
            get { return total_ngb_count; }
            set { total_ngb_count = value; }
        }

        /// <summary>Скорость</summary>
        [XmlElement("speed")]
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        /// <summary>Идентификатор оператора</summary>
        [XmlElement("op_id")]
        public int OpId 
        {
            get { return op_id; }
            set { op_id = value; }
        }

        /// <summary>Имя оператора</summary>
        [XmlElement("op_name")]
        public string OpName
        {
            get { return op_name; }
            set { op_name = value; }
        }

        /// <summary>Признак "Принят оператором"</summary>
        [XmlElement("accepted")]
        public int Accepted        
        {
            get { return accepted; }
            set { accepted = value; }
        }

        /// <summary>Признак наличия видео</summary>
        [XmlElement("has_video")]
        public bool HasVideo
        {
            get { return has_video; }
            set { has_video = value; }
        }

        /// <summary>Инвертировать инвентарные номера из натурного листа</summary>
        [XmlElement("invert_inv_num")]
        public bool InvertInvNum
        {
            get { return invert_inv_num; }
            set { invert_inv_num = value; }
        }
        
        /// <summary>Комментарий</summary>
        [XmlElement("comment")]
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        
        /// <summary>Отставание видеозаписи</summary>
        [XmlElement("record_offset")]
        public int RecordOffset
        {
            get { return record_offset; }
            set { record_offset = value; }
        }

        /// <summary>Наличие натурного листа</summary>
        [XmlElement("is_numbers")]
        public bool IsNumbers
        {
            get { return is_numbers; }
            set { is_numbers = value; }
        }

        /// <summary>Наличие моделей вагонов</summary>
        [XmlElement("is_models")]
        public bool IsModels
        {
            get { return is_models; }
            set { is_models = value; }
        }

        /// <summary>Наличие распознанных номеров из системы АСКИН</summary>
       
        [XmlElement("is_ocr")]
        public bool IsOcr
        {
            get { return is_ocr; }
            set { is_ocr = value; }
        }

        /// <summary>Пустой конструктор</summary>
        public TrainData() { }

        /// <summary>Конструктор с необходимыми параметрами</summary>
        /// <param name="id">Идентификатор поезда</param>
        /// <param name="begin">Дата и время начала записи</param>
        /// <param name="end">Дата и время окончания записи</param>
        /// <param name="speed">Скорость (км/ч)</param>
        /// <param name="direction">Направление</param>
        /// <param name="dir_id">Идентификатор каталога записи</param>
        public TrainData(int id, DateTime begin, DateTime end, int dir_id)
        {
            this.id = id;
            begin_time = begin;
            end_time = end;
            this.dir_id = dir_id;
        }

        /// <summary>Предоставляет формат представления даты/времени в виде строки</summary>
        /// <returns>Формат представления даты/времени в виде строки</returns>
        protected virtual string GetDateTimeConvFormat()
        {
            return "dd.MM.yyyy HH:mm";
        }

    }

}
