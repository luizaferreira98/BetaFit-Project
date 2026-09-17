using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetaFit.Desktop.Helpers
{
    public class ImageUrlResolver
    {
        public static string? Resolve(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            if (Uri.TryCreate(
                imageUrl,
                UriKind.Absolute,
                out var absoluteUri))
            {
                return absoluteUri.Scheme is "http" or "https"
                    ? absoluteUri.ToString()
                    : null;
            }

            var apiBaseUrl = AppConfig.ApiBaseUrl;

            if (!Uri.TryCreate(
                apiBaseUrl,
                UriKind.Absolute,
                out var baseUri))
            {
                return null;
            }

            if (baseUri.Scheme is not ("http" or "https"))
                return null;

            var relativePath = imageUrl.TrimStart('/');

            return new Uri(
                new Uri(apiBaseUrl.TrimEnd('/') + "/"),
                relativePath
            ).ToString();
        }
    }
}
