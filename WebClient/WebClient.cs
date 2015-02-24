using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient
{
    public class WebClient
    {
        private readonly List<CancellationTokenSource> _cancellationTokens = new List<CancellationTokenSource>();

        public void Cancel()
        {
            lock (((ICollection)_cancellationTokens).SyncRoot)
            {
                try
                {
                    foreach (var source in _cancellationTokens.Where(source => source.Token.CanBeCanceled))
                    {
                        source.Cancel();
                        source.Dispose();
                    }
                }
                finally
                {
                    _cancellationTokens.Clear();
                }
            }
        }

        public async Task<HttpResponseMessage> DoPostAsync(string url, object parameters = null, string authorization = null, PostData postData = PostData.FormUrlEncoded)
        {
            var taskSource = new TaskCompletionSource<HttpResponseMessage>();
            var cancellationToken = CreateCancellationToken();

            if (cancellationToken.IsCancellationRequested)
            {
                taskSource.SetCanceled();
            }
            else
            {
                var httpClient = CreateHttpClient(authorization);
                var httpContent = CreateHttpContent(parameters, postData);
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var response = await httpClient.PostAsync(url, httpContent, cancellationToken);
                    taskSource.SetResult(response);
                }
                catch (OperationCanceledException)
                {
                    taskSource.SetCanceled();
                }
                catch (Exception exp)
                {
                    taskSource.SetException(exp);
                }
                finally
                {
                    RemoveCancellationToken(cancellationToken);
                }

                return await taskSource.Task;
            }

            return await taskSource.Task;
        }

        public async Task<HttpResponseMessage> DoGetAsync(string url, string authorizationString = null)
        {
            var taskSource = new TaskCompletionSource<HttpResponseMessage>();
            var cancellationToken = CreateCancellationToken();

            if (cancellationToken.IsCancellationRequested)
            {
                taskSource.SetCanceled();
            }
            else
            {
                var httpClient = CreateHttpClient(authorizationString);
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    taskSource.SetResult(response);
                }
                catch (OperationCanceledException)
                {
                    taskSource.SetCanceled();
                }
                catch (HttpRequestException e)
                {
                    if (e.InnerException is WebException)
                        taskSource.SetException(e.InnerException);
                    else
                        taskSource.SetException(e);
                }
                catch (Exception exp)
                {
                    taskSource.SetException(exp);
                }
                finally
                {
                    RemoveCancellationToken(cancellationToken);
                }

                return await taskSource.Task;
            }

            return await taskSource.Task;
        }

        public async Task<HttpResponseMessage> DoDeleteAsync(string url, string authorization = null, string etag = null)
        {
            var taskSource = new TaskCompletionSource<HttpResponseMessage>();
            var cancellationToken = CreateCancellationToken();

            if (cancellationToken.IsCancellationRequested)
            {
                taskSource.SetCanceled();
            }
            else
            {
                var httpClient = CreateHttpClient(authorization, etag);
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var response = await httpClient.DeleteAsync(url, cancellationToken);
                    taskSource.SetResult(response);
                }
                catch (OperationCanceledException)
                {
                    taskSource.SetCanceled();
                }
                catch (Exception exp)
                {
                    taskSource.SetException(exp);
                }
                finally
                {
                    RemoveCancellationToken(cancellationToken);
                }

                return await taskSource.Task;
            }

            return await taskSource.Task;
        }

        public async Task<HttpResponseMessage> DoHeadAsync(string url)
        {
            var taskSource = new TaskCompletionSource<HttpResponseMessage>();
            var cancellationToken = CreateCancellationToken();

            if (cancellationToken.IsCancellationRequested)
            {
                taskSource.SetCanceled();
            }
            else
            {
                try
                {
                    var httpClient = CreateHttpClient();
                    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, url);
                    cancellationToken.ThrowIfCancellationRequested();
                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    taskSource.SetResult(response);
                }
                catch (OperationCanceledException)
                {
                    taskSource.SetCanceled();
                }
                catch (Exception exp)
                {
                    taskSource.SetException(exp);
                }
                finally
                {
                    RemoveCancellationToken(cancellationToken);
                }

                return await taskSource.Task;
            }

            return await taskSource.Task;
        }

        private static HttpClient CreateHttpClient(string authorizationString = null, string etag = null)
        {
            var httpClientHandler = new HttpClientHandler();
            if (httpClientHandler.SupportsAutomaticDecompression)
            {
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            var httpClient = new HttpClient(httpClientHandler);
            if (!string.IsNullOrEmpty(authorizationString))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationString);
            }
            if (!string.IsNullOrEmpty(etag))
            {
                httpClient.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue(etag));
            }
            httpClient.DefaultRequestHeaders.Add("GData-Version", "2.0");
            return httpClient;
        }

        private HttpContent CreateHttpContent(object parametrs, PostData postData)
        {
            if (parametrs == null) return null;
            switch (postData)
            {
                case PostData.FormUrlEncoded:
                    var p = parametrs as Dictionary<string, string>;
                    if (p != null)
                        return new FormUrlEncodedContent(p);
                    break;
                case PostData.ImageJpeg:
                    if (parametrs.GetType() == typeof(byte[]))
                    {
                        var data = (byte[])parametrs;
                        var content = new ByteArrayContent(data);
                        content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        return content;
                    }
                    break;
                case PostData.String:
                    var s = parametrs as string;
                    if (s != null)
                    {
                        var content = new StringContent(s);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
                        return content;
                    }
                    break;
            }
            return null;
        }

        protected CancellationToken CreateCancellationToken()
        {
            lock (((ICollection)_cancellationTokens).SyncRoot)
            {
                var source = new CancellationTokenSource();
                _cancellationTokens.Add(source);
                return source.Token;
            }
        }

        protected void RemoveCancellationToken(CancellationToken token)
        {
            lock (((ICollection)_cancellationTokens).SyncRoot)
            {
                _cancellationTokens.Remove(_cancellationTokens.SingleOrDefault(s => s.Token.Equals(token)));
            }
        }
    }
}