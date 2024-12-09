using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FramesPlayer.ExportConfiguration
{

    /// <summary>Класс именований</summary>
    public class NamingUtilits
    {
        
        /// <summary>Получить название вагона</summary>
        /// <param name="wagonID">Идентификатор вагона</param>
        /// <param name="channelID">Идентификатор канала</param>
        public static string GetWagonName(int wagonID, int channelID)
        {
            return string.Format("{0}_{1}.avi", wagonID, channelID);
        }

        /// <summary>Получить название вагона</summary>
        /// <param name="wagonID">Идентификатор вагона</param>
        /// <param name="channelID">Идентификатор канала</param>
        /// <param name="trainID">Идентификатор поезда</param>
        public static string GetWagonName(int wagonID, int channelID, int trainID)
        {
            return string.Format("{0}_{1}_{2}.avi",trainID, wagonID, channelID);
        }
        
        public static int GetTrainID(string filePath)
        {
            int res = -1; 
            int.TryParse(Path.GetFileNameWithoutExtension(filePath),out res);
            return res;
        }
        
        /// <summary>Получить имя файла вагона</summary>
        /// <param name="wagonID">Идентификатор вагона</param>
        /// <param name="channelID">Идентификатор канала</param>
        /// <param name="wagonDirectory">Папка вагона</param>
        /// <returns></returns>
        public static string GetWagonFileName(int wagonID, int channelID, string wagonDirectory)
        {
            return wagonDirectory + "\\" + GetWagonName(wagonID, channelID);
        }

        /// <summary>Получить имя файла вагона</summary>
        /// <param name="wagonID">Идентификатор вагона</param>
        /// <param name="channelID">Идентификатор канала</param>
        /// <param name="wagonDirectory">Папка вагона</param>
        /// <param name="trainFileName">Файл покзда</param>
        /// <returns></returns>
        public static string GetWagonFileName(int wagonID, int channelID, string wagonDirectory, string trainFileName)
        {
            return wagonDirectory + "\\" + GetWagonName(wagonID, channelID,GetTrainID(trainFileName));
        }

        /// <summary>Получить текстовый файл для экспорта</summary>
        /// <param name="directory">Каталог</param>
        /// <param name="trainFileName">Имя файла поезда</param>
        public static string GetExportTextFileName(string directory, string trainFileName)
        {
            return string.Format("{0}\\{1}.txt", directory, GetTrainID(trainFileName));
        }

    }
}
