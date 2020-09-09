namespace FunctionAppBlobStorage.Models
{
    public class Blob
    {
        public string RequestId { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string BlobType { get; set; }
        public string Url { get; set; }
    }
}
