using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDFSwindow.HDFSCore
{
    public class WebHdfsAPI
    {
        public const string API_PREFIX = "/webhdfs/v1";

        public const string HDFS_LIVENODES = "/jmx?get=Hadoop:service=NameNode,name=NameNodeInfo::LiveNodes ";

        /*
         * HTTP GET
         */

        //curl -i  "http://<HOST>:<PORT>/webhdfs/v1/<PATH>?op=LISTSTATUS"
        //RESPONSE:   {"FileStatuses":{"FileStatus":[  {"type":"DIRECTORY","pathSuffix":"app","childrenNum":1,"replication":0}    ]}}
        public const string LIST = "LISTSTATUS";

        //curl -i  "http://<HOST>:<PORT>/webhdfs/v1/<PATH>?op=GETFILESTATUS"
        //RESPONSE:   {"FileStatus":{"type":"DIRECTORY","pathSuffix":"app","childrenNum":1,"replication":0}}
        // type enum {FILE, DIRECTORY, SYMLINK}
        public const string FILESTATUS = "GETFILESTATUS";
        
        //curl -i -L "http://<HOST>:<PORT>/webhdfs/v1/<PATH>?op=OPEN
        //            [&offset=<LONG>][&length=<LONG>][&buffersize=<INT>][&noredirect=<true|false>]"
        public const string OPEN = "OPEN";

        /*
         * HTTP PUT
         */

        //curl -i -X PUT "http://<HOST>:<PORT>/webhdfs/v1/<PATH>?op=MKDIRS[&permission=<OCTAL>]"
        //RESPONSE:  {"boolean": true}
        public const string MKDIR = "MKDIRS";

        // Two step create: 1. get DataNode url; 2. upload data.
        //curl -i -X PUT "http://<HOST>:<PORT>/webhdfs/v1/<PATH>?op=CREATE
        //            [&overwrite=<true |false>][&blocksize=<LONG>][&replication=<SHORT>]
        //            [&permission=<OCTAL>][&buffersize=<INT>][&noredirect=<true|false>]"
        // noredirect RESPONSE json : {"Location":"http://<DATANODE>:<PORT>/webhdfs/v1/<PATH>?op=CREATE..."}
        //curl -i -X PUT -T <LOCAL_FILE> "http://<DATANODE>:<PORT>/webhdfs/v1/<PATH>?op=CREATE..."
        public const string UPLOAD = "CREATE";


        /*
         * HTTP POST
         */

        // curl -i -X POST "http://<HOST>:<PORT>/webhdfs/v1/<PATH>?op=CONCAT&sources=<PATHS>"
        //    param [sources] : A list of comma seperated absolute FileSystem paths without scheme and authority. 
        //RESPONSE header:  Content-Length: 0
        public const string CONCAT = "CONCAT";


        /*
         * HTTP DELETE
         */

        //curl -i -X DELETE "http://<host>:<port>/webhdfs/v1/<path>?op=DELETE
        //                      [&recursive=<true |false>]"
        public const string RM = "DELETE";


    }
}
