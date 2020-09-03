using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace XtraUpload.Domain
{
    public interface IWritableOptions<out T> : IOptionsSnapshot<T> where T : class, new()
    {
        Task Update(Action<T> applyChanges);
    }
    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly IFileProvider _fileProvider;
        private readonly IOptionsMonitor<T> _options;
        private readonly string _section;
        private readonly string _file;

        public WritableOptions(
            IFileProvider fileProvider,
            IOptionsMonitor<T> options,
            string section,
            string file)
        {
            _fileProvider = fileProvider;
            _options = options;
            _section = section;
            _file = file;
        }

        public T Value => _options.CurrentValue;
        public T Get(string name) => _options.Get(name);

        public async Task Update(Action<T> applyChanges)
        {
            var fileInfo = _fileProvider.GetFileInfo(_file);
            var physicalPath = fileInfo.PhysicalPath;

            var jObject = JsonConvert.DeserializeObject<JObject>(await File.ReadAllTextAsync(physicalPath));
            var sectionObject = jObject.TryGetValue(_section, out JToken section) ?
                JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

            applyChanges(sectionObject);

            jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
            await File.WriteAllTextAsync(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }
    }
    
}
