using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chirp.Models
{
    public class Message
    {
        private static char[] tagDelimeters = { ' ', '.' };

        private string _text;
        private List<string> _tags;

        public string User { get; set; }
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                _tags = ParseTextForTags(_text);
            }
        }
        public List<string> Tags
        {
            get
            {
                return _tags;
            }
        }
        public DateTime Timestamp { get; set; }

        public string FormattedText
        {
            get
            {
                string text = Text;
                var fText = new System.Text.StringBuilder();

                while (text.Contains("#"))
                {
                    // check for text before tag
                    int tagStart = text.IndexOf("#");
                    fText.Append(text.Substring(0, tagStart));
                    text = text.Substring(tagStart);

                    // find tag

                    string tag;

                    int tagEnd = text.IndexOfAny(tagDelimeters);
                    if (tagEnd < 0) // tag ends text
                    {
                        tag = text.Substring(1);
                        text = string.Empty;
                    }
                    else // more text after tag
                    {
                        tag = text.Substring(1, tagEnd - 1);
                        text = text.Substring(tagEnd);
                    }
                    fText.Append(string.Format(@"<a href='/Home/Search?tag={0}'>#{0}</a>", tag));
                }
                fText.Append(text); // append any remaining text

                return fText.ToString();            
            }
        }

        private static List<String> ParseTextForTags(string text)
        {
            var tags = new List<string>();

            while (text.Contains("#"))
            {
                text = text.Substring(text.IndexOf("#"));
                int tagEnd = text.IndexOfAny(tagDelimeters);
                if (tagEnd < 0)
                {
                    tags.Add(text.Substring(1));
                    break;
                }
                tags.Add(text.Substring(1, tagEnd - 1));
                text = text.Substring(tagEnd);
            }

            return tags;
        }
    }
}