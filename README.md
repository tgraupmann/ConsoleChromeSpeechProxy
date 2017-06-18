# Chrome Speech Proxy

The Chrome Speech Proxy makes it possible to use the `Speech API` on non-WebGL platforms for `Windows` and `Mac`.

# See Also

The WebGL For Speech Detection package is available in the [Unity Asset Store](https://www.assetstore.unity3d.com/en/#!/content/81076).

The WebGL For Speech Synthesis package is available in the [Unity Asset Store](https://www.assetstore.unity3d.com/en/#!/content/81861).

# Dependencies

* The [Chrome Browser](https://www.google.com/chrome/) must be installed in order to use the proxy.

# Intro

The `Chrome Speech Proxy` is a `Console` application that proxies speech data to the `Unity` engine.
The proxy supports speech detection and speech synthesis.
The proxy requires `admin` or `sudo` rights to host the `HTTP` server.
The `Chrome Browser` and `Unity` communicate with the proxy and the proxy passes data between the `Chrome Browser` and `Unity`.

# Releases

* Windows: Download ...

* Mac: Download ...

# Quick Start

1 Test your browser for compatibility with the [Chrome Speech Demo](https://www.google.com/intl/en/chrome/demos/speech.html)

2 Install and launch the proxy which creates a config file so that `Unity` knows where to find the proxy

3 Start `Unity` and connect

4 Unity proxy management is now able to `Launch` and `Close` the proxy

5 Unity proxy management can open and close `Chrome Proxy Tab` when the proxy is running

6 Close the proxy before changing the port in Unity
