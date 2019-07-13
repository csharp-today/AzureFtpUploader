namespace AzureUploader.FtpCommands
{
    internal interface IFtpDirectoryUploader
    {
        void UploadDirectory(string directoryPath, string targetPath);
    }
}
