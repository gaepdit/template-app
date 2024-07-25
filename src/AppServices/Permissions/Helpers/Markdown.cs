using Markdig;
using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.AppServices.Permissions.Helpers
{
    public static class MarkdownHelper
    {
        public static string MarkdownToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var sanitizer = new Ganss.Xss.HtmlSanitizer();
            return sanitizer.Sanitize(Markdig.Markdown.ToHtml(markdown, pipeline));
        }
    }
}
