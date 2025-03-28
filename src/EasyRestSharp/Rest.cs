namespace EasyRestSharp;

using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if NETCOREAPP
public class Rest : IDisposable
#else
public class Rest
#endif
{
    private readonly string? baseUrl;
    private readonly RestClient client;

    public Rest(string baseUrl)
    {
        this.baseUrl = baseUrl;
        client = new RestClient(baseUrl);
    }

    public Rest()
    {
        this.baseUrl = null;
        client = new RestClient();
    }

    public string? BaseUrl => baseUrl;
    public RestClient Client => client;
    public IRestAuthorization? Authorization { get; set; }

    //如果需要返回可空类型，直接给T传可空类型即可
    public Task<T> GetAsync<T>(string url, object? queryParams = null, object? headers = null)
    {
        return ExecuteWithoutBodyAsync<T>(RestMethod.Get, RestUrl.ApplyQueryParameters(url, queryParams), headers);
    }

    public Task<RestResponse> GetAsync(string url, object? queryParams = null, object? headers = null)
    {
        return ExecuteWithoutBodyAsync(RestMethod.Get, RestUrl.ApplyQueryParameters(url, queryParams), headers);
    }

    public Task<T> PostAsync<T>(string url, object? body, object? headers = null)
    {
        return ExecuteAsync<T>(RestMethod.Post, url, body, headers);
    }

    public Task<RestResponse> PostAsync(string url, object? body, object? headers = null)
    {
        return ExecuteAsync(RestMethod.Post, url, body, headers);
    }

    public Task<T> PostMultipartAsync<T>(string url, MultipartData multipart, object? headers = null)
    {
        return ExecuteWithMultipartAsync<T>(RestMethod.Post, url, multipart, headers);
    }

    public Task<RestResponse> PostMultipartAsync(string url, MultipartData multipart, object? headers = null)
    {
        return ExecuteWithMultipartAsync(RestMethod.Post, url, multipart, headers);
    }

    public Task<T> PostStringAsync<T>(string url, string body, string contentType = RestContentTypes.PlainText, object? headers = null)
    {
        return ExecuteWithStringAsync<T>(RestMethod.Post, url, body, contentType, headers);
    }

    public Task<RestResponse> PostStringAsync(string url, string body, string contentType = RestContentTypes.PlainText, object? headers = null)
    {
        return ExecuteWithStringAsync(RestMethod.Post, url, body, contentType, headers);
    }

    public Task<T> PutAsync<T>(string url, object? body, object? headers = null)
    {
        return ExecuteAsync<T>(RestMethod.Put, url, body, headers);
    }

    public Task<RestResponse> PutAsync(string url, object? body, object? headers = null)
    {
        return ExecuteAsync(RestMethod.Put, url, body, headers);
    }

    public Task<T> PatchAsync<T>(string url, object? body, object? headers = null)
    {
        return ExecuteAsync<T>(RestMethod.Patch, url, body, headers);
    }

    public Task<RestResponse> PatchAsync(string url, object? body, object? headers = null)
    {
        return ExecuteAsync(RestMethod.Patch, url, body, headers);
    }

    public Task<T> DeleteAsync<T>(string url, object? queryParams = null, object? headers = null)
    {
        return ExecuteWithoutBodyAsync<T>(RestMethod.Delete, RestUrl.ApplyQueryParameters(url, queryParams), headers);
    }

    public Task<RestResponse> DeleteAsync(string url, object? queryParams = null, object? headers = null)
    {
        return ExecuteAsync(RestMethod.Delete, RestUrl.ApplyQueryParameters(url, queryParams), headers);
    }

    public Task<T> OptionsAsync<T>(string url, object? queryParams = null, object? headers = null)
    {
        return ExecuteAsync<T>(RestMethod.Options, RestUrl.ApplyQueryParameters(url, queryParams), headers);
    }

    public Task<RestResponse> OptionsAsync(string url, object? queryParams = null, object? headers = null)
    {
        return ExecuteAsync(RestMethod.Options, RestUrl.ApplyQueryParameters(url, queryParams), headers);
    }

    public Task<T> ExecuteAsync<T>(RestMethod method, string url, object? body = null, object? headers = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddJsonBody(request, body);
        Authorization?.Authorize(request);

        return NativeExecuteAsync<T>(request);
    }

    public Task<RestResponse> ExecuteAsync(RestMethod method, string url, object? body = null, object? headers = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddJsonBody(request, body);
        Authorization?.Authorize(request);

        return NativeExecuteAsync(request);
    }

    public Task<T> ExecuteWithMultipartAsync<T>(RestMethod method, string url, MultipartData? multipart = null, object? headers = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddMultipartBody(request, multipart);
        Authorization?.Authorize(request);

        return NativeExecuteAsync<T>(request);
    }

    public Task<RestResponse> ExecuteWithMultipartAsync(RestMethod method, string url, MultipartData? multipart = null, object? headers = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddMultipartBody(request, multipart);
        Authorization?.Authorize(request);

        return NativeExecuteAsync(request);
    }

    public Task<T> ExecuteWithStringAsync<T>(RestMethod method, string url, string? body = null, string contentType = RestContentTypes.PlainText, object? headers = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddStringBody(request, body ?? string.Empty, contentType);
        Authorization?.Authorize(request);

        return NativeExecuteAsync<T>(request);
    }

    public Task<RestResponse> ExecuteWithStringAsync(RestMethod method, string url, string? body = null, string contentType = RestContentTypes.PlainText, object? headers = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddStringBody(request, body ?? string.Empty, contentType);
        Authorization?.Authorize(request);

        return NativeExecuteAsync(request);
    }

    public Task<T> ExecuteWithoutBodyAsync<T>(RestMethod method, string url, object? headers = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        Authorization?.Authorize(request);

        return NativeExecuteAsync<T>(request);
    }

    public Task<RestResponse> ExecuteWithoutBodyAsync(RestMethod method, string url, object? headers = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        Authorization?.Authorize(request);

        return NativeExecuteAsync(request);
    }

#if NETCOREAPP
    public static void ThrowIfError(RestResponse response)
#else
    public static void ThrowIfError(IRestResponse response)
#endif
    {
        var exception = response.ResponseStatus switch {
            ResponseStatus.Aborted => new WebException("Request aborted", response.ErrorException),
            ResponseStatus.Error => response.ErrorException,
            ResponseStatus.TimedOut => new TimeoutException("Request timed out", response.ErrorException),
            ResponseStatus.None => null,
            ResponseStatus.Completed => null,
            _ => throw response.ErrorException ?? new ArgumentOutOfRangeException()
        };

        if (exception != null)
            throw exception;

        if (!response.IsSuccessful) {
            throw new Exception($"description:{response.StatusDescription} content:{response.Content}");
        }
    }

    private Method ToNativeMethod(RestMethod rm)
    {
#if NETCOREAPP
        return rm switch {
            RestMethod.Get => Method.Get,
            RestMethod.Post => Method.Post,
            RestMethod.Put => Method.Put,
            RestMethod.Delete => Method.Delete,
            RestMethod.Options => Method.Options,
            RestMethod.Patch => Method.Patch,
            _ => throw new ArgumentOutOfRangeException(nameof(rm), rm, null)
        };
#else
        return rm switch {
            RestMethod.Get => Method.GET,
            RestMethod.Post => Method.POST,
            RestMethod.Put => Method.PUT,
            RestMethod.Delete => Method.DELETE,
            RestMethod.Options => Method.OPTIONS,
            RestMethod.Patch => Method.PATCH,
            _ => throw new ArgumentOutOfRangeException(nameof(rm), rm, null)
        };
#endif
    }

    private async Task<T> NativeExecuteAsync<T>(RestRequest request, CancellationToken token = default)
    {
#if NETCOREAPP
        var response = await client.ExecuteAsync<T>(request).ConfigureAwait(false);
        ThrowIfError(response);
        return response.Data!;
#else
        var response = await client.ExecuteAsync<T>(request, token).ConfigureAwait(false);
        var responseT = response as RestResponse<T> ?? throw new Exception("Invalid response type");
        ThrowIfError(responseT);
        return responseT.Data!;
#endif
    }

    private async Task<RestResponse> NativeExecuteAsync(RestRequest request, CancellationToken token = default)
    {
#if NETCOREAPP
        return await client.ExecuteAsync(request, token).ConfigureAwait(false);
#else
        var response = await client.ExecuteAsync(request, token).ConfigureAwait(false);
        return response as RestResponse ?? throw new Exception("Invalid response type");
#endif
    }


    private static void AddHeaders(RestRequest request, object? headers)
    {
        if (headers == null) return;

        var props = RestUtils.GetNameValues(headers);
        foreach (var nameValue in props) {
            if (nameValue.Value == null) throw new Exception("Header value is not permit to be null");

#if NETCOREAPP
            var p = new HeaderParameter(nameValue.Name, nameValue.Value, false);
#else
            var p = new Parameter(nameValue.Name, nameValue.Value, ParameterType.HttpHeader, false);
#endif
            request.AddParameter(p);
        }
    }

    private static void AddJsonBody(RestRequest request, object? body)
    {
        // body should be able to post null value, but restsharp does not support it, so handle it here
        if (body == null) {
            AddStringBody(request, "null", RestContentTypes.Json);
            return;
        }

#if NETCOREAPP
        request.AddParameter(new JsonParameter(body, ContentType.Json));
#else
        request.AddJsonBody(body);
#endif
    }

    private static void AddStringBody(RestRequest request, string body, string contentType)
    {
#if NETCOREAPP
        var ct = contentType;
        request.AddStringBody(body, ct);
#else
        var bodyParam = new Parameter(string.Empty, body, contentType, ParameterType.RequestBody);
        bodyParam.DataFormat = DataFormat.None;
        request.Parameters.Add(bodyParam);
#endif
    }

    private static void AddMultipartBody(RestRequest request, MultipartData? multipart)
    {
        if (multipart == null) return;

#if NETCOREAPP
        foreach (var part in multipart.Parts) {
            if (part is ByteArrayPart bp) {
                request.AddFile(bp.Name, bp.Bytes, bp.FileName, bp.ContentType);
            }
            else if (part is StreamPart streamPart) {
                request.AddFile(streamPart.Name,
                    () => streamPart.Stream,
                    streamPart.FileName,
                    streamPart.ContentType);
            }
            else if (part is FilePart fp) {
                request.AddFile(fp.Name, fp.FilePath, fp.ContentType);
            }
            else if (part is StringPart strPart) {
                request.AddParameter(strPart.Name, strPart.Value);
            }
        }
#else
        foreach (var part in multipart.Parts) {
            if (part is ByteArrayPart bp) {
                request.AddFileBytes(bp.Name, bp.Bytes, bp.FileName, bp.ContentType ?? RestContentTypes.OctetStream);
            }
            else if (part is StreamPart streamPart) {
                request.AddFile(streamPart.Name,
                    new Action<Stream>(s => streamPart.Stream.CopyTo(s)),
                    streamPart.FileName,
                    streamPart.Stream.Length,
                    streamPart.ContentType ?? RestContentTypes.OctetStream);
            }
            else if (part is FilePart fp) {
                var fi = new FileInfo(fp.FilePath);
                request.Files.Add(new FileParameter {
                    Name = fp.Name,
                    FileName = fp.FileName,
                    ContentLength = fi.Length,
                    Writer = s => {
                        using var fs = new FileStream(fp.FilePath, FileMode.Open, FileAccess.Read);
                        fs.CopyTo(s);
                    },
                    ContentType = fp.ContentType ?? RestContentTypes.OctetStream
                });
            }
            else if (part is StringPart strPart) {
                request.AddParameter(strPart.Name, strPart.Value!);
            }
        }
#endif
    }

#if NETCOREAPP
    /************ Dispose **********/
    public void Dispose()
    {
        client.Dispose();
        GC.SuppressFinalize(this);
    }
#endif
}
