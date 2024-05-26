using MultiFamilyPortal.Data.Views;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MultiFamilyPortal.Extensions
{
    public static class BlogPostExtensions
    {
        private const string HighlightJsTemplate = @"<script src=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/10.1.1/languages/{0}.min.js"" integrity=""{1}"" crossorigin=""anonymous""></script>";

        public static PostDetailView RenderBody(this PostDetailView post, HttpRequest request)
        {
            var sanitized = post.Body
                .Replace("<br>", "<br />")
                .Replace("&nbsp;", "<!-- &nbsp; -->")
                .Replace("http://localhost", $"{request.Protocol}://{request.Host}");

            // <img width="auto" height="auto" class="e-rte-image e-imgbreak e-resize" style="min-height: 0px; min-width: 0px;" alt="AvantiPoint Customer Portal" src="https://cdn.avantipoint.com/images/blog/portal-screenshot.png">
            while (sanitized.Contains("e-rte-image"))
            {
                for (int openingIndex = sanitized.IndexOf("e-rte-image"); openingIndex > 0; openingIndex--)
                {
                    if (sanitized.Substring(openingIndex, 4) == "<img")
                    {
                        var tmp = sanitized.Substring(openingIndex);
                        var imageElement = tmp.Substring(0, tmp.IndexOf('>')) + " />";
                        var cleanedImage = Clean(imageElement, @"\s(width=\""[a-zA-Z\d]+\"")", @"\s(height=\""[a-zA-Z\d]+\"")");
                        cleanedImage = Regex.Replace(cleanedImage, @"\s(class=\""[a-zA-Z\d\-\s]+\"")", " class=\"img-fluid\"");
                        cleanedImage = Regex.Replace(cleanedImage, @"\s(style=\""[a-zA-Z\d\-\:\;\s]+\"")", " style=\"max-height:600px;\"");
                        sanitized = sanitized.Substring(0, openingIndex) + cleanedImage + sanitized.Substring(openingIndex + tmp.IndexOf('>') + 1);
                        break;
                    }

                    if (openingIndex == 0)
                    {
                        post.Body = sanitized;
                        post.Scripts = Array.Empty<string>();
                        return post;
                    }
                }
            }

            // Set up lazy loading of images/iframes
            var replacement = " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
            var pattern = "(<img.*?)(src=[\\\"|'])(?<src>.*?)([\\\"|'].*?[/]?>)";
            sanitized = Regex.Replace(sanitized, pattern, m => m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value);

            // Youtube content embedded using this syntax: [youtube:xyzAbc123]
            var video = "<div class=\"video text-center\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allow=\"fullscreen\"></iframe></div>";
            sanitized = Regex.Replace(
                sanitized,
                @"\[youtube:(.*?)\]",
                m => string.Format(CultureInfo.InvariantCulture, video, m.Groups[1].Value));

            var doc = XDocument.Parse($"<article>{sanitized}</article>");

            var codeLanguages = ProcessCodeElements(ref doc);
            RemoveEmptyParagraphs(ref doc);

            post.Body = doc.ToString().Replace("<!-- &nbsp; -->", "&nbsp;");
            post.Scripts = codeLanguages.Select(x => GetHighlightJs(x)).Distinct();
            return post;
        }

        private static string Clean(string input, params string[] patterns)
        {
            foreach (var pattern in patterns)
                input = Regex.Replace(input, pattern, string.Empty);

            return input;
        }

        private static void RemoveEmptyParagraphs(ref XDocument doc)
        {
            var paragraphs = doc.Root.XPathSelectElements("p").ToArray();
            foreach (XElement p in paragraphs)
            {
                if (!string.IsNullOrEmpty(p.Value))
                    continue;

                var children = p.Nodes().ToList();

                if (children.Count < 1 || !(children.First() is XElement element && element.Name == "span"))
                    continue;

                if (!string.IsNullOrEmpty(element.Value) || element.Nodes().Count() != 1)
                    continue;

                // looking for br or img
                XElement spanChild = element.Nodes().OfType<XElement>().FirstOrDefault();
                if (spanChild is null)
                    continue;

                if (spanChild.Name == "br")
                {
                    p.Remove();
                }
                else if (spanChild.Name == "img")
                {
                    p.AddBeforeSelf(spanChild);
                    p.Remove();
                }
            }
        }

        private static string GetHighlightJs(string language)
        {
            var codeLang = SupportedLanguages.FirstOrDefault(x => x == language);
            if (codeLang is null)
                return $"<!-- Unsupported Language {language} -->";

            return string.Format(HighlightJsTemplate, codeLang.Name, codeLang.Sha);
        }

        private static IEnumerable<string> ProcessCodeElements(ref XDocument doc)
        {
            var languages = new List<string>();
            var preElements = doc.Root.XPathSelectElements("pre");
            foreach (var element in preElements)
            {
                var innerContent = Regex.Replace(element.ToString()
                    .Replace("<br />", "\n"), @"<(/)?pre>", string.Empty)
                    .Replace("\r", string.Empty)
                    .Trim();
                var lines = innerContent.Split('\n').ToList();
                if (lines.Count < 4 || !lines[0].Trim().StartsWith("```"))
                    continue;

                var language = lines[0].Substring(3).Trim();
                var codeLang = SupportedLanguages.FirstOrDefault(x => x == language);
                if (codeLang is null)
                    continue;

                if (!languages.Contains(codeLang.Name))
                    languages.Add(codeLang.Name);

                lines.RemoveAt(0);
                while ((string.IsNullOrEmpty(lines.Last()) || lines.Last().Trim() == "```") && lines.Count > 0)
                    lines.RemoveAt(lines.Count - 1);

                var updatedContent = string.Join('\n', lines).Trim();
                var codeElement = new XElement("code");
                codeElement.SetAttributeValue("class", codeLang.Name);
                codeElement.SetValue(updatedContent);
                element.SetValue(string.Empty);
                element.Add(codeElement);
            }
            return languages;
        }

        private static readonly IEnumerable<CodeLanguage> SupportedLanguages = new[]
        {
            new CodeLanguage{ Name = "bash", Sha = "sha512-F/ixpxoJXlYD1M7iA8u23/OKw5PSb3oQufBxUkDQN0EOEkTSjEw1pozJXTGrt/V7x89Hgr5uzHmpU4MPypddHg==" },
            new CodeLanguage{Name = "cpp", Alias = new[] { "cpp", "c++", "cplusplus" }, Sha = "sha512-wt4s+IH4X6IQQb4+luM0zN+5SKGody3L5YAArjSV+3YhcKSxbPsXpbWFxX45nStWpdQJdlLivbaQWQmnjHF7KQ=="},
            new CodeLanguage{Name = "csharp", Alias = new[] { "csharp", "cs", "c#" }, Sha = "sha512-dQorbxHDJF0jQ9jDdUgFc3PfpIxRV18/EMI7ToQTe2fbD8HAms+eNjpLI+A0SMB/YQIc/NeFhBYSX/UCaEoIzg=="},
            new CodeLanguage{Name = "css", Sha = "sha512-UBNT6+S1FuSLwHTzfo6BqVU4AOKftOiict0fXKr4Vwz3YIjgsVURHxzHg3wWIwDawWumMO7JrluSLost+8i3UA=="},
            new CodeLanguage{Name = "dart", Sha = "sha512-ERKklgBSQ6recDSzPWzYSX0YuQR9LEQk4R2o5qAIsxGHB1UrmcQVYftgkcBaVpp4uaUA81Im3EtX7QNWJD7q5w=="},
            new CodeLanguage{Name = "fsharp", Alias = new[] { "fsharp", "f#", "fs" }, Sha = "sha512-kjZgo9bA1TPElx6epJamR00UeWTyWs/CazbdAwGhoIqmv54ofNQIJY57RAnYleMXhfdzHKe2VcmJogHaQnDEOw=="},
            new CodeLanguage{Name = "gradle", Sha = "sha512-N9gXKUYNzXRcWUh1qbBSAYBVGcohjSN/55Q5WEK1cpqwwsQto11oZQ9rilO4reQE7iKc6s2W9givL6UNEuHR5g=="},
            new CodeLanguage{Name = "java", Sha = "sha512-540O5/LrjNtBV1tTVI4JCRwjZ6Pj7+Tojygd4fxX4CwJ6Y/MG1BzTocoV6fBooGZMHIRqRY262UnvvTPcegCGA=="},
            new CodeLanguage{Name = "json", Sha = "sha512-FoN8JE+WWCdIGXAIT8KQXwpiavz0Mvjtfk7Rku3MDUNO0BDCiRMXAsSX+e+COFyZTcDb9HDgP+pM2RX12d4j+A=="},
            new CodeLanguage{Name = "kotlin", Sha = "sha512-ny0uZBhromvQEMSwpE4w/pXN1rf+3ulazI2sONv0D46Kv9QPeqOj37NkWlNhRUV69PJAfbxkXxHW37w3WzAXLg=="},
            new CodeLanguage{Name = "lua", Sha = "sha512-1HWek0z7Lf36KItuwmD37FvkkkKTs5MhRUjmagKoL06NQ7CU1o+b8iT6GGx0MAsImA4hxvT30MR9+7/rlQ72rw=="},
            new CodeLanguage{Name = "markdown", Alias = new[] { "markdown", "md" }, Sha = "sha512-utO8hnm2PGjXvKsyf/H6ZUaFlctc2aiDmC9fNqcyycD8rEAxFM6rTrcpY9MUfkbrXLF9tfU8kQWD9dotZ77gKQ=="},
            new CodeLanguage{Name = "objectivec", Sha = "sha512-CCRT/PmNRjcpmz7GTK6qqFuXjLG1dc5puEUaOtg+P9lbzInybEiPUH98bNw+1JoW5V7LmTeBWgmiksQt59JX4g=="},
            new CodeLanguage{Name = "plaintext", Alias = new[] { "plaintext", "text", "txt" }, Sha = "sha512-qTSC4RbnoEHf0+aTEsI0Eyvm5HSI4cOOeMifLrpreK8wYt6izIU7dtgFONMvNhSRGqRgfo29mkLTQJQhnGcyaQ=="},
            new CodeLanguage{Name = "powershell", Sha = "sha512-PwV5Q67iPMuWqs6aDWCmrGm7keyzorPmleIF2Qe27hvQIvwxL7RUSCR4ChRjTZYYMM60FxrGOOhudyNYhqTdYw=="},
            new CodeLanguage{Name = "scss", Sha = "sha512-E2Gmd9vH0BXoGHlWshFIjW985slDBATPs4P4OJo9vK9zBvvKUJsQTDTuQAPaZ2xiDAvL6gZ7j9tdj1Nx8I6/8g=="},
            new CodeLanguage{Name = "shell", Sha = "sha512-FiYwprcVrOfz51safCFCCIYW40tRDxjHt4fDqJtK2iwc4oUczdm7iW/J7fVlB77zGvKBM5emYxHfkbhoHYr2hg=="},
            new CodeLanguage{Name = "sql", Sha = "sha512-MU/UZ5in1pyqerHV1uuS1W2r4OkTeEvVtpPheJPCLbu2SN8rGbrJ8nG/AWkoGglpSBq7ckO4QxrodwUGUbltbA=="},
            new CodeLanguage{Name = "swift", Sha = "sha512-EShXEgqHHatDPL5k8+RCRdEvZWVr77b8z8GgkzaFmYUfyWd/X5SHz6VuERyeB729QU5a9MPF1lPGynFXVLgMMg=="},
            new CodeLanguage{Name = "typescript", Alias = new[] { "typescript", "ts" }, Sha = "sha512-aYQq9+L5KVrDBjLttS8EEV1lo2HlqR/2VRzVgz85+K8hAZrZhZPj6IEoDUG0Ntd/1jcaxJB++bJsUew/XbwQRw=="},
            new CodeLanguage{Name = "xml", Sha = "sha512-dICltIgnUP+QSJrnYGCV8943p3qSDgvcg2NU4W8IcOZP4tdrvxlXjbhIznhtVQEcXow0mOjLM0Q6/NvZsmUH4g=="},
            new CodeLanguage{Name = "yaml", Alias = new[] { "yaml", "yml" }, Sha = "sha512-Rd5FUDZGJkdJ4Z/pZTgK+6QMWl0ubeugVNnXMK/QhiDxw5IMuYYxJOoPcof66vzvkpg0XjumgSvdaLOftzR5uQ=="},
        };

        private class CodeLanguage
        {
            public string Name { get; set; }

            public IEnumerable<string> Alias { get; set; } = Array.Empty<string>();

            public string Sha { get; set; }

            public static bool operator !=(CodeLanguage codeLanguage, string language) =>
                !codeLanguage.Name.Equals(language) && !codeLanguage.Alias.Contains(language);

            public static bool operator ==(CodeLanguage codeLanguage, string language) =>
                codeLanguage.Name.Equals(language) || codeLanguage.Alias.Contains(language);

            public override bool Equals(object obj)
            {
                if (obj is CodeLanguage codeLanguage)
                    return Name.Equals(codeLanguage.Name);

                if(obj is string language)
                    return Name.Equals(language) || Alias.Equals(language);

                return false;
            }

            public override int GetHashCode() => Name.GetHashCode();
        }
    }
}
