using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace AlfaPribor.AviFile
{
    /// <summary>Класс, описывающий функции и структуры данных для работы с AVI файлами</summary>
    public class Avi
    {

        #region Constants

        #region Тип потока в AVI файле

        /// <summary>Видео данные</summary>
        public const int StreamtypeVIDEO = 0x73646976;

        /// <summary>Аудио данные</summary>
        public const int StreamtypeAUDIO = 0x73647561;

        /// <summary>Команды для музыкальных синтезаторов</summary>
        public const int StreamtypeMIDI = 0x7364696D;

        /// <summary>Текст</summary>
        public const int StreamtypeTEXT = 0x73747874;

        #endregion

        #region Уровень доступа к открытому AVI файлу

        /// <summary>Чтение</summary>
        public const int OF_READ = 0x00000000;

        /// <summary>Запись</summary>
        public const int OF_WRITE = 0x00000001;

        /// <summary>Чтение или запись</summary>
        public const int OF_READWRITE = 0x00000002;

        /// <summary>Эксклюзивный уровень доступа</summary>
        public const int OF_SHARE_EXCLUSIVE = 0x00000010;

        /// <summary>Запрет записи в файл другими процессами</summary>
        public const int OF_SHARE_DENY_WRITE = 0x00000020;

        /// <summary>Запрет чтения файла другими процессами</summary>
        public const int OF_SHARE_DENY_READ = 0x00000030;

        /// <summary>Нет ограничений для других процессов - могут открывать файл для чтения/записи</summary>
        public const int OF_SHARE_DENY_NONE = 0x00000040;

        /// <summary>Файл должен быть создан. Если файл уже существует - он будет перезаписан</summary>
        public const int OF_CREATE = 0x00001000;

        #endregion

        #region Формат изображения

        /// <summary>Несжатый формат изображения</summary>
        public const int BI_RGB = 0x00000000;

        /// <summary>Изображение, содержащее не более 256 цветов. Цвета содержатся в таблице индексов</summary>
        public const int BI_RLE8 = 0x00000001;

        /// <summary>Изображение, содержащее не более 128 цветов. Цвета содержатся в таблице индексов</summary>
        public const int BI_RLE4 = 0x00000002;

        /// <summary>Информация о цвете закодирована в 4-х байтах на каждый цветовой канал - красный, синий, зеленый</summary>
        public const int BI_BITFIELDS = 0x00000003;

        /// <summary>Изображение в формате Jpeg</summary>
        public const int BI_JPEG = 0x00000004;

        /// <summary>Изображение в формате Png</summary>
        public const int BI_PNG = 0x00000005;

        #endregion

        #region Флаги качества сжатых данных

        /// <summary>Самое низкое качество</summary>
        public const int ICQUALITY_LOW = 0;

        /// <summary>Самое высокое качество</summary>
        public const int ICQUALITY_HIGH = 10000;

        /// <summary>Качество по-умолчанию (на усмотрение компрессора данных)</summary>
        public const int ICQUALITY_DEFAULT = -1;

        #endregion

        /// <summary>Тип файла для хранения bitmap</summary>
        public const int BMP_MAGIC_COOKIE = 19778;

        #region Флаги для индексации кадров в потоке

        /// <summary>'LIST' - сэмпл</summary>
        public const int AVIIF_LIST = 0x00000001;

        /// <summary>Ключевой кадр (для видеопотока)</summary>
        public const int AVIIF_KEYFRAME = 0x00000010;

        /// <summary>Кадр не привязан к определенному времени (для видеопотока)</summary>
        public const int AVIIF_NOTIME = 0x00000100;

        /// <summary>Данные для использования кодеком</summary>
        public const int AVIIF_COMPUSE = 0x0FFF0000;

        /// <summary>Флаг говорит о том, что флаг ключевого кадра недействительный, хотя должен проверяться</summary>
        public const int AVIIF_FIXKEYFRAME = 0x00001000;    /* invented; used to say that */
                                                            /* the keyframe flag isn't a true flag */
                                                            /* but have to be verified */
        #endregion

        #region Флаги компрессора (AVICOMPRESSOPTIONS.dwFlags)

        /// <summary>Чередовать кадры потока через каждые AVICOMPRESSOPTIONS.dwInterleaveEvery кадров с первым потоком</summary>
        public const int AVICOMPRESSF_INTERLEAVE = 0x00000001;

        /// <summary>Сжимать видеопоток с учетом данных о скорости выходного видеопотока,
        /// заданной AVICOMPRESSOPTIONS.dwBytesPerSecond
        /// </summary>
        public const int AVICOMPRESSF_DATARATE = 0x00000002;

        /// <summary>Сохранять поток используя ключевые кадры, которые брать из основного потока через каждые
        /// AVICOMPRESSOPTIONS.dwKeyFrameEvery кадров.
        /// </summary>
        public const int AVICOMPRESSF_KEYFRAMES = 0x00000004;

        /// <summary>Определяет использование настроек по умолчанию.
        /// Если этот флаг не установлен, используются установки по умолчанию
        /// </summary>
        public const int AVICOMPRESSF_VALID = 0x00000008;

        #endregion

        #region Флаги компрессора (COMPVARS.dwFlags)

        /// <summary>Установка этого флага свидетельствует о том, что структура COMPVARS была заполнена "вручную" программистом</summary>
        public const int ICMF_COMPVARS_VALID = 0x00000001;

        #endregion

        #region Флаги компрессора (ICINFO.dwFlags)

        /// <summary>Поддерживается настройка качества сжатия</summary>
        public const uint VIDCF_QUALITY = 0x0001;

        /// <summary>Поддерживается сжатие по размеру кадра</summary>
        public const uint VIDCF_CRUNCH = 0x0002;

        /// <summary>Поддерживается внутрикадровое сжатие</summary>
        public const uint VIDCF_TEMPORAL = 0x0004;

        /// <summary>Драйвер запрашивает сжатие всех кадров сообщения ICM_COMPRESS_FRAMES_INFO</summary>
        public const uint VIDCF_COMPRESSFRAMES = 0x0008;

        /// <summary>Поддерживается отрисовка изображения</summary>
        public const uint VIDCF_DRAW = 0x0010;

        /// <summary>При сжатии текущего кадра не требуется изображение предыдущего</summary>
        public const uint VIDCF_FASTTEMPORALC = 0x0020;

        /// <summary>При декомпрессии текущего кадра не требуется изображение предыдущего </summary>
        public const uint VIDCF_FASTTEMPORALD = 0x0080;

        #endregion

        #region Флаги окна выбора настроек компрессора ICCompressorChoose.uiFlags

        /// <summary>Показать элементы, позволяющие редактировать частоту следования ключевых кадров</summary>
        public const uint ICMF_CHOOSE_KEYFRAME = 0x0001;	// show KeyFrame Every box

        /// <summary>Показать элементы, позволяющие редактировать скорость выходного потока</summary>
        public const uint ICMF_CHOOSE_DATARATE = 0x0002;

        /// <summary>Показывать окно предварительного просмотра результатов сжатия</summary>
        public const uint ICMF_CHOOSE_PREVIEW = 0x0004;

        /// <summary>Отображает в окне выбора компрессора все компрессоры, зарегестированные в системе.
        /// Если флаг не задан, отображаются только те из зарегестрированных компрессорав, которые
        /// работают с форматом данных, заданным параметром ICCompressorChoose.lpIn
        /// </summary>
        public const uint ICMF_CHOOSE_ALLCOMPRESSORS = 0x0008;

        #endregion

        #region Коды ошибок при работе с AVI-файлом

        /// <summary>Операция с AVI-файлом завершилась успешно</summary>
        public const uint AVIERR_OK = 0x00000000;

        /// <summary>Компрессия заданного типа данных не поддерживается</summary>
        public const uint AVIERR_UNSUPPORTED = 0x80044065; // MAKE_AVIERR(101)

        /// <summary>Файл не может быть прочитан - поврежден или имеет неизвестную структуру</summary>
        public const uint AVIERR_BADFORMAT = 0x80044066; // MAKE_AVIERR(102)

        /// <summary>Недостаточно свободной памяти для открытия файла</summary>
        public const uint AVIERR_MEMORY = 0x80044067; // MAKE_AVIERR(103)

        //public const uint AVIERR_INTERNAL = 0x80044068; // MAKE_AVIERR(104)

        //public const uint AVIERR_BADFLAGS = 0x80044069; // MAKE_AVIERR(105)

        //public const uint AVIERR_BADPARAM = 0x8004406A; // MAKE_AVIERR(106)

        //public const uint AVIERR_BADSIZE = 0x8004406B; // MAKE_AVIERR(107)

        //public const uint AVIERR_BADHANDLE = 0x8004406C; // MAKE_AVIERR(108)

        /// <summary>Файл не может быть прочитан из-за ошибки на диске</summary>
        public const uint AVIERR_FILEREAD = 0x8004406D; // MAKE_AVIERR(109)

        /// <summary>Файл не может быть записан из-за ошибки на диске</summary>
        public const uint AVIERR_FILEWRITE = 0x8004406E; // MAKE_AVIERR(110)

        /// <summary>Файл не может быть открыт из-за ошибки на диске</summary>
        public const uint AVIERR_FILEOPEN = 0x8004406F; // MAKE_AVIERR(111)

        //public const uint AVIERR_COMPRESSOR = 0x80044070; // MAKE_AVIERR(112)

        /// <summary>Подходящий компрессор данных не обнаружен</summary>
        public const uint AVIERR_NOCOMPRESSOR = 0x80044071; // MAKE_AVIERR(113)

        /// <summary>Невозможно записать в AVI-файл, т.к. файл открыт только для чтения</summary>
        public const uint AVIERR_READONLY = 0x80044072; // MAKE_AVIERR(114)

        /// <summary>AVI-файл не содержит потоков с заданным типом</summary>
        public const uint AVIERR_NODATA = 0x80044073; // MAKE_AVIERR(115)

        /// <summary>Размер буфера слишком мал</summary>
        public const uint AVIERR_BUFFERTOOSMALL = 0x80044074; // MAKE_AVIERR(116)

        //public const uint AVIERR_CANTCOMPRESS = 0x80044075; // MAKE_AVIERR(117)

        //public const uint AVIERR_USERABORT = 0x800440C6; // MAKE_AVIERR(198)

        //public const uint AVIERR_ERROR = 0x800440C7; // MAKE_AVIERR(199)

        #endregion

        #endregion

        #region Types

        /// <summary>Описывает размеры и цифрвой формат изображения</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPINFOHEADER
        {
            /// <summary>Размер самой структуры в байтах</summary>
            public UInt32 biSize;

            /// <summary>Ширина изображения в пикселах</summary>
            public Int32 biWidth;

            /// <summary>Высота изображения в пикселах</summary>
            public Int32 biHeight;

            /// <summary>Число плоскостей изображения. Должно быть равно 1</summary>
            public Int16 biPlanes;

            /// <summary>Число бит, необходимых для кодирования цветовой информации одного пиксела изображения</summary>
            public Int16 biBitCount;

            /// <summary>Для сжатого видео или видео в формате YUYV - FOURCC код,
            /// например FOURCC('Y','U','Y','V') = 0x56595559. Для несжатых форматов:
            /// <para>0x00000000 - RGB формат</para>
            /// <para>0x00000003 - RGB с цветовой маской</para>
            /// </summary>
            public UInt32 biCompression;

            /// <summary>Размер изображения в байтах. Для несжатого формата RGB - долно быть равным нулю</summary>
            public UInt32 biSizeImage;

            /// <summary>Разрешение изображение по горизонтали (пикселей на метр)</summary>
            public Int32 biXPelsPerMeter;

            /// <summary>Разрешение изображение по вертикали (пикселей на метр)</summary>
            public Int32 biYPelsPerMeter;

            /// <summary>Цисло цветов, присутствующих в изображении</summary>
            public UInt32 biClrUsed;

            /// <summary>Число цветов, необходимых для отрисовки изображения</summary>
            public UInt32 biClrImportant;
        }

        /// <summary>Определяет размерность и цветовую информацию графического изображения 
        /// (DIB - device-independent bitmap)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPINFO
        {
            /// <summary>Описывает размеры и цифрвой формат изображения</summary>
            public BITMAPINFOHEADER bmiHeader;

            /// <summary>Определяет цвета, используемые в изображении</summary>
            public uint bmiColors;
        }

        /// <summary>Содержит информацию об отдельном потоке в AVI файле</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct AVISTREAMINFO
        {
            /// <summary>Тип потока</summary>
            public UInt32 fccType;

            /// <summary>Код дескриптора компрессора (для сжатых данных)</summary>
            public UInt32 fccHandler;

            /// <summary>Флаги, специфичные для содержимого потока</summary>
            public UInt32 dwFlags;

            /// <summary>Не используется</summary>
            public UInt32 dwCaps;

            /// <summary>Приоритет потока</summary>
            public UInt16 wPriority;

            /// <summary>Язык содержимого</summary>
            public UInt16 wLanguage;

            /// <summary>Шкала времени. Позволяет вычислить необходимую скорость проигрывания потока</summary>
            public UInt32 dwScale;

            /// <summary>Частота, необходимая для воспроизведения содержимого</summary>
            public UInt32 dwRate;

            /// <summary>Номер первого кадра для воспроизведения</summary>
            public UInt32 dwStart;

            /// <summary>Длина потока</summary>
            public UInt32 dwLength;

            /// <summary>Отклонение аудио данных при воспроизведении видео</summary>
            public UInt32 dwInitialFrames;

            /// <summary>Рекомендуемый размер буфера при воспроизведении данных</summary>
            public UInt32 dwSuggestedBufferSize;

            /// <summary>Качество видеоданных</summary>
            public UInt32 dwQuality;

            /// <summary>Размер одного семпла в байтах. Если равен нулю - каждый семпл имеет разный размер</summary>
            public UInt32 dwSampleSize;

            /// <summary>Размер области для отображения видео</summary>
            public RECT rcFrame;

            /// <summary>Header of times the stream has been edited. The stream handler maintains this samples</summary>
            public UInt32 dwEditCount;

            /// <summary>Header of times the stream format has changed. The stream handler maintains this samples</summary>
            public UInt32 dwFormatChangeCount;

            /// <summary>Описание содержимого потока</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public char[] szName;
        }

        /// <summary>Описывает прямоугольную область</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RECT
        {
            /// <summary>Горизонтальная координата левого верхнего угла</summary>
            public UInt32 left;

            /// <summary>Вертикальная координата левого верхнего угла</summary>
            public UInt32 top;

            /// <summary>Горизонтальная координата правого нижнего угла</summary>
            public UInt32 right;

            /// <summary>Вертикальная координата правого нижнего угла</summary>
            public UInt32 bottom;
        }

        /// <summary>Заголовок bitmap-файла</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPFILEHEADER
        {
            /// <summary>Тип файла</summary>
            public Int16 bfType; //Должен быть "BM"

            /// <summary>Размер файла</summary>
            public Int32 bfSize;

            /// <summary>Не используется</summary>
            public Int16 bfReserved1;

            /// <summary>Не используется</summary>
            public Int16 bfReserved2;

            /// <summary>Смещение от начала струкруры до начала массива данными изображения (байт)</summary>
            public Int32 bfOffBits;
        }

        /// <summary>Содержит информацию о потоке и о том, как его сжимать (кодировать) и хранить</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct AVICOMPRESSOPTIONS
        {
            /// <summary>Тип потока в AVI-файле</summary>
            public UInt32 fccType;

            /// <summary>Дескриптор компрессора (кодека), которым будет сжиматься поток</summary>
            public UInt32 fccHandler;

            /// <summary>Максимальный промежуток между ключевыми видеокадрами. Используется только в случае, если
            /// установлен флаг AVICOMPRESSF_KEYFRAMES, в противном случае считается, что каждый видеокадр
            /// является ключевым
            /// </summary>
            public UInt32 dwKeyFrameEvery; 

            /// <summary>Качество сжатия потока (параметр не учитывается для аудиопотоков)</summary>
            public UInt32 dwQuality; 

            /// <summary>Скорость видеопотока.
            /// Учитывается только при установленном флаге AVICOMPRESSF_DATARATЕ
            /// </summary>
            public UInt32 dwBytesPerSecond; 

            /// <summary>Флаги дополнительной информации для компрессора</summary>
            public UInt32 dwFlags; 

            /// <summary>Указатель на структуру, определяющую формат данных</summary>
            public IntPtr lpFormat; 

            /// <summary>Размер структуры данных, на которую ссылается параметр lpFormat</summary>
            public UInt32 cbFormat; 

            /// <summary>Блок вспомогательной информации, используемый компрессором для внутреннего использования</summary>
            public UIntPtr lpParms;

            /// <summary>Размер блока данных, на который ссылается параметр lpParms</summary>
            public UInt32 cbParms;

            /// <summary>Фактор чередования данных с первым потоком (для чередующихся потоков).
            /// Используется только если установлен флаг AVICOMPRESSF_INTERLEAVE
            /// </summary>
            public UInt32 dwInterleaveEvery;

            /// <summary>Загружает данные из структуры COMPVARS</summary>
            /// <param name="comp_v">Описывает настройки компрессора видеоизображения</param>
            public void Load(COMPVARS comp_v)
            {
                fccHandler = comp_v.fccHandler;
                fccType = comp_v.fccType;
                if (comp_v.lKey != 0)
                {
                    dwKeyFrameEvery = (uint)comp_v.lKey;
                    dwFlags |= Avi.AVICOMPRESSF_KEYFRAMES;
                }
                else
                {
                    dwKeyFrameEvery = 0U;
                    dwFlags &= ~(uint)Avi.AVICOMPRESSF_KEYFRAMES;
                }
                dwQuality = (uint)comp_v.lQ;
                if (comp_v.lDataRate != 0)
                {
                    dwBytesPerSecond = (uint)comp_v.lDataRate * 1024U;
                    dwFlags |= Avi.AVICOMPRESSF_DATARATE;
                }
                else
                {
                    dwBytesPerSecond = 0U;
                    dwFlags &= ~(uint)Avi.AVICOMPRESSF_DATARATE;
                }
                dwFlags |= (uint)Avi.AVICOMPRESSF_VALID;
            }

            /// <summary>Сохраняет данные в структуру COMPVARS</summary>
            /// <param name="comp_v">Заполняемая структура</param>
            public void Save(ref COMPVARS comp_v)
            {
                fccHandler = comp_v.fccHandler;
                fccType = comp_v.fccType;
                if ((dwFlags & (uint)Avi.AVICOMPRESSF_KEYFRAMES) != 0)
                {
                    comp_v.lKey = (int)dwKeyFrameEvery;
                }
                else
                {
                    comp_v.lKey = 0;
                    dwFlags &= ~(uint)Avi.AVICOMPRESSF_KEYFRAMES;
                }
                comp_v.lQ = (int)dwQuality;
                if ((dwFlags & (uint)Avi.AVICOMPRESSF_DATARATE) != 0)
                {
                    comp_v.lDataRate = (int)(dwBytesPerSecond / 1024U);
                }
                else
                {
                    comp_v.lDataRate = 0;
                }
                comp_v.dwFlags = Avi.ICMF_COMPVARS_VALID;
            }

            /// <summary>Сбрасывает настройки сжатия в значения по умолчанию.
            /// В этом случае компрессор использует свои настройки.
            /// </summary>
            public void Default()
            {
                dwKeyFrameEvery = 0U;
                dwQuality = 0U;
                dwBytesPerSecond = 0U;
                dwFlags = 0U;
                lpFormat = IntPtr.Zero;
                cbFormat = 0U;
                lpParms = UIntPtr.Zero;
                cbParms = 0U;
                dwInterleaveEvery = 0U;
            }
        }

        /// <summary>Описывает настройки компрессора видеоизображения</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct COMPVARS
        {
            /// <summary>Размер самой стуктуры (области памяти, занимаемой структурой данных)</summary>
            public Int32 cbSize;

            /// <summary>Определяет, каким именно образом были заданы настройки - вручную или вызовом метода
            /// ICCompressorChoose. Если настройки задаются вручную, то флаг надо выставлять
            /// </summary>
            public UInt32 dwFlags;

            /// <summary>Указатель на интерфейс выбранного компрессора видеоданных</summary>
            public IntPtr hic;

            /// <summary>Тип используемого компрессора. На данный момент поддерживается только ICTYPE_VIDEO (VIDC).
            /// Должно быть равно нулю
            /// </summary>
            public UInt32 fccType;

            /// <summary>Код используемого компрессора</summary>
            public UInt32 fccHandler;

            /// <summary>Зарезервировано. Не используется</summary>
            public IntPtr lpbiIn;

            /// <summary>Указатель на структуру данных BITMAPINFO с информацией о результирующем видеоизображении</summary>
            public IntPtr lpbiOut;

            /// <summary>Зарезервировано. Не используется</summary>
            public IntPtr lpBitsOut;

            /// <summary>Зарезервировано. Не используется</summary>
            public IntPtr lpBitsPrev;

            /// <summary>Зарезервировано. Не используется</summary>
            public Int32 lFrame;

            /// <summary>Частота следования ключевых кадров или ноль, если ключевые кадры не используются</summary>
            public Int32 lKey;

            /// <summary>Скорость видеопотока (килобайт/сек)</summary>
            public Int32 lDataRate;

            /// <summary>Настройки качества. (1..10000 или  ICQUALITY_DEFAULT )</summary>
            public Int32 lQ;

            /// <summary>Зарезервировано. Не используется</summary>
            public Int32 lKeyCount;

            /// <summary>Зарезервировано. Не используется</summary>
            public IntPtr lpState;

            /// <summary>Зарезервировано. Не используется</summary>
            public Int32 cbState;

            /// <summary>Загружает данные из структуры AVICOMPRESSOPTIONS</summary>
            /// <param name="comp_opts">Содержит информацию о потоке и о том, как его сжимать (кодировать) и хранить</param>
            public void Load(AVICOMPRESSOPTIONS comp_opts)
            {
                fccHandler = comp_opts.fccHandler;
                fccType = comp_opts.fccType;
                if ((comp_opts.dwFlags & (uint)Avi.AVICOMPRESSF_KEYFRAMES) != 0)
                {
                    lKey = (int)comp_opts.dwKeyFrameEvery;
                }
                else
                {
                    lKey = 0;
                }
                lQ = (int)comp_opts.dwQuality;
                if ((comp_opts.dwFlags & Avi.AVICOMPRESSF_DATARATE) != 0)
                {
                    lDataRate = (int)(comp_opts.dwBytesPerSecond / 1024U);
                }
                else
                {
                    lDataRate = 0;
                }
                dwFlags = Avi.ICMF_COMPVARS_VALID;
            }

            /// <summary>Заполняет структуру данных AVICOMPRESSOPTIONS</summary>
            /// <param name="comp_opts">Заполняемая структура</param>
            public void Save(ref AVICOMPRESSOPTIONS comp_opts)
            {
                comp_opts.fccHandler = fccHandler;
                comp_opts.fccType = fccType;
                if (lKey != 0)
                {
                    comp_opts.dwKeyFrameEvery = (uint)lKey;
                    comp_opts.dwFlags |= Avi.AVICOMPRESSF_KEYFRAMES;
                }
                else
                {
                    comp_opts.dwKeyFrameEvery = 0U;
                    comp_opts.dwFlags &= ~(uint)Avi.AVICOMPRESSF_KEYFRAMES;
                }
                comp_opts.dwQuality = (uint)lQ;
                if (lDataRate != 0)
                {
                    comp_opts.dwBytesPerSecond = (uint)lDataRate * 1024U;
                    comp_opts.dwFlags |= Avi.AVICOMPRESSF_DATARATE;
                }
                else
                {
                    comp_opts.dwBytesPerSecond = 0U;
                    comp_opts.dwFlags &= ~(uint)Avi.AVICOMPRESSF_DATARATE;
                }
                comp_opts.dwFlags |= (uint)Avi.AVICOMPRESSF_VALID;
            }
        }

        /// <summary>Содержит параметры сжатия, применимые к заданному компрессору</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
        public struct ICINFO
        { 
            /// <summary>Размер структуры (байт)</summary>
            public Int32 dwSize;
            /// <summary>Тип используемого компрессора (аудио/видео)</summary>
            public UInt32 fccType;
            /// <summary>Идентификационный код используемого компрессора</summary>
            public UInt32 fccHandler; 
            /// <summary>Возможности компрессора</summary>
            public UInt32 dwFlags; 
            /// <summary>Номер версии драйвера</summary>
            public UInt32 dwVersion; 
            /// <summary>Версия VCM (Video compressor manager), поддерживаемая драйвером.
            /// Будет утановлено значение ICVERSION
            /// </summary>
            public UInt32 dwVersionICM; 
            /// <summary>Кратное название компресора</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string szName; 
            /// <summary>Полное название компрессора</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szDescription; 
            /// <summary>Название модуля, в котором содержится драйвер. Обычно не заполняется</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szDriver; 
        }

        #endregion

        #region Enums

        /// <summary>Возможные значения флагов использования компрессора/декомпрессора данных в методах VCM</summary>
        public enum ICMODE
        {
            /// <summary>Использовать обычное сжатие</summary>
            ICMODE_COMPRESS = 1,

            /// <summary>Использовать обычную декомпрессию</summary>
            ICMODE_DECOMPRESS = 2,

            /// <summary>Использовать быструю декомпрессию (в реальном времени)</summary>
            ICMODE_FASTDECOMPRESS = 3,

            /// <summary>Получить дополнительную информацию о компрессоре/декомпрессоре данных</summary>
            ICMODE_QUERY = 4,

            /// <summary>Использовать быстрое сжатие (в реальном времени)</summary>
            ICMODE_FASTCOMPRESS = 5,

            /// <summary>Производить декомпрессию и аппаратную отрисовку видеоданных</summary>
            ICMODE_DRAW = 8
        }

        #endregion

        #region Methods

        /// <summary>Инициализирует AVI библиотеку</summary>
        [DllImport("avifil32.dll")]
        public static extern void AVIFileInit();

        /// <summary>Открывает AVI файл</summary>
        /// <param name="ppfile">Указатель на новый интерфейс для работы с открытым AVI файлом</param>
        /// <param name="szFile">Строка с завершающим нулевым символом, содержащая имя открываемого файла</param>
        /// <param name="uMode">Уровень доступа, который будет установлен на открытый файл</param>
        /// <param name="pclsidHandler">Указатель на идентификатор класса для работы с AVI файлами.
        /// Если равен NULL - идентификатор определяется автоматически
        /// </param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        [DllImport("avifil32.dll", PreserveSig = true)]
        public static extern uint AVIFileOpen(ref IntPtr ppfile, String szFile, int uMode, int pclsidHandler);

        /// <summary>Возвращает указатель на интерфейс потока, связанного с открытым AVI файлом</summary>
        /// <param name="pfile">Дескрипрор открытого AVI файла</param>
        /// <param name="ppavi">Указатель на новый интерфейс для работы с потоком в открытом AVI файле</param>
        /// <param name="fccType">Тип запрашиваемого потока</param>
        /// <param name="lParam">Номер потока указанного типа</param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        [DllImport("avifil32.dll")]
        public static extern uint AVIFileGetStream(IntPtr pfile, out IntPtr ppavi,int fccType,int lParam);

        /// <summary>Создает новый поток в существующем AVI-файле и интерфейс для работы с ним</summary>
        /// <param name="pfile">Дескриптор открытого AVI-файла</param>
        /// <param name="ppavi">Указатель на интерфейс созданного потока</param>
        /// <param name="psi">Ссылка на структуру с информацией о создаваемом потоке</param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        [DllImport("avifil32.dll")]
        public static extern uint AVIFileCreateStream(IntPtr pfile, out IntPtr ppavi, ref AVISTREAMINFO psi);

        /// <summary>Устанавливает формат потока в заданую позицию AVI-файла</summary>
        /// <param name="pAviStram">Указатель на открытый поток AVI-файла</param>
        /// <param name="pos">Позиция в заданом потоке для сохранения формата</param>
        /// <param name="lpFormat">Указатель на структуру, содержащую данные о новом формате</param>
        /// <param name="cbFormat">Размер блока памяти, занимаемого структурой lpFormat</param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        [DllImport("avifil32.dll")]
        public static extern uint AVIStreamSetFormat(IntPtr pAviStram, int pos, IntPtr lpFormat, int cbFormat);

        /// <summary>Получает номер первого семпла в потоке</summary>
        /// <param name="pavi">Дескриптор потока в открытом AVI файле</param>
        /// <returns>Номер первого сампла в потоке в случае успеха или минус 1 в противном случае</returns>
        [DllImport("avifil32.dll", PreserveSig = true)]
        public static extern int AVIStreamStart(int pavi);

        /// <summary>Получает длину потока в кадрах</summary>
        /// <param name="pavi">Дескриптор потока в открытом AVI файле</param>
        /// <returns>Возвращает длину заданного потока в семплах в случае успеха или минус 1 в противном случае</returns>
        [DllImport("avifil32.dll", PreserveSig = true)]
        public static extern int AVIStreamLength(int pavi);

        /// <summary>Получает информацию о заголовке потока</summary>
        /// <param name="pAVIStream">Дескриптор открытого потока</param>
        /// <param name="psi">Указатель на структуру, содержащую данные заголовка потока</param>
        /// <param name="lSize">Длина заданной структуры в байтах </param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        [DllImport("avifil32.dll")]
        public static extern uint AVIStreamInfo(IntPtr pAVIStream, ref AVISTREAMINFO psi, int lSize);

        /// <summary>Получает информацию о формате данных потока открытого AVI файла</summary>
        /// <param name="pavi">Дескриптор открытого потока</param>
        /// <param name="lPos">Позиция в потоке, используемая для получения формата данных</param>
        /// <param name="lpFormat">Указатель на буфер, содержащий информацию о формате данных</param>
        /// <param name="lpcbFormat">Указатель на длину используемого буфера.После выполнения функции
        /// параметр содержит длину прочитанного блока данных</param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        /// <remarks>В случае, если lpFormat равен нулевому указателю, функция запишет в параметр lpcbFormat
        /// требуемую длину буфера данных</remarks>
        [DllImport("avifil32.dll")]
        public static extern uint AVIStreamReadFormat(IntPtr pavi, int lPos, IntPtr lpFormat, ref int lpcbFormat);
        
        /// <summary>Читает данные, содержащиеся в потоке</summary>
        /// <param name="pavi">Дескриптор открытого потока</param>
        /// <param name="lStart">Номер первого читаемого сэмпла</param>
        /// <param name="lSamples">Количиство читаемых сэмплов</param>
        /// <param name="lpBuffer">Указатель на буфер с прочитанными данными</param>
        /// <param name="cbBuffer">Длина буфера с прочитанными данными в байтах</param>
        /// <param name="plBytes">Количество прочитанных из потока байт</param>
        /// <param name="plSamples">Количество прочитанных из потока семплов</param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        [DllImport("avifil32.dll")]
        public static extern uint AVIStreamRead(IntPtr pavi, int lStart, int lSamples, IntPtr lpBuffer,
                                                int cbBuffer, ref int plBytes, ref int plSamples);

        /// <summary>Записывает данные в поток</summary>
        /// <param name="pavi">Дескриптор открытого потока</param>
        /// <param name="lStart">Номер первого записываемого сэмпла</param>
        /// <param name="lSamples">Количиство записываемых сэмплов</param>
        /// <param name="lpBuffer">Указатель на буфер с записываемыми данными</param>
        /// <param name="cbBuffer">Длина буфера с записываемыми данными в байтах</param>
        /// <param name="dwFlag">Флаг, описывающий тип записываемых данных (см. Флаги для индексации кадров в потоке)</param>
        /// <param name="plSamples">Количество записанных в поток семплов</param>
        /// <param name="plBytes">Количество записанных в поток байт</param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        [DllImport("avifil32.dll")]
        public static extern uint AVIStreamWrite(IntPtr pavi, int lStart, int lSamples, IntPtr lpBuffer,
                                                 int cbBuffer, int dwFlag, ref int plSamples, ref int plBytes);

        /// <summary>Уменьшает количество ссылок на дескриптор потока 
        /// и закрывает поток, если количество ссылок на него равно нулю
        /// </summary>
        /// <param name="pavi">Дескриптор потока открытого AVI файла</param>
        /// <returns>Оставшееся число действующих ссылок на указанный дескриптор потока</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamRelease(IntPtr pavi);

        /// <summary>Уменьшает количество ссылок на дескриптор открытого AVI файла 
        /// и закрывает файл, если количество ссылок на него равно нулю
        /// </summary>
        /// <param name="pfile">Дескриптор открытого AVI файла</param>
        /// <returns>Оставшееся число действующих ссылок на указанный дескриптор AVI файла</returns>
        [DllImport("avifil32.dll")]
        public static extern int AVIFileRelease(IntPtr pfile);

        /// <summary>Закрывает AVI библиотеку</summary>
        [DllImport("avifil32.dll")]
        public static extern void AVIFileExit();

        /// <summary>Подготавливает декомпрессор для заданного потока</summary>
        /// <param name="pAVIStream">Дескриптор потока в открытом avi-файле</param>
        /// <param name="bih">Сылка на структуру с данными о декомпрессируемых данных</param>
        /// <returns>Указатель на декомпрессор данных</returns>
        [DllImport("avifil32.dll")]
        public static extern IntPtr AVIStreamGetFrameOpen(IntPtr pAVIStream, ref BITMAPINFOHEADER bih);

        /// <summary>Освобождает ресурсы, связанные с декомпрессией потока</summary>
        /// <param name="pGetFrameObj">Дескриптор декомпрессора данных</param>
        /// <returns>Ноль в случае успеха или код ошибки в противном случае</returns>
        [DllImport("avifil32.dll")]
        public static extern uint AVIStreamGetFrameClose(IntPtr pGetFrameObj);

        /// <summary>Декомпрессия видеоизображения</summary>
        /// <param name="pGetFrameObj">Дескриптор декомпрессора видеоданных</param>
        /// <param name="lPos">Позиция кадра в потоке открытого avi-файла</param>
        /// <returns>Указатель на упакованный DIB в случае успеха. Ноль - в противном случае</returns>
        [DllImport("avifil32.dll")]
        public static extern IntPtr AVIStreamGetFrame(IntPtr pGetFrameObj, int lPos);

        /// <summary>Создает сжатый поток на основе несжатого. Поддерживает аудио и видео компрессию</summary>
        /// <param name="psCompressed">Ссылка на сжатый поток</param>
        /// <param name="psSource">Ссылка на исходный поток для сжатия</param>
        /// <param name="lpOptions">Структура с параметрами сжатия для компрессора</param>
        /// <param name="clsidHandler">Ссылка на идентификатор класса, используемая для создания сжатого потока</param>
        /// <returns></returns>
        [DllImport("avifil32.dll")]
        public static extern uint AVIMakeCompressedStream(out IntPtr psCompressed, IntPtr psSource,
                                                          ref AVICOMPRESSOPTIONS lpOptions, IntPtr clsidHandler);

        /// <summary>Отображает окно для выбора пользователем параметров сжатия видеоданных</summary>
        /// <param name="hwnd">Дескриптор родительского окна</param>
        /// <param name="uiFalgs">Настройки содержимого окна</param>
        /// <param name="lpIn">Указатель на входной формат несжатых видеоданных</param>
        /// <param name="lpData">Указатель на интерфейс AVI-потока, используемого для отображения предпросмотра
        /// результатов сжатия
        /// </param>
        /// <param name="pc">Указатель на структуру с результатами выбранных пользователем настроек</param>
        /// <param name="Title">Заголовок окна</param>
        /// <returns>TRUE - в случае подтрерждения пользователем заданных настроек, FALSE - в противном случае</returns>
        [DllImport("msvfw32.dll")]
        public static extern bool ICCompressorChoose(IntPtr hwnd, uint uiFalgs, IntPtr lpIn, IntPtr lpData,
                                                     ref Avi.COMPVARS pc, char[] Title);

        /// <summary>Освобождает ресурсы, выделенные ранее другими функциями 
        /// (ICCompressorChoose, ICSeqCompressFrameStart, ICSeqCompressFrame, and ICSeqCompressFrameEnd )
        /// </summary>
        /// <param name="pc">Настройки кодека</param>
        [DllImport("msvfw32.dll")]
        public static extern void ICCompressorFree(ref Avi.COMPVARS pc);

        /// <summary>Получает информацию об установленном компрессоре/декомпрессоре</summary>
        /// <param name="fccType">Тип данных (аудио/видео)</param>
        /// <param name="fccHandler">Идентификационный код компрессора данных</param>
        /// <param name="info">Информация о компрессоре</param>
        /// <returns>TRUE если компрессор установлен в системе и информация о нем получена, FALSE - в противном случае</returns>
        [DllImport("msvfw32.dll")]
        public static extern bool ICInfo(uint fccType, uint fccHandler, IntPtr info);

        /// <summary>Открывает компрессор</summary>
        /// <param name="fccType">Тип данных (аудио/видео)</param>
        /// <param name="fccHandler">Идентификационный код компрессора данных</param>
        /// <param name="wMode">Режим открытия компрессора</param>
        /// <returns>HIC кодека</returns>
        [DllImport("MSVFW32.dll"), PreserveSig]
        public static extern int ICOpen(uint fccType, uint fccHandler, ICMODE wMode);

        /// <summary>Получает информацию об установленном компрессоре/декомпрессоре</summary>
        /// <param name="hic">HIC кодека</param>
        /// <param name="lpicinfo">Указатель на структуру ICINFO параметров кодека</param>
        /// <param name="cb">Размер структуры ICINFO</param>
        /// <returns>Число байт, скопированных в структуру</returns>
        [DllImport("MSVFW32.dll", CharSet = CharSet.Ansi)]
        public static extern int ICGetInfo(int hic, IntPtr lpicinfo, int cb);

        /// <summary>Закрытие кодека</summary>
        /// <param name="hic">HIC кодека</param>
        /// <returns>Возвращает ICERR_OK в случае успешного завершения или код ошибки</returns>
        [DllImport("MSVFW32.dll")]
        public static extern int ICClose(int hic);

        /// <summary>Отправка сообщения кодеку</summary>
        /// <param name="hic">HIC кодека</param>
        /// <param name="wMsg">Сообщение</param>
        /// <param name="dw1">Параметр сообщения 1</param>
        /// <param name="dw2">Параметр сообщения 2</param>
        /// <returns></returns>
        [DllImport("MSVFW32.dll")]
        static extern int ICSendMessage(int hic, int wMsg, int dw1, int dw2);

        /// <summary>Вычисляет FCC код (FourCC)</summary>
        /// <param name="buffer">Массив байт, содержащий последовательность для вычисления FCC</param>
        /// <param name="index">Индекс первого элемента массива, с которого начинать вычисление FCC</param>
        /// <exception cref="System.ArgumentNullException">Не задан массив данных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Индекс первого элемента последовательности для вычисления FCC выходит за границы массива</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение аргумента</exception>
        /// <returns>FCC-код</returns>
        public static uint GetFourCC(char[] buffer, int index)
        {
            byte[] data = Encoding.Default.GetBytes(buffer, index, 4);
            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>Вычисляет FCC код (FourCC)</summary>
        /// <param name="buffer">Массив байт, содержащий последовательность для вычисления FCC</param>
        /// <param name="index">Индекс первого элемента массива, с которого начинать вычисление FCC</param>
        /// <exception cref="System.ArgumentNullException">Не задан массив данных</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Индекс первого элемента последовательности для вычисления FCC выходит за границы массива</exception>
        /// <exception cref="System.ArgumentException">Недопустимое значение аргумента</exception>
        /// <returns>FCC-код</returns>
        public static uint GetFourCC(byte[] buffer, int index)
        {
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>Вычисляет FCC код (FourCC)</summary>
        /// <param name="fcc">Символьное представление FCC</param>
        /// <exception cref="System.ArgumentNullException">Не задано символьное представление FCC-кода</exception>
        /// <returns>FCC-код</returns>
        public static uint GetFourCC(string fcc)
        {
            if (fcc == null)
            {
                throw new ArgumentNullException();
            }
            if (fcc.Length < 4)
            {
                string new_fcc_str = fcc;
                for (int i = 0; i < 4 - fcc.Length; new_fcc_str += " ", i++) ;
                fcc = new_fcc_str;
            }
            byte[] data = Encoding.Default.GetBytes(fcc);
            return GetFourCC(data, 0);
        }

        /// <summary>Возвращает символьное представление FCC-кода (FourCC)</summary>
        /// <param name="fcc">FCC-код</param>
        /// <returns>Символьное представление FCC-кода</returns>
        public static string GetFourCCstr(uint fcc)
        {
            byte[] data = BitConverter.GetBytes(fcc);
            char[] chars = Encoding.Default.GetChars(data);
            string result = new string(chars);
            return result.Replace('\0', ' ').Trim();
        }

        /// <summary>Получение списка кодеков, доступных в системе</summary>
        /// <returns>Список структур Avi.ICINFO информаций о кодеках</returns>
        /// <exception cref="System.OutOfMemoryException">Не достаточно памяти</exception>
        public static List<Avi.ICINFO> GetCodecs()
        {
            uint vidc_type = GetFourCC("vidc");
            List<ICINFO> list = new List<ICINFO>();
            uint codeId = 0;
            IntPtr _pnt = Marshal.AllocHGlobal(Marshal.SizeOf(new ICINFO()));
            while (Avi.ICInfo(vidc_type, codeId, _pnt))
            {
                ICINFO drv = (ICINFO)Marshal.PtrToStructure(_pnt, typeof(ICINFO));
                int hic = ICOpen(vidc_type, (uint)drv.fccHandler, ICMODE.ICMODE_QUERY);
                int size = Marshal.SizeOf(drv);
                int res = ICGetInfo(hic, _pnt, size);
                ICClose(hic);
                drv = (ICINFO)Marshal.PtrToStructure(_pnt, typeof(ICINFO));
                list.Add(drv);
                codeId++;
            }
            Marshal.Release(_pnt);
            return list;
        }

        /// <summary>Запрос есть ли у кодека окно конфигурации</summary>
        /// <param name="fccHandler">Дескриптор видеокодека</param>
        /// <returns>TRUE - для заданного кодека имеется окно конфигурации, FALSE - в противном случае</returns>
        public static bool IsConfig(uint fccHandler)
        {
            uint vidc_type = GetFourCC("vidc");// 0x63646976; //VIDC
            int h = Avi.ICOpen(vidc_type, fccHandler, Avi.ICMODE.ICMODE_COMPRESS);
            int res = 0;
            int d_user = 0x4000;//DRV_USER
            res = ICSendMessage(h, d_user + 0x1000 + 10, -1, 0);
            return res == 0;
        }

        /// <summary>Вызов окна конфигурации кодека</summary>
        /// <param name="fccHandler">Дескриптор видеокодека</param>
        public static void ConfigCodec(uint fccHandler)
        {
            uint vidc_type = GetFourCC("vidc");// 0x63646976; //VIDC
            int hic = ICOpen(vidc_type, fccHandler, Avi.ICMODE.ICMODE_COMPRESS);
            int d_user = 0x4000;//DRV_USER
            if (hic != 0) ICSendMessage(hic, d_user + 0x1000 + 10, 0, 0);
            ICClose(hic);
        }

        #endregion

    }
}
