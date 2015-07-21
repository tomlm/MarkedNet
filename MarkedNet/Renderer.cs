﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace MarkedNet
{
    public class Renderer
    {
        #region Fields

        private Options _options;

        #endregion

        #region Constructors

        public Renderer()
            : this(null)
        {
        }

        public Renderer(Options options)
        {
            _options = options ?? new Options();
        }

        #endregion

        #region Methods

        #region Block Level Renderer

        public virtual string Code(string code, string lang, bool escaped)
        {
            if (_options.Highlight != null)
            {
                var @out = _options.Highlight(code, lang);
                if (@out != null && @out != code)
                {
                    escaped = true;
                    code = @out;
                }
            }

            if (String.IsNullOrEmpty(lang))
            {
                return "<pre><code>" + (escaped ? code : StringHelper.Escape(code, true)) + "\n</code></pre>";
            }

            return "<pre><code class=\""
                + _options.LangPrefix
                + StringHelper.Escape(lang, true)
                + "\">"
                + (escaped ? code : StringHelper.Escape(code, true))
                + "\n</code></pre>\n";
        }

        public virtual string Blockquote(string quote)
        {
            return "<blockquote>\n" + quote + "</blockquote>\n";
        }

        public virtual string Html(string html)
        {
            return html;
        }

        public virtual string Heading(string text, int level, string raw)
        {
            return "<h"
                + level
                + " id=\""
                + _options.HeaderPrefix
                + Regex.Replace(raw.ToLower(), @"[^\w]+", "-")
                + "\">"
                + text
                + "</h"
                + level
                + ">\n";
        }

        public virtual string Hr()
        {
            return _options.XHtml ? "<hr/>\n" : "<hr>\n";
        }

        public virtual string List(string body, bool ordered)
        {
            var type = ordered ? "ol" : "ul";
            return "<" + type + ">\n" + body + "</" + type + ">\n";
        }

        public virtual string ListItem(string text)
        {
            return "<li>" + text + "</li>\n";
        }

        public virtual string Paragraph(string text)
        {
            return "<p>" + text + "</p>\n";
        }

        public virtual string Table(string header, string body)
        {
            return "<table>\n"
                + "<thead>\n"
                + header
                + "</thead>\n"
                + "<tbody>\n"
                + body
                + "</tbody>\n"
                + "</table>\n";
        }

        public virtual string TableRow(string content)
        {
            return "<tr>\n" + content + "</tr>\n";
        }

        public virtual string TableCell(string content, TableCellFlags flags)
        {
            var type = flags.Header ? "th" : "td";
            var tag = !String.IsNullOrEmpty(flags.Align)
                ? "<" + type + " style=\"text-align:" + flags.Align + "\">"
                : "<" + type + ">";

            return tag + content + "</" + type + ">\n";
        }

        #endregion

        #region Span Level Renderer

        public virtual string Strong(string text)
        {
            return "<strong>" + text + "</strong>";
        }

        public virtual string Em(string text)
        {
            return "<em>" + text + "</em>";
        }

        public virtual string Codespan(string text)
        {
            return "<code>" + text + "</code>";
        }

        public virtual string Br()
        {
            return _options.XHtml ? "<br/>" : "<br>";
        }

        public virtual string Del(string text)
        {
            return "<del>" + text + "</del>";
        }

        public virtual string Link(string href, string title, string text)
        {
            if (_options.Sanitize)
            {
                string prot = null;
                
                try
                {
                    prot = Regex.Replace(StringHelper.DecodeURIComponent(StringHelper.Unescape(href)), @"[^\w:]", String.Empty).ToLower();
                }
                catch (Exception)
                {
                    return String.Empty;
                }

                if (prot.IndexOf("javascript:") == 0 || prot.IndexOf("vbscript:") == 0)
                {
                    return String.Empty;
                }
            }

            var @out = "<a href=\"" + href + "\"";
            if (!String.IsNullOrEmpty(title))
            {
                @out += " title=\"" + title + "\"";
            }

            @out += ">" + text + "</a>";
            return @out;
        }

        public virtual string Image(string href, string title, string text)
        {
            var @out = "<img src=\"" + href + "\" alt=\"" + text + "\"";
            if (!String.IsNullOrEmpty(title))
            {
                @out += " title=\"" + title + "\"";
            }

            @out += _options.XHtml ? "/>" : ">";
            return @out;
        }

        public virtual string Text(string text)
        {
          return text;
        }
    
        #endregion

        #endregion
    }
}
