namespace AzureUploader.FtpCommands
{
    internal interface IFtpUploader
    {
        void UploadFile(string filePath, string targetPath);
    }
}
