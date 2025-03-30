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

public struct StringPart : IRestPart
{
    public StringPart(string name, string? value)
    {
        Name = name;
        Value = value;
    }
    public string Name { get; private set; }
    public string? Value { get; private set; }
    public string? ContentType => RestContentTypes.PlainText;
}

public struct ByteArrayPart : IRestFilePart
{
    public ByteArrayPart(string name, byte[] bytes, string fileName, string? contentType = null)
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

public struct StreamPart : IRestFilePart
{
    public StreamPart(string name, System.IO.Stream stream, string fileName, string? contentType = null)
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

public struct FilePart : IRestFilePart
{
    public FilePart(string name, string filePath, string fileName, string? contentType = null)
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

public class MultipartData
{
    public MultipartData()
    {
        Parts = new List<IRestPart>();
    }

    public List<IRestPart> Parts { get; private set; }

    public void AddString(string name, string? value)
    {
        Parts.Add(new StringPart(name, value));
    }

    public void AddByteArray(string name, byte[] value, string fileName, string? contentType = null)
    {
        Parts.Add(new ByteArrayPart(name, value, fileName, contentType));
    }

    public void AddStream(string name, System.IO.Stream stream, string fileName, string? contentType = null)
    {
        Parts.Add(new StreamPart(name, stream, fileName, contentType));
    }

    public void AddFile(string name, string filePath, string fileName, string? contentType = null)
    {
        Parts.Add(new FilePart(name, filePath, fileName, contentType));
    }

    public void AddFile(string name, string filePath, string? contentType = null)
    {
        Parts.Add(new FilePart(name, filePath, Path.GetFileName(filePath), contentType));
    }
}