namespace ZweigEngine.Common.Services.Interfaces.Files;

public interface IFileSystem
{
	string GetDocumentPath(string path);
	string GetDataPath(string path);
	bool   FileExists(string path);
	Stream OpenRead(string path);
	Stream OpenWrite(string path);
}