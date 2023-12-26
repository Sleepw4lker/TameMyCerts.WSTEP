## Changelog for the TameMyCerts certificate enrollment proxy

### Version 1.1.359.1211

_This version was released on Dec 26, 2023._

- Migrate from .NET Framework 4.7.2 to .NET Core 8.0 utilizing CoreWCF.
- Automatic Registration Authority certificate detection now honors the target certification authority as issuer.
- Request attributes received by the client are now transformed (with a "wstep" prefix) and transmitted to the REST API (and thus the certification authority).
- A request attribute containing the proxy server host name ("wstepproxy") is transmitted to the REST API (and thus the certification authority).
- The "cn" attribute of the requesting account is transferred into a "CN" request attribute and transmitted to the REST API (and thus the certification authority). This allows to issue certificates with a properly populated Subject Distinguished Name if you enable the _CRLF_ALLOW_REQUEST_ATTRIBUTE_SUBJECT_ flag on the certification authority. But **do this at your own risk**, as enabling the flag can potentially be dangerous.
- Denials of certificate requests that origin on the certificate enrollment proxy now contain meaningful error codes that are sent back to the requesting client.
- Answers from the REST API are now compared case-insensitive, thus making the proxy compatible with the TameCerts REST API version 1.1 and above.

### Version 1.0.345.1176

_This version was released on Dec 12, 2023._

This is the initial release made publicly available.