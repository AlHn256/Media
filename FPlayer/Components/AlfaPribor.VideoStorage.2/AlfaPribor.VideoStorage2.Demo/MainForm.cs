using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using AlfaPribor.VideoStorage;

namespace AlfaPribor.VideoStorage.Demo
{
    public partial class MainForm : Form
    {
        private BaseVideoStorage _Storage;

        private MainFormState _State;

        private StorageInfo _Info;

        public enum MainFormState
        {
            /// <summary>Просмотр настроек хранилища видеоданных</summary>
            Browse = 0,

            /// <summary>Добавление раздела хранилища видеоданных</summary>
            Addition,

            /// <summary>Изменение настроек раздела хранилища видеоданных</summary>
            Editing
        };

        public MainFormState State
        {
            get { return _State; }
        }

        public MainForm()
        {
            InitializeComponent();
            numFreeSpace.Maximum = Int64.MaxValue;
        }

        /// <summary>Отобразить сообщение в строке состояния</summary>
        /// <param name="text">Текст сообщения</param>
        public void ShowStatusMessage(string text)
        {
            slblMessage.Text = text;
        }

        /// <summary>Создает настройки раздела хранилища на основании визуальных данных формы</summary>
        /// <returns>Раздел хранилища видеоданных</returns>
        private VideoPartitionSettings GetPartitionSettings()
        {
            VideoPartitionSettings settings = new VideoPartitionSettings(
                Int32.Parse(txtbId.Text), chbxActive.Checked, txtbPartitionPath.Text, (long)numFreeSpace.Value * 1024L *1024L
            );
            return settings;
        }

        /// <summary>Получить значение для элемента item сиписка lvwPartitions по имени столбца</summary>
        /// <param name="col_header">Столбец, из которого нужно прочитать значение</param>
        /// <param name="item">Элемент списка lvwPartitions, из которого нужно прочитать значение</param>
        private string GetValueByColumn(ColumnHeader col_header, ListViewItem item)
        {
            if (!lvwPartitions.Columns.Contains(col_header))
            {
                throw new ApplicationException("Column '" + col_header.Name + "' not found in control '" + lvwPartitions.Name + "'!");
            }
            int index = lvwPartitions.Columns.IndexOf(col_header);
            ListViewItem.ListViewSubItemCollection sub_items = item.SubItems;
            return sub_items[index].Text;
        }

        /// <summary>Задать значение для элемента item сиписка lvwPartitions по имени столбца</summary>
        /// <param name="item">Элемента списка lvwPartitions, значение которого изменяется</param>
        /// <param name="col_header">Столбец, в котором нужно изменить значение</param>
        /// <param name="value"></param>
        /// <exception cref="System.ApplicationException">
        /// Столбец с заданным именем не найден в контроле lvwPartitions
        /// </exception>
        private void SetValueByColumn(ListViewItem item, ColumnHeader col_header, string value)
        {
            if (!lvwPartitions.Columns.Contains(col_header))
            {
                throw new ApplicationException("Column '" + col_header.Name + "' not found in control '" + lvwPartitions.Name + "'!");
            }
            int index = lvwPartitions.Columns.IndexOf(col_header);
            ListViewItem.ListViewSubItemCollection sub_items = item.SubItems;
            if (index < sub_items.Count)
            {
                sub_items[index].Text = value;
            }
            else
            {
                ListViewItem.ListViewSubItem sub_item = new ListViewItem.ListViewSubItem(item, value);
                sub_items.Insert(index, sub_item);
            }
        }

        /// <summary>Получить настройки раздела хранилища на основании данных визуального контрола lvwPartitions</summary>
        /// <param name="item">Элемент списка визуального компонента</param>
        /// <returns>Настройки раздела хранилища</returns>
        private VideoPartitionSettings GetPartitionSettings(ListViewItem item)
        {
            int id = Int32.Parse(GetValueByColumn(clhId, item));
            bool active = Boolean.Parse(GetValueByColumn(clhActive, item));
            string path = GetValueByColumn(clhPath, item);
            long space_limit = Int64.Parse(GetValueByColumn(clhFreeSpaceLimit, item)) * 1024L * 1024L;
            VideoPartitionSettings settings = new VideoPartitionSettings(
                id, active, path, space_limit
            );
            return settings;
        }

        /// <summary>Записать данные настроек раздела хранилища в визуальный контрол</summary>
        /// <param name="item">Элемент списка lvwPartitions, содержащий данные о разделе хранилища</param>
        /// <param name="settings">Настройки раздела хранилища</param>
        private void SetPartitionSettings(ListViewItem item, VideoPartitionSettings settings)
        {
            SetValueByColumn(item, clhId, settings.Id.ToString());
            SetValueByColumn(item, clhPath, settings.Path);
            SetValueByColumn(item, clhFreeSpaceLimit, (settings.FreeSpaceLimit / (1024 * 1024)).ToString());
            SetValueByColumn(item, clhActive, settings.Active.ToString());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _State = MainFormState.Addition;
            SetPartitionPropertiesDefault();
            gbxPartitionProperties.Enabled = true;
            CheckButtonsState();
        }

        /// <summary>Устанавливает визуальные компоненты свойств раздела хранилища
        /// в состояние по умолчанию
        /// </summary>
        private void SetPartitionPropertiesDefault()
        {
            txtbPartitionPath.Text = string.Empty;
            numFreeSpace.Value = 1024;
            int max_id =
                (from part_info in _Storage.Info.Partitions.AsEnumerable()
                select part_info.Id).Max();
            txtbId.Text = (max_id + 1).ToString();
            chbxActive.Checked = true;
        }

        /// <summary>Устанавливает визуальные компоненты свойств раздела хранилища
        /// в соответствии с указанными данными
        /// </summary>
        /// <param name="settings">Объект с настройками раздела хранилища</param>
        private void SetPartitionSettings(VideoPartitionSettings settings)
        {
            txtbPartitionPath.Text = settings.Path;
            txtbId.Text = settings.Id.ToString();
            numFreeSpace.Value = settings.FreeSpaceLimit / (1024L * 1024L);
            chbxActive.Checked = settings.Active;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            switch (_State)
            {
                case MainFormState.Addition:
                    AddPartition();
                    break;
                case MainFormState.Editing:
                    EditPartition();
                    break;
                default:
                    break;
            }
            _State = MainFormState.Browse;
            gbxPartitionProperties.Enabled = false;
            CheckButtonsState();
        }

        /// <summary>Добавляет новый
        /// раздел на основании указанных в визульных контролах пользователем данных
        /// </summary>
        private void AddPartition()
        {
            ListViewItem item = new ListViewItem();
            SetPartitionSettings(item, GetPartitionSettings());
            lvwPartitions.Items.Add(item);
        }

        /// <summary>
        /// Изменяет настройки текущего выбранного раздела
        /// на основании указанных в визульных контролах пользователем данных
        /// </summary>
        private void EditPartition()
        {
            ListView.SelectedListViewItemCollection items = lvwPartitions.SelectedItems;
            if (items.Count == 0)
            {
                return;
            }
            SetPartitionSettings(items[0], GetPartitionSettings());
        }

        /// <summary>Разрешает или запрещает действия со списком разделов хранилища
        /// в зависимости от текущего состояния формы
        /// </summary>
        private void CheckButtonsState()
        {
            btnAdd.Enabled = _State == MainFormState.Browse;
            btnDelete.Enabled = (lvwPartitions.Items.Count > 0) && (_State == MainFormState.Browse);
            btnChange.Enabled = (lvwPartitions.Items.Count > 0) && (_State == MainFormState.Browse);
            btnApply.Enabled = _State != MainFormState.Browse;
            btnCancel.Enabled = _State != MainFormState.Browse;
        }

        /// <summary>Отобразить в виртуальных контролах настройки хранилища видеоданных</summary>
        /// <param name="settings"></param>
        private void SetStorageSettings(VideoStorageSettings settings)
        {
            lvwPartitions.BeginUpdate();
            lvwPartitions.Items.Clear();
            foreach (VideoPartitionSettings partition in settings.Partitions)
            {
                ListViewItem list_item = new ListViewItem();
                SetPartitionSettings(list_item, partition);
                lvwPartitions.Items.Add(list_item);
            }
            lvwPartitions.EndUpdate();
            numCheckInterval.Value = settings.CircleBufferCheckInterval;
        }

        /// <summary>Кофигурировать хранилище, 
        /// основываясь на данных, введенных пользователем в визуальные контролы
        /// </summary>
        private void SaveStorageSettings()
        {
            VideoStorageSettings settings = GetStorageSettings();
            _Storage.SetSettings(settings);
            _Storage.DeleteDuplicateRecords = chbWriteDuplicates.Checked;
        }

        /// <summary>Формирует объект с настройками хранилища на основании введенных пользователем данных</summary>
        /// <returns>Объект с настройками хранилища</returns>
        private VideoStorageSettings GetStorageSettings()
        {
            VideoStorageSettings settings = new VideoStorageSettings();
            for (int i = 0; i < lvwPartitions.Items.Count; ++i )
            {
                settings.Partitions.Add(GetPartitionSettings(lvwPartitions.Items[i]));
            }
            settings.CircleBufferCheckInterval = (int)numCheckInterval.Value;
            return settings;
        }

        private void btnChoicePath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtbPartitionPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _State = MainFormState.Browse;
            gbxPartitionProperties.Enabled = false;
            CheckButtonsState();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = lvwPartitions.SelectedItems;
            for (int i = 0; i < items.Count; i++)
            {
                lvwPartitions.Items.Remove(items[i]);
            }
            CheckButtonsState();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = lvwPartitions.SelectedItems;
            if (items.Count == 0)
            {
                return;
            }
            _State = MainFormState.Editing;
            SetPartitionSettings(GetPartitionSettings(items[0]));
            gbxPartitionProperties.Enabled = true;
            CheckButtonsState();
        }

        private void btnGetSettings_Click(object sender, EventArgs e)
        {
            LoadStorageSettings();
            ShowStatusMessage("Настройки хранилища прочитаны");
        }

        private void LoadStorageSettings()
        {
            VideoStorageSettings settings = _Storage.GetSettings();
            SetStorageSettings(settings);
            chbWriteDuplicates.Checked = _Storage.DeleteDuplicateRecords;
            _State = MainFormState.Browse;
            CheckButtonsState();
        }


        private void btnSetSettings_Click(object sender, EventArgs e)
        {
            SaveStorageSettings();
            ShowStatusMessage("Настройки хранилища изменены");
        }

        private void btnGetFreeSpace_Click(object sender, EventArgs e)
        {
            if (txtbPartitionPath.Text == string.Empty)
            {
                return;
            }
            try
            {
                DriveInfo drive = new DriveInfo(Path.GetPathRoot(txtbPartitionPath.Text));
                numFreeSpace.Value = drive.TotalFreeSpace / (1024 * 1024);
            }
            catch 
            {
                numFreeSpace.Value = numFreeSpace.Maximum;
                ShowStatusMessage("Невозможно рассчитать свободное место на диске");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _Storage.Active = false;
            SaveConfig();
        }

        private void btnWriteRecord_Click(object sender, EventArgs e)
        {
            using (FramesWriter writer = new FramesWriter(_Storage))
            {
                writer.ShowDialog();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>Хранилище видеоданных</summary>
        public BaseVideoStorage Storage
        {
            get { return _Storage; }
        }

        /// <summary>Сохранить настройки конфигурации хранилища в файл</summary>
        private void SaveConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(VideoStorageSettings));
            using (FileStream file = new FileStream("VideoStorage.settings", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(file, GetStorageSettings());
            }
        }

        private void LoadConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(VideoStorageSettings));
            using (FileStream file = new FileStream("VideoStorage.settings", FileMode.Open, FileAccess.Read))
            {
                VideoStorageSettings settings = (VideoStorageSettings)serializer.Deserialize(file);
                SetStorageSettings(settings);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _Storage = new VideoStorage();
            _Info = new StorageInfo(_Storage);
            try
            {
                LoadConfig();
            }
            catch { }
            CheckButtonsState();
            SaveStorageSettings();
            _Storage.OnCircularBufferDeleting += new CancelEventHandler(_Storage_OnCircularBufferDeleting);
            _Storage.OnSyncComplete += new EvSyncComplete(_Storage_OnSyncComplete);
            _Storage.Active = true;
            ShowStatusMessage("Хранилище создано и активировано");
        }

        void _Storage_OnSyncComplete(object sender, SyncCompleteEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EvSyncComplete(_Storage_OnSyncComplete), sender, e);
                return;
            }
            ShowStatusMessage("Синхронизация хранилища завершена");
        }

        void _Storage_OnCircularBufferDeleting(object sender, CancelEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new CancelEventHandler(_Storage_OnCircularBufferDeleting), sender, e);
                return;
            }
            ShowStatusMessage("Хранилище заполнено. Требуется высвободить место для новых записей");
            DialogResult result = MessageBox.Show(
                "Требуется высвободить место для новых записей.\n\nНажмите <OK> для удаления записей со всех разделов.\n" +
                "Нажмите <Cancel> если хотите очистить хранилище вручную.",
                this.Text,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
                );
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            _Storage.DeleteAll();
        }

        private void btnReadRecord_Click(object sender, EventArgs e)
        {
            using (FramesReader reader = new FramesReader(_Storage))
            {
                reader.ShowDialog();
            }
        }

        private void btnDeleteRecord_Click(object sender, EventArgs e)
        {
            using (RecordEraser eraser = new RecordEraser(_Storage))
            {
                eraser.ShowDialog();
            }
            ShowStatusMessage("");
        }

        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            _Info.Show();
            _Info.RefreshInfo();
        }

        private void btnSynchronize_Click(object sender, EventArgs e)
        {
            using (Synchronizer sync = new Synchronizer(_Storage))
            {
                sync.Owner = this;
                sync.ShowDialog();
            }
        }

        private void btnRewrite_Click(object sender, EventArgs e)
        {
            using (FramesRewriter rewriter = new FramesRewriter(_Storage))
            {
                rewriter.ShowDialog();
            }
        }
    }
}
