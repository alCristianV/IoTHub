using IoTHubAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IoTHubAPI.Helpers
{
    public static class JsonUtils
    {
        public static List<DeviceDataFieldValue> JsonToDeviceDataFieldValues(string stringJson, IEnumerable<DeviceDataField> deviceDataFields) {
            JObject json = JObject.Parse(stringJson);
            List<DeviceDataFieldValue> values = new List<DeviceDataFieldValue>();
            foreach (var deviceDataField in deviceDataFields) {
                foreach (var field in json) {
                    string name = field.Key;
                    string value = field.Value.ToString();
                    if(deviceDataField.Name == name && IsType(value, deviceDataField.Type.ToString())) {
                        values.Add(new DeviceDataFieldValue()
                        {
                            DataField = deviceDataField,
                            Value = value
                        });
                    }
                }
            }
            return values;
        }

        private static bool IsType(string value, string type) {
            switch (type) {
                case "Numeric":
                    if (double.TryParse(value, out _)) {
                        return true;
                    }
                    break;
                case "String":
                    return true;
                case "Boolean":
                    if (bool.TryParse(value, out _)) {
                        return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }


    }
}
