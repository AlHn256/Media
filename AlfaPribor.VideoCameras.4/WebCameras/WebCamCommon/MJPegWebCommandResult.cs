using System;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace WebCamCommon
{
	delegate void OnCommandStartedHandler(CommandResult cmd);
	delegate void OnCommandFinishedHandler(CommandResult cmd);

	class MJPegWebCommandResult : CommandResult
	{
		public event OnCommandFinishedHandler OnCommandFinished;
		public event OnCommandStartedHandler OnOnCommandStarted;
		public event OnFrameHandler OnFrame;

		protected WebRequest _request;
		protected HttpWebResponse _response;
		protected bool _stop;
		protected ulong _imageOrdinalNumber;

		public MJPegWebCommandResult(HttpWebResponse response)
		{
			_stop = true;
			_imageOrdinalNumber = 0;
			_response = response;
		}

		public MJPegWebCommandResult(WebRequest request)
		{
			_stop = true;
			_imageOrdinalNumber = 0;
			_request = request;
		}

		public void StopProcessing()
		{
			_stop = true;
			_imageOrdinalNumber = 0;
		}

		public void StartProcessing()
		{
			_imageOrdinalNumber = 0;
            Thread processing_thread = new Thread(new ParameterizedThreadStart(ThreadStart));
            processing_thread.Priority = ThreadPriority.AboveNormal;
            processing_thread.Start(_response);
		}

		protected virtual void ProcessAsyncResult(object obj)
		{
			try
			{
                _stop = false;
                int block_size = 256;
                byte[] buf = new byte[1024];
                int buf_size = 0;
                HttpWebResponse response = null;
				if (_request != null)
				{
					if (_request is HttpWebRequest)
					{
						HttpWebRequest r = _request as HttpWebRequest;
						response = r.GetResponse() as HttpWebResponse;
					}
					else
					{
						response = _request.GetResponse() as HttpWebResponse;
					}
				}
				else
				{
					response = obj as HttpWebResponse;
				}

                string rct = response.ContentType.ToUpper();
                if (rct.IndexOf("MULTIPART/X-MIXED-REPLACE") == -1) return;

				Stream dataStream = response.GetResponseStream();
				dataStream.ReadTimeout = 5000;

				if (OnOnCommandStarted != null)
				{
					OnOnCommandStarted(this);
				}

                try // finally close data stream and response
                {
                    while (_stop == false)
                    {
                        int size = dataStream.Read(buf, buf_size, block_size);
                        if (size == 0)
                        {
                            break;
                        }
                        buf_size += size;
                        string str1 = Encoding.ASCII.GetString(buf, 0, buf_size).ToUpper();
                        int nStartOfHeader = str1.IndexOf("CONTENT-TYPE");//--myboundary");
                        if (nStartOfHeader == -1)
                        {
                            if (buf_size >= block_size * 3)
                            {
                                Array.Copy(buf, block_size, buf, 0, buf_size - block_size);
                                buf_size -= block_size;
                            }
                            continue;
                        }
                        else
                        {
                            str1 = Encoding.ASCII.GetString(buf, nStartOfHeader, buf_size - nStartOfHeader);
                        }
                        int nEndOfHeader = str1.IndexOf("\r\n\r\n");
                        if (nEndOfHeader == -1)
                        {   // header incomplete
                            if (buf_size >= block_size * 3)
                            {
                                Array.Copy(buf, block_size, buf, 0, buf_size - block_size);
                                buf_size -= block_size;
                            }
                        }
                        else
                        {   // header complete
                            string header = str1.Substring(0, nEndOfHeader);
                            int nImageSize = GetContentLength(header.ToUpper());
                            if (nImageSize <= 0)
                            {   // Wrong header
                                int exclude_pos = nStartOfHeader + 1;
                                Array.Copy(buf, exclude_pos, buf, 0, buf_size - exclude_pos);
                                buf_size -= exclude_pos;
                                continue;
                            }
                            byte[] image = new byte[nImageSize];
                            int start_pos = nStartOfHeader + nEndOfHeader + 4;
                            int nReadImageSize = buf_size - start_pos;

                            if (nReadImageSize > 0)
                            {
                                Array.Copy(buf, start_pos, image, 0, nReadImageSize);
                            }
                            while (nReadImageSize < nImageSize)
                            {
                                int bytes_to_read = nImageSize - nReadImageSize;
                                size = dataStream.Read(image, nReadImageSize, bytes_to_read);
                                if (size == 0 || _stop == true) break;
                                nReadImageSize += size;
                            }

                            buf_size = 0;

                            if (OnFrame != null)
                            {
                                if (image.Length >= 2 && image[0] == 0xff && image[1] == 0xd8 &&
                                    image[image.Length - 2] == 0xff && image[image.Length - 1] == 0xd9)
                                {
                                    OnFrame(image, ++_imageOrdinalNumber);
                                }
                                else
                                {
                                    //битый кадр с камеры
                                    System.Diagnostics.Debug.WriteLine("Битый кадр");
                                }
                            }
                        }
                    }
                }
                finally
                {
                    dataStream.Close();
                    response.Close();
                }
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

        private int GetContentLength(string header)
        {
            int result = 0;
            bool is_jpeg = false;
            string[] headers = header.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    string header_item = headers[i];
                    if (header_item.IndexOf("CONTENT-TYPE") != -1)
                    {
                        string[] s = header_item.Split(new Char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        is_jpeg = s[1] == "IMAGE/JPEG";
                    }
                    if (header_item.IndexOf("CONTENT-LENGTH") != -1)
                    {
                        string[] s = header_item.Split(new Char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        result = Convert.ToInt32(s[1]);
                    }
                }
                if (is_jpeg == false) result = 0;
            }
            catch (Exception) 
            {
                result = 0;
            }
            return result;
        }

        private void ThreadStart(object obj)
		{
			_stop = false;
			ProcessAsyncResult(obj);
            _stop = true;
			try
			{
				if (OnCommandFinished != null)
				{
					OnCommandFinished(this);
				}
			}
			catch (Exception) { }
		}
	}
}