using System;
using System.IO;
using GTFS.IO;

namespace GTFS
{
  /// <summary>
  /// Contains extension methods for the GTFS writer.
  /// </summary>
  public static class GTFSWriterExtensions
  {
    /// <summary>
    /// Writes a GTFS feed.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="feed">The feed.</param>
    /// <param name="path">The path.</param>
    /// <param name="includeEmptyFiles">export empty files?</param>
    /// <typeparam name="T">The feed type.</typeparam>
    public static void Write<T>(this GTFSWriter<T> writer, T feed, string path, bool includeEmptyFiles = false) where T : IGTFSFeed, new()
    {
      if (path == null) throw new ArgumentNullException(nameof(path));

      if (Directory.Exists(path))
      {
        var target = new GTFSDirectoryTarget(new DirectoryInfo(path));

        target.BuildTargets(targetName => includeEmptyFiles || feed.GetItems(targetName).HasAny());

        writer.Write(feed, target, includeEmptyFiles);
        return;
      }

      throw new ArgumentException("Could not write GTFS feed, directory not found.", nameof(path));
    }
  }
}