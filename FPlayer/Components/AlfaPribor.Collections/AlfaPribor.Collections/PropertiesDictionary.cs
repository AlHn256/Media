using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace AlfaPribor.Collections
{
    /// <summary>Коллекция Dictionary_Of(string, string), способная представлять свои элементы в виде строки,
    /// где каждый элемент коллекции представлен в виде 'ключ'='значение',
    /// разделенные между собой символом 'разделитель'.
    /// </summary>
    public class PropertiesDictionary: Dictionary<string, string>
    {
        #region Fields

        /// <summary>Символ-разделитель, используемый для разделения одтельных пар 'ключ'='значение'
        /// в строковом представлении коллекции</summary>
        private char _Separator;

        #endregion

        #region Methods

        /// <summary>Инициализирует новый пустой экземпляр класса с параметрами по умолчанию.
        /// Сепаратор по умолчанию - ';'
        /// </summary>
        public PropertiesDictionary()
        {
            SetDefault();
        }

        /// <summary>Инициализирует новый пустой экземпляр класса с заданным сепаратором элементов</summary>
        /// <param name="separator">Сепаратор элементов</param>
        public PropertiesDictionary(char separator)
        {
            _Separator = separator;
        }

        /// <summary>Инициализирует новый экземпляр класса элементами,
        /// перечисленными в заданной строке с заданным разделителем элементов
        /// </summary>
        /// <param name="properties">Строка, содержащая значения ключей,
        /// где каждая пара 'ключ=значение'отделена друг от друга символом разделителем,
        /// заданным параметром separator
        /// </param>
        /// <param name="separator">Символ-разделитель пар 'ключ=значение'</param>
        public PropertiesDictionary(string properties, char separator)
        {
            _Separator = separator;
            Parse(properties);
        }

        /// <summary>Инициализирует новый экземпляр класса , который содержит элементы,
        /// скопированные из указанного словаря IDictionary_Of(string, string)
        /// </summary>
        /// <param name="separator">Словарь , элементы которого копируются в новый словарь</param>
        public PropertiesDictionary(IDictionary<string, string> dictionary)
            : base(dictionary)
        {
            SetDefault();
        }

        /// <summary>Инициализирует новый пустой экземпляр класса с начальной емкостью по умолчанию,
        /// использующий указанный компаратор IEqualityComparer_Of(string)
        /// </summary>
        /// <param name="comparer">Реализация IEqualityComparer_Of(string),
        /// которую следует использовать при сравнении ключей, или null,
        /// если для данного типа ключа должна использоваться реализация EqualityComparer_Of(string) по умолчанию
        /// </param>
        public PropertiesDictionary(IEqualityComparer<string> comparer)
            : base(comparer)
        {
            SetDefault();
        }

        /// <summary>Инициализирует новый пустой экземпляр класса Dictionary_Of(string, string)
        /// с указанной начальной емкостью
        /// </summary>
        /// <param name="capacity">Начальная емкость словаря</param>
        public PropertiesDictionary(int capacity)
            : base(capacity)
        {
            SetDefault();
        }

        /// <summary>Инициализирует новый экземпляр класса Dictionary_Of(string, string),
        /// который содержит элементы, скопированные из заданного словаря IDictionary_Of(string, string),
        /// и использует указанный компаратор IEqualityComparer_Of(string)
        /// </summary>
        /// <param name="dictionary">Словарь IDictionary_Of(string, string),
        /// элементы которого копируются в новый словарь Dictionary_Of(string, string)
        /// </param>
        /// <param name="comparer">Реализация IEqualityComparer_Of(T),
        /// которую следует использовать при сравнении ключей, или null, 
        /// если для данного типа ключа должна использоваться реализация EqualityComparer_Of(string) по умолчанию
        /// </param>
        public PropertiesDictionary(IDictionary<string, string> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer)
        {
            SetDefault();
        }

        /// <summary>Инициализирует новый пустой экземпляр класса Dictionary_Of(string, string)
        /// с указанной начальной емкостью и использует указанный компаратор IEqualityComparer_Of(string)
        /// </summary>
        /// <param name="capacity">Начальное количество элементов, которое может содержать коллекция</param>
        /// <param name="comparer">Реализация IEqualityComparer_Of(string), которую следует использовать при сравнении ключей,
        /// или null, если для данного типа ключа должна использоваться реализация EqualityComparer_Of(string) по умолчанию</param>
        public PropertiesDictionary(int capacity, IEqualityComparer<string> comparer)
            : base(capacity, comparer)
        {
            SetDefault();
        }

        /// <summary>Инициализирует новый экземпляр класса Dictionary_Of(string, string)
        /// с сериализованными данными</summary>
        /// <param name="info">Объект, содержащий сведения, которые требуются для сериализации
        /// коллекции Dictionary_Of(string, string)
        /// </param>
        /// <param name="context">Структура, содержащая исходный и конечный объекты для сериализованного потока,
        /// связанного с коллекцией Dictionary_Of(string, string)
        /// </param>
        public PropertiesDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            SetDefault();
        }

        /// <summary>Присваивает свойствам значения по умолчанию</summary>
        private void SetDefault()
        {
            _Separator = ';';
        }

        /// <summary>Формирует список свойст и их значений на основании содержимого заданной строки.
        /// <para>При анализе строки начальные и конечные пробелы ключей и значений 
        /// не учитываются (игнорируются)</para>
        /// </summary>
        /// <param name="properties">Строка со списком свойств</param>
        public void Parse(string properties)
        {
            Clear();
            string[] parts = properties.Split(new char[] { _Separator }, StringSplitOptions.RemoveEmptyEntries);
            char separator = '=';
            foreach (string part in parts)
            {
                int pos = part.IndexOf(separator);
                if (pos < 0)
                {
                    pos = part.Length;
                }
                string key = part.Substring(0, pos);
                string value = (pos < part.Length - 1) ? part.Substring(pos + 1) : string.Empty;
                if (key.Length > 0)
                {
                    key = key.Trim();
                }
                if (value.Length > 0)
                {
                    value = value.Trim();
                }
                Add(key, value);
            }
        }

        /// <summary>Предсвавляет коллекцию свойств в виде строки, где все свойства представлены парами 'имя=значение'
        /// и отделены друг от друга символом 'разделитель'
        /// </summary>
        /// <returns>Коллекция свойств и значений в виде строки с разделителями</returns>
        public override string ToString()
        {
            string result = string.Empty;
            foreach (KeyValuePair<string, string> property in this)
            {
                string value = property.Value;
                if (value.Length > 0)
                {
                    result += property.Key + "=" + property.Value + _Separator + " ";
                }
                else
                {
                    result += property.Key + _Separator + " ";
                }
            }
            if (result.Length > 0)
            {
                result = result.Remove(result.Length - 2, 2);
            }
            return result;
        }

        #endregion

        #region Properties

        /// <summary>Символ-разделитель, используемый для разделения одтельных пар 'ключ'='значение'
        /// в строковом представлении коллекции</summary>
        public char Separator
        {
            get { return _Separator; }
            set { _Separator = value; }
        }

        #endregion
    }
}
