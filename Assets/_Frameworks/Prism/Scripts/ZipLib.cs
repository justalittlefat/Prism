using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ZipLib{

    private static float _process = 0f;    
    public static float process
    {
        get
        {
            return _process;
        }
    }
    private static string _currentFile = "";
    public static string currentFile
    {
        get
        {
            return _currentFile;
        }
    }
    private static bool _isUnZipping = false;
    public static bool isUnZipping
    {
        get
        {
            return _isUnZipping;
        }
    }
    private const int _bufferSize = 1024;
    public static string info = "";

    public static bool UnZip(byte[] ZipByte, string descPath, string password = "")
    {
        if (_isUnZipping) return false;
        _isUnZipping = true;
        _process = 0f;
        bool result = true;
        string targetPath;
        Stream stream = null;
        FileStream fs = null;
        ZipInputStream zipStream = null;
        ZipEntry ent = null;
        try
        {
            stream = new MemoryStream(ZipByte);
            zipStream = new ZipInputStream(stream);
            if (!string.IsNullOrEmpty(password))
            {
                zipStream.Password = password;
            }
            long fileSize = 0;
            long finishedSize = 0;
            while ((ent = zipStream.GetNextEntry()) != null)
            {
                fileSize += zipStream.Length;
            }            
            zipStream.Close();
            zipStream.Dispose();
            stream = new MemoryStream(ZipByte);
            zipStream = new ZipInputStream(stream);
            while ((ent = zipStream.GetNextEntry()) != null)
            {
                if (!string.IsNullOrEmpty(ent.Name))
                {
                    _currentFile = ent.Name;
                    targetPath = Path.Combine(descPath, ent.Name);
                    if (targetPath.EndsWith("/")) continue;
                    var folderPath = Path.GetDirectoryName(targetPath);
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);                   
                    fs = File.Create(targetPath);                                       
                    int size = _bufferSize;
                    byte[] data = new byte[size];
                    while (true)
                    {
                        size = zipStream.Read(data, 0, data.Length);
                        if (size <= 0) break;
                        finishedSize += size;
                        _process = (float)finishedSize / (float)fileSize;
                        info = _process.ToString();                     
                        fs.Write(data, 0, size);                  
                    }
                }
            }
        }
        catch (Exception e)
        {
            info = e.ToString();
            Debug.Log(e.ToString());
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
            if (zipStream != null)
            {
                zipStream.Close();
                zipStream.Dispose();
            }
            if (ent != null)
            {
                ent = null;
            }
            GC.Collect();
            GC.Collect(1);
        }
        _isUnZipping = false;
        return result;
    }

    private static bool _isZipping = false;
    public static bool isZipping
    {
        get
        {
            return _isZipping;
        }
    }
    public static bool CompressFile(string targetFolder, string outputPath,
         string comment = null, string password = null, int compressionLevel = 6)
    {
        bool result = false;
        try
        {            
            string outputFolder = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            var fileList = Directory.GetFiles(targetFolder, "*.*", SearchOption.AllDirectories);
            if (fileList.Length == 0) return false;
            using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(outputPath)))
            {
                zipStream.Password = password;
                zipStream.SetComment(comment);
                zipStream.SetLevel(Math.Min(9, Math.Max(0, compressionLevel)));
                foreach(var file in fileList)
                {
                    FileInfo fileItem = new FileInfo(file);
                    using (FileStream readStream = fileItem.Open(FileMode.Open,FileAccess.Read, FileShare.Read))
                    {
                        var resultPath = file.Replace(targetFolder, "").Replace("\\", "/");
                        if (resultPath.StartsWith("/") || resultPath.StartsWith("\\")) resultPath = resultPath.Substring(1);
                        Debug.Log(resultPath);
                        ZipEntry zipEntry = new ZipEntry(resultPath);
                        zipEntry.DateTime = fileItem.LastWriteTime;
                        zipEntry.Size = readStream.Length;
                        zipStream.PutNextEntry(zipEntry);
                        int readLength = 0;
                        byte[] buffer = new byte[_bufferSize];
                        do
                        {
                            readLength = readStream.Read(buffer, 0, _bufferSize);
                            zipStream.Write(buffer, 0, readLength);
                        } while (readLength == _bufferSize);
                        readStream.Close();
                    }
                }
                zipStream.Flush();
                zipStream.Finish();
                zipStream.Close();
            }
            result = true;
        }
        catch (System.Exception ex)
        {
            throw new Exception("压缩文件失败", ex);
        }
        Debug.Log("done");
        return result;
    }
}