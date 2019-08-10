namespace AzureUploader.FtpCommands
{
    internal interface IFtpDirectoryRemover
    {
        void CleanDirectory(string path);
        void RemoveDirectory(string path);
    }
}
