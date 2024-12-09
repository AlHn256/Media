using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using AlfaPribor.VideoStorage;
using AlfaPribor.VideoStorage.Server;
using AlfaPribor.VideoStorage.Client;

namespace AlfaPribor.VideoStorage.Client.Demo
{
    public partial class FormClient : Form
    {
        private VideoStorageClient _VideoStorageClient;

        private VideoReaderClient _VideoReaderClient;

        private VideoIndexClient _VideoIndexClient;

        public FormClient()
        {
            InitializeComponent();
            _VideoStorageClient = new VideoStorageClient();
            _VideoReaderClient = new VideoReaderClient();
            _VideoIndexClient = new VideoIndexClient();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                IVideoReaderService reader = GetReader();
                if (reader == null)
                {
                    return;
                }
                IVideoIndexService index = GetIndex();
                if (index == null)
                {
                    return;
                }
                VideoPlayer player = new VideoPlayer();
                player.VideoReader = reader;
                player.VideoIndex = index;
                player.Show();
                player.Focus();
            }
            catch { }
        }

        private IVideoIndexService GetIndex()
        {
            VideoStorageIntResult result;
            try
            {
                result = _VideoStorageClient.VideoStorageService.GetVideoIndex(textBoxRecordId.Text);
            }
            catch (FaultException)
            {
                MessageBox.Show(
                    "Невозможно получить доступ к сервису чтения индексов видеоданных!\n" +
                    "Исключение на стороне сервера хранилища видеоданных.",
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            catch (CommunicationObjectFaultedException)
            {
                DialogResult res = MessageBox.Show(
                    "Поврежден канал связи с сервером хранилища видеоданных!\n" +
                    "После нажатия кнопки <OK> будет предпринята попытка восстановления канала...",
                    this.Text,
                    MessageBoxButtons.OK
                );
                if (res == DialogResult.Cancel)
                {
                    return null;
                }
                CloseVideoStorageClient();
                OpenStorageClient();
                return GetIndex();
            }
            catch (CommunicationException E)
            {
                MessageBox.Show(
                    "Ошибка доступа к серверу хранилища видеоданных!\n" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            if (result.Status != VideoStorageIntStat.Ok)
            {
                MessageBox.Show(
                    "Невозможно получить доступ к сервису чтения индексов видеоданных!\n" +
                    "Статус запрошенного интерфейса: " + result.Status,
                    this.Text,
                    MessageBoxButtons.OK
                );
                return null;
            }
            IVideoIndexService reader;
            try
            {
                reader = _VideoIndexClient[new Uri(result.URI).AbsolutePath];
            }
            catch (Exception E)
            {
                MessageBox.Show(
                    "Невозможно получить доступ к сервису чтения видеоданных!\n" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            return reader;
        }

        private IVideoReaderService GetReader()
        {
            VideoStorageIntResult result;
            try
            {
                if (checkBoxUsePartitionId.Checked)
                {
                    result = _VideoStorageClient.VideoStorageService.GetReader(textBoxRecordId.Text, (int)numericUpDownPartitionId.Value);
                }
                else
                {
                    result = _VideoStorageClient.VideoStorageService.GetReader(textBoxRecordId.Text);
                }
            }
            catch (FaultException)
            {
                MessageBox.Show(
                    "Невозможно получить доступ к интерфейсу чтения видеоданных!\n" +
                    "Исключение на стороне сервера хранилища видеоданных.",
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            catch (CommunicationObjectFaultedException)
            {
                DialogResult res = MessageBox.Show(
                    "Поврежден канал связи с сервером хранилища видеоданных!\n" +
                    "После нажатия кнопки <OK> будет предпринята попытка восстановления канала...",
                    this.Text,
                    MessageBoxButtons.OKCancel
                );
                if (res == DialogResult.Cancel)
                {
                    return null;
                }
                CloseVideoStorageClient();
                OpenStorageClient();
                return GetReader();
            }
            catch (CommunicationException E)
            {
                MessageBox.Show(
                    "Ошибка доступа к серверу хранилища видеоданных!\n" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            if (result.Status != VideoStorageIntStat.Ok)
            {
                MessageBox.Show(
                    "Невозможно получить доступ к интерфейсу чтения видеоданных!\n" +
                    "Статус запрошенного интерфейса: " + result.Status,
                    this.Text,
                    MessageBoxButtons.OK
                );
                return null;
            }
            IVideoReaderService reader;
            try
            {
                reader = _VideoReaderClient[new Uri(result.URI).AbsolutePath];
            }
            catch (Exception E)
            {
                MessageBox.Show(
                    "Невозможно получить доступ к интерфейсу чтения видеоданных!\n" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            return reader;
        }

        private void textBoxAddress_TextChanged(object sender, EventArgs e)
        {
            buttonApplySettings.Enabled = true;
        }

        private void buttonApplySettings_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            buttonApplySettings.Enabled = false;

            try
            {
                if (_VideoStorageClient.State != CommunicationState.Closed)
                {
                    CloseVideoStorageClient();
                }
                if (_VideoReaderClient.State != CommunicationState.Closed)
                {
                    CloseVideoReaderClient();
                }
                if (_VideoIndexClient.State != CommunicationState.Closed)
                {
                    CloseVideoIndexClient();
                }
                OpenStorageClient();
                OpenReaderClient();
                OpenIndexClient();
            }
            catch
            {
                buttonApplySettings.Enabled = true;
            }
            this.Cursor = Cursors.Default;
        }

        private void CloseVideoIndexClient()
        {
            try
            {
                _VideoIndexClient.Faulted -= _VideoStorageClient_Faulted;
                IAsyncResult res = _VideoIndexClient.BeginClose(new AsyncCallback(Client_BeginClose), _VideoIndexClient);
            }
            catch (CommunicationObjectFaultedException)
            {
                /// Канал связи неисправен
            }
            catch
            {
                MessageBox.Show(
                    "Истекло время ожидания закрытия канала связи с сервисом чтения индексов видеоданных!",
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
        }

        private void CloseVideoReaderClient()
        {
            try
            {
                _VideoReaderClient.Faulted -= _VideoStorageClient_Faulted;
                IAsyncResult res = _VideoReaderClient.BeginClose(new AsyncCallback(Client_BeginClose), _VideoReaderClient);
            }
            catch (CommunicationObjectFaultedException)
            {
                /// Канал связи неисправен
            }
            catch
            {
                MessageBox.Show(
                    "Истекло время ожидания закрытия канала связи с сервисом чтения данных хранилища!",
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
        }

        private void CloseVideoStorageClient()
        {
            try
            {
                _VideoStorageClient.Faulted -= _VideoStorageClient_Faulted;
                IAsyncResult res = _VideoStorageClient.BeginClose(new AsyncCallback(Client_BeginClose), _VideoStorageClient);
            }
            catch (CommunicationObjectFaultedException)
            {
                /// Канал связи неисправен
            }
            catch
            {
                MessageBox.Show(
                    "Истекло время ожидания закрытия канала связи с сервером '" +
                    _VideoStorageClient.Address + ":" + _VideoStorageClient.Port.ToString() + "'!",
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
        }

        private void OpenStorageClient()
        {
            try
            {
                _VideoStorageClient.Address = textBoxAddress.Text;
                _VideoStorageClient.Port = (int)numericUpDownPort.Value;
                _VideoStorageClient.RelativeUri = textBoxRelativeUri.Text;
                _VideoStorageClient.Faulted += new EventHandler(_VideoStorageClient_Faulted);
                _VideoStorageClient.BeginOpen(new AsyncCallback(Client_BeginOpen), _VideoStorageClient);
            }
            catch (TimeoutException)
            {
                MessageBox.Show(
                    "Истекло время ожидания открытия канала связи с сервером '" +
                    _VideoStorageClient.Address + ":" + _VideoStorageClient.Port.ToString() + "'!",
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            catch (Exception E)
            {
                MessageBox.Show(
                    "Ошибка активации клиента хранилища видеоданных!\n'" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
        }

        void _VideoStorageClient_Faulted(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Неисправен канал связи с сервером хранилища видеоданных!\n" +
                "После нажатия на кнопку <OK> будет предпринята попытка восстановления канала связи",
                this.Text,
                MessageBoxButtons.OK
            );
            buttonApplySettings_Click(sender, e);
        }

        private void OpenReaderClient()
        {
            try
            {
                _VideoReaderClient.Address = textBoxAddress.Text;
                _VideoReaderClient.Port = (int)numericUpDownPort.Value;
                _VideoReaderClient.Faulted +=new EventHandler(_VideoStorageClient_Faulted);
                _VideoReaderClient.BeginOpen(new AsyncCallback(Client_BeginOpen),_VideoReaderClient);
            }
            catch (TimeoutException)
            {
                MessageBox.Show(
                    "Истекло время ожидания открытия канала связи с сервисом чтения данных хранилища!",
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            catch (Exception E)
            {
                MessageBox.Show(
                    "Ошибка активации клиента чтения видеоданных!\n'" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
        }

        private void OpenIndexClient()
        {
            try
            {
                _VideoIndexClient.Address = textBoxAddress.Text;
                _VideoIndexClient.Port = (int)numericUpDownPort.Value;
                _VideoIndexClient.Faulted += new EventHandler(_VideoStorageClient_Faulted);
                _VideoIndexClient.BeginOpen(new AsyncCallback(Client_BeginOpen), _VideoIndexClient);
            }
            catch (TimeoutException)
            {
                MessageBox.Show(
                    "Истекло время ожидания открытия канала связи с сервисом чтения индексов видеоданных!",
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
            catch (Exception E)
            {
                MessageBox.Show(
                    "Ошибка активации клиента чтения индексов видеоданных!\n'" +
                    E.Message,
                    this.Text,
                    MessageBoxButtons.OK
                );
                throw;
            }
        }

        private void Client_BeginOpen(IAsyncResult arg)
        {
            if (arg.IsCompleted)
            {
                try
                {
                    if (arg.AsyncState is VideoStorageClient)
                    {
                        ((VideoStorageClient)arg.AsyncState).EndOpen(arg);
                    }
                    else if (arg.AsyncState is VideoReaderClient)
                    {
                        ((VideoReaderClient)arg.AsyncState).EndOpen(arg);
                    }
                    else if (arg.AsyncState is VideoIndexClient)
                    {
                        ((VideoIndexClient)arg.AsyncState).EndOpen(arg);
                    }
                }
                catch { }
            }
        }

        private void Client_BeginClose(IAsyncResult arg)
        {
            if (arg.IsCompleted)
            {
                try
                {
                    if (arg.AsyncState is VideoStorageClient)
                    {
                        ((VideoStorageClient)arg.AsyncState).EndClose(arg);
                    }
                    else if (arg.AsyncState is VideoReaderClient)
                    {
                        ((VideoReaderClient)arg.AsyncState).EndClose(arg);
                    }
                    else if (arg.AsyncState is VideoIndexClient)
                    {
                        ((VideoIndexClient)arg.AsyncState).EndClose(arg);
                    }
                }
                catch { }
            }
        }

        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                CloseVideoIndexClient();
                CloseVideoReaderClient();
                CloseVideoStorageClient();
            }
            catch { }
        }
    }
}
