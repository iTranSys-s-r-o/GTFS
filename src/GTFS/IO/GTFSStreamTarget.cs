using System;
using System.Collections.Generic;
using System.IO;

namespace GTFS.IO
{
  /// <summary>
  /// Implemented by: J. Tomana
  /// Represents a collection of GTFS target files that can be built, enumerated, and exported as a ZIP archive.
  /// </summary>
  /// <remarks>GTFSStreamTarget provides methods to construct in-memory GTFS files, enumerate them, and package
  /// them into a ZIP format suitable for distribution or storage. The class implements <see
  /// cref="IEnumerable{IGTFSTargetFile}"/> for file enumeration and <see cref="IDisposable"/> to release resources
  /// associated with the target files. Instances are not thread-safe and should not be accessed concurrently from
  /// multiple threads.</remarks>
  public class GTFSStreamTarget : IEnumerable<IGTFSTargetFile>, IDisposable
  {
    private readonly List<GTFSTargetMemoryStream> _targets = new List<GTFSTargetMemoryStream>();

    /// <summary>
    /// Builds the new target files.
    /// </summary>
    public void BuildTargets(Func<string, bool>? addTarget = null)
    {
      if (_targets.Count == 0)
      {
        foreach (var targetName in GTFSFiles.Names)
        {
          if (addTarget == null || addTarget(targetName))
            _targets.Add(new GTFSTargetMemoryStream(targetName));
        }
      }
    }

    /// <summary>
    /// Returns the enumerator.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IGTFSTargetFile> GetEnumerator()
    {
      BuildTargets();
      return _targets.GetEnumerator();
    }

    /// <summary>
    /// Returns the enumerator.
    /// </summary>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      BuildTargets();
      return _targets.GetEnumerator();
    }

    public byte[] ToZip()
    {
      using var zipStream = new MemoryStream();
      using (var archive =
             new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
      {
        foreach (var target in _targets)
        {
          var entry = archive.CreateEntry(target.Name + ".txt");
          using var entryStream = entry.Open();

          var data = target.ToZip();
          if (data == null)
            continue;

          entryStream.Write(data, 0, data.Length);

          target.Close();
        }
      }

      zipStream.Position = 0;
      zipStream.Flush();

      return zipStream.ToArray();
    }

    public void Dispose() =>
      _targets.ForEach(t => t.Close());
  }
}