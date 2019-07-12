namespace AzureUploader.FtpCommands
{
    internal interface IFtpUploader
    {
        void UploadDirectory(string directoryPath, string targetPath);
        void UploadFile(string filePath, string targetPath);
    }
}
