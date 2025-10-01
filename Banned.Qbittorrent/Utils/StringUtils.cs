namespace Banned.Qbittorrent.Utils;

public static class StringUtils
{
    /// <summary>
    /// 使用指定分隔符连接集合中的元素。<br/>
    /// Automatically ignores null or whitespace items when the element type is string.
    /// </summary>
    /// <typeparam name="T">集合中元素的类型。</typeparam>
    /// <param name="separator">分隔符（字符串）。</param>
    /// <param name="values">要连接的集合。</param>
    /// <returns>连接后的字符串；若集合为空或全为空白则返回空字符串。</returns>
    public static string Join<T>(string separator, IEnumerable<T>? values)
    {
        if (values == null)
            return string.Empty;

        IEnumerable<string> filtered;

        if (typeof(T) == typeof(string))
        {
            // 对 string 类型专门处理空白过滤与 Trim
            filtered = values
                      .Cast<string>()
                      .Where(v => !string.IsNullOrWhiteSpace(v))
                      .Select(v => v.Trim());
        }
        else
        {
            // 其他类型，直接 ToString()
            filtered = values
                      .Where(v => v != null)
                      .Select(v => v!.ToString()!);
        }

        return string.Join(separator, filtered);
    }

    /// <summary>
    /// 使用指定分隔符连接集合中的元素。<br/>
    /// Automatically ignores null or whitespace items when the element type is string.
    /// </summary>
    /// <typeparam name="T">集合中元素的类型。</typeparam>
    /// <param name="separator">分隔符（字符）。</param>
    /// <param name="values">要连接的集合。</param>
    /// <returns>连接后的字符串；若集合为空或全为空白则返回空字符串。</returns>
    public static string Join<T>(char separator, IEnumerable<T>? values)
    {
        if (values == null)
            return string.Empty;

        IEnumerable<string> filtered;

        if (typeof(T) == typeof(string))
        {
            filtered = values
                      .Cast<string>()
                      .Where(v => !string.IsNullOrWhiteSpace(v))
                      .Select(v => v.Trim());
        }
        else
        {
            filtered = values
                      .Where(v => v != null)
                      .Select(v => v!.ToString()!);
        }

        // ✅ 直接调用 string.Join(char, IEnumerable<string>)，零分配
        return string.Join(separator, filtered);
    }
}
