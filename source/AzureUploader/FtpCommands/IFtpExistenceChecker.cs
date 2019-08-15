namespace AzureUploader.FtpCommands
{
    internal interface IFtpExistenceChecker
    {
        bool FileExist(string path);
    }
}
