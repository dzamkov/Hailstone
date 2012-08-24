using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace Hailstone.Interface
{
    public partial class EditorForm : Form
    {
        public EditorForm()
        {
            InitializeComponent();
            this._TextEditor.SetHighlighting("Lua");
        }

        /// <summary>
        /// A syntax mode file provider for Lua syntax highlighting.
        /// </summary>
        private class _LuaSyntaxModeFileProvider : ISyntaxModeFileProvider
        {
            /// <summary>
            /// The only syntax mode this provider will be providing.
            /// </summary>
            public SyntaxMode Mode = new SyntaxMode("Lua.xml", "Lua", ".lua");

            XmlTextReader ISyntaxModeFileProvider.GetSyntaxModeFile(SyntaxMode syntaxMode)
            {
                if (syntaxMode == Mode)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    string res = assembly.GetManifestResourceNames().First(x => x.EndsWith(syntaxMode.FileName));
                    Stream stream = assembly.GetManifestResourceStream(res);
                    return new XmlTextReader(stream);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            ICollection<SyntaxMode> ISyntaxModeFileProvider.SyntaxModes
            {
                get
                {
                    return new SyntaxMode[] { Mode };
                }
            }

            void ISyntaxModeFileProvider.UpdateSyntaxModeList()
            {
                
            }
        }

        static EditorForm()
        {
            HighlightingManager.Manager.AddSyntaxModeFileProvider(new _LuaSyntaxModeFileProvider());
        }
    }
}
