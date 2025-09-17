using System;
using System.Collections;

namespace GTFS
{
  public static class GTFSFiles
  {
    /// <summary>
    /// Names of all GTFS files.
    /// </summary>
    public static readonly string[] Names = {
      "agency",
      "calendar_dates",
      "calendar",
      "fare_attributes",
      "fare_rules",
      "feed_info",
      "frequencies",
      "routes",
      "shapes",
      "stops",
      "stop_times",
      "transfers",
      "trips",
      "levels",
      "pathways"
    };

    /// <summary>
    /// From fileName get the corresponding collection from the feed.
    /// </summary>
    /// <param name="feed"></param>
    /// <param name="fileName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable GetItems<T>(this T feed, string fileName)
      where T : IGTFSFeed, new()
    {
      switch (fileName)
      {
        case "feed_info": return new[] { feed.GetFeedInfo() };

        case "agency": return feed.Agencies;
        case "calendar": return feed.Calendars;
        case "calendar_dates": return feed.CalendarDates;
        case "fare_attributes": return feed.FareAttributes;
        case "fare_rules": return feed.FareRules;
        case "frequencies": return feed.Frequencies;
        case "routes": return feed.Routes;
        case "shapes": return feed.Shapes;
        case "stops": return feed.Stops;
        case "stop_times": return feed.StopTimes;
        case "transfers": return feed.Transfers;
        case "trips": return feed.Trips;
        case "levels": return feed.Levels;
        case "pathways": return feed.Pathways;
        default:
          throw new ArgumentOutOfRangeException(nameof(fileName));
      }
    }
  }
}