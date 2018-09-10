﻿using ACBC.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ACBC.Buss
{
    public class UploadBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UploadApi;
        }

        public object Do_Upload(BaseApi baseApi)
        {
            var upload = (IFormCollection)baseApi.param;
            List<string> fileList = new List<string>();
            foreach (IFormFile iFormFile in upload.Files)
            {
                string fileName = baseApi.code + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString();
                using (Stream sm = iFormFile.OpenReadStream())
                {
                    byte[] b = new byte[sm.Length];
                    sm.Read(b, 0, b.Length);
                    string aLastName = iFormFile.FileName.Substring(iFormFile.FileName.LastIndexOf(".") + 1, (iFormFile.FileName.Length - iFormFile.FileName.LastIndexOf(".") - 1));
                    string path = Path.Combine(Path.GetDirectoryName(typeof(UploadBuss).Assembly.Location), "upload", fileName + "." + aLastName);
                    DirectoryInfo TheFolder = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(typeof(UploadBuss).Assembly.Location), "upload"));
                    if(!TheFolder.Exists)
                    {
                        Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(typeof(UploadBuss).Assembly.Location), "upload"));
                    }
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                    }
                    sm.Close();
                    fileName = fileName + "." + aLastName;
                }
                fileList.Add(fileName);
            }

            return new { fileName = fileList };
        }
    }
}
