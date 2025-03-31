namespace EasyRestSharp;

using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if NET6_0_OR_GREATER
public class EasyRestClient : IDisposable
#else
public class EasyRestClient
#endif
{
    private readonly string? baseUrl;
    private readonly RestClient client;

    public EasyRestClient(string baseUrl)
    {
        this.baseUrl = baseUrl;
        client = new RestClient(baseUrl);
    }

    public EasyRestClient()
    {
        this.baseUrl = null;
        client = new RestClient();
    }

    public string? BaseUrl => baseUrl;
    public RestClient Client => client;
    public IAuthenticator? Authenticator { get; set; }

    //如果需要返回可空类型，直接给T传可空类型即可
    public Task<T> GetAsync<T>(string url, object? queryParams = null,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteWithoutBodyAsync<T>(RestMethod.Get, RestUrl.ApplyQueryParameters(url, queryParams), headers, authenticator);
    }

    public Task<RestResponse> GetAsync(string url, object? queryParams = null, 
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteWithoutBodyAsync(RestMethod.Get, RestUrl.ApplyQueryParameters(url, queryParams), headers, authenticator);
    }

    public Task<T> PostAsync<T>(string url, object? body,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync<T>(RestMethod.Post, url, body, headers, authenticator);
    }

    public Task<RestResponse> PostAsync(string url, object? body,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync(RestMethod.Post, url, body, headers, authenticator);
    }

    public Task<T> PostMultipartAsync<T>(string url, RestMultipart multipart,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteWithMultipartAsync<T>(RestMethod.Post, url, multipart, headers, authenticator);
    }

    public Task<RestResponse> PostMultipartAsync(string url, RestMultipart multipart,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteWithMultipartAsync(RestMethod.Post, url, multipart, headers, authenticator);
    }

    public Task<T> PostStringAsync<T>(string url, string body, string contentType = RestContentTypes.PlainText,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteWithStringAsync<T>(RestMethod.Post, url, body, contentType, headers, authenticator);
    }

    public Task<RestResponse> PostStringAsync(string url, string body, string contentType = RestContentTypes.PlainText,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteWithStringAsync(RestMethod.Post, url, body, contentType, headers, authenticator);
    }

    public Task<T> PutAsync<T>(string url, object? body,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync<T>(RestMethod.Put, url, body, headers, authenticator);
    }

    public Task<RestResponse> PutAsync(string url, object? body, 
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync(RestMethod.Put, url, body, headers, authenticator);
    }

    public Task<T> PatchAsync<T>(string url, object? body, 
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync<T>(RestMethod.Patch, url, body, headers, authenticator);
    }

    public Task<RestResponse> PatchAsync(string url, object? body, 
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync(RestMethod.Patch, url, body, headers, authenticator);
    }

    public Task<T> DeleteAsync<T>(string url, object? queryParams = null, 
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteWithoutBodyAsync<T>(RestMethod.Delete, RestUrl.ApplyQueryParameters(url, queryParams), headers, authenticator);
    }

    public Task<RestResponse> DeleteAsync(string url, object? queryParams = null, 
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync(RestMethod.Delete, RestUrl.ApplyQueryParameters(url, queryParams), headers, authenticator);
    }

    public Task<T> OptionsAsync<T>(string url, object? queryParams = null,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync<T>(RestMethod.Options, RestUrl.ApplyQueryParameters(url, queryParams), headers, authenticator);
    }

    public Task<RestResponse> OptionsAsync(string url, object? queryParams = null, 
        object? headers = null, IAuthenticator? authenticator = null)
    {
        return ExecuteAsync(RestMethod.Options, RestUrl.ApplyQueryParameters(url, queryParams), headers, authenticator);
    }

    public Task<T> ExecuteAsync<T>(RestMethod method, string url, object? body = null, 
        object? headers = null, IAuthenticator? authenticator = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddJsonBody(request, body);
        (authenticator ?? Authenticator)?.Authenticate(client, request);

        return NativeExecuteAsync<T>(request);
    }

    public Task<RestResponse> ExecuteAsync(RestMethod method, string url, object? body = null,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddJsonBody(request, body);
        (authenticator ?? Authenticator)?.Authenticate(client, request);

        return NativeExecuteAsync(request);
    }

    public Task<T> ExecuteWithMultipartAsync<T>(RestMethod method, string url, RestMultipart multipart,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddMultipartBody(request, multipart);
        (authenticator ?? Authenticator)?.Authenticate(client, request);

        return NativeExecuteAsync<T>(request);
    }

    public Task<RestResponse> ExecuteWithMultipartAsync(RestMethod method, string url, RestMultipart multipart,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddMultipartBody(request, multipart);
        (authenticator ?? Authenticator)?.Authenticate(client, request);

        return NativeExecuteAsync(request);
    }

    public Task<T> ExecuteWithStringAsync<T>(RestMethod method, string url, string body, string contentType = RestContentTypes.PlainText,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddStringBody(request, body ?? string.Empty, contentType);
        (authenticator ?? Authenticator)?.Authenticate(client, request);

        return NativeExecuteAsync<T>(request);
    }

    public Task<RestResponse> ExecuteWithStringAsync(RestMethod method, string url, string body, string contentType = RestContentTypes.PlainText,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        AddStringBody(request, body ?? string.Empty, contentType);
        (authenticator ?? Authenticator)?.Authenticate(client, request);

        return NativeExecuteAsync(request);
    }

    public Task<T> ExecuteWithoutBodyAsync<T>(RestMethod method, string url,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        (authenticator ?? Authenticator)?.Authenticate(client, request);

        return NativeExecuteAsync<T>(request);
    }

    public Task<RestResponse> ExecuteWithoutBodyAsync(RestMethod method, string url,
        object? headers = null, IAuthenticator? authenticator = null)
    {
        var request = new RestRequest(url, ToNativeMethod(method));
        AddHeaders(request, headers);
        (authenticator ?? Authenticator)?.Authenticate(client, request);

        return NativeExecuteAsync(request);
    }

    private Method ToNativeMethod(RestMethod rm)
    {
#if NET6_0_OR_GREATER
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
#if NET6_0_OR_GREATER
        var response = await client.ExecuteAsync<T>(request).ConfigureAwait(false);
        ThrowIfError(response);
        if (!response.IsSuccessful) {
            throw new Exception($"Status code: {response.StatusCode} Description:{response.StatusDescription}");
        }

        return response.Data!;
#else
        var response = await client.ExecuteAsync<T>(request, token).ConfigureAwait(false);
        ThrowIfError(response!);
        if (!response.IsSuccessful) {
            throw new Exception($"Status code: {response.StatusCode} Description:{response.StatusDescription}");
        }
        var responseT = response as RestResponse<T> ?? throw new Exception("Invalid response type");
        return responseT.Data!;
#endif
    }

    private async Task<RestResponse> NativeExecuteAsync(RestRequest request, CancellationToken token = default)
    {
#if NET6_0_OR_GREATER
        var response = await client.ExecuteAsync(request, token).ConfigureAwait(false);
        ThrowIfError(response);
        return response;
#else
        var response = await client.ExecuteAsync(request, token).ConfigureAwait(false);
        ThrowIfError(response);
        return response as RestResponse ?? throw new Exception("Invalid response type");
#endif
    }

#if NET6_0_OR_GREATER
    private static void ThrowIfError(RestResponse response)
#else
    private static void ThrowIfError(IRestResponse response)
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
    }

    private static void AddHeaders(RestRequest request, object? headers)
    {
        if (headers == null) return;

        var props = RestUtils.GetNameValues(headers);
        foreach (var nameValue in props) {
            if (nameValue.Value == null) throw new Exception("Header value is not permitted to be null");

#if NET6_0_OR_GREATER
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

#if NET6_0_OR_GREATER
        request.AddParameter(new JsonParameter(body, ContentType.Json));
#else
        request.AddJsonBody(body);
#endif
    }

    private static void AddStringBody(RestRequest request, string body, string contentType)
    {
#if NET6_0_OR_GREATER
        var ct = contentType;
        request.AddStringBody(body, ct);
#else
        var bodyParam = new Parameter(string.Empty, body, contentType, ParameterType.RequestBody);
        bodyParam.DataFormat = DataFormat.None;
        request.Parameters.Add(bodyParam);
#endif
    }

    private static void AddMultipartBody(RestRequest request, RestMultipart? multipart)
    {
        if (multipart == null) return;

#if NET6_0_OR_GREATER
        foreach (var part in multipart.Parts) {
            if (part is RestByteArrayPart bp) {
                request.AddFile(bp.Name, bp.Bytes, bp.FileName, bp.ContentType);
            }
            else if (part is RestStreamPart streamPart) {
                request.AddFile(streamPart.Name,
                    () => streamPart.Stream,
                    streamPart.FileName,
                    streamPart.ContentType);
            }
            else if (part is RestFilePart fp) {
                request.AddFile(fp.Name, fp.FilePath, fp.ContentType);
            }
            else if (part is RestStringPart strPart) {
                request.AddParameter(strPart.Name, strPart.Value);
            }
        }
#else
        foreach (var part in multipart.Parts) {
            if (part is RestByteArrayPart bp) {
                request.AddFileBytes(bp.Name, bp.Bytes, bp.FileName, bp.ContentType ?? RestContentTypes.OctetStream);
            }
            else if (part is RestStreamPart streamPart) {
                request.AddFile(streamPart.Name,
                    new Action<Stream>(s => streamPart.Stream.CopyTo(s)),
                    streamPart.FileName,
                    streamPart.Stream.Length,
                    streamPart.ContentType ?? RestContentTypes.OctetStream);
            }
            else if (part is RestFilePart fp) {
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
            else if (part is RestStringPart strPart) {
                request.AddParameter(strPart.Name, strPart.Value!);
            }
        }
#endif
    }

#if NET6_0_OR_GREATER
    /************ Dispose **********/
    public void Dispose()
    {
        client.Dispose();
        GC.SuppressFinalize(this);
    }
#endif
}
