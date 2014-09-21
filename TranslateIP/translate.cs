using System;
using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace XenMe.com
{
    public class TranslateIP : IHttpModule
    {
        public static List<string> rulesList;
        public void Init(HttpApplication app)
        {
            app.ReleaseRequestState += new EventHandler(InstallResponseFilter);
        }
        public void Dispose()
        { 
            
        }

        public void InstallResponseFilter(Object s, EventArgs e)
        {
            HttpContext context = ((HttpApplication)s).Context;
            HttpResponse response = context.Response;

            if (response.ContentType.IndexOf("application/x-ica")>=0)
            {
                #region load rules
                rulesList = new List<string>();
                if (!File.Exists("TranslateIP_Rules.txt"))
                {
                    return;
                }
                string[] lines = File.ReadAllLines("TranslateIP_Rules.txt");
                foreach (string line in lines)
                {
                    string l = line.Trim();
                    try { 
                        string[] tmp = l.Split(new char[] { ',' });
                        if (tmp.Length == 4)
                        {
                            rulesList.Add(l);
                        }
                    }
                    catch { }
                }
                #endregion
                response.Filter = new OutputFilter(response.Filter,rulesList);
            }
        }
    }
    public class OutputFilter : Stream
    {
        Stream responseStream;
        long position;
        StringBuilder responseHtml;
        List<string> rulesList;

        public OutputFilter(Stream inputStream,List<string> natRules)
        {
            responseStream = inputStream;
            responseHtml = new StringBuilder();
            rulesList = natRules;
        }

        #region override
        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return true; }
        }
        public override void Close()
        {
            responseStream.Close();
        }
        public override void Flush()
        {
            responseStream.Flush();
        }
        public override long Length
        {
            get { return 0; }
        }
        public override long Position
        {
            get { return position; }
            set { position = value; }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return responseStream.Seek(offset, origin);
        }
        public override void SetLength(long length)
        {
            responseStream.SetLength(length);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return responseStream.Read(buffer, offset, count);
        }
        #endregion

        public override void Write(byte[] buffer, int offset, int count)
        {
            string strBuffer = System.Text.UTF8Encoding.UTF8.GetString(buffer, offset, count);
            
            //starting replace
            foreach(string rule in rulesList)
            {
                string[] tmp = rule.Split(new char[] { ',' });
                if (strBuffer.IndexOf(tmp[0]) > 0)
                {
                    strBuffer = strBuffer.Replace(tmp[0], tmp[1]);
                    strBuffer = strBuffer.Replace(tmp[2], tmp[3]);
                }
            }
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(strBuffer);

            //responseStream.Write(buffer, offset, count);
            responseStream.Write(data, 0, data.Length);
        }
    }
}
