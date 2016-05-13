using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Lua.LanguageService.Format
{
    class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input Lua source code file to format.")]
        public string InputFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    class FormattedText
    {
        public string Text;
    }

    class Program
    {
        static int Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                //try
                {
                    using (FileStream fs = File.Open(options.InputFile, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            var formattedText = new FormattedText
                            {
                                Text = LuaFormatter.Instance.DoFormatting(sr.ReadToEnd())
                            };

                            Console.WriteLine(JsonConvert.SerializeObject(
                                formattedText,
                                Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
                                })
                            );
                        }
                    }

                    return 0;
                }
                //catch (Exception ex)
                //{
                //    Console.Error.WriteLine("Error:");
                //    Console.Error.WriteLine(ex.Message);
                //    return -1;
                //}
            }
            return 0;
        }
    }
}
