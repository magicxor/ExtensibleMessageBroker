using System;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Emb.Configurator.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand GenerateSchemaCommand { get; set; }
        public RelayCommand ValidateCommand { get; set; }
        public RelayCommand EscapeCommand { get; set; }
        public RelayCommand UnescapeCommand { get; set; }

        public string Class { get; set; }
        public string Schema { get; set; }
        public string Json { get; set; }

        private const string ClassRegex = @"(?s)class\s+(\w+).*?{";
        
        public MainViewModel()
        {
            GenerateSchemaCommand = new RelayCommand(GenerateSchema);
            ValidateCommand = new RelayCommand(Validate);
            EscapeCommand = new RelayCommand(Escape);
            UnescapeCommand = new RelayCommand(Unescape);
        }

        private async void GenerateSchema()
        {
            var matches = Regex.Matches(Class, ClassRegex, RegexOptions.Multiline);
            if (matches.Count == 1)
            {
                var firstMatch = matches[0];
                if (firstMatch.Groups.Count == 2)
                {
                    var className = firstMatch.Groups[1];
                    var classType = await CSharpScript.EvaluateAsync<Type>(Class + $" return typeof({className});", 
                        ScriptOptions.Default
                            .WithImports("System.IO")
                            .WithImports("System.Collections.Generic"));
                    var generator = new JSchemaGenerator();
                    var schema = generator.Generate(classType);
                    Schema = schema.ToString();
                }
                else
                {
                    // todo: throw
                }
            }
            else
            {
                // todo: throw
            }
        }

        private void Validate()
        {
            var schema = JSchema.Parse(Schema);
            var json = JObject.Parse(Json);
            if (json.IsValid(schema))
            {
                // todo: ok
            }
            else
            {
                // todo: failed
            }
        }

        private void Escape()
        {
            var value = JsonConvert.ToString(Json);
            Json = value;
        }

        private void Unescape()
        {
            var value = JsonConvert.DeserializeObject<string>(Json);
            Json = value;
        }
    }
}