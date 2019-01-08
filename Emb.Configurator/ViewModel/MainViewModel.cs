using System;
using System.Text.RegularExpressions;
using Emb.Configurator.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using PropertyChanged;

namespace Emb.Configurator.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        private readonly WindowService _windowService;

        public RelayCommand<MainWindow> GenerateSchemaCommand { get; set; }
        public RelayCommand<MainWindow> ValidateCommand { get; set; }
        public RelayCommand EscapeCommand { get; set; }
        public RelayCommand UnescapeCommand { get; set; }

        public string Class { get; set; }
        public string Schema { get; set; }
        public string Json { get; set; }
        
        private const string ClassRegex = @"(?s)class\s+(\w+).*?{";
        
        public MainViewModel(WindowService windowService)
        {
            _windowService = windowService;

            GenerateSchemaCommand = new RelayCommand<MainWindow>(GenerateSchema);
            ValidateCommand = new RelayCommand<MainWindow>(Validate);
            EscapeCommand = new RelayCommand(Escape);
            UnescapeCommand = new RelayCommand(Unescape);
        }

        private async void GenerateSchema(MainWindow mainWindow)
        {
            void ShowError()
            {
                _windowService.ShowDialogWindow(mainWindow, new DialogViewModel()
                {
                    Title = "Class not found",
                    MessageText = "Please provide a valid C# class",
                    ButtonText = "OK",
                });
            }

            if (string.IsNullOrWhiteSpace(Class))
            {
                ShowError();
                return;
            }

            var matches = Regex.Matches(Class, ClassRegex, RegexOptions.Multiline);
            if (matches.Count == 1)
            {
                var firstMatch = matches[0];
                if (firstMatch.Groups.Count >= 2)
                {
                    var className = firstMatch.Groups[1];
                    var classType = await CSharpScript.EvaluateAsync<Type>(Class + $" return typeof({className});", 
                        ScriptOptions.Default
                            .WithReferences(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute).Assembly)
                            .WithImports("System.Collections.Generic", "System.ComponentModel.DataAnnotations")
                        );
                    var generator = new JSchemaGenerator();
                    var schema = generator.Generate(classType);
                    Schema = schema.ToString();
                }
                else
                {
                    ShowError();
                }
            }
            else
            {
                ShowError();
            }
        }

        private void Validate(MainWindow mainWindow)
        {
            if (string.IsNullOrWhiteSpace(Schema) || string.IsNullOrWhiteSpace(Json))
            {
                _windowService.ShowDialogWindow(mainWindow, new DialogViewModel()
                {
                    Title = "JSON schema and JSON are required",
                    MessageText = "Please provide JSON schema and JSON",
                    ButtonText = "OK",
                });
                return;
            }

            try
            {
                var schema = JSchema.Parse(Schema);
                var json = JObject.Parse(Json);
                if (json.IsValid(schema))
                {
                    _windowService.ShowDialogWindow(mainWindow, new DialogViewModel()
                    {
                        Title = "JSON is valid",
                        MessageText = "JSON is valid",
                        ButtonText = "OK",
                    });
                }
                else
                {
                    _windowService.ShowDialogWindow(mainWindow, new DialogViewModel()
                    {
                        Title = "JSON is invalid",
                        MessageText = "JSON is invalid",
                        ButtonText = "OK",
                    });
                }
            }
            catch (Exception e)
            {
                _windowService.ShowDialogWindow(mainWindow, new DialogViewModel()
                {
                    Title = "Error",
                    MessageText = e.ToString(),
                    ButtonText = "OK",
                });
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