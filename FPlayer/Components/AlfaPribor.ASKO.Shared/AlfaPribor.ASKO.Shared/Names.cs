using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AlfaPribor.ASKO.Shared
{

    /// <summary>Класс "Наименования устройств, датчиков и т.д."</summary>
    public static class Names
    {

        /// <summary>Получить название уровня доступа оператора </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public string GetUserLevelName(UserLevel level)
        {
            switch (level)
            {
                case UserLevel.Administrator: return "Администратор";
                case UserLevel.PowerUser: return "Опытный пользователь";
                default: return "Пользователь";
            }
        }

        /// <summary>Получить наименование датчика негабарита</summary>
        /// <param name="num">Номер датчика (0...12)</param>
        /// <returns>Наименование датчика негабарита</returns>
        public static string GetNgbSensorName(int num)
        {
            string result = string.Empty;
            if (num == 0)
            {
                result = "Левый нижний, боковой негабарит погрузки";
            }
            else if (num == 1)
            {
                result = "Левая верхняя 1–я негабаритность";
            }
            else if (num == 2)
            {
                result = "Левая верхняя 2–я негабаритность";
            }
            else if (num == 3)
            {
                result = "Левая верхняя 3–я негабаритность";
            }
            else if (num == 4)
            {
                result = "Вертикальная негабаритность";
            }
            else if (num == 5)
            {
                result = "Правая верхняя 4–я негабаритность";
            }
            else if (num == 6)
            {
                result = "Правая верхняя 5–я негабаритность";
            }
            else if (num == 7)
            {
                result = "Правая верхняя 6–я негабаритность";
            }
            else if (num == 8)
            {
                result = "Правый нижний, боковой негабарит погрузки";
            }
            else if (num == 9)
            {
                result = "Левый негабарит подвижного состава";
            }
            else if (num == 10)
            {
                result = "Правый негабарит подвижного состава";
            }
            else if (num == 11)
            {
                result = "Левый основной негабарит";
            }
            else if (num == 12)
            {
                result = "Правый основной негабарит";
            }
            return result;
        }

        /// <summary>Получить наименование датчика негабарита с обозначением по проекту (БИ-БФ)</summary>
        /// <param name="num">Номер датчика (0...12)</param>
        /// <returns>Наименование датчика негабарита</returns>
        public static string GetNgbSensorNameEx(int num)
        {
            string result = string.Empty;
            if (num == 0)
            {
                result = "Левый нижний, боковой негабарит погрузки, БИ-БФ 9";
            }
            else if (num == 1)
            {
                result = "Левая верхняя 1–я негабаритность, БИ-БФ 10";
            }
            else if (num == 2)
            {
                result = "Левая верхняя 2–я негабаритность, БИ-БФ 11";
            }
            else if (num == 3)
            {
                result = "Левая верхняя 3–я негабаритность, БИ-БФ 1";
            }
            else if (num == 4)
            {
                result = "Вертикальная негабаритность, БИ-БФ 2";
            }
            else if (num == 5)
            {
                result = "Правая верхняя 4–я негабаритность, БИ-БФ 3";
            }
            else if (num == 6)
            {
                result = "Правая верхняя 5–я негабаритность, БИ-БФ 4";
            }
            else if (num == 7)
            {
                result = "Правая верхняя 6–я негабаритность, БИ-БФ 7";
            }
            else if (num == 8)
            {
                result = "Правый нижний, боковой негабарит погрузки, БИ-БФ 8";
            }
            else if (num == 9)
            {
                result = "Левый негабарит подвижного состава, БИ-БФ 13";
            }
            else if (num == 10)
            {
                result = "Правый негабарит подвижного состава, БИ-БФ 14";
            }
            else if (num == 11)
            {
                result = "Левый основной негабарит, БИ-БФ 15";
            }
            else if (num == 12)
            {
                result = "Правый основной негабарит, БИ-БФ 16";
            }
            return result;
        }

        /// <summary>Получить наименование датчика подключенного к БИС</summary>
        /// <param name="num">Номер датчика (0...31)</param>
        /// <returns>Наименование датчика</returns>
        public static string GetBisSensorName(int num)
        {
            string result = string.Empty;
            if (num >= 0 && num <= 12)
            {
                result = GetNgbSensorName(num);
            }
            else if (num == 26)
            {
                result = "Реле КН";
            }
            else if (num == 27)
            {
                result = "Вскрытие шкафа";
            }
            else if (num == 28)
            {
                result = "Датчик начала состава";
            }
            else if (num == 29)
            {
                result = "Датчик начала вагона";
            }
            else if (num == 30)
            {
                result = "Датчик счета колес";
            }
            else if (num == 31)
            {
                result = "Датчик скорости";
            }
            return result;
        }

        /// <summary>Получить наименование датчика подключенного к БИС с обозначением по проекту (БИ-БФ)</summary>
        /// <param name="num">Номер датчика (0...31)</param>
        /// <returns>Наименование датчика</returns>
        public static string GetBisSensorNameEx(int num)
        {
            string result = string.Empty;
            if (num >= 0 && num <= 12)
            {
                result = GetNgbSensorName(num);
            }
            else if (num == 26)
            {
                result = "Реле КН";
            }
            else if (num == 27)
            {
                result = "Вскрытие шкафа";
            }
            else if (num == 28)
            {
                result = "Датчик начала состава, БИ-БФ 12";
            }
            else if (num == 29)
            {
                result = "Датчик начала вагона, БИ-БФ 5";
            }
            else if (num == 30)
            {
                result = "Датчик счета колес, БИ-БФ 6";
            }
            else if (num == 31)
            {
                result = "Датчик скорости, БИ-БФ 17";
            }
            return result;
        }

        /// <summary>Получить наименование датчика подключенного к БС.32</summary>
        /// <param name="num">Номер датчика (0...31)</param>
        /// <returns>Наименование датчика</returns>
        public static string GetBs32SensorName(int num)
        {
            string result = string.Empty;
            if (num >= 0 && num <= 12)
            {
                result = GetNgbSensorName(num);
            }
            else if (num == 24)
            {
                result = "Неисправность педали 1";
            }
            else if (num == 25)
            {
                result = "Неисправность педали 2";
            }
            else if (num == 26)
            {
                result = "Реле КН";
            }
            else if (num == 27)
            {
                result = "Вскрытие шкафа";
            }
            else if (num == 28)
            {
                result = "Педаль 1, сенсор 1";
            }
            else if (num == 29)
            {
                result = "Педаль 1, сенсор 2";
            }
            else if (num == 30)
            {
                result = "Педаль 2, сенсор 1";
            }
            else if (num == 31)
            {
                result = "Педаль 2, сенсор 2";
            }
            return result;
        }

        /// <summary>Получить наименование датчика подключенного к БС.32 с обозначением по проекту (БИ-БФ)</summary>
        /// <param name="num">Номер датчика (0...31)</param>
        /// <returns>Наименование датчика</returns>
        public static string GetBs32SensorNameEx(int num)
        {
            string result = string.Empty;
            if (num >= 0 && num <= 12)
            {
                result = GetNgbSensorNameEx(num);
            }
            else if (num == 24)
            {
                result = "Неисправность педали 1";
            }
            else if (num == 25)
            {
                result = "Неисправность педали 2";
            }
            else if (num == 26)
            {
                result = "Реле КН";
            }
            else if (num == 27)
            {
                result = "Вскрытие шкафа";
            }
            else if (num == 28)
            {
                result = "Педаль 1, сенсор 1";
            }
            else if (num == 29)
            {
                result = "Педаль 1, сенсор 2";
            }
            else if (num == 30)
            {
                result = "Педаль 2, сенсор 1";
            }
            else if (num == 31)
            {
                result = "Педаль 2, сенсор 2";
            }
            return result;
        }

        /// <summary>Получить наименование статуса датчика (входа БИС/БС.32)</summary>
        /// <param name="sensor_num">Номер датчика (0...31)</param>
        /// <param name="stat">Статус датчика</param>
        /// <returns>Наименование статуса</returns>
        public static string GetSensorStatName(int sensor_num, SensorStat stat)
        {
            string result = string.Empty;
            if (sensor_num < 0) return result;
            if (sensor_num > 31) return result;
            switch (stat)
            {
                case SensorStat.Unknown:
                    result = "Неизвестно";
                    break;
                case SensorStat.Fault:
                    if ((sensor_num >= 0 && sensor_num <= 12) || (sensor_num >= 28 && sensor_num <= 31))
                    {
                        // Датчики негабарита и счетные датчики
                        result = "Неисправен";
                    }
                    else if (sensor_num == 27)
                    {
                        // Датчик вскрытия шкафа
                        result = "Вскрыт";
                    }
                    else if (sensor_num == 26)
                    {
                        // Датчик состояния реле КН
                        result = "Сбой питания";
                    }
                    else
                    {
                        result = "Неисправен";
                    }
                    break;
                case SensorStat.Alarm:
                    if ((sensor_num >= 0 && sensor_num <= 12) || (sensor_num >= 28 && sensor_num <= 31))
                    {
                        // Датчики негабарита и счетные датчики
                        result = "Перекрыт";
                    }
                    else if (sensor_num == 27)
                    {
                        // Датчик вскрытия шкафа
                        result = "Вскрыт";
                    }
                    else if (sensor_num == 26)
                    {
                        // Датчик состояния реле КН
                        result = "Сбой питания";
                    }
                    else if (sensor_num == 25 || sensor_num == 24)
                    {
                        // Неисправность датчиков педалей
                        result = "Неисправен";
                    }
                    else
                    {
                        result = "Тревога";
                    }
                    break;
                case SensorStat.Secure:
                    if ((sensor_num >= 0 && sensor_num <= 12) || (sensor_num >= 28 && sensor_num <= 31))
                    {
                        // Датчики негабарита и счетные датчики
                        result = "Не перекрыт";
                    }
                    else if (sensor_num == 27)
                    {
                        // Датчик вскрытия шкафа
                        result = "Закрыт";
                    }
                    else if (sensor_num == 26)
                    {
                        // Датчик состояния реле КН
                        result = "Норма питания";
                    }
                    else if (sensor_num == 25 || sensor_num == 24)
                    {
                        // Неисправность датчиков педалей
                        result = "Исправен";
                    }
                    else
                    {
                        result = "Норма";
                    }
                    break;
            }
            return result;
        }

        /// <summary>Получить наименование реле БИС/БС.32</summary>
        /// <param name="num">Номер реле (1...8)</param>
        /// <returns>Наименование реле</returns>
        public static string GetRelayName(int num)
        {
            string result = string.Empty;
            if (num == 1) result = "Сирена";
            else if (num == 2) result = "Освещение";
            else if (num == 3) result = "Питание камеры 1";
            else if (num == 4) result = "Питание камеры 2";
            else if (num == 5) result = "Питание камеры 3";
            else if (num == 6) result = "Питание камеры 4";
            return result;
        }

        /// <summary>Получить наименование статус реле БИС/БС.32</summary>
        /// <param name="num">Номер реле (1...8)</param>
        /// <param name="on">Включено/выключено</param>
        /// <returns>Наименование статуса реле</returns>
        public static string GetRelayStatName(int num, bool on)
        {
            string result = string.Empty;
            if (num == 1) result = on ? "Включена" : "Выключена";
            else if (num == 2) result = on ? "Включено" : "Выключено";
            else if (num == 3 || num == 4 || num == 5 || num == 6)
            {
                result = on ? "Выключено" : "Включено";
            }
            return result;
        }

        /// <summary>Получить наименование датчика БИС/БС</summary>
        /// <param name="id">Идентификатор датчика</param>
        /// <returns>Наименование датчика</returns>
        public static string GetBisSensorName(BisSensorId id)
        {
            switch (id)
            {
                case BisSensorId.Ngb1: return "Левый нижний, боковой негабарит погрузки";
                case BisSensorId.Ngb2: return "Левая верхняя 1–я негабаритность";
                case BisSensorId.Ngb3: return "Левая верхняя 2–я негабаритность";
                case BisSensorId.Ngb4: return "Левая верхняя 3–я негабаритность";
                case BisSensorId.Ngb5: return "Вертикальная негабаритность";
                case BisSensorId.Ngb6: return "Правая верхняя 4–я негабаритность";
                case BisSensorId.Ngb7: return "Правая верхняя 5–я негабаритность";
                case BisSensorId.Ngb8: return "Правая верхняя 6–я негабаритность";
                case BisSensorId.Ngb9: return "Правый нижний, боковой негабарит погрузки";
                case BisSensorId.Ngb10: return "Левый негабарит подвижного состава";
                case BisSensorId.Ngb11: return "Правый негабарит подвижного состава";
                case BisSensorId.Ngb12: return "Левый основной негабарит";
                case BisSensorId.Ngb13: return "Правый основной негабарит";
                case BisSensorId.PowerShock: return "Реле КН";
                case BisSensorId.Tamper: return "Вскрытие шкафа";
                case BisSensorId.DNS: return "Датчик начала состава";
                case BisSensorId.DSW: return "Датчик начала вагона";
                case BisSensorId.DSK: return "Датчик счета колес";
                case BisSensorId.DOS: return "Датчик скорости";
                case BisSensorId.DP1: return "Педаль 1, сенсор 1";
                case BisSensorId.DP2: return "Педаль 1, сенсор 2";
                case BisSensorId.DP3: return "Педаль 2, сенсор 1";
                case BisSensorId.DP4: return "Педаль 2, сенсор 2";
            }
            return string.Empty;
        }

        /// <summary>Получить расширенное наименование датчика БИС/БС (с обозначениями по проекту)</summary>
        /// <param name="id">Идентификатор датчика</param>
        /// <returns></returns>
        public static string GetBisSensorNameEx(BisSensorId id)
        {
            switch (id)
            {
                case BisSensorId.Ngb1: return "Левый нижний, боковой негабарит погрузки, БИ-БФ 9";
                case BisSensorId.Ngb2: return "Левая верхняя 1–я негабаритность, БИ-БФ 10";
                case BisSensorId.Ngb3: return "Левая верхняя 2–я негабаритность, БИ-БФ 11";
                case BisSensorId.Ngb4: return "Левая верхняя 3–я негабаритность, БИ-БФ 1";
                case BisSensorId.Ngb5: return "Вертикальная негабаритность, БИ-БФ 2";
                case BisSensorId.Ngb6: return "Правая верхняя 4–я негабаритность, БИ-БФ 3";
                case BisSensorId.Ngb7: return "Правая верхняя 5–я негабаритность, БИ-БФ 4";
                case BisSensorId.Ngb8: return "Правая верхняя 6–я негабаритность, БИ-БФ 7";
                case BisSensorId.Ngb9: return "Правый нижний, боковой негабарит погрузки, БИ-БФ 8";
                case BisSensorId.Ngb10: return "Левый негабарит подвижного состава, БИ-БФ 13";
                case BisSensorId.Ngb11: return "Правый негабарит подвижного состава, БИ-БФ 14";
                case BisSensorId.Ngb12: return "Левый основной негабарит, БИ-БФ 15";
                case BisSensorId.Ngb13: return "Правый основной негабарит, БИ-БФ 16";
                case BisSensorId.PowerShock: return "Реле КН";
                case BisSensorId.Tamper: return "Вскрытие шкафа";
                case BisSensorId.DNS: return "Датчик начала состава, БИ-БФ 12";
                case BisSensorId.DSW: return "Датчик начала вагона, БИ-БФ 5";
                case BisSensorId.DSK: return "Датчик счета колес, БИ-БФ 6";
                case BisSensorId.DOS: return "Датчик скорости, БИ-БФ 17";
                case BisSensorId.DP1: return "Педаль 1, сенсор 1";
                case BisSensorId.DP2: return "Педаль 1, сенсор 2";
                case BisSensorId.DP3: return "Педаль 2, сенсор 1";
                case BisSensorId.DP4: return "Педаль 2, сенсор 2";
            }
            return string.Empty;
        }

        /// <summary>Получить наименование поставщика данных о поездах</summary>
        /// <param name="contractor">Тип поставщика данных</param>
        /// <returns>Наименование поставщика</returns>
        public static string GetAskoContractorName(AskoContractor contractor)
        {
            switch (contractor)
            {
                case AskoContractor.Unknown: return "Неизвестно";
                case AskoContractor.ARMPKO: return "АРМ ПКО";
                case AskoContractor.EASAPR: return "ЕАСАПР";
                default: return string.Empty;
            }
        }

        /// <summary>Получить наименование хоста ВидеоИнспектор сервер для обмена данными</summary>
        /// <param name="base_host_name">Базовое имя хоста</param>
        /// <returns>Строка с наименование хоста ВидеоИнспектор</returns>
        public static string GetHostNameForCommands(string base_host_name)
        {
            return base_host_name;
        }

        /// <summary>Получить наименование хоста ВидеоИнспектор сервер для получения видеопотоков</summary>
        /// <param name="base_host_name">Базовое имя хоста</param>
        /// <returns>Строка с наименование хоста ВидеоИнспектор</returns>
        public static string GetHostNameForVideo(string base_host_name)
        {
            return base_host_name + "/video";
        }
    }

}
