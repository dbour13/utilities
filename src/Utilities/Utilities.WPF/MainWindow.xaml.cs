using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities.WPF.Models;

namespace Utilities.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClipboardCellsToJSON_Click(object sender, RoutedEventArgs e)
        {
            string clipboardContents = Clipboard.GetText();
            string[] lines = clipboardContents.Split("\r\n").Where(s => !String.IsNullOrWhiteSpace(s)).ToArray();

            if (!lines.Any())
            {
                MessageBox.Show("No text in clipboard to convert");
            }
            else
            {
                // Get header from first line
                string[] headers = lines[0].Split("\t").Select(s => $"\"{s.Replace("\"","\\\"")}\"").ToArray();

                if (!headers.Any())
                {
                    MessageBox.Show("Can't convert empty string");
                }
                else
                {
                    StringBuilder result = new StringBuilder();

                    // Start of JSON string
                    result.Append($"{{\n\t\"{txtClipboardCellsToJSON.Text}\":\n\t[");

                    // Add each entity (skip header)
                    int cnt = 0;
                    foreach (var line in lines.Skip(1))
                    {
                        if (cnt > 0)
                        {
                            result.Append(',');
                        }

                        string[] entityValues = line.Split("\t").Select(s => 
                            {
                                if (Int32.TryParse(s, out int intResult))
                                {
                                    // Don't put double quotes around it if it's an int
                                    return $"{s}";
                                }
                                else
                                {
                                    return $"\"{s.Replace("\"", "\\\"")}\"";
                                }
                            }
                        ).ToArray();
                        StringBuilder entityString = new StringBuilder();
                        
                        // Start of Entity JSON string
                        entityString.Append("\n\t\t{\n");

                        // Add each value to entity
                        int cnt2 = 0;
                        foreach (var value in entityValues)
                        {
                            if (cnt2 > 0)
                            {
                                entityString.Append(",\n");
                            }

                            entityString.Append($"\t\t\t{headers[cnt2]}: {value}");
                            cnt2++;
                        }

                        // End of Entity JSON string
                        entityString.Append("\n\t\t}");
                        result.Append(entityString.ToString());
                        cnt++;
                    }

                    // End of JSON string
                    result.Append($"\n\t]\n}}");

                    Clipboard.SetText(result.ToString());
                }
            }
        }

        private ClassMaker ParseJsonElement(JsonElement element, string name = null, string value = null)
        {
            ClassMaker result = new ClassMaker();
            result.Name = name;

            switch (element.ValueKind)
            {
                case JsonValueKind.Array:
                    {
                        bool first = true;

                        result.Properties = new List<ClassMaker>();
                        foreach (var property in element.EnumerateArray())
                        {
                            var jsonElement = ParseJsonElement(property, name);

                            result.Properties.Add(jsonElement);

                            if (first)
                            {
                                if (jsonElement.Type == ClassMakerType.Class)
                                {
                                    result.Type = ClassMakerType.ArrayClass;
                                    result.Value = value;

                                    first = false;
                                }
                                else
                                {
                                    result.Type = ClassMakerType.Property;
                                    result.Value = jsonElement.Properties.Select(p => p.Value).ToArray();
                                }
                            }
                        }
                    }
                    break;
                case JsonValueKind.Object:
                    {
                        result.Type = ClassMakerType.Class;
                        result.Value = value;

                        result.Properties = new List<ClassMaker>();
                        foreach (var property in element.EnumerateObject())
                        {
                            result.Properties.Add(ParseJsonElement(property.Value, property.Name, property.Name));
                        }
                    }
                    break;
                case JsonValueKind.String:
                    result.Type = ClassMakerType.Property;
                    result.Value = element.GetString();
                    break;
                case JsonValueKind.Number:
                    result.Type = ClassMakerType.Property;
                    result.Value = element.GetInt32();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private void btnClipboardJSONtoCSharpClassWithData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string clipboardContents = Clipboard.GetText();

                if (!String.IsNullOrWhiteSpace(clipboardContents))
                {
                    //var dynamicObject = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(clipboardContents)!;
                    //Dictionary<string, object> values = ((object)dynamicObject).GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(dynamicObject));

                    var values = JsonSerializer.Deserialize<JsonElement>(clipboardContents);

                    var classMaker = ParseJsonElement(values, txtClipboardJSONtoCSharpClassWithData.Text);

                    string classString = classMaker.ToString();
                    classString += "\n\n" + classMaker.ValuesToString();

                    Clipboard.SetText(classString);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to Deserialize JSON: {ex.Message}");
            }
        }
    }

}
