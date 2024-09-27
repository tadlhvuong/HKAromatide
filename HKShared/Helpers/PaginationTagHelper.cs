using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace HKShared.Helpers
{

    public class PaginationTagHelper : TagHelper
    {
        public string CssStyle { get; set; }
        public string PrevText { get; set; }
        public string NextText { get; set; }
        public bool ShowRecords { get; set; }
        public int PagesCount { get; set; }
        public PagedModel Model { get; set; }
        public QueryString QueryString { get; set; }

        public PaginationTagHelper()
        {
            CssStyle = "pagination";
            PrevText = "« Trước";
            NextText = "Sau »";
            ShowRecords = false;
            PagesCount = 7;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.Attributes.Add("class", CssStyle);
            buildContent(output);
        }

        private void buildContent(TagHelperOutput output)
        {
            List<TagBuilder> liTags = new List<TagBuilder>();

            if (Model.TotalRows > 0)
            {
                int lastPage = Model.TotalPages;
                Model.CurPage = Math.Min(Model.CurPage, lastPage);
                Model.CurPage = Math.Max(Model.CurPage, 1);

                buildFirstPages(liTags, PrevText);
                buildMidPages(liTags, lastPage, PagesCount);
                buildLastPages(liTags, lastPage, NextText);

                if (ShowRecords)
                    liTags.Add(createRecordsInfo());
            }

            using (var writer = new StringWriter())
            {
                foreach (var item in liTags)
                    item.WriteTo(writer, HtmlEncoder.Default);

                output.Content.SetHtmlContent(writer.ToString());
            }
        }

        private void buildFirstPages(List<TagBuilder> liTags, string PrevText)
        {
            int prevPage = Math.Max(Model.CurPage - 1, 1);
            liTags.Add(createPageLink(prevPage, PrevText));
            liTags.Add(createPageLink(1, "1", "active"));
        }

        private void buildLastPages(List<TagBuilder> liTags, int lastPage, string NextText)
        {
            if (lastPage > 1)
                liTags.Add(createPageLink(lastPage, lastPage.ToString(), "active"));
            int nextPage = Math.Min(Model.CurPage + 1, lastPage);
            liTags.Add(createPageLink(nextPage, NextText));
        }

        private void buildMidPages(List<TagBuilder> liTags, int lastPage, int linkCount)
        {
            int numLast = Model.CurPage;
            int numFirst = Model.CurPage;
            while (numFirst > 1 || numLast < lastPage)
            {
                if (numLast < lastPage) { if (--linkCount <= 0) break; numLast++; }
                if (numFirst > 1) { if (--linkCount <= 0) break; numFirst--; }
            }

            if (numFirst > 1)
                liTags.Add(createPageLink(Model.CurPage, "...", "disabled"));

            for (int i = numFirst + 1; i <= numLast - 1; i++)
                liTags.Add(createPageLink(i, i.ToString(), "active"));

            if (numLast < lastPage)
                liTags.Add(createPageLink(Model.CurPage, "...", "disabled"));
        }

        private TagBuilder createRecordsInfo()
        {
            int firstRow = (Model.CurPage - 1) * Model.PageSize + 1;
            int lastRow = firstRow + Model.PageSize - 1;
            if (lastRow > Model.TotalRows)
                lastRow = Model.TotalRows;

            TagBuilder liTag = new TagBuilder("li");
            liTag.AddCssClass("page-item");
            liTag.AddCssClass("disabled");
            liTag.InnerHtml.SetHtmlContent(string.Format("<span class=\"page-link\">Rows {0} to {1} of {2}</span>", firstRow, lastRow, Model.TotalRows));
            return liTag;
        }

        private TagBuilder createPageLink(int page, string text, string style = "disabled")
        {
            TagBuilder liTag = new TagBuilder("li");

            liTag.AddCssClass("page-item");
            liTag.AddCssClass("paginate_button");

            if (page == Model.CurPage)
            {
                liTag.AddCssClass(style);
                liTag.InnerHtml.SetHtmlContent($"<span class=\"page-link\">{text}</span>");
            }
            else
            {
                var query = QueryHelpers.ParseQuery(QueryString == null ? "" : QueryString.Value);
                query["CurPage"] = page.ToString();

                var qItems = query.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
                var qBuilder = new QueryBuilder(qItems);
                string pageLink = qBuilder.ToString();
                liTag.InnerHtml.SetHtmlContent($"<a href=\"{pageLink}\" class=\"page-link\">{text}</a>");
            }

            return liTag;
        }
    }
}
