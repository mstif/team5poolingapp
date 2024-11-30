using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ApplicationUsers
{


    [JsonSerializable(typeof(SettingsUser))]
    internal partial class Context : JsonSerializerContext
    {
    }
    public class SettingsUser
    {
        public long? CompanyId { get; set; }

     
        public string ConvertToJson()
        {

            var json = JsonSerializer.Serialize(this, Context.Default.SettingsUser);
            return json;
        }


        public static SettingsUser? GetFromJson(string? setting)
        {
            if (setting == null) return null;
            return JsonSerializer.Deserialize<SettingsUser>(setting);
        }
    }


}
