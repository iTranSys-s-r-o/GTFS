// The MIT License (MIT)

// Copyright (c) 2014 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;

namespace GTFS.IO
{
  /// <summary>
  /// Represents a GTFS directory target.
  /// </summary>
  public class GTFSDirectoryTarget : IEnumerable<IGTFSTargetFile>
  {
    private readonly DirectoryInfo _directory;
    private readonly List<IGTFSTargetFile> _targets;

    /// <summary>
    /// Creates a new GTFS directory target.
    /// </summary>
    /// <param name="directory"></param>
    public GTFSDirectoryTarget(DirectoryInfo directory)
    {
      _directory = directory;
      _targets = new List<IGTFSTargetFile>();
    }

    /// <summary>
    /// Builds the new target files.
    /// </summary>
    public void BuildTargets(Func<string, bool>? addTarget = null)
    {
      if (_targets.Count == 0)
      {
        // write files on-by-one.

        foreach (var targetName in GTFSFiles.Names)
        {
          if (addTarget == null || addTarget(targetName)) 
            _targets.Add(new GTFSTargetFileStream(OpenWrite(_directory.FullName, targetName), targetName));
        }
      }
    }

    /// <summary>
    /// Opens a file for writing.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private Stream OpenWrite(string path, string name)
    {
      return File.Open(Path.Combine(path, name + ".txt"), FileMode.Create);
    }

    /// <summary>
    /// Returns the enumerator.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IGTFSTargetFile> GetEnumerator()
    {
      this.BuildTargets();
      return _targets.GetEnumerator();
    }

    /// <summary>
    /// Returns the enumerator.
    /// </summary>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      this.BuildTargets();
      return _targets.GetEnumerator();
    }
  }
}