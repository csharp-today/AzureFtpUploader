namespace AzureUploader.FtpCommands
{
    internal interface IFtpTextUploader
    {
        void UploadText(string text, string targetPath);
    }
}
