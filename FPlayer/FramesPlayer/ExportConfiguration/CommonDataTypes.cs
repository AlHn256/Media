using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FramesPlayer.DataTypes
{

    /// <summary>Тип авторизации к SQL серверу/ </summary>
    public enum SQLAutorizationType 
    { 
        /// <summary>Windows</summary>
        Windows=0,
        /// <summary>SQL-сервер и Windows</summary>
        SQLSErverMode=1
    };

    /// <summary>Тип провайдера</summary>
    public enum ProviderType
    {
        /// <summary>АСКИН</summary>
        ASKIN=0,
        /// <summary>АСКО</summary>
        ASKO=1
    }
}
