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
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace Hailstone.Interface
{
    public partial class EditorForm : Form
    {
        public EditorForm()
        {
            InitializeComponent();
            this._CodeCompletionImageList.Images.Add(new System.Drawing.Bitmap(16, 16));
            this._TextEditor.SetHighlighting("Lua");
            this._TextEditor.ActiveTextAreaControl.TextArea.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.Control && e.KeyCode == Keys.Space)
                {
                    e.SuppressKeyPress = true;
                    this._ShowCodeCompletion(' ');
                    return;
                }
                if (e.KeyCode == Keys.OemPeriod)
                {
                    this._ShowCodeCompletion('.');
                    return;
                }
            };
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

        /// <summary>
        /// The data provider for code completion.
        /// </summary>
        private class _CompletionDataProvider : ICompletionDataProvider
        {
            public _CompletionDataProvider(ImageList _ImageList)
            {
                this._ImageList = _ImageList;
            }

            /// <summary>
            /// The image list for this data provider.
            /// </summary>
            private ImageList _ImageList;

            int ICompletionDataProvider.DefaultIndex
            {
                get
                {
                    return -1;
                }
            }

            ICompletionData[] ICompletionDataProvider.GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
            {
                return new ICompletionData[]
                {
                    new DefaultCompletionData("Test", "Test", 0)
                };
            }

            ImageList ICompletionDataProvider.ImageList
            {
                get
                {
                    return this._ImageList;
                }
            }

            bool ICompletionDataProvider.InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
            {
                textArea.Caret.Position = textArea.Document.OffsetToPosition(Math.Min(insertionOffset, textArea.Document.TextLength));
                return data.InsertAction(textArea, key);
            }

            string ICompletionDataProvider.PreSelection
            {
                get
                {
                    return null;
                }
            }

            CompletionDataProviderKeyResult ICompletionDataProvider.ProcessKey(char key)
            {
                if (char.IsLetterOrDigit(key) || key == '_')
                {
                    return CompletionDataProviderKeyResult.NormalKey;
                }
                return CompletionDataProviderKeyResult.InsertionKey;
            }
        }

        /// <summary>
        /// Shows the code completion window, if possible.
        /// </summary>
        private bool _ShowCodeCompletion(char Typed)
        {
            ICompletionDataProvider provider = new _CompletionDataProvider(this._CodeCompletionImageList);
            CodeCompletionWindow window = CodeCompletionWindow.ShowCompletionWindow(this, this._TextEditor, "", provider, Typed);
            if (window != null)
            {
                this.TopMost = false;
                window.FormClosed += delegate { this.TopMost = true; };
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
