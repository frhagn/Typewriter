using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Typewriter.Generation.Parsing;
using Typewriter.TemplateEditor;
using Typewriter.TemplateEditor.Lexing;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class FileInfo
        {
            public string Name { get; set; }
            public ICollection<ClassInfo> Classes { get; set; }
        }
        
        public class ClassInfo
        {
            public string Name { get; set; }
            public bool Bool { get; set; }
        }

        private void source_TextChanged(object sender, EventArgs e)
        {
            var stopwatch = Stopwatch.StartNew();
            var tokens = new Lexer().Tokenize(source.Text);
            stopwatch.Stop();
            var parser = new Parser();

            var file = new FileInfo { Name = "File", Classes = new[] { new ClassInfo { Name = "Class 1" }, new ClassInfo { Name = "Class 2", Bool = true} } };

            target.Text = string.Format("{0} ms\r\n", stopwatch.ElapsedMilliseconds) + parser.Parse(source.Text, file);

            foreach (var token in tokens.List)
            {
                target.Text += string.Format("{0}\t{1}\t{2}\t{3}\r\n", token.Line, token.Start, token.Length, token.Classification);
            }
            
            preview.ResetText();

            int end = 0;
            for (var i = 0; i < source.TextLength; i++)
            {
                var context = tokens.GetContext(i);
                switch (context.Name)
                {
                    case "File":
                        preview.SelectionBackColor = Color.Beige;
                        break;

                    case "Class":
                        preview.SelectionBackColor = Color.LightBlue;
                        break;

                    default:
                        preview.SelectionBackColor = Color.White;
                        break;
                }

                var token = tokens.GetToken(i);
                if (token != null)
                {
                    end = token.Start + token.Length;

                    switch (token.Classification)
                    {
                        case Classifications.Comment:
                            preview.SelectionColor = Color.Green;
                            break;

                        case Classifications.Keyword:
                            preview.SelectionColor = Color.Blue;
                            break;

                        case Classifications.Property:
                            preview.SelectionColor = Color.DarkRed;
                            break;

                        case Classifications.Operator:
                            preview.SelectionColor = Color.DimGray;
                            break;

                        case Classifications.String:
                            preview.SelectionColor = Color.Chocolate;
                            break;

                        case Classifications.Number:
                            preview.SelectionColor = Color.BlueViolet;
                            break;

                        //case TokenType.OpenBrace:
                        //case TokenType.CloseBrace:
                        //    preview.SelectionColor = Color.HotPink;
                        //    break;

                        //case TokenType.OpenCurlyBrace:
                        //case TokenType.CloseCurlyBrace:
                        //    preview.SelectionColor = Color.DeepPink;
                        //    break;

                        //case TokenType.OpenFunctionBrace:
                        //case TokenType.CloseFunctionBrace:
                        //    preview.SelectionColor = Color.LightPink;
                        //    break;
                    }
                }
                else if (i == end)
                {
                    preview.SelectionColor = Color.Black;
                }

                if (source.Text[i] != '\n')
                    preview.AppendText(source.Text[i].ToString());
            }
        }
    }
}
