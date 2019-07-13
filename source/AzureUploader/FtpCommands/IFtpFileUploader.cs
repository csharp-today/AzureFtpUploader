namespace AzureUploader.FtpCommands
{
    internal interface IFtpFileUploader
    {
        void UploadFile(string filePath, string targetPath);
    }
}
