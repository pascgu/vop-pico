using System.Web;
using VopPico.App.Models;

namespace VopPico.App.Interfaces;

public static class JsTools
{
    public static string Encode(string message)
    {
#if ANDROID
        // On Android we don't encode, because it seems to double escape characters.
        return message;
#else
        return HttpUtility.JavaScriptStringEncode(message);
#endif
    }

    public static async Task InvokeJsAsync(HybridWebView hwv, string function, params object?[] parameters)
    {
        await hwv.Dispatcher.DispatchAsync(async () =>
        {
            string js_params = string.Join(",", parameters.Select(p => p is null ? "null" : $"""{p.ToString()}"""));
            string js_func = $"{function}({js_params})";
            Console.WriteLine($"InvokeJsAsync: {js_func}");
            // create paramValues from parameters values
            List<object?> paramValuesList = new();
            // create paramJsonTypeInfos from parameters types
            List<System.Text.Json.Serialization.Metadata.JsonTypeInfo?> paramJsonTypeInfos = new();
            foreach(object? p in parameters)
            {
                object? p_value = p;
                System.Text.Json.Serialization.Metadata.JsonTypeInfo? p_type = null;
                if (p != null)
                    p_type = VopHybridJSContext.Default.GetTypeInfo(p.GetType());
                switch (p)
                {
                    case string s:
                        p_value = JsTools.Encode(s);
                        break;
                }
                paramValuesList.Add(p_value);
                paramJsonTypeInfos.Add(p_type);
            }
           
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            await hwv.InvokeJavaScriptAsync<object>(
                function, // ex: "window.logMessage" or "window.vopHost.receiveDataFromDevice"
                null, // use it if no return type (void)
                paramValuesList.ToArray(),
                paramJsonTypeInfos.ToArray()
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        });
    }
}
