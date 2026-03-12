namespace ERP.BusinessModels.ParameterVM
{
    public class ImageUploadModel
    {
        public long Id { get; set; }
        public string ImageName { get; set; }
        public string FileSource { get; set; }
        public string Extension { get; set; }
        public long? EmployeeDocumentTypeId { get; set; }
    }
}