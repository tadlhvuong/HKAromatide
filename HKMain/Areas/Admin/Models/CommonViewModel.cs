using HKShared.Data;
using HKShared.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace HKMain.Areas.Admin.Models
{
    public class CommonModel
    {
        public string Key;
        public string Value;
    }
    public class CommonModelRemove
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get { return false; } set { } }
    }
    public class ModalFormResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string data { get; set; }
        public string? SubData { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class FileUploadResult
    {
        public string[] initialPreview { get; set; }
        public object[] initialPreviewConfig { get; set; }
    }
    public class FileUploadModel
    {
        public int? Id { get; set; }

        public string FileName { get; set; }

        public string FileData { get; set; }
    }

    public class EventDrag
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
    public class EditEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool AllDay { get; set; } = true;
    }

    public class RemoveEvent
    {
        public int Id { get; set; }
    }

    public class SelectItemModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
        public List<SelectItemModel> ItemsChild { get; set; }
    }
    public class SelectItemStringModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
        public List<SelectItemModel> ItemsChild { get; set; }
    }
}
