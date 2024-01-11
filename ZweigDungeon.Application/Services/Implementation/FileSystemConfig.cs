using System;
using System.IO;
using ZweigEngine.Common.Services.Interfaces.Files;

namespace ZweigDungeon.Application.Services.Implementation;

public class FileSystemConfig : IFileSystem
{
	public string GetDocumentPath(string path)
	{
		var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		return Path.Combine(docs, path);
	}

	public string GetDataPath(string path)
	{
		return Path.Combine("Data", path);
	}

	public bool FileExists(string path)
	{
		return File.Exists(path);
	}

	public Stream OpenRead(string path)
	{
		return File.OpenRead(path);
	}

	public Stream OpenWrite(string path)
	{
		return File.Create(path);
	}
}