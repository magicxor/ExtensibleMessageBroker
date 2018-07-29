using System;
using AngleSharp;
using AngleSharp.Dom;

namespace Emb.DataSourceProvider.Dvach.Formatting
{
    public class PlainTextMarkupFormatter : IMarkupFormatter
    {
        string IMarkupFormatter.Comment(IComment comment)
        {
            return string.Empty;
        }

        string IMarkupFormatter.Doctype(IDocumentType doctype)
        {
            return string.Empty;
        }

        string IMarkupFormatter.Processing(IProcessingInstruction processing)
        {
            return string.Empty;
        }

        string IMarkupFormatter.Text(string text)
        {
            return text;
        }

        string IMarkupFormatter.OpenTag(IElement element, bool selfClosing)
        {
            switch (element.LocalName)
            {
                case "p":
                    return Environment.NewLine;
                case "br":
                    return Environment.NewLine;
            }

            return string.Empty;
        }

        string IMarkupFormatter.CloseTag(IElement element, bool selfClosing)
        {
            return string.Empty;
        }

        string IMarkupFormatter.Attribute(IAttr attr)
        {
            return string.Empty;
        }
    }
}
