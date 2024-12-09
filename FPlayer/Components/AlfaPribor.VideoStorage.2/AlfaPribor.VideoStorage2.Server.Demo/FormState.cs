using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

namespace AlfaPribor.VideoStorage.Server.Demo
{
    public partial class FormState : Form
    {
        private IVideoStorageServer _Server;

        public FormState(IVideoStorageServer server)
        {
            InitializeComponent();
            _Server = server;
            ShowInfo();
        }

        private void ShowInfo()
        {
            Clear();
            if (_Server == null)
            {
                return;
            }
            textBoxUrl.Text = _Server.Url;
            string state;
            switch (_Server.State)
            {
                case CommunicationState.Closed:
                    state = "закрыт";
                    break;
                case CommunicationState.Closing:
                    state = "закрывыется";
                    break;
                case CommunicationState.Created:
                    state = "конфигурирутеся";
                    break;
                case CommunicationState.Faulted:
                    state = "ошибка";
                    break;
                case CommunicationState.Opened:
                    state = "открыт";
                    break;
                case CommunicationState.Opening:
                    state = "открывается";
                    break;
                default:
                    state = "неизвестно";
                    break;
            }
            textBoxStatus.Text = state;
        }

        private void Clear()
        {
            textBoxUrl.Text = string.Empty;
            textBoxStatus.Text = string.Empty;
            textBoxLog.Text = string.Empty;
        }

        public void LoadEvents(string[] events)
        {
            textBoxLog.Lines = events;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
