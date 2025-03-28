namespace EasyRestSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class RestUrl
{
    public static string ApplyQueryParameters(string url, object? queryParameters)
    {
        if (queryParameters == null) return url;
        var nvList = RestUtils.GetNameValues(queryParameters)
            .Select(e => {
                var name = e.Name != null ? RestUtils.UrlEncode(e.Name) : e.Name;
                var val = e.Value != null ? RestUtils.UrlEncode(e.Value) : e.Value;
                return val == null ? name : $"{name}={val}";
            });
        var query = string.Join("&", nvList);
        return $"{url}?{query}";
    }

    public static string ApplySegmentParameters(string url, object? segmentParameters)
    {
        if (segmentParameters == null) return url;

        var nvList = RestUtils.GetNameValues(segmentParameters);
        foreach (var nv in nvList) {
            if (nv.Value == null) throw new Exception("value in segment can't be null");
            url = url.Replace($"{{{nv.Name}}}", RestUtils.UrlEncode(nv.Value));
        }
        return url;
    }
}
