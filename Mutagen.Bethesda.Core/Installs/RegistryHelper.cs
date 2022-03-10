using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Noggog;

namespace Mutagen.Bethesda.Installs
{
    internal static class RegistryHelper
    {
        private static GetResponse<object> GetObjectFromRegistry(RegistryKey key, string valueName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var value = key.GetValue(valueName);
                return value == null 
                    ? GetResponse<object>.Fail($"RegistryKey {key} does not have value {valueName}!") 
                    : GetResponse<object>.Succeed(value);
            }
            throw new NotImplementedException();
        }
        
        internal static GetResponse<string> GetStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var objectRes = GetObjectFromRegistry(key, valueName);
            if (objectRes.Failed)
            {
                return objectRes.BubbleFailure<string>();
            }
            
            var sValue = objectRes.Value.ToString() ?? string.Empty;
            
            return string.IsNullOrEmpty(sValue) 
                ? GetResponse<string>.Fail($"Value {valueName} in RegistryKey {key} is null or empty!") 
                : GetResponse<string>.Succeed(sValue);
        }
    }
}