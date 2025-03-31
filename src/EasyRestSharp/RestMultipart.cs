namespace EasyRestSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IRestPart
{
    string Name { get; }
    string? ContentType { get; }
}

public interface IRestFilePart : IRestPart
{
    string FileName { get; }
}

public struct RestStringPart : IRestPart
{
    public RestStringPart(string name, string? value)
    {
        Name = name;
        Value = value;
    }
    public string Name { get; private set; }
    public string? Value { get; private set; }
    public string? ContentType => RestContentTypes.PlainText;
}

public struct RestByteArrayPart : IRestFilePart
{
    public RestByteArrayPart(string name, byte[] bytes, string fileName, string? contentType = null)
    {
        Name = name;
        Bytes = bytes;
        FileName = fileName;
        ContentType = contentType;
    }

    public string Name { get; private set; }
    public byte[] Bytes { get; private set; }
    public string FileName { get; private set; }
    public string? ContentType { get; private set; }
}

public struct RestStreamPart : IRestFilePart
{
    public RestStreamPart(string name, System.IO.Stream stream, string fileName, string? contentType = null)
    {
        Name = name;
        Stream = stream;
        FileName = fileName;
        ContentType = contentType;
    }

    public string Name { get; private set; }
    public System.IO.Stream Stream { get; private set; }
    public string FileName { get; private set; }
    public string? ContentType { get; private set; }
}

public struct RestFilePart : IRestFilePart
{
    public RestFilePart(string name, string filePath, string fileName, string? contentType = null)
    {
        Name = name;
        FilePath = filePath;
        FileName = fileName;
        ContentType = contentType ?? RestContentTypes.OctetStream;
    }

    public string Name { get; private set; }
    public string FilePath { get; private set; }
    public string FileName { get; private set; }
    public string? ContentType { get; private set; }
}

public class RestMultipart
{
    public RestMultipart()
    {
        Parts = new List<IRestPart>();
    }

    public List<IRestPart> Parts { get; private set; }

    public void AddString(string name, string? value)
    {
        Parts.Add(new RestStringPart(name, value));
    }

    public void AddByteArray(string name, byte[] value, string fileName, string? contentType = null)
    {
        Parts.Add(new RestByteArrayPart(name, value, fileName, contentType));
    }

    public void AddStream(string name, System.IO.Stream stream, string fileName, string? contentType = null)
    {
        Parts.Add(new RestStreamPart(name, stream, fileName, contentType));
    }

    public void AddFile(string name, string filePath, string fileName, string? contentType = null)
    {
        Parts.Add(new RestFilePart(name, filePath, fileName, contentType));
    }

    public void AddFile(string name, string filePath)
    {
        Parts.Add(new RestFilePart(name, filePath, Path.GetFileName(filePath), null));
    }
}