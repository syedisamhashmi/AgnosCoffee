using System;

namespace AgnosCoffee.Data;

public static class Utils
{
  /// <summary>
  /// Rounds a decimal value to two digits.
  /// </summary>
  public static decimal RoundToTwo(this decimal value)
  {
    return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
  }
  /// <summary>
  /// Rounds a decimal value to two digits and
  /// formats itself as a string.
  /// </summary>
  /// <returns>A two digit decimal string representation of a decimal.</returns>
  public static string FormatToTwo(this decimal value)
  {
    return RoundToTwo(value).ToString("0.##");
  }
}