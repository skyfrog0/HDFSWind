using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HDFSwindow.HDFSCore
{
    public class HdfsFileInfo
    {
        public string Name { get; set; }
        public string PathName { get; set; }
        public long Size { get; set; }
        public int Replication { get; set; }
        public bool IsDirectory { get; set; }
        public int ChildrenNum { get; set; }
        public DateTime ModificationTime { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}\t{2} Bytes",
                IsDirectory ? "Dir " : "File",
                Name, Size);
        }

        public static HdfsFileInfo FromJson(JObject filestatus)
        {
            if (null == filestatus)
                return null;
            return new HdfsFileInfo()
            {
                Name = filestatus["pathSuffix"].ToString(),
                Replication = (int)filestatus["replication"],
                Size = (long)filestatus["length"],
                IsDirectory = "DIRECTORY".Equals(filestatus["type"].ToString()),
                ChildrenNum = (int)filestatus["childrenNum"],
                ModificationTime = ConvertTime(filestatus["modificationTime"].ToString())
            };
        }

        private static DateTime ConvertTime(string timestamp)
        {
            long javatime = 0;
            long.TryParse(timestamp, out javatime);     //单位：ms
            return new DateTime(1970, 1, 1, 8, 0, 0).AddTicks(javatime * 10000);
        }

        public static HdfsFileInfo FromJson(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;
            var jObj = JsonConvert.DeserializeObject(status);
            if (null == jObj)
                throw new Exception("HdfsFileInfo Json解析错误");
            try
            {
                var jStatus = (jObj as JObject)["FileStatus"] as JObject;
                return FromJson(jStatus);
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return null;
            }
        }

        public static List<HdfsFileInfo> ParseJsonArray(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return null;
                var jObj = JsonConvert.DeserializeObject(json);
                if (null == jObj)
                    throw new Exception("HdfsFileInfo Json解析错误");
                var statusArray = (jObj as JObject)["FileStatuses"]["FileStatus"] as JArray;
                List<HdfsFileInfo> files = new List<HdfsFileInfo>();
                foreach (var item in statusArray)
                {
                    files.Add(FromJson(item as JObject));
                }
                return files;
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return null;
            }
        }

    }
}
