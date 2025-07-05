namespace WebApplication1
{
    public class HostOverrideHandler : DelegatingHandler
    {
        private readonly string _host;
        private readonly string _ip;

        public HostOverrideHandler(string host, string ip)
        {
            _host = host;
            _ip = ip;
            InnerHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Reemplazamos el host por la IP, pero mantenemos el Host header original
            request.RequestUri = new UriBuilder(request.RequestUri)
            {
                Host = _ip
            }.Uri;

            request.Headers.Host = _host;

            return base.SendAsync(request, cancellationToken);
        }
    }
}
