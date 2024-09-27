using Microsoft.AspNetCore.Mvc.Rendering;

namespace HKShared.Helpers
{
    public class PagedModel
    {
        private static SelectList pagesList;
        static PagedModel()
        {
            var pages = new SortedList<int, string>();
            for (int i = 5; i <= 50; i += 5)
                pages.Add(i, string.Format("{0} rows", i));

            pagesList = new SelectList(pages, "Key", "Value");
        }

        public object Master { get; set; }
        public string Group { get; set; }
        public string Filter { get; set; }
        public string Search { get; set; }

        public int PageSize { get; set; }
        public int CurPage { get; set; }
        public int TotalRows { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalRows / PageSize); }
        }

        public SelectList PageSelectList
        {
            get { return pagesList; }
        }

        public PagedModel()
        {
            CurPage = 1;
            PageSize = 15;
        }
    }

    public class PagedList<T> : PagedModel
    {
        public string Name { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public List<T> Content { get; set; }
    }
}
