using System;
using System.Collections.Generic;
using System.Text;

/// <summary>���������� � ���� ������</summary>
namespace AlfaPribor.ASKIN.DataBase
{

    /// <summary>����� ���������� � ���� ���� ������</summary>
    public class DataBaseFieldInfo
    {

        string name = "";
        string type = "";
        string desc = "";
        string table_name = "";
        bool allow_null = false;

        /// <summary>����������� ������ ���������� � ���� ���� ������</summary>
        /// <param name="_tablename">�������� �������</param>
        /// <param name="_name">��� ����</param>
        /// <param name="_type">���</param>
        /// <param name="_description">�������� ����</param>
        /// <param name="_allow_null">��������� null</param>
        public DataBaseFieldInfo(string _tablename, string _name, string _type, string _description, bool _allow_null)
        {
            table_name = _tablename;
            name = _name;
            type = _type;
            desc = _description;
            allow_null = _allow_null;
        }

        /// <summary>��� �������</summary>
        public string ColName
        {
            get { return name; }
        }

        /// <summary>��������</summary>
        public string FullName
        {
            get { return table_name + "." + name; }
        }

        /// <summary>�������� �������</summary>
        public string TableName
        {
            get { return table_name; }
        }

        /// <summary>��� ����</summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>��������� null</summary>
        public bool AllowNull
        {
            get { return allow_null; }
        }

        /// <summary>��������</summary>
        public string Description
        {
            get { return desc; }
        }

    }

    public static class DB
    {

        static DB()
        {
            DBInit();
        }

        /// <summary>������������� ����� ������ ���� ������</summary>
        public static void DBInit()
        {

            //������� "���������"
            Config.ParamName = new DataBaseFieldInfo(Config.TableName, "ParamName", "nvarchar(50)", "������������ ���������", false);
            Config.ParamValue = new DataBaseFieldInfo(Config.TableName, "ParamValue", "nvarchar(MAX)", "�������� ���������", true);

            //������� "��������"
            Directories.DirId = new DataBaseFieldInfo(Directories.TableName, "DirId", "int", "������������� �������� �����������", false);
            Directories.DirPath = new DataBaseFieldInfo(Directories.TableName, "DirPath", "nvarchar(512)", "���� � �������� ��������", true);
            Directories.DirType = new DataBaseFieldInfo(Directories.TableName, "DirType", "int", "��� ��������", true);
            Directories.DirStat = new DataBaseFieldInfo(Directories.TableName, "DirStat", "int", "������ ��������", true);

            //������� �������
            Eventlog.Sn = new DataBaseFieldInfo(Eventlog.TableName, "Sn", "int", "�������� ����� �������", false);
            Eventlog.EvTime = new DataBaseFieldInfo(Eventlog.TableName, "EvTime", "datetime", "���� � ����� �������", false);
            Eventlog.MsgId = new DataBaseFieldInfo(Eventlog.TableName, "MsgId", "int", "������������� ���������", false);
            Eventlog.EvSource = new DataBaseFieldInfo(Eventlog.TableName, "EvSource", "nvarchar(250)", "�������� ������� / ������������ ����������", true);
            Eventlog.EvData = new DataBaseFieldInfo(Eventlog.TableName, "EvData", "nvarchar(MAX)", "�������������� ������ �������", true);

            //������� �������� ���������
            Messages.MsgId = new DataBaseFieldInfo(Messages.TableName, "MsgId", "int", "������������� ���� �������", false);
            Messages.MsgText = new DataBaseFieldInfo(Messages.TableName, "MsgText", "nvarchar(255)", "������������ �������", false);
            Messages.Category = new DataBaseFieldInfo(Messages.TableName, "Category", "int", "��������� �������", false);

            //������� "���������"
            Operators.OpId = new DataBaseFieldInfo(Operators.TableName, "OpId", "int", "������������� ������", false);
            Operators.OpLogin = new DataBaseFieldInfo(Operators.TableName, "OpLogin", "nvarchar(32)", "������� �����", false);
            Operators.OpPassword = new DataBaseFieldInfo(Operators.TableName, "OpPassword", "nvarchar(32)", "������", false);
            Operators.FullName = new DataBaseFieldInfo(Operators.TableName, "FullName", "nvarchar(50)", "�������� ������������", false);
            Operators.OpLevel = new DataBaseFieldInfo(Operators.TableName, "OpLevel", "int", "���������� ������������", false);

            //������� ��������
            Status.Name = new DataBaseFieldInfo(Status.TableName, "Name", "nvarchar(50)", "��� �������", true);
            Status.DeviceStatus = new DataBaseFieldInfo(Status.TableName, "DeviceStatus", "nvarchar(50)", "���������", true);

            //������� ���������� � �������
            Trains.TrainId = new DataBaseFieldInfo(Trains.TableName, "TrainId", "int", "������������� ������", false);
            Trains.TimeBegin = new DataBaseFieldInfo(Trains.TableName, "TimeBegin", "datetime", "���� � ����� ������ ������", false);
            Trains.TimeEnd = new DataBaseFieldInfo(Trains.TableName, "TimeEnd", "datetime", "���� � ����� ��������� ������", true);
            Trains.Speed = new DataBaseFieldInfo(Trains.TableName, "Speed", "int", "��������", true);
            Trains.Direction = new DataBaseFieldInfo(Trains.TableName, "Direction", "int", "�����������", true);
            Trains.DirId = new DataBaseFieldInfo(Trains.TableName, "DirId", "int", "������������� ������� ��������������", true);

            //������� ���������� � �������
            Wagons.Sn = new DataBaseFieldInfo(Wagons.TableName, "Sn", "int", "������������� ������ ������ � ����", true);
            Wagons.TrainId = new DataBaseFieldInfo(Wagons.TableName, "TrainId", "int", "������������� ������� ��� ������", true);
            Wagons.WagonSn = new DataBaseFieldInfo(Wagons.TableName, "WagonSn", "int", "���������� ����� ������ � �������", true);
            Wagons.InvNum = new DataBaseFieldInfo(Wagons.TableName, "InvNum", "nvarchar(10)", "����������� ����� ������", true);
            Wagons.InvType = new DataBaseFieldInfo(Wagons.TableName, "InvType", "int", "��� ������������� ������������ ������", true);
            Wagons.RecordTime = new DataBaseFieldInfo(Wagons.TableName, "WeightTime", "datetime", "���� � ����� ������ ������", true);
            Wagons.Comment = new DataBaseFieldInfo(Wagons.TableName, "Comment", "nvarchar(MAX)", "�����������", true);
            Wagons.TimeSpanBegin = new DataBaseFieldInfo(Wagons.TableName, "TimeSpanBegin", "int", "����� ������� ������ ������", true);
            Wagons.TimeSpan = new DataBaseFieldInfo(Wagons.TableName, "TimeSpan", "int", "����� ������� ��������� ������", true);
            Wagons.TimeChanged = new DataBaseFieldInfo(Wagons.TableName, "Changed", "datetime", "���� � ����� ���������� ���������", true);

            //������ �������� ������ �� ��������
            TrainsArrival.Id = new DataBaseFieldInfo(TrainsArrival.TableName, "id", "int", "������������� ������ ��������� ����� ������� �� ��������", true);
            TrainsArrival.CodeDirection = new DataBaseFieldInfo(TrainsArrival.TableName, "code_direction", "nvarchar(50)", "��� �����������", true);
            TrainsArrival.CodeStation = new DataBaseFieldInfo(TrainsArrival.TableName, "code_station", "nvarchar(50)", "��� �������", true);
            TrainsArrival.Number = new DataBaseFieldInfo(TrainsArrival.TableName, "number", "nvarchar(4)", "����� ������", true);
            TrainsArrival.Index = new DataBaseFieldInfo(TrainsArrival.TableName, "number", "nvarchar(11)", "������ ������", true);
            TrainsArrival.Feature = new DataBaseFieldInfo(TrainsArrival.TableName, "feature", "int", "������� ���������� (� ������, � ������)", true);
            TrainsArrival.Date = new DataBaseFieldInfo(Trains.TableName, "date", "datetime", "��������� ���� � ����� ��������", true);

            //����������� ������ ������� �� ��������
            NumbersArrival.Id = new DataBaseFieldInfo(NumbersArrival.TableName, "id", "int", "������������� ������ ������ ��������� ����� �� ��������", true);
            NumbersArrival.TrainId = new DataBaseFieldInfo(NumbersArrival.TableName, "train_id", "int", "������������� ������ ��������� ����� �� ��������", true);
            NumbersArrival.Sn = new DataBaseFieldInfo(NumbersArrival.TableName, "sn", "int", "���������� ����� ������ � �������� ����� �� ��������", true);
            NumbersArrival.Number = new DataBaseFieldInfo(NumbersArrival.TableName, "number", "nvarchar(8)", "����������� ����� ������ � �������� ����� �� ��������", true);

        }

        #region ��������� ������ ������

        /// <summary>������� ��������</summary>
        public struct Config
        {

            /// <summary>�������� �������</summary>
            public const string TableName = "config";

            /// <summary>�������� ���������</summary>
            public static DataBaseFieldInfo ParamName;
            /// <summary>�������� ���������</summary>
            public static DataBaseFieldInfo ParamValue;

        }

        /// <summary>������� ��������� ��������� ���������</summary>
        public struct Directories
        {

            /// <summary>�������� �������</summary>
            public const string TableName = "directories";

            /// <summary>������������� �������� �����������</summary>
            public static DataBaseFieldInfo DirId;
            /// <summary>���� � �������� �����������</summary>
            public static DataBaseFieldInfo DirPath;
            /// <summary>��� ��������</summary>
            public static DataBaseFieldInfo DirType;
            /// <summary>������ ��������</summary>
            public static DataBaseFieldInfo DirStat;

        }

        /// <summary>������� �������</summary>
        public struct Eventlog
        {

            /// <summary>�������� �������</summary>
            public const string TableName = "eventlog";

            /// <summary>�������� ����� �������</summary>
            public static DataBaseFieldInfo Sn;
            /// <summary>���� � ����� �������</summary>
            public static DataBaseFieldInfo EvTime;
            /// <summary>������������� ���������</summary>
            public static DataBaseFieldInfo MsgId;
            /// <summary>�������� ������� / ������������ ����������</summary>
            public static DataBaseFieldInfo EvSource;
            /// <summary>�������������� ������ �������</summary>
            public static DataBaseFieldInfo EvData;

        }

        /// <summary>������� �������� �������</summary>
        public struct Messages
        {
            /// <summary>�������� �������</summary>
            public const string TableName = "messages";

            /// <summary>������������� ���� �������</summary>
            public static DataBaseFieldInfo MsgId;
            /// <summary>������������ �������</summary>
            public static DataBaseFieldInfo MsgText;
            /// <summary>��������� �������</summary>
            public static DataBaseFieldInfo Category;

        }

        /// <summary>������� ���������� � �������������</summary>
        public struct Operators
        {
            /// <summary>�������� �������</summary>
            public const string TableName = "operators";

            /// <summary>������������� ������</summary>
            public static DataBaseFieldInfo OpId;
            /// <summary>������� "�����"</summary>
            public static DataBaseFieldInfo OpLogin;
            /// <summary>������</summary>
            public static DataBaseFieldInfo OpPassword;
            /// <summary>�������� ������������</summary>
            public static DataBaseFieldInfo FullName;
            /// <summary>���������� ������������</summary>
            public static DataBaseFieldInfo OpLevel;

        }

        /// <summary>������� ������� ���������</summary>
        public struct Status
        {
            /// <summary>�������� �������</summary>
            public const string TableName = "status";

            /// <summary>��� �������</summary>
            public static DataBaseFieldInfo Name;
            /// <summary>���������</summary>
            public static DataBaseFieldInfo DeviceStatus;
        }

        /// <summary>������� ���������� � ��������</summary>
        public struct Trains
        {

            /// <summary>�������� �������</summary>
            public const string TableName = "trains";

            /// <summary>������������� ������</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>���� � ����� ������ ������</summary>
            public static DataBaseFieldInfo TimeBegin;
            /// <summary>���� � ����� ��������� ������</summary>
            public static DataBaseFieldInfo TimeEnd;
            /// <summary>��������</summary>
            public static DataBaseFieldInfo Speed;
            /// <summary>�����������</summary>
            public static DataBaseFieldInfo Direction;
            /// <summary>������������� ������� ��������������</summary>
            public static DataBaseFieldInfo DirId;

        }

        /// <summary>������� ������� �������</summary>
        public struct Wagons
        {

            /// <summary>�������� �������</summary>
            public const string TableName = "wagons";

            /// <summary>������������� ������ ������ � ����</summary>
            public static DataBaseFieldInfo Sn;
            /// <summary>������������� ������� ��� ������</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>���������� ����� ������ � �������</summary>
            public static DataBaseFieldInfo WagonSn;
            /// <summary>����������� ����� ������</summary>
            public static DataBaseFieldInfo InvNum;
            /// <summary>��� ������������ ������ ������ (���������, ������ �������...)</summary>
            public static DataBaseFieldInfo InvType;
            /// <summary>���� � ����� ������ ������</summary>
            public static DataBaseFieldInfo RecordTime;
            /// <summary>�����������</summary>
            public static DataBaseFieldInfo Comment;
            /// <summary>����� ������� � ������������� ��������� ������ (�������� �� ������ �����������)</summary>
            public static DataBaseFieldInfo TimeSpan;
            /// <summary>����� ������� � ������������� ������ ������ (�������� �� ������ �����������)</summary>
            public static DataBaseFieldInfo TimeSpanBegin;
            /// <summary>���� � ����� ���������� ���������</summary>
            public static DataBaseFieldInfo TimeChanged;

        }

        /// <summary>������� ������� �� ��������</summary>
        public struct TrainsArrival
        {
            /// <summary>�������� �������</summary>
            public const string TableName = "trains_arrival";

            /// <summary>������������� ������</summary>
            public static DataBaseFieldInfo Id;
            /// <summary>��� �����������</summary>
            public static DataBaseFieldInfo CodeDirection;
            /// <summary>��� �������</summary>
            public static DataBaseFieldInfo CodeStation;
            /// <summary>����� �������</summary>
            public static DataBaseFieldInfo Number;
            /// <summary>������ �������</summary>
            public static DataBaseFieldInfo Index;
            /// <summary>������� ����������� ����������</summary>
            public static DataBaseFieldInfo Feature;
            /// <summary>��������� ���� ��������</summary>
            public static DataBaseFieldInfo Date;
        }

        /// <summary>������� ������� �� ��������</summary>
        public struct NumbersArrival
        {
            /// <summary>�������� �������</summary>
            public const string TableName = "numbers_arrival";

            /// <summary>������������� ������</summary>
            public static DataBaseFieldInfo Id;
            /// <summary>������������� ��������� ����� ������ �� ��������</summary>
            public static DataBaseFieldInfo TrainId;
            /// <summary>���������� ����� ������</summary>
            public static DataBaseFieldInfo Sn;
            /// <summary>����������� �����</summary>
            public static DataBaseFieldInfo Number;
        }

        #endregion

    }

}