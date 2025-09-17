using System;
using System.Collections;

namespace GTFS
{
  public static class EnumerableExtensions
  {
    public static bool HasAny(this IEnumerable? source)
    {
      if (source == null) 
        return false;
      
      var enumerator = source.GetEnumerator();
      
      using var enumerator1 = enumerator as IDisposable;

      return enumerator.MoveNext();
    }
  }
}
