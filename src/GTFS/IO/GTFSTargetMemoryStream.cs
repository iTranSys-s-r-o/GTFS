using System.Collections.Generic;
using System.IO;
using GTFS.IO.CSV;

namespace GTFS.IO
{
  /// <summary>
  /// Implemented by: J. Tomana
  /// Represents an in-memory target file for GTFS data, allowing data to be written, cleared, and exported as a ZIP
  /// archive.
  /// </summary>
  /// <remarks>This class provides a memory-based implementation of the IGTFSTargetFile interface, storing GTFS
  /// data rows in memory and supporting export to a ZIP-formatted byte array. It is useful for scenarios where GTFS
  /// data needs to be manipulated or accessed without persisting to disk.</remarks>
  public class GTFSTargetMemoryStream : IGTFSTargetFile
  {
    public GTFSTargetMemoryStream(string name) =>
      Name = name;

    public string Name { get; }

    public List<string[]> Data { get; } = new List<string[]>();

    public bool Exists => true;

    public void Clear()
    {
    }

    public void Write(string[] data) =>
      Data.Add((string[])data.Clone());

    public void Close()
    {
    }

    public byte[] ToZip()
    {
      using var memoryStream = new MemoryStream();
      CSVStreamWriter sw = new CSVStreamWriter(memoryStream);

      try
      {
        foreach (var data in Data)
          sw.Write(data);
        sw.Flush();

        memoryStream.Position = 0;
        memoryStream.Flush();

        return memoryStream.ToArray();
      }
      finally
      {
        sw.Dispose();
      }
    }
  }
}